using Amazon.S3;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;

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
       app.UseExceptionHandler(a=>a.Run(async context =>
       {
           var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>(); 
           var exception = exceptionHandlerPathFeature?.Error;
           var result = JsonConvert.SerializeObject(new { error = exception?.Message });
           context.Response.ContentType = "application/json";
           await context.Response.WriteAsync(result);
       }));
       app.UseMvc();
    }
}