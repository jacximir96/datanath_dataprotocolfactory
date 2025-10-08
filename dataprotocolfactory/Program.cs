using application.Interfaces;
using application.Services;
using dataprotocolfactory.workers;
using domain.Entities;
using domain.Interfaces;
using domain.Repositories;
using infrastructure.Adapter;
using infrastructure.Factory;
using infrastructure.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Threading.Tasks;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/app-log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = Host.CreateApplicationBuilder(args);
var auxApp = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(Log.Logger);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<NatsWorker>();
builder.Services.AddMemoryCache();
builder.Services.AddControllers();

builder.Services.AddScoped<IGenericRepository<Requirement>, RequirementRepository>();
builder.Services.AddScoped<IConnectioDataBaseDomain, SqlServerDataBase>();
builder.Services.AddScoped<IConnectioDataBaseDomain, CosmosDataBase>();
builder.Services.AddScoped<IConnectionFactoryDomain, ConnectionFactory>();
builder.Services.AddScoped<IRequirement, RequirementService>();
builder.Services.AddScoped<ITemplateLogDomain, RequirementLog>();
builder.Services.AddScoped<ITestConnection, TestConnection>();
builder.Services.AddScoped<ISendEtl, SendEtlAdapter>();
builder.Services.AddScoped<IEntityRepository<Entity1>, EntityRepository>();
builder.Services.AddScoped<ITransFormRepository<Transform>, TransFormRepository>();
builder.Services.AddScoped<ITargetConfigRepository, TargetConfigRepository>();
builder.Services.AddScoped<ISubRequestRepository<SubRequest>, SubRequestRepository>();
builder.Services.AddScoped<ISubRequestCompleted, SubRequestCompletedService>();
var app = builder.Build();
auxApp.Services.AddEndpointsApiExplorer();
auxApp.Services.AddSwaggerGen();
auxApp.Services.AddControllers();
var appBuilt = auxApp.Build();
// Configure the HTTP request pipeline.
if (auxApp.Environment.IsDevelopment())
{
    appBuilt.UseSwagger();
    appBuilt.UseSwaggerUI();
}
var workerTask=app.RunAsync();
var apiTask = appBuilt.RunAsync();
await Task.WhenAny(workerTask,apiTask);
  

