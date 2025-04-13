using Amazon.S3;
using DotNetBucketry.Core.Interfaces;
using DotNetBucketry.Infrastructure.Repositories;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;
using Serilog;
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
    public void ConfigureServices(WebApplicationBuilder builder, Serilog.ILogger logger)
    {
        builder.Services.AddSwaggerGen(options =>
        {
            // add a custom operation filter which sets default values
            options.OperationFilter<SwaggerDefaultValues>();
            var fileName = typeof(Program).Assembly.GetName().Name + ".xml";
            var filePath = Path.Combine(AppContext.BaseDirectory, fileName);

            // integrate xml comments
            options.IncludeXmlComments(filePath);
        });
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
    }
}