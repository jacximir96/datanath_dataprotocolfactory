using application.Interfaces;
using application.Services;
using dataprotocolfactory.workers;
using domain.Entities;
using domain.Interfaces;
using domain.Repositories;
using infrastructure.Adapter;
using infrastructure.Factory;
using infrastructure.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/app-log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = Host.CreateApplicationBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(Log.Logger);
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<NatsWorker>();
builder.Services.AddMemoryCache();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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
var app = builder.Build();
await app.RunAsync();

