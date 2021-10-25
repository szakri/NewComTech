using Common.Data;
using GraphQL.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using HotChocolate;
using Microsoft.Extensions.Logging;

namespace GraphQL
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
            services.AddPooledDbContextFactory<SchoolContext>((s, o) => o
                        .UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
                        .UseLoggerFactory(s.GetRequiredService<ILoggerFactory>()))
                    .AddGraphQLServer()
                    .AddQueryType<Query>()
                    .AddMutationType<Mutation>()
                    .SetPagingOptions(new HotChocolate.Types.Pagination.PagingOptions
                    {
                        DefaultPageSize = 10,
                        MaxPageSize = 100,
                        IncludeTotalCount = true
                    })
                    .AddProjections()
                    .AddFiltering()
                    .AddSorting()
                    .AddMaxExecutionDepthRule(5);

            services.AddAutoMapper(typeof(Startup));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGraphQL();
            });
        }
    }
}
