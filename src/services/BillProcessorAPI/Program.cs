using BillProcessorAPI.Data;
using BillProcessorAPI.Dtos;
using BillProcessorAPI.Dtos.BroadcastMessage;
using BillProcessorAPI.Dtos.Configs;
using BillProcessorAPI.Extensions;
using BillProcessorAPI.Helpers;
using BillProcessorAPI.Helpers.Flutterwave;
using BillProcessorAPI.Helpers.Paythru;
using BillProcessorAPI.Helpers.Revpay;
using BillProcessorAPI.Middlewares;
using BillProcessorAPI.Validators;
using Domain.Common.ShortLink.ValueObjects;
using FluentValidation;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddClientDbContext(builder.Configuration);
builder.Services.ConfigureMvc();
builder.Services.AddControllers()
    .AddJsonOptions(options=> {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });
builder.Services.AddSerilog();
Log.Logger = new LoggerConfiguration()
    // .WriteTo.File("./Logger/BillProcessorLogs.txt", rollingInterval: RollingInterval.Day)
    .MinimumLevel.Information()
    .CreateLogger();

//builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddValidatorsFromAssemblyContaining<TransactionValidator>();
builder.Services.Configure<BillTransactionSettings>(builder.Configuration.GetSection("BillTransactionSettings"));
builder.Services.Configure<RevpayOptions>(builder.Configuration.GetSection("RevpayConfig"));
builder.Services.Configure<PaythruOptions>(builder.Configuration.GetSection("PaythruOptions"));
builder.Services.Configure<CutlyOptions>(builder.Configuration.GetSection(nameof(CutlyOptions)));
builder.Services.Configure<PaymentConfirmationDelayInSec>(
    builder.Configuration.GetSection(nameof(PaymentConfirmationDelayInSec)));
builder.Services.Configure<FlutterwaveOptions>(builder.Configuration.GetSection("FlutterWaveOptions"));
builder.Services.AddOptions<BusinessesPhoneNumber>().BindConfiguration("businessesPhoneNumberConfig");
builder.Services.AddOptions<ReceiptBroadcastConfig>().BindConfiguration("receiptBroadcastSettings");
builder.Services.AddOptions<TransactionOptions>().BindConfiguration("transactionOptions");
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.ConfigureHttpPollyExtension();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(opts =>
{
    opts.AddPolicy("CorsPolicy", builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
});
//builder.WebHost.UseSentry(o => {
//    o.Dsn = "https://84a76b43608f4aecbd98622afde5310e@o373456.ingest.sentry.io/6181869";// When configuring for the first time, to see what the SDK is doing:
//    o.Debug = true;// Set TracesSampleRate to 1.0 to capture 100% of transactions for performance monitoring.// We recommend adjusting this value in production.
//    o.TracesSampleRate = 1.0;
//});
builder.Services.ConfigService(builder.Configuration);


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

JsonConvert.DefaultSettings = () => new JsonSerializerSettings
{
    ContractResolver = new CamelCasePropertyNamesContractResolver()
};


app.UseHttpsRedirection();

app.UseCors("CorsPolicy");
app.UseRouting();


app.UseAuthorization();

app.UseErrorHandler();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
