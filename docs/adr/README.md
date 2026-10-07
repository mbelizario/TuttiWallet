# Registros de decisões de arquitetura (ADRs)

Cada arquivo registra **uma** decisão técnica relevante: o contexto, o que foi decidido e as consequências. Servem para que humanos e agentes de IA entendam o *porquê* sem inflar o `AGENTS.md`.

## Como usar

- Antes de propor uma mudança que contrarie uma decisão abaixo (ex.: trocar Dapper por EF Core), leia o ADR correspondente e discuta com o autor.
- Para uma decisão nova, copie o modelo abaixo para `NNNN-titulo-em-kebab-case.md` (próximo número sequencial) e adicione-a ao índice.
- ADRs não são editados para refletir mudanças de rumo: crie um novo e marque o antigo como `Substituído por NNNN`.
- Trechos marcados com **A confirmar** são inferências feitas a partir do código e do `AGENTS.md`, e aguardam confirmação do autor.

## Índice

| Nº | Decisão | Status |
|---|---|---|
| [0001](0001-dapper-sem-ef-core.md) | Dapper em vez de EF Core | Aceita |
| [0002](0002-jwt-implementado-a-mao.md) | JWT implementado à mão | Aceita |
| [0003](0003-blazor-wasm-standalone.md) | Blazor WebAssembly standalone como client desacoplado | Aceita |
| [0004](0004-dbup-com-migrator-separado.md) | DbUp com scripts SQL puros e serviço migrator separado | Aceita |
| [0005](0005-testes-de-integracao-com-testcontainers.md) | Testes de integração com Postgres real via Testcontainers | Aceita |

## Modelo

```markdown
# NNNN. Título da decisão

- Status: Proposta | Aceita | Substituída por NNNN
- Data: AAAA-MM-DD

## Contexto
O problema e as forças em jogo.

## Decisão
O que foi decidido.

## Consequências
O que fica mais fácil, o que fica mais difícil, o que passa a ser obrigatório.
```
