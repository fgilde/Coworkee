# prometheus/Dockerfile
FROM prom/prometheus:latest

# Kopiere alle Konfigurationsdateien aus dem Build-Kontext in das Zielverzeichnis im Container
COPY . /etc/prometheus

# Setze die gewünschten Startparameter für Prometheus
CMD [ "--web.enable-otlp-receiver", "--config.file=/etc/prometheus/prometheus.yml" ]
