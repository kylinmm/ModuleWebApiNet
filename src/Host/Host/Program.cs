using Microsoft.Extensions.Hosting.Internal;
using Microsoft.AspNetCore.Hosting;
using CommonModule.DllCommon;
using Host.ServiceExtensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

#region 模块化配置
var HostEnvironment=builder.Services.BuildServiceProvider().GetService<IWebHostEnvironment>();
GlobalConfiguration.WebRootPath = HostEnvironment.WebRootPath;
GlobalConfiguration.ContentRootPath = HostEnvironment.ContentRootPath;

builder.Services.AddModules()
        .AddCustomizedMvc()
        .AddInitModules()
        .AddLocalization();

#endregion 模块化配置

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
