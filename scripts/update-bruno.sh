#!/usr/bin/env bash
set -euo pipefail

echo "==> Gerando openapi/v1.json..."
dotnet build ./backend/AgentesDaAlegria.API.csproj --nologo -v minimal

echo ""
echo "==> Atualizando coleção Bruno..."
node ./scripts/openapi-to-bruno.js

echo ""
echo "Pronto! Abra a pasta bruno/ no Bruno para ver as requisições."