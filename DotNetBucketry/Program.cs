using Amazon.S3;
using DotNetBucketry;
using DotNetBucketry.Core.Interfaces;
using DotNetBucketry.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();

builder.WebHost.ConfigureKestrel(options=>options.Limits.MaxRequestBodySize = null);
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(options =>
{
    // add a custom operation filter which sets default values
    options.OperationFilter<SwaggerDefaultValues>();
    var fileName = typeof(Program).Assembly.GetName().Name + ".xml";
    var filePath = Path.Combine(AppContext.BaseDirectory, fileName);

    // integrate xml comments
    options.IncludeXmlComments(filePath);
});
builder.Services.AddAWSService<IAmazonS3>(builder.Configuration.GetAWSOptions());
builder.Services.AddScoped<IBucketRepository, BucketRepository>();
builder.Services.AddScoped<IFilesRepository, FilesRepository>();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c =>
    {
        c.RouteTemplate = "swagger/{documentName}/swagger.json";
    });
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "DotNetBucketry API V1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    });
}
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
app.MapGet("/", () => "Hello World!");

app.Run();
