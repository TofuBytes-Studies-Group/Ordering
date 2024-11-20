using Microsoft.EntityFrameworkCore;
using Ordering.API.Kafka;
using Ordering.API.Services;
using Ordering.Infrastructure;
using Ordering.Infrastructure.Kafka;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<OrderingContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));  //TODO: TEMPORARY ATTEMPT TO FIX System.TypeLoadException: Could not load type 'Microsoft.EntityFrameworkCore.Metadata.Internal.AdHocMapper' from assembly 'Microsoft.EntityFrameworkCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=adb9793829ddae60'.



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add the producer service as singletons:
builder.Services.AddSingleton<KafkaProducer>();
// Add the kafka consumer service as a hosted service (background service that runs for the lifetime of the application):
builder.Services.AddHostedService<KafkaConsumer>();
builder.Services.AddSingleton<TestService>();

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
