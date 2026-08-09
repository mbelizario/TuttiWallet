CREATE TABLE Categorias
(
    Id             uuid PRIMARY KEY,
    UsuarioId      uuid NOT NULL REFERENCES Usuarios (Id) ON DELETE CASCADE,
    Nome           text NOT NULL,
    Tipo           text NOT NULL CHECK (Tipo IN ('receita', 'despesa')),
    CategoriaPaiId uuid REFERENCES Categorias (Id) ON DELETE CASCADE,
    CriadoEm       timestamptz NOT NULL DEFAULT now()
);

CREATE INDEX ix_categorias_usuario_id ON Categorias (UsuarioId);
CREATE INDEX ix_categorias_categoria_pai_id ON Categorias (CategoriaPaiId);
