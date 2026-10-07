# 0003. Blazor WebAssembly standalone como client desacoplado

- Status: Aceita
- Data: 2026-10-07

## Contexto
O frontend precisa consumir a API. Blazor Server acoplaria a UI ao servidor da aplicação; Blazor WebAssembly standalone permite tratar a UI como um SPA que só conversa com a API por HTTP.

## Decisão
O frontend é um Blazor WebAssembly standalone (`src/TuttiWallet.Web`), servido por nginx em container próprio e consumindo a API via HTTP, do mesmo jeito que um SPA em React faria.

## Consequências
- `Web` depende somente de `Contracts` (DTOs compartilhados); nunca de `Domain`, `Application` ou `Infrastructure`. A regra é verificada por teste de arquitetura.
- A URL da API é configurada em tempo de execução: `appsettings.template.json` + `envsubst` no entrypoint do container (`API_BASE_URL`).
- É necessário CORS configurado na API para a origem do Web.
