using DepartmentalSystemAPI.Data;
using DepartmentalSystemAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel for HTTP and HTTPS
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5088); // HTTP
    options.ListenLocalhost(7089, listenOptions =>
    {
        listenOptions.UseHttps(); // HTTPS
    });
});

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Departmental System API",
        Version = "v1",
        Description = "API for departmental management with Windows Authentication"
    });

    // Add JWT Bearer Authentication
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

    // Add Windows Authentication support
    c.AddSecurityDefinition("Windows", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "Negotiate",
        Description = "Windows Authentication"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Windows"
                }
            },
            new string[] {}
        }
    });
});

// Database
builder.Services.AddDbContext<DepartmentContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Services
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ISkillService, SkillService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAdService, AdService>();

// ADD EMAIL SERVICES HERE
builder.Services.AddScoped<IEmailService, EmailService>();


// HTTP Context Accessor for Auth
builder.Services.AddHttpContextAccessor();

// JWT Authentication
var jwtSecret = builder.Configuration["Jwt:Secret"]
    ?? throw new ArgumentNullException("Jwt:Secret is required");
var key = Encoding.ASCII.GetBytes(jwtSecret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
})
.AddNegotiate(); // Windows Authentication

// Authorization
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme, NegotiateDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser()
        .Build();
});

// Enhanced CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSwaggerAndReact", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5088",
                "https://localhost:7089",
                "http://localhost:3000",
                "https://localhost:3000"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Departmental System API v1");
        c.RoutePrefix = "swagger"; // This makes Swagger available at /swagger
        c.OAuthClientId("swagger-ui");
        c.OAuthAppName("Departmental System API - Swagger");
        c.OAuthUsePkce();
    });
}

app.UseCors("AllowSwaggerAndReact");

app.UseHttpsRedirection();
app.UseRouting();

// Authentication & Authorization MUST be in this order
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();