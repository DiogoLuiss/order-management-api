using OrderManagementApi.Middlewares;
using OrderManagementApi.Service;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
builder.Services.AddSingleton<IValidationService, ValidationService>();
builder.Services.AddControllers(options =>
{
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    options.AllowEmptyInputInBodyModelBinding = true;
});
builder.Services.AddValidators();
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IDbConnection>(sp =>
    new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddRepositories(Assembly.GetExecutingAssembly());

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

#region Cors

app.UseCors(builder =>
{

    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
});

#endregion
app.UseErrorHandlingMiddleware();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
