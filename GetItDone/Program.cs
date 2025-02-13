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


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();

builder.Services.AddAuthentication().AddCookie(IdentityConstants.ApplicationScheme)
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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));


builder.Services.AddDbContext<GetItDoneDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("GetItDoneDbConnectionString")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Urls.Add("http://0.0.0.0:8080");
app.Urls.Add("https://0.0.0.0:8081");

app.UseHttpsRedirection();

app.MapIdentityApi<User>();

app.UseAuthentication();

app.UseAuthorization();

app.UseExceptionHandler();

app.MapControllers();

app.Run();
