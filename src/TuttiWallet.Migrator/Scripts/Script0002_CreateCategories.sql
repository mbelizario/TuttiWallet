CREATE TABLE categories
(
    id                 uuid PRIMARY KEY,
    user_id            uuid NOT NULL REFERENCES users (id) ON DELETE CASCADE,
    name               text NOT NULL,
    type               text NOT NULL CHECK (type IN ('income', 'expense')),
    parent_category_id uuid REFERENCES categories (id) ON DELETE CASCADE,
    created_at         timestamptz NOT NULL DEFAULT now()
);

CREATE INDEX ix_categories_user_id ON categories (user_id);
CREATE INDEX ix_categories_parent_category_id ON categories (parent_category_id);
