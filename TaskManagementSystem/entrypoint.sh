#!/bin/bash

set -e

# Ждем немного перед применением миграций
echo "Waiting for database to be ready..."
sleep 5

# Применяем миграции
echo "Applying database migrations..."
dotnet ef database update

# Запускаем приложение
echo "Starting application..."
exec dotnet YourProject.dll "$@"