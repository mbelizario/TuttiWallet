ALTER TABLE Transacoes DROP CONSTRAINT IF EXISTS transacoes_tipoid_fkey;
DROP INDEX IF EXISTS ix_transacoes_tipo_id;
ALTER TABLE Transacoes DROP COLUMN IF EXISTS TipoId;
