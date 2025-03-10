using ObservabilityDemo.Api;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

var serviceName = "ObservabilityDemo";
var otelUri = new Uri("http://otel-collector:4317");

builder.Services.AddOpenApi();
builder.Services.AddLogging();
builder.Logging.ClearProviders();

builder.Services.AddOpenTelemetry()
      .ConfigureResource(resource => resource.AddService(
          serviceName: serviceName,
                            serviceVersion: typeof(Program).Assembly.GetName().Version?.ToString() ?? "1.0",
                            serviceInstanceId: Environment.MachineName))
      .WithTracing(builder => builder
          .AddSource(Diagnostic.ActivitySourceName)
          .SetSampler(new AlwaysOnSampler())
          .AddAspNetCoreInstrumentation()
          .AddConsoleExporter()
          .AddOtlpExporter(o =>
          {
              o.Protocol = OtlpExportProtocol.Grpc;
              o.Endpoint = otelUri;
          }))
      .WithMetrics(metrics => metrics
          .AddMeter(Diagnostic.MeterName)
          .AddAspNetCoreInstrumentation()
          .AddHttpClientInstrumentation()
          .AddRuntimeInstrumentation()
          .SetExemplarFilter(ExemplarFilterType.TraceBased)
          .AddConsoleExporter() //ConsoleExporter is used for demo purpose only.
          .AddOtlpExporter((options, metricReaderOptions) =>
          {
              options.Protocol = OtlpExportProtocol.Grpc;
              options.Endpoint = otelUri;
              metricReaderOptions.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 1000;
          }))
      .WithLogging(logging => logging
          .AddConsoleExporter() //ConsoleExporter is used for demo purpose only.
          .AddOtlpExporter(options =>
          {
              options.Protocol = OtlpExportProtocol.Grpc;
              options.Endpoint = otelUri;
          }));

builder.Services.AddSingleton<Diagnostic>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/order", (Diagnostic diagnostic) =>
{
    using var activity = diagnostic.ActivitySource.StartActivity("Order Processing");
    diagnostic.ProcessingOrder(Activity.Current?.TraceId);
    diagnostic.RegisterOrder();
    var duration = new Random().NextDouble() * 1000;
    diagnostic.RecordOrderDuration(duration);
    activity?.SetTag("order_duration", duration);
    return Results.Ok(new { OrderId = Guid.NewGuid(), Duration = duration });
});

app.MapGet("/orders", () =>
{
    return Results.Ok(new { Message = "Retrieving all orders." });
});

app.MapGet("/order/{id}", (Guid id) =>
{
    return Results.Ok(new { OrderId = id, Status = "Processing" });
});

app.Run();