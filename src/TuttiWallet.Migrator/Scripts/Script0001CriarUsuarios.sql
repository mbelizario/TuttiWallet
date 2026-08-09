CREATE TABLE Usuarios
(
    Id        uuid PRIMARY KEY,
    Email     citext NOT NULL UNIQUE,
    HashSenha text NOT NULL,
    CriadoEm  timestamptz NOT NULL DEFAULT now()
);
