# 0004. DbUp com scripts SQL puros e serviço migrator separado

- Status: Aceita
- Data: 2026-10-07

## Contexto
Sem EF Core (ver [0001](0001-dapper-sem-ef-core.md)), o schema precisa ser versionado de outra forma, e a API não deve subir contra um banco desatualizado.

## Decisão
Migrações com DbUp, usando scripts `.sql` puros em `src/TuttiWallet.Migrator/Scripts`, executados por um serviço `migrator` separado (console app) que roda antes da API no `docker compose up`.

## Consequências
- Scripts já aplicados são imutáveis: mudanças viram um novo script com o próximo número (`Script000xNomeScript`). O CI barra a edição de scripts existentes, salvo com a label `editar-script-aplicado`.
- **A confirmar**: manter a migração fora do processo da API evita corridas entre várias instâncias e permite que a API suba com permissões reduzidas no banco.
- Os testes de integração também usam o migrator para preparar o banco.
