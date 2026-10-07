# 0002. JWT implementado à mão

- Status: Aceita
- Data: 2026-10-07

## Contexto
O sistema precisa de login por usuário. As opções eram o ASP.NET Identity completo, um provedor OAuth externo ou uma implementação própria enxuta.

## Decisão
Autenticação com JWT implementado à mão (geração e validação de tokens, refresh tokens), sem ASP.NET Identity completo e sem OAuth externo. Apenas o hash de senha reaproveita `PasswordHasher<T>` de `Microsoft.Extensions.Identity.Core`.

## Consequências
- Controle total do fluxo e do schema (`Usuarios`, `RefreshTokens`), sem as tabelas e abstrações do Identity.
- **A confirmar**: motivação de aprendizado e escopo de uso pessoal, sem necessidade de login social.
- A chave de assinatura vem de configuração (`JWT_CHAVE` no `.env`; User Secrets localmente) e nunca é commitada.
- Por ser código de segurança escrito à mão, mudanças aqui exigem atenção redobrada na revisão e testes automatizados.
