using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MoveiAppApi.Services.classes;
using MoveiAppApi.Services.Interfaces;
using MovieApp.Data;
using MovieApp.Infrastructure.Interface;
using MovieApp.Infrastructure.Repository;
using MovieApp.Models;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.FileProviders;
using Quartz;
using MovieApp.QuartzServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c=>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    { 
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IRatingRepository, RatingRepository>();
builder.Services.AddScoped<IEmailRepository, EmailRepository>();
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<IRatingService, RatingService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
/*
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
        .AddEntityFrameworkStores<ApplicationDbContext>();*/
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    });
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme= JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    string jwtValidAudience = builder.Configuration["JWT:ValidAudience"];
    string jwtValidIssuer = builder.Configuration["JWT:ValidIssuer"];
    string jwtSecret = builder.Configuration["JWT:Secret"];
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtValidIssuer,
        ValidAudience = jwtValidAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy =>
        policy.RequireRole("Admin"));
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
        });
});

//notification scheduler
builder.Services.AddSignalR();

builder.Services.AddQuartz(q =>
{
    var conconcurrentJobKey = new JobKey("ConconcurrentJob");
    q.AddJob<ConconcurrentJob>(opts => opts.WithIdentity(conconcurrentJobKey));
    q.AddTrigger(opts => opts
        .ForJob(conconcurrentJobKey)
        .WithIdentity("ConconcurrentJob-trigger")
        .StartNow()
        .WithSimpleSchedule(x => x
            .WithIntervalInHours(24)
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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(Directory.GetCurrentDirectory(), "..","MovieApp", "wwwroot")),
    RequestPath = "/MovieApp"
});
app.UseAuthentication();
app.UseAuthorization();
app.UseCors();
app.MapHub<JobsHub>("/jobshub");

app.MapControllers();

app.Run();
