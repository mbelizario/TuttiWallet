CREATE TABLE Transacoes
(
    Id             uuid PRIMARY KEY,
    UsuarioId      uuid NOT NULL REFERENCES Usuarios (Id) ON DELETE CASCADE,
    CategoriaId    uuid NOT NULL REFERENCES Categorias (Id) ON DELETE RESTRICT,
    Tipo           text NOT NULL CHECK (Tipo IN ('receita', 'despesa')),
    Valor          numeric(14, 2) NOT NULL CHECK (Valor > 0),
    DataOcorrencia date NOT NULL,
    Descricao      text,
    CriadoEm       timestamptz NOT NULL DEFAULT now()
);

CREATE INDEX ix_transacoes_usuario_id ON Transacoes (UsuarioId);
CREATE INDEX ix_transacoes_categoria_id ON Transacoes (CategoriaId);
CREATE INDEX ix_transacoes_data_ocorrencia ON Transacoes (DataOcorrencia);
