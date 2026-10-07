# TuttiWallet

Sistema de controle financeiro pessoal: lançamentos de receitas e despesas, organizados em categorias e subcategorias, com login por usuário.

Projeto de portfólio, também usado no dia a dia em um home-server via Docker, e usado pelo autor para estudar Clean Architecture, testes automatizados e DevOps.

## Stack

- C# / ASP.NET Core (.NET 10), Minimal APIs
- Dapper + Npgsql (sem EF Core, de propósito) e PostgreSQL
- Migrações com DbUp (scripts SQL puros)
- Blazor WebAssembly standalone no frontend
- Autenticação JWT implementada à mão
- xUnit + FluentAssertions; testes de integração com Testcontainers
- Docker + docker-compose

## Estrutura

```
src/
  TuttiWallet.Domain/           entidades e regras de negócio
  TuttiWallet.Application/      casos de uso e interfaces de repositório
  TuttiWallet.Infrastructure/   Dapper, Npgsql, JWT, hashing
  TuttiWallet.Contracts/        DTOs compartilhados entre Api e Web
  TuttiWallet.Migrator/         console app que roda os scripts DbUp
  TuttiWallet.Api/              host ASP.NET Core (Minimal API)
  TuttiWallet.Web/              Blazor WebAssembly
tests/
  TuttiWallet.Domain.Tests/
  TuttiWallet.Application.Tests/
  TuttiWallet.Api.IntegrationTests/
```

As dependências entre projetos só apontam para dentro: `Api`/`Web` → `Application`/`Infrastructure`/`Contracts` → `Domain`. `Domain` não referencia nenhum outro projeto.

## Pré-requisitos

- [.NET SDK](https://dotnet.microsoft.com/download) na versão indicada em `global.json`
- Docker (para rodar o sistema completo e os testes de integração)

## Como rodar

### Com Docker (recomendado)

```
cp .env.example .env
docker compose up --build
```

Edite o `.env` e troque os valores `change-me`. Depois de subir:

- API: `http://localhost:8080`
- Web: `http://localhost:8081`
- Seq (logs): `http://localhost:8082`

As portas são configuráveis no `.env`.

### Localmente, sem Docker

1. Suba um Postgres local (ou `docker compose up postgres`).
2. Rode o migrator com a variável `CONNECTION_STRING` apontando para esse Postgres.
3. Configure a connection string da API via User Secrets, a partir de `src/TuttiWallet.Api`:
   ```
   dotnet user-secrets set "ConnectionStrings:Postgres" "Host=localhost;Port=5432;Database=tuttiwallet;Username=<usuario>;Password=<senha>"
   ```
4. Suba a API e o Web:
   ```
   dotnet run --project src/TuttiWallet.Api
   dotnet run --project src/TuttiWallet.Web
   ```

Nenhum segredo real é commitado: `appsettings*.json` só tem placeholders; os valores reais vêm do `.env` (Docker) ou dos User Secrets (local).

## Testes

```
dotnet test TuttiWallet.slnx
```

Os testes de integração da API sobem um Postgres real via Testcontainers, então precisam do Docker rodando.

## Contribuindo (humanos e agentes de IA)

Antes de abrir um PR, rode o mesmo que o CI executa:

```
dotnet build TuttiWallet.slnx --configuration Release
dotnet test TuttiWallet.slnx --configuration Release
dotnet format TuttiWallet.slnx --verify-no-changes
```

As convenções de código, banco de dados, git/PR e o fluxo de trabalho esperado estão em [`AGENTS.md`](AGENTS.md), que é a fonte única de instruções para qualquer agente de IA.
