using Autofac.Core;
using BillProcessorAPI.Data;
using BillProcessorAPI.Extensions;
using BillProcessorAPI.Helpers;
using BillProcessorAPI.Helpers.Paythru;
using BillProcessorAPI.Helpers.Revpay;
using BillProcessorAPI.Middlewares;
using BillProcessorAPI.Services.Implementations;
using BillProcessorAPI.Services.Interfaces;
using BillProcessorAPI.Validators;
using FluentValidation;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddClientDbContext(builder.Configuration);
builder.Services.ConfigureMvc();
builder.Services.AddControllers()
    .AddJsonOptions(options=> {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });
//builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddValidatorsFromAssemblyContaining<TransactionValidator>();
builder.Services.Configure<BillTransactionSettings>(builder.Configuration.GetSection("BillTransactionSettings"));
builder.Services.Configure<RevpayOptions>(builder.Configuration.GetSection("RevpayConfig"));
builder.Services.Configure<PaythruOptions>(builder.Configuration.GetSection("PaythruOptions"));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.ConfigureHttpPollyExtension();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
});

builder.Services.ConfigService(builder.Configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

JsonConvert.DefaultSettings = () => new JsonSerializerSettings
{
    ContractResolver = new CamelCasePropertyNamesContractResolver()
};

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseErrorHandler();

app.MapControllers();

app.Run();
