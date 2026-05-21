$ErrorActionPreference = 'Stop'

Write-Host '==> Gerando openapi/v1.json...' -ForegroundColor Cyan
dotnet build ./backend/AgentesDaAlegria.API.csproj --nologo -v minimal
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host ''
Write-Host '==> Atualizando colecao Bruno...' -ForegroundColor Cyan
node ./scripts/openapi-to-bruno.js
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host ''
Write-Host 'Pronto! Abra a pasta bruno/ no Bruno para ver as requisicoes.' -ForegroundColor Green