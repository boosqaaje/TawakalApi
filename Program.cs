using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using TawakalApi.app.Data;
using TawakalApi.app.Extensions;

var builder = WebApplication.CreateBuilder(args);

// 1. Add native .NET 10 OpenAPI support to the container.
builder.Services.AddOpenApi();

// 2. Register controllers to the container.
builder.Services.AddControllers()
.AddJsonOptions(options =>
{
    // This tells ASP.NET Core to skip any property whose value is null
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

// 3. Repository registration
builder.Services.AddRepositoryExtensions();

// 4. Service registration
builder.Services.AddServiceExtensions();

// 5. Register all your JWT scheme using the extension method!
builder.Services.AddJwtAuthentication(builder.Configuration);

// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
// .AddJwtBearer(options =>
// {
//     options.TokenValidationParameters = new TokenValidationParameters
//     {
//         ValidateIssuer = true,
//         ValidateAudience = true,
//         ValidateLifetime = true,
//         ValidateIssuerSigningKey = true,
//         ValidIssuer = builder.Configuration["Jwt:Issuer"],
//         ValidAudience = builder.Configuration["Jwt:Audience"],
//         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
//     };
// });

// Add authorization with a Fallback policy that requires authentication for all endpoints by default
builder.Services.AddAuthorizationBuilder()
    .SetFallbackPolicy(new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build());

// Add database context service to the container.
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DBConnection")));
builder.Services.AddHttpContextAccessor();

var app = builder.Build();
app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().AllowAnonymous();
    app.MapScalarApiReference().AllowAnonymous();
}


app.UseAuthentication();
app.UseAuthorization();
app.Run();

