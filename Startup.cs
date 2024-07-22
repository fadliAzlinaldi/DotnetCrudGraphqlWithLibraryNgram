using DotnetGraphQLCRUD.Mutation;
using DotnetGraphQLCRUD.Queries;
using Microsoft.EntityFrameworkCore;
using Library.Ngram;
using Microsoft.Extensions.Options;
using DotnetGraphQLCRUD.Model;

namespace DotnetGraphQLCRUD
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }   
        public void ConfigureServices(IServiceCollection services)
        {
            #region service

            var elasticsearchConfig = new ElasticSearchConfig();
            _configuration.GetSection("Elasticsearch").Bind(elasticsearchConfig);

            services.AddSingleton<IElasticsearchClientProvider>(new ElasticsearchClientProvider(elasticsearchConfig.Uri));

            services.AddTransient<IElasticsearchServices, ElasticSearchService>();

            services.AddGraphQLServer()
                    .AddQueryType<ProductQueries>()
                    .AddMutationType<MutationProduct>();

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(_configuration.GetConnectionString("SQLSERVERCONNECTION"));
            });

            #endregion
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IElasticsearchServices elasticsearchServices)
        {
            if (env.IsDevelopment())
            {
                elasticsearchServices.CreateIndex();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGraphQL();
            });

            using var scope = app.ApplicationServices.CreateScope();
            var serviceProvider = scope.ServiceProvider;

            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.EnsureCreated();

        }
    }
}
