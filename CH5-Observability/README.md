# Observability Demo

This project demonstrates observability using various tools such as OpenTelemetry, Jaeger, Loki, Prometheus, and Grafana. The project is built with .NET 9 and uses Docker for containerization.

## Prerequisites

- Docker
- Docker Compose
- .NET 9 SDK

## Getting Started

### Clone the Repository
```bash
git clone https://github.com/RebeldeCuantico/Simplifying-Cloud-Native-Architecture-with-.NET-9-Book.git 

cd simplifying-Cloud-Native-Architecture-with-.NET-9-Book\CH5-Observability\
```
### Build and Run the Containers

```bash
docker-compose -f docker-compose.yml -f docker-compose.override.yml up --build
```

This command will build and start the following services:
- `observabilitydemo.api`: The main API service.
- `otel-collector`: OpenTelemetry Collector.
- `jaeger`: Jaeger for tracing.
- `loki`: Loki for logging.
- `prometheus`: Prometheus for metrics.
- `grafana`: Grafana for visualization.

### Accessing the Services

- **API Service**: Accessible at `http://localhost:<api-port>`
- **Jaeger UI**: Accessible at `http://localhost:16686`
- **Prometheus**: Accessible at `http://localhost:9090`
- **Grafana**: Accessible at `http://localhost:3000`

## Configuration

### OpenTelemetry Collector

The OpenTelemetry Collector configuration is located at `Configuration/otel-collector-config.yaml`.

### Prometheus

The Prometheus configuration is located at `Configuration/prometheus.yaml`.

### Grafana

The Grafana data sources configuration is located at `Configuration/grafana-datasources.yaml`.

## Project Structure

- `ObservabilityDemo.Api/`: Contains the main API project.
- `Configuration/`: Contains configuration files for OpenTelemetry Collector, Prometheus, and Grafana.
- `docker-compose.yml`: Docker Compose file to set up the services.
- `docker-compose.override.yml`: Override file for Docker Compose.

## License

This project is licensed under the MIT License. See the [LICENSE](../LICENCE) file for details.

## Acknowledgements

- [OpenTelemetry](https://opentelemetry.io/)
- [Jaeger](https://www.jaegertracing.io/)
- [Loki](https://grafana.com/oss/loki/)
- [Prometheus](https://prometheus.io/)
- [Grafana](https://grafana.com/)