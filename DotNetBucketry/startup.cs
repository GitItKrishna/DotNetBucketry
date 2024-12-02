namespace DotNetBucketry;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method configures the services for the application
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMvc();
    }

    // This method configures the HTTP request pipeline
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
       if(env.IsDevelopment())
       {
           app.UseDeveloperExceptionPage();
       }
       app.UseMvc();
    }
}