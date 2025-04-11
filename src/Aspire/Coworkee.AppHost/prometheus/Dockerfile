# prometheus/Dockerfile
# FROM prom/prometheus:latest

FROM prom/prometheus:v3.2.1
COPY . /etc/prometheus
CMD [ "--web.enable-otlp-receiver", "--config.file=/etc/prometheus/prometheus.yml" ]
