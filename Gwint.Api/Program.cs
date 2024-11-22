using Gwint.Api.Hubs;
using Gwint.Api.Hubs.Filters;

using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR(opt =>
{
    opt.AddFilter<ExceptionFilter>();
});

builder.Services.AddCors(opt => opt.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins("https://localhost:7078")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    }));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseStaticFiles();

app.MapHub<GameHub>("/gameHub");

app.Run();
