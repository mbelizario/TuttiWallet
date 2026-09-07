ALTER TABLE Transacoes DROP COLUMN IF EXISTS Tipo;

ALTER TABLE Transacoes ADD COLUMN IF NOT EXISTS Observacoes text NULL;

UPDATE Transacoes SET Descricao = '' WHERE Descricao IS NULL;
ALTER TABLE Transacoes ALTER COLUMN Descricao SET NOT NULL;
ALTER TABLE Transacoes ADD CONSTRAINT ck_transacoes_descricao_tamanho CHECK (char_length(Descricao) <= 100);
