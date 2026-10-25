using Microsoft.AspNetCore.ResponseCompression;
using NSwag;
using NSwag.Generation.Processors.Security;
using RedKey.Application.DependencyInjection;
using RedKey.Infrastructure.DependencyInjection;
using RedKey.Persistence.Seed;
using RedKey.Server.Middlewares;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// True while NSwag boots the app on build to generate the Angular client (see RedKey.Server.csproj):
// no database work and no static assets manifest are needed then.
var isGeneratingApiClient = builder.Configuration.GetValue<bool>("GENERATING_API_CLIENT");

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.Title = "RedKey API";
    config.Version = "v1";

    config.AddSecurity("JWT", new OpenApiSecurityScheme
    {
        Type = OpenApiSecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Paste the access token returned by /api/auth/login."
    });
    config.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
});

var app = builder.Build();

if (!isGeneratingApiClient)
    await app.Services.InitialiseDatabaseAsync(applyMigrations: app.Environment.IsDevelopment());

app.UseExceptionHandler();
app.UseResponseCompression();

app.UseDefaultFiles();
if (!isGeneratingApiClient)
    app.MapStaticAssets();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(settings => settings.PersistAuthorization = true);
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("/index.html");

app.Run();
