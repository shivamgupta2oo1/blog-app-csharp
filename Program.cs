using Bloggie.Web.Data;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure the BloggieDbContext to use PostgreSQL
builder.Services.AddDbContext<BloggieDbContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("BloggieDbConnectionString")));

// Configure the AuthDbContext to use PostgreSQL
builder.Services.AddDbContext<AuthDbContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("BloggieAuthDbConnectionString")));

// Configure Identity with default options and entity framework stores
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddDefaultTokenProviders();

// Configure custom repositories
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IBlogPostRepository, BlogPostRepository>();
builder.Services.AddScoped<IImageRepository, CloudinaryImageRepository>();
builder.Services.AddScoped<IBlogPostLikeRepository, BlogPostLikeRepository>(); 
builder.Services.AddScoped<IBlogPostCommentRepository, BlogPostCommentRepository>(); 
builder.Services.AddScoped<IUserRepository, UserRepository>(); 

// Configure Identity options
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{urlHandler?}");

app.Run();
