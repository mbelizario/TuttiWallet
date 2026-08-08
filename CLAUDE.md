# TuttiWallet

Sistema de controle financeiro pessoal: lançamentos de receitas e despesas, organizados em categorias e subcategorias, com login por usuário.

Objetivos do projeto (relevantes para as decisões técnicas abaixo): portfólio público no GitHub, uso pessoal em home-server via Docker, e estudo de Clean Architecture, testes automatizados e DevOps por parte do autor.

## Stack

- **Backend**: C# / ASP.NET Core (.NET 10), Minimal APIs.
- **Acesso a dados**: Dapper + Npgsql. Sem EF Core de propósito — decisão consciente para não esconder o SQL.
- **Banco de dados**: PostgreSQL.
- **Migrações**: DbUp, com scripts SQL puros em `src/TuttiWallet.Migrator/Scripts`, rodados por um serviço `migrator` separado (console app) antes da API subir.
- **Frontend**: Blazor WebAssembly standalone (`src/TuttiWallet.Web`), consumindo a API via HTTP — não é Blazor Server. Tratado como um client desacoplado, do mesmo jeito que um SPA em React trataria a API.
- **Autenticação**: JWT implementado à mão (sem ASP.NET Identity completo, sem OAuth externo). Hash de senha via `Microsoft.Extensions.Identity.Core` (`PasswordHasher<T>`).
- **Testes**: xUnit + FluentAssertions. Testes de integração da API usam `Testcontainers.PostgreSql` (sobe um Postgres real em container, não mocka o banco).
- **Containers**: Docker + docker-compose. Todo o sistema (Postgres, migrator, API, Web) sobe com um único `docker compose up`.

## Estrutura e regra de dependência

```
src/
  TuttiWallet.Domain/           entidades e regras de negócio — não depende de nada
  TuttiWallet.Application/      casos de uso, interfaces de repositório — depende só de Domain
  TuttiWallet.Infrastructure/   Dapper, Npgsql, JWT, hashing — depende de Application + Domain
  TuttiWallet.Contracts/        DTOs de request/response compartilhados entre Api e Web
  TuttiWallet.Migrator/         console app, roda os scripts DbUp
  TuttiWallet.Api/              host ASP.NET Core (Minimal API) — depende de Application + Infrastructure + Contracts
  TuttiWallet.Web/               Blazor WebAssembly — depende só de Contracts
tests/
  TuttiWallet.Domain.Tests/
  TuttiWallet.Application.Tests/
  TuttiWallet.Api.IntegrationTests/
```

Regra: as setas de dependência (`ProjectReference`) só podem apontar para dentro (`Api`/`Web` → `Application`/`Infrastructure`/`Contracts` → `Domain`), nunca o contrário. `Domain` nunca referencia nenhum outro projeto do repositório. Ao adicionar código, respeite essa direção — é o ponto central do que este projeto está estudando.

Schema do banco (ver `src/TuttiWallet.Migrator/Scripts`): `users`, `categories` (com `parent_category_id` auto-relacionado — a mesma tabela cobre categoria e subcategoria) e `transactions`.

## Como rodar

**Via Docker (recomendado, replica o ambiente de produção/home-server):**
```
cp .env.example .env
docker compose up --build
```
API fica em `http://localhost:8080` (porta configurável em `.env`), Web em `http://localhost:8081`.

**Localmente, sem Docker (dev do dia a dia):**
- Suba um Postgres local (ou use `docker compose up postgres`).
- Rode o migrator apontando `CONNECTION_STRING` para esse Postgres.
- `dotnet run --project src/TuttiWallet.Api`
- `dotnet run --project src/TuttiWallet.Web`

## Testes

```
dotnet test TuttiWallet.slnx
```
Os testes de integração da API precisam de Docker rodando (sobem um Postgres via Testcontainers automaticamente — não é preciso configurar nada à mão).

## Convenções

- Sem comentários explicando o óbvio; comentário só quando o código não consegue explicar o "porquê" sozinho.
- Entidades de domínio validam seus próprios invariantes no construtor (ver `TuttiWallet.Domain`) em vez de aceitar estado inválido e validar depois.
- DTOs de request/response ficam em `TuttiWallet.Contracts`, nunca expõe entidades de `Domain` diretamente pela API.

## Fluxo de trabalho

Este é o primeiro projeto do autor usando um agente de IA para desenvolvimento, e ele tem 8 anos de experiência em .NET/Dapper mas está estudando conceitos novos (arquitetura em camadas, DevOps, testes). Por isso:

- Decisões arquiteturais (nova dependência entre camadas, escolha de biblioteca, mudança de schema) devem ser discutidas antes de implementadas, não apenas executadas silenciosamente.
- Prefira explicar o *porquê* de uma abordagem, não só aplicá-la — o objetivo do projeto é aprender, não apenas ter o código pronto.
- Novas features (endpoints, telas, casos de uso) devem ser construídas incrementalmente, em conjunto, e não geradas de uma vez de forma completa.
