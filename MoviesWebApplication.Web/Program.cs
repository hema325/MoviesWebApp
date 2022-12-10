using Microsoft.AspNetCore.Identity;
using MoviesWebApplication.DAL;
using MoviesWebApplication.DAL.Data;
using MoviesWebApplication.DAL.DataBaseManagement.DataBaseBuilder;
using MoviesWebApplication.DAL.DataBaseManagement.DataBaseInitializer;
using MoviesWebApplication.DAL.DataReposiotry;
using MoviesWebApplication.DAL.IDataRepository;
using MoviesWebApplication.Web.Constrains;
using MoviesWebApplication.Web.DALOptions;
using MoviesWebApplication.Web.Services.Email;
using MoviesWebApplication.Web.WebOptions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.Configure<ConnectionStringsOption>(builder.Configuration.GetSection(ConnectionStringsOption.ConnectionStrings));
builder.Services.Configure<SMTPConfigurationOption>(builder.Configuration.GetSection(SMTPConfigurationOption.SMTPConfiguration));
builder.Services.Configure<AccountOption>(builder.Configuration.GetSection(AccountOption.DefaultAccount));
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddSingleton<IDataBaseBuilder, DataBaseBuilder>();
builder.Services.AddSingleton<IUnitOfWork, UnitOfWork>();
builder.Services.AddSingleton<IDataInitializer, DataInitializer>();
//builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddAuthentication(options => 
{ 
    options.DefaultScheme = _Scheme.Default;
    //options.DefaultSignInScheme= _Scheme.External;
})
.AddCookie(_Scheme.Default, options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
})
.AddCookie(_Scheme.External)
.AddFacebook(options =>
{
    IConfigurationSection facebook = builder.Configuration.GetSection("Authentication:Facebook");
    options.AppId = facebook["AppId"];
    options.AppSecret = facebook["AppSecret"];
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});
builder.Services.AddAuthorization();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );

    endpoints.MapControllerRoute(
      name: "default",
      pattern: "{controller=Home}/{action=Index}/{id?}");


});

await BuildAndInitializeDataBase();

app.Run();

async Task BuildAndInitializeDataBase()
{
    using(var scope = app.Services.CreateScope())
    {
         await scope.ServiceProvider.GetRequiredService<IDataBaseBuilder>().Build();
         await scope.ServiceProvider.GetRequiredService<IDataInitializer>().Initialize();
    }
}