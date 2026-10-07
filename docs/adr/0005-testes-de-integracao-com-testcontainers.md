# 0005. Testes de integração com Postgres real via Testcontainers

- Status: Aceita
- Data: 2026-10-07

## Contexto
Com SQL escrito à mão em Dapper (ver [0001](0001-dapper-sem-ef-core.md)), o comportamento relevante está nas queries e no schema, algo que um repositório mockado ou um banco em memória não reproduz.

## Decisão
Os testes de integração da API (`tests/TuttiWallet.Api.IntegrationTests`) sobem um PostgreSQL real em container com `Testcontainers.PostgreSql`, aplicam os scripts do migrator e exercitam a API de ponta a ponta. O banco não é mockado.

## Consequências
- Os testes exigem Docker em execução, localmente e no CI.
- Ficam mais lentos que testes de unidade, mas validam SQL, constraints e migrações de verdade.
- Regras de negócio puras continuam testadas em `Domain.Tests` e `Application.Tests`, sem Docker.
