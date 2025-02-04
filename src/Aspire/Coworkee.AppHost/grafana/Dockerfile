# grafana/Dockerfile
FROM grafana/grafana:latest

# Kopiere den Inhalt des lokalen config-Verzeichnisses in das Container-Verzeichnis /etc/grafana
COPY config/ /etc/grafana/

# Kopiere die Dashboards in das Container-Verzeichnis /var/lib/grafana/dashboards
COPY dashboards/ /var/lib/grafana/dashboards/
