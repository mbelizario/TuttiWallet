# TuttiWallet.Web

Instruções específicas desta pasta. As regras gerais estão no [AGENTS.md da raiz](../../AGENTS.md).

Blazor WebAssembly **standalone**, tratado como um client desacoplado da API ([ADR 0003](../../docs/adr/0003-blazor-wasm-standalone.md)).

- Depende **somente** de `TuttiWallet.Contracts`. Nunca referencie `Domain`, `Application` ou `Infrastructure` (o teste de arquitetura falha).
- Fale com a API apenas por HTTP, usando os DTOs `*Request`/`*Response` de `Contracts`.
- A URL da API vem de `ApiBaseUrl` em `wwwroot/appsettings.json`. No container, `appsettings.template.json` + `docker-entrypoint.sh` (`envsubst` de `API_BASE_URL`) geram o arquivo; ao criar uma nova configuração de runtime, atualize os dois.
- Não coloque segredos no Web: tudo que está em `wwwroot` é público no navegador.
- Páginas em `Pages/`, layout em `Layout/`. As páginas `Counter` e `Weather` são o exemplo do template do Blazor e ainda não foram removidas.
- Código novo segue as mesmas convenções de nomenclatura (domínio em português) do AGENTS.md raiz.
