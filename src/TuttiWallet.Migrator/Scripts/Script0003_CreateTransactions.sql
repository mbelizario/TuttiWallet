CREATE TABLE transactions
(
    id          uuid PRIMARY KEY,
    user_id     uuid NOT NULL REFERENCES users (id) ON DELETE CASCADE,
    category_id uuid NOT NULL REFERENCES categories (id) ON DELETE RESTRICT,
    type        text NOT NULL CHECK (type IN ('income', 'expense')),
    amount      numeric(14, 2) NOT NULL CHECK (amount > 0),
    occurred_on date NOT NULL,
    description text,
    created_at  timestamptz NOT NULL DEFAULT now()
);

CREATE INDEX ix_transactions_user_id ON transactions (user_id);
CREATE INDEX ix_transactions_category_id ON transactions (category_id);
CREATE INDEX ix_transactions_occurred_on ON transactions (occurred_on);
