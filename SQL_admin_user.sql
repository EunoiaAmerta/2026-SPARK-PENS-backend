-- Catatan Penting / Important Note:
-- Kamu TIDAK PERLU menjalankan SQL ini secara manual!
-- Cukup login dengan username "admin" dan password "admin" di halaman login,
-- dan sistem akan otomatis membuat akun admin untukmu.

-- SQL ini hanya sebagai backup jika auto-creation tidak bekerja

-- JIKA ADMIN TIDAK BISA LOGIN, JALANKAN SQL INI DULU UNTUK HAPUS USER ADMIN LAMA:
DELETE FROM "Users" WHERE "Role" = 'Admin';

-- SQL to insert admin user directly into PostgreSQL database
-- This creates an admin user with username "admin" and password "admin"

INSERT INTO "Users" ("Email", "Name", "Role", "PasswordHash", "CreatedAt", "GoogleId")
VALUES (
    'admin@sparkpens.com',  -- Email
    'Admin',                -- Name
    'Admin',                -- Role (pastikan uppercase)
    '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyYfQ3Z9x9Wq',  -- BCrypt hash for "admin"
    NOW(),                  -- CreatedAt
    NULL                    -- GoogleId (null for manual login)
) ON CONFLICT DO NOTHING;

-- Verify the user was created
SELECT * FROM "Users" WHERE "Role" = 'Admin';

