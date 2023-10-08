using JobManager.Data;
using JobManager.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDbContext>();


builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{

    //options.SignIn.RequireConfirmedAccount = false;
    //options.SignIn.RequireConfirmedPhoneNumber = false;
    // options.User.RequireUniqueEmail = false;
    //options.Password.RequireDigit = false;
    //options.Password.RequiredLength = 8;
    //options.Password.RequiredUniqueChars = 1;
    //options.Password.RequireLowercase = false;
    //options.Password.RequireNonAlphanumeric = false;
    //options.Password.RequireUppercase = false;
}).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders().AddDefaultUI();

builder.Services.AddSingleton<DataProtectionPurposeString>();


builder.Services.AddAuthentication();
//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("AdminRolePolicy", policy => policy.RequireRole("Admin"));
//    //options.AddPolicy("EditRolePolicy", policy => policy.RequireRole("Admin"));
//    //options.AddPolicy("AddStudentPolicy", policy => policy.RequireAssertion(context => (context.User.IsInRole("Staff") && context.User.HasClaim(claim => claim.Type == "Create" && claim.Value == "true"))));
//    //options.AddPolicy("LeavingCertificatePrintPolicy", policy => policy.RequireAssertion(context => (context.User.IsInRole("Leaving") && context.User.HasClaim(claim => claim.Type == "Print" && claim.Value == "true"))));
//    //options.AddPolicy("LeavingCertificatePolicy", policy => policy.RequireRole("Leaving"));
//    //options.AddPolicy("FeesStructurePolicy", policy => policy.RequireRole("FeesStructure"));
//    //options.AddPolicy("RollListAdminPolicy", policy => policy.RequireAssertion(context => (context.User.IsInRole("Staff") && context.User.IsInRole("RollListAdmin"))));
//});
builder.Services.AddTransient<IJobInterface, IJobRepository>();

builder.Services.AddRazorPages();


builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
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
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
