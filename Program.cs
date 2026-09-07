using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using TawakalApi.app.Data;
using TawakalApi.app.Extensions;

var builder = WebApplication.CreateBuilder(args);

// 1. Add native .NET OpenAPI support to the container.
builder.Services.AddOpenApi();

builder.Services.AddOpenApi("partner", options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        // Create a new collection for paths you want to expose to partners
        var filteredPaths = new OpenApiPaths();

        foreach (var path in document.Paths)
        {
            // Only expose routes starting with /partner
            if (path.Key.StartsWith("/partner", StringComparison.OrdinalIgnoreCase))
            {
                filteredPaths.Add(path.Key, path.Value);
            }
        }

        // Replace the document paths with only the filtered ones
        document.Paths = filteredPaths;

        // Customize the title for partner consumers
        document.Info.Title = "Softway Partner API";
        document.Info.Version = "v1";

        // --- Clean up orphaned/empty tags ---
        var activeTags = document.Paths.Values
            .SelectMany(p => p.Operations.Values)
            .SelectMany(op => op.Tags)
            .Select(t => t.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (document.Tags != null)
        {
            var validTags = document.Tags
                .Where(tag => activeTags.Contains(tag.Name))
                .ToList();

            document.Tags.Clear();
            foreach (var tag in validTags)
            {
                document.Tags.Add(tag);
            }
        }

        // --- Prune unreferenced schemas/models & remove ProblemDetails ---
        if (document.Components?.Schemas != null)
        {
            // Explicitly remove ProblemDetails
            document.Components.Schemas.Remove("ProblemDetails");

            // Keep only models that start with your partner-specific DTO prefixes
            var allowedSchemaPrefixes = new[] { "Partner", "Token" }; 

            var keysToRemove = document.Components.Schemas.Keys
                .Where(schemaName => !allowedSchemaPrefixes.Any(prefix => schemaName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            foreach (var key in keysToRemove)
            {
                document.Components.Schemas.Remove(key);
            }
        }

        return Task.CompletedTask;
    });
});

// 2. Register controllers to the container.
builder.Services.AddControllers()
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

// 3. Repository registration
builder.Services.AddRepositoryExtensions();

// 4. Service registration
builder.Services.AddServiceExtensions();

// 5. Register all your JWT scheme using the extension method!
builder.Services.AddJwtAuthentication(builder.Configuration);

// Add authorization with a Fallback policy that requires authentication for all endpoints by default
builder.Services.AddAuthorizationBuilder()
    .SetFallbackPolicy(new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build());

// Add database context service to the container.
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DBConnection")));
builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options =>
{
    options.AddPolicy("PortalCors", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:5174")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// === ADD THIS AUTOMATIC MIGRATION BLOCK HERE ===
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // This applies any pending EF Core migrations automatically on startup
    dbContext.Database.Migrate();
}
// ===============================================

app.UseCors("PortalCors");
app.MapControllers();

// Configure the HTTP request pipeline.
app.MapOpenApi().AllowAnonymous();

// Map Scalar specifically to your partner docs route with configuration options
app.MapScalarApiReference("/partner/docs", options =>
{
    options
        .WithTitle("Softway Partner API Documentation")
        .AddDocument("partner", "Partner API")
        .HideDeveloperTools()
        .WithTheme(ScalarTheme.BluePlanet);
})
.AllowAnonymous();

app.UseAuthentication();
app.UseAuthorization();
app.Run();