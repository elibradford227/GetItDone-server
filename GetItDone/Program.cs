using System.Security.Claims;
using GetItDone.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using GetItDone.models;
using Microsoft.EntityFrameworkCore;
using GetItDone.services;
using GetItDone.Infrastructure;
using GetItDone.repositories;
using AutoMapper;
using GetItDone.Utils;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserTaskRepository, UserTaskRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();

builder.Services.AddAuthentication().AddCookie(IdentityConstants.ApplicationScheme, options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; 
    options.Cookie.SameSite = SameSiteMode.None; 
    options.Cookie.Path = "/";
})
    .AddBearerToken(IdentityConstants.BearerScheme);

builder.Services.AddIdentityCore<User>()
    .AddEntityFrameworkStores<GetItDoneDbContext>()
    .AddApiEndpoints();

builder.Services.AddAuthorization(options =>
{
    var policy = new AuthorizationPolicyBuilder(IdentityConstants.ApplicationScheme, IdentityConstants.BearerScheme)
    .RequireAuthenticatedUser()
    .Build();

    options.DefaultPolicy = policy;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost3000",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000") 
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials(); 
        });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));


builder.Services.AddDbContext<GetItDoneDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("GetItDoneDbConnectionString")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<GetItDoneDbContext>();
    var retries = 5;
    while (retries > 0)
    {
        try
        {
            db.Database.Migrate();
            break;
        }
        catch (Npgsql.NpgsqlException)
        {
            retries--;
            Thread.Sleep(2000); // wait 2 seconds before retry
        }
    }
}


// experimenting with preloading queries to reduce initial response time after app startup

using (IServiceScope scope = app.Services.CreateScope())
{
    ITaskRepository taskRepo = scope.ServiceProvider.GetRequiredService<ITaskRepository>();
    await taskRepo.GetBaseTaskQuery().FirstOrDefaultAsync();
}

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseSwagger();
app.UseSwaggerUI();


app.UseCors("AllowLocalhost3000");

app.Urls.Add("http://0.0.0.0:8080");
app.Urls.Add("https://0.0.0.0:8081");

app.UseHttpsRedirection();

app.MapIdentityApi<User>();

app.UseAuthentication();

app.UseAuthorization();

app.UseExceptionHandler();

app.MapControllers();

app.Run();
