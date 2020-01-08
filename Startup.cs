using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using TodoApi.Models;
using Swashbuckle.AspNetCore.Swagger;

namespace TodoApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        
        public IHostingEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;

            var test = Configuration["Authentication:Authority"];
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<TodoContext>(opt => opt.UseInMemoryDatabase("TodoList"));
            services.AddMvc(setupOptions =>
            {
                var authorizationPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
                setupOptions.Filters.Add(new AuthorizeFilter(authorizationPolicy));
            });

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Sample dotnet Core API", Version = "v1" });
            });
            
            ConfigureAuthentication(services);
        }

        public void Configure(IApplicationBuilder app)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sample dotnet Core API");
            });

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseMvc();
        }

        private void ConfigureAuthentication(IServiceCollection services)
        {
            services.AddAuthentication(authOptions =>
            {
                authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(jwtOptions =>
            {
                jwtOptions.Authority = Configuration["Authentication:Authority"];
                jwtOptions.Audience = Configuration["Authentication:Audience"];

                jwtOptions.SaveToken = false;
                jwtOptions.IncludeErrorDetails = true;

                if (Environment.IsDevelopment())
                {
                    jwtOptions.RequireHttpsMetadata = false;
                }

                jwtOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidIssuer = Configuration["Authentication:Authority"],
                    ValidateLifetime = true,
                };

                jwtOptions.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = failedAuthContext =>
                    {
                        failedAuthContext.NoResult();
                        failedAuthContext.Response.StatusCode = 500;
                        failedAuthContext.Response.ContentType = "text/plain";

                        var errorMessage = failedAuthContext.Exception switch
                        {
                            SecurityTokenExpiredException _ => "Expired token.",
                            SecurityTokenNotYetValidException _ => "Token not yet valid.",
                            SecurityTokenInvalidLifetimeException _ => "Invalid token lifetime.",
                            SecurityTokenNoExpirationException _ => "Missing token expiration.",
                            SecurityTokenSignatureKeyNotFoundException _ => "Invalid token. Key not found.",
                            _ => "An error occured processing your authentication."
                        };

                        return failedAuthContext.Response.WriteAsync(errorMessage);
                    },
                    
                    // OnMessageReceived = debugContext =>
                    // {
                    //     var response = debugContext.Response;
                    //     return Task.CompletedTask;
                    // }
                };
            });

            // services.AddAuthorization(policyOptions =>
            // {
            //     policyOptions.AddPolicy("Keycloak", new AuthorizationPolicyBuilder()
            //         .RequireAuthenticatedUser()
            //         .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
            //         .Build());
            // });
        }
    }
}
