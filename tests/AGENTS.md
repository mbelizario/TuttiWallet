# tests

Instruções específicas desta pasta. As regras gerais estão no [AGENTS.md da raiz](../AGENTS.md).

| Projeto | O que testa | Precisa de Docker? |
|---|---|---|
| `TuttiWallet.Domain.Tests` | Entidades e invariantes do `Domain` | Não |
| `TuttiWallet.Application.Tests` | Casos de uso e validadores (sem banco) | Não |
| `TuttiWallet.Api.IntegrationTests` | API de ponta a ponta contra Postgres real (Testcontainers + migrator). Ver [ADR 0005](../docs/adr/0005-testes-de-integracao-com-testcontainers.md) | **Sim** |
| `TuttiWallet.Architecture.Tests` | Regra de dependência entre projetos (lê os `.csproj` de `src/`) | Não |

## Regras

- xUnit + FluentAssertions. Não mocke o banco: o que depende de SQL vai para os testes de integração.
- Nomes de teste: PascalCase, em português, descrevendo o cenário e o resultado (ex.: `CriarCategoriaSemTokenRetornaUnauthorized`). Sem padrão `Metodo_Cenario_Resultado`.
- Organize os testes espelhando a feature do código (ex.: `Transacoes/Cadastro`).
- Teste novo de integração: reutilize `ApiWebApplicationFactory`; não suba containers à mão.
- Projeto novo em `src/`: declare as dependências permitidas em `TuttiWallet.Architecture.Tests/RegraDeDependenciaTests.cs`, senão o teste falha.
- Para rodar um projeto: `dotnet test tests/TuttiWallet.Domain.Tests`. Sem Docker ligado, a suíte de integração falha por completo (não é bug do código).
