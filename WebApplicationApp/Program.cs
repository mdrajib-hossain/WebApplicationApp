using Microsoft.Extensions.Options;
using System.ComponentModel.Design;
using WebApplicationApp.Date;
using WebApplicationApp.Repositories.SelesManService;

using Microsoft.EntityFrameworkCore;
using DinkToPdf.Contracts;
using DinkToPdf;
using Rotativa.AspNetCore;


var builder = WebApplication.CreateBuilder(args);



// Add this near the top of Program.cs
System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);



// Add services to the container.
builder.Services.AddControllersWithViews();



// Inject DbContext with SQL Server configuration


builder.Services.AddDbContext<SqlDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EmpManagementDBConnection")));



// Register Service Dependency

builder.Services.AddScoped<ISalesManServices, SalesManServices>();


var app = builder.Build();



RotativaConfiguration.Setup(app.Environment.WebRootPath, "Rotativa");



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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
