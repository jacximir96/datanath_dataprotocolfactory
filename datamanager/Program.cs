using application.Interfaces;
using application.Services;
using dataprotocolfactory.workers;
using domain.Entities;
using domain.Entities.Collections;
using domain.Interfaces;
using domain.Repositories;
using infrastructure.Adapter;
using infrastructure.Factory;
using infrastructure.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NATS.Client.Core;
using Serilog;
using System;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information() // Set minimum log level
     .WriteTo.Console() // Log to console
    .WriteTo.File("logs/serilog-file.txt", rollingInterval: RollingInterval.Day) // Log to file, daily roll
    .CreateLogger();
//var builder = WebApplication.CreateBuilder(args);
var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<NatsWorker>();
builder.Services.AddMemoryCache();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
builder.Services.AddScoped<ICollectionStoreRepository<CollectionStore>, CollectionStoreRepository>();   
//builder.Host.UseSerilog();

var app = builder.Build();
await app.RunAsync();

