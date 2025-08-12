-- users table
CREATE TABLE IF NOT EXISTS users (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  email varchar(255) NOT NULL UNIQUE,
  full_name varchar(255) NOT NULL,
  created_at timestamptz NOT NULL DEFAULT now()
);

-- todo_lists table
CREATE TABLE IF NOT EXISTS todo_lists (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  title varchar(255) NOT NULL,
  owner_id uuid NOT NULL REFERENCES users(id) ON DELETE CASCADE,
  created_at timestamptz NOT NULL DEFAULT now(),
  updated_at timestamptz NOT NULL DEFAULT now()
);

-- shared todo_lists table
CREATE TABLE IF NOT EXISTS todo_list_shares (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  todo_list_id uuid NOT NULL REFERENCES todo_lists(id) ON DELETE CASCADE,
  user_id uuid NOT NULL REFERENCES users(id) ON DELETE CASCADE,
  created_at timestamptz NOT NULL DEFAULT now(),
  UNIQUE (todo_list_id, user_id)
);

-- Indexes
CREATE INDEX IF NOT EXISTS idx_users_email ON users(email);
CREATE INDEX IF NOT EXISTS idx_todo_lists_owner_id ON todo_lists(owner_id);
CREATE INDEX IF NOT EXISTS idx_todo_list_shares_todo_list_id ON todo_list_shares(todo_list_id);
CREATE INDEX IF NOT EXISTS idx_todo_list_shares_user_id ON todo_list_shares(user_id);
