# 0001. Dapper em vez de EF Core

- Status: Aceita
- Data: 2026-10-07

## Contexto
O projeto é de portfólio e de estudo. O autor tem experiência com .NET e Dapper e quer ver e controlar o SQL que roda no banco, em vez de deixá-lo escondido atrás de um ORM.

## Decisão
O acesso a dados usa Dapper + Npgsql. O EF Core não é usado, de propósito.

## Consequências
- O SQL é escrito à mão nos repositórios da camada `Infrastructure`; o mapeamento é explícito.
- Sem change tracking nem migrações geradas pelo ORM: o schema é versionado por scripts SQL (ver [0004](0004-dbup-com-migrator-separado.md)).
- Agentes não devem introduzir EF Core nem outro ORM sem discutir com o autor.
