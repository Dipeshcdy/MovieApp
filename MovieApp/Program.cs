using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using MovieApp.Data;
using MovieApp.Infrastructure.Interface;
using MovieApp.Infrastructure.Repository;
using MovieApp.Models;
using MovieApp.QuartzServices;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IMovieRepository,MovieRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IRatingRepository, RatingRepository>();
builder.Services.AddScoped<IEmailRepository, EmailRepository>();
builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

/*builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();*/
/*builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
        .AddEntityFrameworkStores<ApplicationDbContext>();*/
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


//notification scheduler
builder.Services.AddSignalR();

builder.Services.AddQuartz(q =>
{
    var conconcurrentJobKey = new JobKey("ConconcurrentJob");
    q.AddJob<ConconcurrentJob>(opts => opts.WithIdentity(conconcurrentJobKey));
    q.AddTrigger(opts => opts
        .ForJob(conconcurrentJobKey)
        .WithIdentity("ConconcurrentJob-trigger")
        .WithSimpleSchedule(x => x
            .WithIntervalInSeconds(5)
            .RepeatForever()));

    /* var nonConconcurrentJobKey = new JobKey("NonConconcurrentJob");
     q.AddJob<NonConconcurrentJob>(opts => opts.WithIdentity(nonConconcurrentJobKey));
     q.AddTrigger(opts => opts
         .ForJob(nonConconcurrentJobKey)
         .WithIdentity("NonConconcurrentJob-trigger")
         .WithSimpleSchedule(x => x
             .WithIntervalInSeconds(5)
             .RepeatForever()));
 */
});

builder.Services.AddQuartzHostedService(
    q => q.WaitForJobsToComplete = true);

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

app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<JobsHub>("/jobshub");
});

app.MapControllerRoute(
    name: "default",
    pattern: "{Area=User}/{controller=Home}/{action=Index}/{id?}");

app.Run();
