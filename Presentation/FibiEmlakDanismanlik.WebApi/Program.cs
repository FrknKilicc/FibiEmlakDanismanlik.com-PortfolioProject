using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.BlogInterfaces;
using FibiEmlakDanismanlik.Application.Interfaces.ListingTypeInterfaces;
using FibiEmlakDanismanlik.Application.Interfaces.LocationInterfaces;
using FibiEmlakDanismanlik.Application.Interfaces.MapInterfaces;
using FibiEmlakDanismanlik.Application.Interfaces.NearbyInterfaces;
using FibiEmlakDanismanlik.Application.Interfaces.PropertyInterfaces;
using FibiEmlakDanismanlik.Application.Helpers;
using FibiEmlakDanismanlik.Application.Services;
using FibiEmlakDanismanlik.Persistence.Context;
using FibiEmlakDanismanlik.Persistence.Repositories;
using FibiEmlakDanismanlik.Persistence.Repositories.BlogRepositories;
using FibiEmlakDanismanlik.Persistence.Repositories.ListingTypeRepositories;
using FibiEmlakDanismanlik.Persistence.Repositories.LocationRepositories;
using FibiEmlakDanismanlik.Persistence.Repositories.MapRepositories;
using FibiEmlakDanismanlik.Persistence.Repositories.NearbyRepositories;
using FibiEmlakDanismanlik.Persistence.Repositories.PropertyRepositories;
using FibiEmlakDanismanlik.Application.Interfaces.CityGalleryInterfaces;
using FibiEmlakDanismanlik.Persistence.Repositories.CityGalleryRepositories;
using FibiEmlakDanismanlik.Application.Interfaces.AgentInterfaces;
using FibiEmlakDanismanlik.Persistence.Repositories.AgentRepositories;
using FibiEmlakDanismanlik.Application.Interfaces.BestDealInterfaces;
using FibiEmlakDanismanlik.Persistence.Repositories.BestDealRepositories;
using FibiEmlakDanismanlik.Application.Interfaces.PopularPlaceInterfaces;
using FibiEmlakDanismanlik.Persistence.Repositories.PopularPlaceRepositories;
using FibiEmlakDanismanlik.Application.Interfaces.FibiServiceInterfaces;
using FibiEmlakDanismanlik.Persistence.Repositories.FibiServiceRepositories;
using FibiEmlakDanismanlik.Application.Interfaces.AuditLogInterfaces;
using FibiEmlakDanismanlik.Persistence.Repositories.AuditLogRepositories;
using FibiEmlakDanismanlik.Application.Interfaces.ListingImageInterfaces;
using FibiEmlakDanismanlik.Persistence.Repositories.ListingImageRepositories;
using FibiEmlakDanismanlik.Application.Interfaces.AdminListingInterfaces;
using FibiEmlakDanismanlik.Persistence.Repositories.AdminListingRepositories;
using FibiEmlakDanismanlik.Application.Interfaces.LeadInterfaces;
using FibiEmlakDanismanlik.Persistence.Repositories.LeadRepositories;
using FibiEmlakDanismanlik.Application.Interfaces.ListingAlertInterfaces;
using FibiEmlakDanismanlik.Persistence.Repositories.ListingAlertRepositories;
using FibiEmlakDanismanlik.Application.Interfaces.ListingDealInterfaces;
using FibiEmlakDanismanlik.Persistence.Repositories.ListingDealRepositories;
using FibiEmlakDanismanlik.Persistence.Interceptors;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.HttpOverrides;
using System.Net;
using System.Security.Claims;
using System.Threading.RateLimiting;
using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Infrastructure;
using FibiEmlakDanismanlik.WebApi.ExceptionHandlers;
using FibiEmlakDanismanlik.WebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<AuditSaveChangesInterceptor>();
builder.Services.AddDbContext<FibiEmlakDanismanlikContext>((sp, options) => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")).AddInterceptors(sp.GetRequiredService<AuditSaveChangesInterceptor>()));

// Allow Frontend Request
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost",
        policy =>
        {
            policy.WithOrigins("https://localhost:7012", "http://localhost:7012", "http://localhost:5146", "http://localhost:5175")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Add services to the container.

builder.Services.AddScoped<FibiEmlakDanismanlikContext>();


builder.Services.AddControllers();
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddProblemDetails();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "api/Auth/login ile alınan token'ı girin."
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });
});

// JWT: imzalama anahtarı (Jwt:Key) User Secrets'ta, Issuer/Audience appsettings'te
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(JwtTokenGenerator.GetSigningKey(builder.Configuration)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var nameIdClaim = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(nameIdClaim, out var agentId))
                {
                    context.Fail("Oturum geçersiz.");
                    return;
                }

                var cache = context.HttpContext.RequestServices.GetRequiredService<IMemoryCache>();
                var repository = context.HttpContext.RequestServices.GetRequiredService<IAgentRepository>();

                var cacheKey = AuthConstants.AuthStateCacheKey(agentId);
                var authState = await cache.GetOrCreateAsync(cacheKey, async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(AuthConstants.AuthStateCacheSeconds);
                    return await repository.GetAuthStateAsync(agentId);
                });

                var tokenSecurityStamp = context.Principal?.FindFirst(AuthConstants.SecurityStampClaimType)?.Value;
                var tokenRole = context.Principal?.FindFirst(ClaimTypes.Role)?.Value;

                if (authState == null || !authState.IsActive || tokenSecurityStamp != authState.SecurityStamp || tokenRole != authState.Role.ToString())
                {
                    context.Fail("Oturum geçersiz.");
                }
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    var staffPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireRole(AuthConstants.RoleOwner, AuthConstants.RoleManager, AuthConstants.RoleConsultant)
        .Build();

    options.AddPolicy(AuthConstants.PolicyStaff, staffPolicy);
    options.AddPolicy(AuthConstants.PolicyManagerOrAbove, policy =>
        policy.RequireAuthenticatedUser().RequireRole(AuthConstants.RoleOwner, AuthConstants.RoleManager));
    options.AddPolicy(AuthConstants.PolicyOwnerOnly, policy =>
        policy.RequireAuthenticatedUser().RequireRole(AuthConstants.RoleOwner));

    options.DefaultPolicy = staffPolicy;
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddSingleton<IAuthStateCache, AuthStateCache>();

