using Amazon.S3;

namespace DotNetBucketry;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public class Startup
{
    private readonly IConfiguration _configuration;
    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // This method configures the services for the application
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAWSService<IAmazonS3>(_configuration.GetAWSOptions());
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