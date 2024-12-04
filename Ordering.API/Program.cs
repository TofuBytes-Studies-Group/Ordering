using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Ordering.API.Kafka;
using Ordering.API.Repositories;
using Ordering.API.RequestDTOs;
using Ordering.API.Services;
using Ordering.API.Validators;
using Ordering.Domain.Interfaces;
using Ordering.Infrastructure;
using Ordering.Infrastructure.Kafka;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

//load environment variables from .env file
Env.Load();


// Add services to the container.
builder.Configuration.AddEnvironmentVariables();
builder.Services.AddDbContext<OrderingContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IValidator<CartDto>, CartRequestValidator>();

// Add the producer service as singletons:
builder.Services.AddScoped<IKafkaProducer, KafkaProducer>();
// Add the kafka consumer service as a hosted service (background service that runs for the lifetime of the application):
builder.Services.AddHostedService<KafkaConsumer>();
builder.Services.AddScoped<IKafkaProducerService, KafkaProducerService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
