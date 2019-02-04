using AccountBalance.Application.Accounts;
using AccountBalance.Application.Helpers;
using AccountBalance.Application.Interfaces;
using AccountBalance.Domain.Helpers;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using EventFlow;
using EventFlow.Aspnetcore.Middlewares;
using EventFlow.AspNetCore.Extensions;
using EventFlow.Autofac.Extensions;
using EventFlow.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System;
using AccountBalance.Application.Accounts.Queries.Abstract;
using AccountBalance.Infrastructure.Data.EventStore;


namespace AccountBalance.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            IEventFlowOptions eventFlowOptions = EventFlowOptions.New;
            eventFlowOptions.AddAspNetCoreMetadataProviders();

            services.AddTransient<IAccountApplicationService, AccountApplicationService>();
            services.AddTransient<IAccountQueryService, AccountQueryService>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new Info { Title = "Account Balance", Version = "v1" });
                c.DescribeAllEnumsAsStrings();
            });
            var containerBuilder = new ContainerBuilder();
            var container = EventFlowOptions.New
                .UseAutofacContainerBuilder(containerBuilder)
                .AddDefaults(AccountDomainHelpers.Assembly)
                .AddDefaults(AccountApplicationHelpers.Assembly)
                .UseInMemoryReadStoreFor<AccountReadModel>()
                .AddAspNetCoreMetadataProviders();
            containerBuilder.RegisterType<AccountReadStore>().As<IAccountReadStore>().SingleInstance();
            containerBuilder.Populate(services);
            return new AutofacServiceProvider(containerBuilder.Build());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseCors(
                    options => options.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                );
            }
            else {
                app.UseHsts();
            }
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Account Balance V1");
            });
            app.UseMiddleware<CommandPublishMiddleware>();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
