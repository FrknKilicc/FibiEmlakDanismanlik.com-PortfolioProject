using System.Net;
using Microsoft.AspNetCore.HttpOverrides;
using FibiEmlakDanismanlik.WebUI.Models;
using FibiEmlakDanismanlik.WebUI.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration.ReadFrom.Configuration(context.Configuration).ReadFrom.Services(services).Enrich.FromLogContext());

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHealthChecks();
builder.Services.AddHttpClient();
// API'ye ulaşılamazsa istisna yerine 503 yanıtı döner; sayfalar boş içerikle açılır
builder.Services.AddTransient<ApiUnavailableHandler>();
builder.Services.ConfigureHttpClientDefaults(http => http.AddHttpMessageHandler<ApiUnavailableHandler>());
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<FormSpamGuard>();
builder.Services.AddSingleton<ListingLocationResolver>();
builder.Services.AddSingleton<HtmlContentSanitizer>();
builder.Services.AddSingleton<SiteTextService>();
// İlan detay adresindeki kategori segmenti; il/ilçe açılış sayfalarıyla çakışmayı önler
builder.Services.Configure<RouteOptions>(o => o.ConstraintMap["listingcategory"] = typeof(ListingCategoryRouteConstraint));

// IConfiguration aracılığıyla AppSettings.json'daki yapılandırmayı yükleme


// Reverse proxy arkasında form sınırının gerçek ziyaretçi IP'sini görmesi için.
// Yalnızca güvenilen proxy'lerden gelen X-Forwarded-For kabul edilir (varsayılan: loopback).
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    foreach (var proxy in builder.Configuration.GetSection("ForwardedHeaders:KnownProxies").Get<string[]>() ?? Array.Empty<string>())
    {
        if (IPAddress.TryParse(proxy, out var address))
            options.KnownProxies.Add(address);
    }
});

var app = builder.Build();

app.UseForwardedHeaders();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// 404 vb. durumlar Türkçe sayfayla, durum kodu korunarak gösterilir
app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSerilogRequestLogging();

app.UseRouting();

app.UseAuthorization();

app.MapHealthChecks("/health");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=UILayout}/{action=Index}/{id?}");

app.Run();
