using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace ObservabilityDemo.Api
{
    public partial class Diagnostic : IDisposable
    {
        internal const string ActivitySourceName = "ObservabilityDemo";
        internal const string MeterName = "ObservabilityDemo";
        private readonly Meter _meter;
        private readonly ILogger _logger;
        private readonly Random _random;

        public Diagnostic(ILoggerFactory loggerFactory)
        {
            _ = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = loggerFactory.CreateLogger<Diagnostic>();
            _random = new Random();
            var version = typeof(Diagnostic).Assembly.GetName().Version?.ToString();
            ActivitySource = new ActivitySource(ActivitySourceName, version);
            _meter = new Meter(MeterName, version);
            OrderCounter = _meter.CreateCounter<long>("orders_placed", "Number of orders placed");
            OrderDuration = _meter.CreateHistogram<double>("order_processing_duration", "Processing time per order");
        }
        
        public ActivitySource ActivitySource { get; }        
        public Counter<long> OrderCounter { get; }        
        public Histogram<double> OrderDuration { get; }

        public void RegisterOrder()
        {
            OrderCounter.Add(1);
        }

        public void RecordOrderDuration(double duration)
        {
            OrderDuration.Record(duration, new("orderType", "standard"), new("operation", "record_duration"));
        }

        [LoggerMessage(LogLevel.Information, "Processing order with Trace ID {traceId}")]
        public partial void ProcessingOrder(ActivityTraceId? traceId);

        public void Dispose()
        {
            ActivitySource.Dispose();
            _meter.Dispose();            
        }
    }
}
