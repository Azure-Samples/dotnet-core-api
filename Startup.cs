using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RundooApi.Models;
using Azure.Data.Tables;
using RundooApi.Services;
using Microsoft.Azure.Cosmos;

namespace RundooApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Rundoo API", Version = "v1" });
            });

            var connectionString = Configuration.GetConnectionString("CosmosDocDatabase");
            var tableConnectionString = Configuration.GetConnectionString("CosmosStorageTables");

            var tableClient = new TableClient(tableConnectionString, "SupplierData");
            var cosmosClient = new CosmosClient(connectionString);
            var cosmosDatabasename = "TransactionDB";
            var cosmosContainerId = "transactions2";

            services.AddSingleton<TablesService>(new TablesService(tableClient));
            services.AddSingleton<DocDBService>(new DocDBService(cosmosClient, cosmosDatabasename, cosmosContainerId));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Rundoo API V1");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            app.UseDefaultFiles();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
