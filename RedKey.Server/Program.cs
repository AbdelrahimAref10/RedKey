using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using NSwag;
using NSwag.Generation.Processors.Security;
using RedKey.Application.Contracts.Identity;
using RedKey.Application.Contracts.Localization;
using RedKey.Application.DependencyInjection;
using RedKey.Infrastructure.DependencyInjection;
using RedKey.Infrastructure.Notifications;
using RedKey.Infrastructure.Storage;
using RedKey.Persistence.Seed;
using RedKey.Server.Middlewares;
using RedKey.Server.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// True while NSwag boots the app on build to generate the Angular client (see RedKey.Server.csproj):
// no database work and no static assets manifest are needed then.
var isGeneratingApiClient = builder.Configuration.GetValue<bool>("GENERATING_API_CLIENT");

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<ILanguageContext, RequestLanguageContext>();
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

// Uploaded unit images / type icons (file names are unique, so they are cached forever).
var storage = app.Services.GetRequiredService<IOptions<StorageOptions>>().Value;
Directory.CreateDirectory(storage.RootPath);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(storage.RootPath),
    RequestPath = storage.RequestPath,
    OnPrepareResponse = context =>
    {
        var headers = context.Context.Response.Headers;
        headers.CacheControl = "public, max-age=31536000, immutable";
        headers.XContentTypeOptions = "nosniff";
        // SVG icons are user uploads: never let them run anything.
        headers.ContentSecurityPolicy = "default-src 'none'; img-src 'self'; style-src 'unsafe-inline'; sandbox";
    }
});

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
app.MapHub<NotificationsHub>(NotificationsHub.Route);
app.MapFallbackToFile("/index.html");

app.Run();
