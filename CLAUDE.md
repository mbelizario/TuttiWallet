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

## Banco de dados

### Convenção de nomenclatura

- Nomes de tabelas e colunas são em **português**, escritos em **PascalCase** (ex.: `Usuarios`, `Categorias`, `CategoriaPaiId`, `Transacoes`).
- Assim como no código, os nomes devem ser significativos — evite abreviações que não sejam óbvias.
- Essa convenção segue a mesma lógica do idioma no código: vocabulário de domínio em português; termos de padrão/convenção (ex.: sufixo `Id` para chave estrangeira) podem permanecer como estão, sem misturar dentro do mesmo termo.

> **Pendente**: o schema atual (`users`, `categories`, `parent_category_id`, `transactions`) ainda está em inglês/snake_case, criado antes desta convenção. A migração dessas tabelas para o novo padrão está planejada como uma refatoração futura, ainda não iniciada — não renomeie tabelas existentes sem que essa refatoração seja pedida explicitamente.

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

### Idioma: português vs. inglês

- **Vocabulário de domínio** — nomes de classes, métodos, variáveis, atributos e pastas de domínio — é sempre em **português**, sem misturar idiomas dentro do mesmo termo. Exemplos: `Categoria`, `Transacao`, `ObterPorIdAsync`, `valorTotal`.
- **Termos de arquitetura/padrão** (Clean Architecture e padrões de projeto) ficam em **inglês**, mesmo quando aparecem como sufixo colado a um nome de domínio em português: `Domain`, `Application`, `Infrastructure`, `Repository`, `Service`, `Controller`. É por isso que `CategoriaRepository` e `CategoriaService` estão corretos — o termo de domínio (`Categoria`) está em pt-br, o termo de padrão (`Repository`/`Service`) está em inglês, e cada um mantém seu idioma inteiro.
- O que não pode acontecer é picotar um único termo entre os dois idiomas (ex.: `CategoryRepositorio` está errado — ou o termo é de domínio e vai inteiro em português, ou é de convenção e vai inteiro em inglês).
- Mensagens de exceção e validação voltadas ao usuário final são em português.

### Nomenclatura — regras fixas (sem exceção)

| Ação | Prefixo do método |
|---|---|
| Buscar/obter dado (banco ou outra fonte) | `Obter` |
| Inserir dado | `Inserir` |
| Atualizar dado | `Atualizar` |
| Excluir dado | `Excluir` |

- Nomes de classes, métodos e atributos em PascalCase; nomes de variáveis em camelCase.
- Nomes de classes iniciam preferencialmente com um substantivo.
- Métodos assíncronos têm sufixo `Async`; sempre que possível e necessário, escreva métodos assíncronos.
- Nomes de métodos de teste seguem a mesma convenção de nomenclatura dos métodos comuns (PascalCase, significativos, em português) — sem um padrão fixo do tipo `Metodo_Cenario_Resultado`.
- Todos os nomes (classes, métodos, variáveis) devem ser significativos.

### Nomenclatura e estilo — negociáveis (podem ceder num caso concreto e justificado)

- Preferência por "early return" em vez de aninhamento.
- Preferência por evitar `else`, priorizando if + early return.
- Responsabilidade única por método.

Nesses três pontos, o agente pode se afastar da preferência quando segui-la à risca tornaria o código pior ou mais confuso — mas deve explicar o porquê da escolha.

### Estilo geral

- Sem comentários explicando o óbvio; comentário só quando o código não consegue explicar o "porquê" sozinho.
- Entidades de domínio validam seus próprios invariantes no construtor (ver `TuttiWallet.Domain`) em vez de aceitar estado inválido e validar depois.
- DTOs de request/response ficam em `TuttiWallet.Contracts`, com sufixo `Request`/`Response` (ex.: `CriarCategoriaRequest`, `CategoriaResponse`). Nunca expor entidades de `Domain` diretamente pela API.
- Métodos devem ser pequenos e testáveis.

## Limites — exigem autorização explícita antes de agir

- **Scripts de migração já aplicados** (`src/TuttiWallet.Migrator/Scripts`): avisar antes de alterar um script existente — o padrão é criar um novo script, não editar um já aplicado.
- **Comandos destrutivos** (ex.: `docker compose down -v`, `DROP TABLE`, reset de banco): nunca executar sem minha autorização explícita no momento.
- **Novas dependências/pacotes** (NuGet ou outros): sempre perguntar antes de instalar.

## Fluxo de trabalho

Este é o primeiro projeto do autor usando um agente de IA para desenvolvimento. Ele tem 8 anos de experiência em .NET/Dapper, mas está estudando conceitos novos (arquitetura em camadas, DevOps, testes) e, por ser a primeira vez com um agente, quer manter controle alto sobre o que é codado. Por isso:

- **Discuta antes de implementar, inclusive em decisões menores** — não só arquiteturais (nova dependência entre camadas, biblioteca, schema), mas também escolhas menores de implementação (ex.: nome de uma tabela nova, formato de um endpoint). Prefira perguntar a assumir.
- **Explique o porquê** de uma abordagem, não só a aplique — o objetivo do projeto é aprender, não apenas ter o código pronto.
- **Construa por caso de uso completo**: implemente todas as camadas envolvidas em um caso de uso (Domain → Application → Infrastructure → Api) e pare para revisão antes de seguir para o próximo caso de uso. Não gere múltiplos casos de uso de uma vez.
- O autor pretende abrir Pull Requests no GitHub para revisar o código formalmente — estruture o trabalho em unidades que façam sentido como um PR coeso (um caso de uso por PR).
- Priorize pequenas melhorias que facilitem o code review, em vez de mudanças grandes e difíceis de revisar.