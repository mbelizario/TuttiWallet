#!/bin/sh
set -eu

envsubst '${API_BASE_URL}' < /usr/share/nginx/html/appsettings.template.json > /usr/share/nginx/html/appsettings.json
