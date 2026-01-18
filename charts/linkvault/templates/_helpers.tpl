{{/*
Expand the name of the chart.
*/}}
{{- define "linkvault.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Create a default fully qualified app name.
*/}}
{{- define "linkvault.fullname" -}}
{{- if .Values.fullnameOverride }}
{{- .Values.fullnameOverride | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- $name := default .Chart.Name .Values.nameOverride }}
{{- printf "%s" $name | trunc 63 | trimSuffix "-" }}
{{- end }}
{{- end }}

{{/*
Create chart name and version as used by the chart label.
*/}}
{{- define "linkvault.chart" -}}
{{- printf "%s-%s" .Chart.Name .Chart.Version | replace "+" "_" | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Common labels
*/}}
{{- define "linkvault.labels" -}}
helm.sh/chart: {{ include "linkvault.chart" . }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
{{- end }}

{{/*
Backend labels
*/}}
{{- define "linkvault.backend.labels" -}}
{{ include "linkvault.labels" . }}
app.kubernetes.io/name: {{ .Values.backend.name }}
app.kubernetes.io/instance: {{ .Release.Name }}-backend
app.kubernetes.io/component: backend
{{- end }}

{{/*
Backend selector labels
*/}}
{{- define "linkvault.backend.selectorLabels" -}}
app.kubernetes.io/name: {{ .Values.backend.name }}
app.kubernetes.io/instance: {{ .Release.Name }}-backend
{{- end }}

{{/*
Frontend labels
*/}}
{{- define "linkvault.frontend.labels" -}}
{{ include "linkvault.labels" . }}
app.kubernetes.io/name: {{ .Values.frontend.name }}
app.kubernetes.io/instance: {{ .Release.Name }}-frontend
app.kubernetes.io/component: frontend
{{- end }}

{{/*
Frontend selector labels
*/}}
{{- define "linkvault.frontend.selectorLabels" -}}
app.kubernetes.io/name: {{ .Values.frontend.name }}
app.kubernetes.io/instance: {{ .Release.Name }}-frontend
{{- end }}

{{/*
PostgreSQL labels
*/}}
{{- define "linkvault.postgresql.labels" -}}
{{ include "linkvault.labels" . }}
app.kubernetes.io/name: {{ .Values.postgresql.name }}
app.kubernetes.io/instance: {{ .Release.Name }}-postgresql
app.kubernetes.io/component: database
{{- end }}

{{/*
PostgreSQL selector labels
*/}}
{{- define "linkvault.postgresql.selectorLabels" -}}
app.kubernetes.io/name: {{ .Values.postgresql.name }}
app.kubernetes.io/instance: {{ .Release.Name }}-postgresql
{{- end }}

{{/*
PostgreSQL connection string
*/}}
{{- define "linkvault.postgresql.connectionString" -}}
Host={{ .Values.postgresql.name }};Port={{ .Values.postgresql.service.port }};Database={{ .Values.postgresql.auth.database }};User ID={{ .Values.postgresql.auth.username }};Password={{ .Values.postgresql.auth.password }};
{{- end }}
