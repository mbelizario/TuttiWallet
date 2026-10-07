# TuttiWallet.Migrator

Instruções específicas desta pasta. As regras gerais estão no [AGENTS.md da raiz](../../AGENTS.md).

Console app que cria o banco (se não existir) e aplica os scripts SQL com DbUp antes da API subir. Decisão registrada em [ADR 0004](../../docs/adr/0004-dbup-com-migrator-separado.md).

## Scripts (`Scripts/`)

- **Scripts já aplicados são imutáveis.** Mudança de schema = **novo** script com o próximo número. Alterar, remover ou renomear um existente só com autorização explícita do autor.
- O CI (`.github/scripts/validar-scripts-migracao.sh`) falha o PR se um script existente for modificado/removido, a menos que o PR tenha a label `editar-script-aplicado` (aplicada só pelo autor). Também exige nome no padrão e número maior que o último da base.
- Nome: `Script000xNomeScript.sql` — número de 4 dígitos sequencial, depois nome em português, PascalCase e significativo (ex.: `Script0010AdicionarCampoXCategorias.sql`).
- SQL puro, identificadores **sem aspas**, tabelas e colunas em português/PascalCase (ver seção Banco de dados do AGENTS.md raiz). O schema legado ainda em inglês/snake_case só muda numa refatoração pedida explicitamente.
- `Id`: `uuid` (gerado em código) para tabelas de dados do usuário; `smallint` semeado por `INSERT` para tabelas de domínio/lookup.
- Os scripts são `EmbeddedResource`: confirme que o novo arquivo entra no build e rode a suíte de integração (ela aplica todos os scripts num Postgres real).

## Código

- `Program.cs` deve continuar enxuto; a lógica fica em métodos de extensão (`GarantirBancoDeDadosExtensions`, `AplicarScriptsExtensions`).
- A conexão vem de `CONNECTION_STRING` (variável de ambiente). Nunca coloque connection string em arquivo versionado.