// Data Protection anahtarları kalıcı dosya sisteminde saklanır; anahtarlar kaybolursa kayıtlı 2FA anahtarları çözülemez, sunucuda yedeklenmelidir.
var dataProtection = builder.Services.AddDataProtection()
    .SetApplicationName("FibiEmlakDanismanlik.WebApi")
    .PersistKeysToFileSystem(new DirectoryInfo(builder.Configuration["DataProtection:KeysPath"] ?? Path.Combine(builder.Environment.ContentRootPath, "App_Data", "dp-keys")));
if (OperatingSystem.IsWindows())
    dataProtection.ProtectKeysWithDpapi(protectToLocalMachine: true);

//Configure IRepositories 
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped(typeof(IPropertyRepository), typeof(PropertyRepository));
builder.Services.AddScoped(typeof(IBlogRepository), typeof(BlogRepository));
builder.Services.AddScoped(typeof(ILocationRepository), typeof(LocationRepository));
builder.Services.AddScoped(typeof(INearbyRepository), typeof(NearbyRepository));
builder.Services.AddScoped(typeof(IMapRepository), typeof(MapRepository));
builder.Services.AddScoped(typeof(ICityGalleryRepository), typeof(CityGalleryRepository));
builder.Services.AddScoped(typeof(IListingTypeRepository), typeof(ListingTypeRepository));
builder.Services.AddScoped(typeof(IAgentRepository), typeof(AgentRepository));
builder.Services.AddScoped(typeof(IBestDealRepository), typeof(BestDealRepository));
builder.Services.AddScoped(typeof(IPopularPlaceRepository), typeof(PopularPlaceRepository));
builder.Services.AddScoped(typeof(IMarketStatRepository), typeof(MarketStatRepository));
builder.Services.AddScoped(typeof(IMarketDataSyncRepository), typeof(MarketDataSyncRepository));
builder.Services.AddScoped(typeof(IAuditLogRepository), typeof(AuditLogRepository));
builder.Services.AddScoped(typeof(IListingImageRepository), typeof(ListingImageRepository));
builder.Services.AddScoped(typeof(IAdminListingRepository), typeof(AdminListingRepository));
builder.Services.AddScoped(typeof(ILeadRepository), typeof(LeadRepository));
builder.Services.AddScoped(typeof(IListingAlertRepository), typeof(ListingAlertRepository));
builder.Services.AddScoped(typeof(IListingDealRepository), typeof(ListingDealRepository));
//Configure IRepositories

//MediaTR tried
builder.Services.AddApplicationService(builder.Configuration);
builder.Services.AddFibiInfrastructure(builder.Configuration);
//MediaTR 

// Reverse proxy arkasında rate limit'in gerçek istemci IP'sini görmesi için.
// Yalnızca güvenilen proxy'lerden gelen X-Forwarded-For kabul edilir (varsayılan: loopback);
// herkese güvenmek IP sahteciliğiyle rate limit'in aşılmasına izin verir.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    foreach (var proxy in builder.Configuration.GetSection("ForwardedHeaders:KnownProxies").Get<string[]>() ?? Array.Empty<string>())
    {
        if (IPAddress.TryParse(proxy, out var address))
            options.KnownProxies.Add(address);
    }
});

// Anonim lead ucunu spam'e karşı IP başına sınırlar.
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy(FibiServiceConstants.LeadRateLimitPolicy, httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = FibiServiceConstants.LeadRateLimitPermitCount,
                Window = TimeSpan.FromMinutes(FibiServiceConstants.LeadRateLimitWindowMinutes),
                QueueLimit = 0
            }));
    options.AddPolicy(CustomerContactConstants.RateLimitPolicy, httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = CustomerContactConstants.RateLimitPermitCount,
                Window = TimeSpan.FromMinutes(CustomerContactConstants.RateLimitWindowMinutes),
                QueueLimit = 0
            }));
    options.AddPolicy(ListingAlertConstants.RateLimitPolicy, httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = ListingAlertConstants.RateLimitPermitCount,
                Window = TimeSpan.FromMinutes(ListingAlertConstants.RateLimitWindowMinutes),
                QueueLimit = 0
            }));
    options.AddPolicy(AuthConstants.AuthRateLimitPolicy, httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = AuthConstants.AuthRateLimitPermitCount,
                Window = TimeSpan.FromMinutes(AuthConstants.AuthRateLimitWindowMinutes),
                QueueLimit = 0
            }));
});

var app = builder.Build();

app.UseForwardedHeaders();
app.UseExceptionHandler();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var uploadsRoot = app.Configuration["FileStorage:RootPath"];
if (string.IsNullOrWhiteSpace(uploadsRoot))
{
    uploadsRoot = Path.Combine(app.Environment.ContentRootPath, "uploads");
}
if (!Directory.Exists(uploadsRoot))
{
    Directory.CreateDirectory(uploadsRoot);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsRoot),
    RequestPath = ListingImageConstants.UploadsRequestPath,
    ServeUnknownFileTypes = false,
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers["Cache-Control"] = "public,max-age=31536000,immutable";
        ctx.Context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    }
});

app.UseCors("AllowLocalhost");

app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.MapControllers();

app.Run();
