#!/usr/bin/env bash
# Valida os scripts de migração (src/TuttiWallet.Migrator/Scripts) de um PR.
# Uso: BASE_REF=preprod LIBERADO=false bash .github/scripts/validar-scripts-migracao.sh
set -euo pipefail

PASTA="src/TuttiWallet.Migrator/Scripts"
BASE_REF="${BASE_REF:?Defina BASE_REF com a branch de destino do PR}"
LIBERADO="${LIBERADO:-false}"
BASE="origin/${BASE_REF}"
erros=0

falhar() {
  echo "::error::$1"
  erros=$((erros + 1))
}

# 1. Scripts já existentes na base não podem ser modificados nem removidos (renomear aparece como remoção).
while IFS=$'\t' read -r status arquivo; do
  [[ "$arquivo" == "$PASTA"/* ]] || continue
  [[ "$status" == "A" ]] && continue
  if [[ "$LIBERADO" == "true" ]]; then
    echo "Alteração em script aplicado liberada pela label editar-script-aplicado: $status $arquivo"
    continue
  fi
  falhar "Script já aplicado não pode ser alterado ($status): $arquivo. Crie um novo script, ou peça ao autor a label 'editar-script-aplicado'."
done < <(git diff --no-renames --name-status "${BASE}...HEAD")

# 2. Nome no padrão Script000xNomeScript e numeração única.
declare -A numeros=()
for caminho in "$PASTA"/*.sql; do
  nome=$(basename "$caminho")
  if [[ ! "$nome" =~ ^Script([0-9]{4})[A-Z][A-Za-z0-9]*\.sql$ ]]; then
    falhar "Nome fora do padrão Script000xNomeScript: $nome"
    continue
  fi
  numero="${BASH_REMATCH[1]}"
  if [[ -n "${numeros[$numero]:-}" ]]; then
    falhar "Número $numero repetido: ${numeros[$numero]} e $nome"
  fi
  numeros[$numero]="$nome"
done

# 3. Scripts novos precisam vir depois do último script que já existe na base.
maior_na_base=$(git ls-tree --name-only "${BASE}" "$PASTA/" | sed -E 's#.*/Script([0-9]{4}).*#\1#' | sort | tail -n 1)
while IFS=$'\t' read -r status arquivo; do
  [[ "$status" == "A" && "$arquivo" == "$PASTA"/* ]] || continue
  numero=$(basename "$arquivo" | sed -E 's/^Script([0-9]{4}).*/\1/')
  if [[ "$numero" =~ ^[0-9]{4}$ && -n "$maior_na_base" && "10#$numero" -le "10#$maior_na_base" ]]; then
    falhar "Script novo $(basename "$arquivo") deve ter número maior que o último da base ($maior_na_base)."
  fi
done < <(git diff --no-renames --name-status "${BASE}...HEAD")

if [[ "$erros" -gt 0 ]]; then
  exit 1
fi
echo "Scripts de migração válidos."
