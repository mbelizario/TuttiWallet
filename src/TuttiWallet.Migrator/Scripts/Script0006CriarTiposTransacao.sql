CREATE TABLE IF NOT EXISTS TiposTransacao
(
    Id   smallint PRIMARY KEY,
    Nome text NOT NULL UNIQUE
);

INSERT INTO TiposTransacao (Id, Nome)
VALUES (1, 'Receita'), (2, 'Despesa')
ON CONFLICT (Id) DO NOTHING;

ALTER TABLE Categorias ADD COLUMN IF NOT EXISTS TipoId smallint REFERENCES TiposTransacao (Id);

UPDATE Categorias
SET TipoId = CASE Tipo WHEN 'receita' THEN 1 WHEN 'despesa' THEN 2 END
WHERE TipoId IS NULL;

ALTER TABLE Categorias ALTER COLUMN TipoId SET NOT NULL;
ALTER TABLE Categorias DROP COLUMN IF EXISTS Tipo;

CREATE INDEX IF NOT EXISTS ix_categorias_tipo_id ON Categorias (TipoId);
