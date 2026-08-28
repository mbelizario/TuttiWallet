CREATE TABLE RefreshTokens
(
    Id         uuid PRIMARY KEY,
    UsuarioId  uuid NOT NULL REFERENCES Usuarios (Id),
    HashToken  text NOT NULL UNIQUE,
    CriadoEm   timestamptz NOT NULL DEFAULT now(),
    ExpiraEm   timestamptz NOT NULL,
    RevogadoEm timestamptz NULL
);

CREATE INDEX IX_RefreshTokens_UsuarioId ON RefreshTokens (UsuarioId);
