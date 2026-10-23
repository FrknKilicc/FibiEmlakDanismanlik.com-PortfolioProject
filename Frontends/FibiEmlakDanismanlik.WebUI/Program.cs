using FibiEmlakDanismanlik.WebUI.Models;
using FibiEmlakDanismanlik.WebUI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<FormSpamGuard>();
builder.Services.AddSingleton<ListingLocationResolver>();
// İlan detay adresindeki kategori segmenti; il/ilçe açılış sayfalarıyla çakışmayı önler
builder.Services.Configure<RouteOptions>(o => o.ConstraintMap["listingcategory"] = typeof(ListingCategoryRouteConstraint));

// IConfiguration aracılığıyla AppSettings.json'daki yapılandırmayı yükleme


var app = builder.Build();


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

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=UILayout}/{action=Index}/{id?}");

app.Run();
