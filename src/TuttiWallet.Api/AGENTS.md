# TuttiWallet.Api

Instruções específicas desta pasta. As regras gerais estão no [AGENTS.md da raiz](../../AGENTS.md).

Host ASP.NET Core com Minimal APIs. Depende de `Application`, `Infrastructure` e `Contracts`.

- **Organização por feature**: cada pasta (`Categorias`, `Transacoes`, `Usuarios`, `Autenticacao`...) tem um `<Feature>Endpoints.cs` com um método de extensão `Map<Feature>Endpoints()`. Endpoint novo entra na pasta da feature; feature nova ganha pasta e um `Map...` registrado no `Program.cs`.
- `Program.cs` fica enxuto: configuração transversal (Serilog, CORS, JWT, tratamento de exceções) vive em métodos de extensão nas próprias pastas (`AddCorsConfigurado`, `AddAutenticacaoJwt`, `UseTratamentoDeExcecoes`...). Não acumule lógica inline.
- Endpoints são finos: convertem o `Request` (de `Contracts`) em comando, chamam o caso de uso de `Application` e convertem o resultado em `Response`. Nunca exponha entidades de `Domain`.
- Erros de validação de campo voltam como `BadRequest` com erro por campo; exceções não tratadas passam pelo middleware de tratamento de exceções, sem vazar detalhes internos.
- Rotas novas são protegidas por padrão (`RequireAuthorization`), salvo as públicas de cadastro/autenticação e `/health`.
- Toda rota nova precisa de teste em `tests/TuttiWallet.Api.IntegrationTests`, incluindo o caso sem token (`Unauthorized`).
- Segredos (connection string, chave JWT) vêm de configuração: `.env` no Docker, User Secrets localmente. Nunca em `appsettings*.json`.
