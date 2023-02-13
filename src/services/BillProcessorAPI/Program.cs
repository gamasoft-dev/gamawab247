using Autofac.Core;
using BillProcessorAPI.Data;
using BillProcessorAPI.Extensions;
using BillProcessorAPI.Helpers;
using BillProcessorAPI.Services.Implementations;
using BillProcessorAPI.Services.Interfaces;
using BillProcessorAPI.Validators;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddClientDbContext(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddValidatorsFromAssemblyContaining<TransactionValidator>();
builder.Services.Configure<BillTransactionSettings>(builder.Configuration.GetSection("BillTransactionSettings"));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.ConfigureHttpPollyExtension();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigService(builder.Configuration);

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
