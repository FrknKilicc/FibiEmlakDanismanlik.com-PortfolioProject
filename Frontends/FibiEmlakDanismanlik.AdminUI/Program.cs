using System.Globalization;
using System.Net;
using System.Security.Claims;
using FibiEmlakDanismanlik.AdminUI.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Otomatik Antiforgery doğrulamalı MVC Controller ve View kayıtları
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
    // Tarayıcıdan nokta ayraçlı gelen ondalıklı sayıların doğru ayrıştırılması için
    options.ModelBinderProviders.Insert(0, new InvariantDecimalModelBinderProvider());
});

// Antiforgery Cookie Güvenlik Ayarları
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name = "__Host-fibi-af";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

// Data Protection (Anahtar Yönetimi)
var dpKeysPath = builder.Configuration["DataProtection:KeysPath"];
if (string.IsNullOrWhiteSpace(dpKeysPath))
{
    dpKeysPath = Path.Combine(builder.Environment.ContentRootPath, "App_Data", "dp-keys");
}

var dpBuilder = builder.Services.AddDataProtection()
    .SetApplicationName("FibiEmlakDanismanlik.AdminUI")
    .PersistKeysToFileSystem(new DirectoryInfo(dpKeysPath));

if (OperatingSystem.IsWindows())
{
    dpBuilder.ProtectKeysWithDpapi(protectToLocalMachine: true);
}

// Oturum Zaman Aşımı Değerleri
var idleTimeoutMinutes = builder.Configuration.GetValue<int?>("AdminSession:IdleTimeoutMinutes") ?? 30;
var absoluteTimeoutHours = builder.Configuration.GetValue<int?>("AdminSession:AbsoluteTimeoutHours") ?? 8;

// Cookie Kimlik Doğrulama
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "__Host-fibi-admin";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.LoginPath = "/giris";
        options.LogoutPath = "/cikis";
        options.AccessDeniedPath = "/yetkisiz";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(idleTimeoutMinutes);

        options.Events.OnValidatePrincipal = async context =>
        {
            var user = context.Principal;
            if (user == null)
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return;
            }

            // issued_at_utc mutlak zaman aşımı kontrolü
            var issuedAtClaim = user.FindFirst(AdminClaimTypes.IssuedAtUtc)?.Value;
            if (DateTimeOffset.TryParse(issuedAtClaim, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var issuedAtUtc))
            {
                if (DateTimeOffset.UtcNow - issuedAtUtc > TimeSpan.FromHours(absoluteTimeoutHours))
                {
                    context.RejectPrincipal();
                    await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    return;
                }
            }
            else
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return;
            }

            // access_token_expires_utc token geçerlilik kontrolü
            var expiresClaim = user.FindFirst(AdminClaimTypes.AccessTokenExpiresUtc)?.Value;
            if (DateTimeOffset.TryParse(expiresClaim, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var expiresUtc))
            {
                if (DateTimeOffset.UtcNow >= expiresUtc)
                {
                    context.RejectPrincipal();
                    await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    return;
                }
            }
            else
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return;
            }
        };
    });

// Yetkilendirme Politikaları
builder.Services.AddAuthorization(options =>
{
    // Varsayılan olarak tüm sayfalar kimlik doğrulaması gerektirir
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    options.AddPolicy("ManagerOrAbove", policy =>
        policy.RequireRole("Owner", "Manager"));

    options.AddPolicy("OwnerOnly", policy =>
        policy.RequireRole("Owner"));
});

// Reverse Proxy Arkası (Forwarded Headers) Ayarları
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    var knownProxies = builder.Configuration.GetSection("ForwardedHeaders:KnownProxies").Get<string[]>();
    if (knownProxies != null)
    {
        foreach (var proxy in knownProxies)
        {
            if (IPAddress.TryParse(proxy, out var ip))
            {
                options.KnownProxies.Add(ip);
            }
        }
    }
});

// Servis Kayıtları
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<CurrentAdmin>();
builder.Services.AddSingleton<LoginChallengeStore>();
builder.Services.AddSingleton<SiteImageUrl>();
builder.Services.AddScoped<AdminSignInService>();

// WebApi HTTP İstemcisi
var apiUrl = builder.Configuration["Url:ApiUrl"];
builder.Services.AddHttpClient("api", client =>
{
    if (!string.IsNullOrWhiteSpace(apiUrl))
    {
        client.BaseAddress = new Uri(apiUrl);
    }
    client.Timeout = TimeSpan.FromSeconds(30);
});
builder.Services.AddScoped<ApiClient>();

var app = builder.Build();

// HTTP Pipeline Sıralaması
app.UseForwardedHeaders();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/hata");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/hata/{0}");

app.UseMiddleware<SecurityHeadersMiddleware>();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.Run();
