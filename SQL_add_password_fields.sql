-- This script adds the new columns needed for the Account Synchronization feature
-- Run this on your Neon PostgreSQL database

ALTER TABLE "Users" ADD COLUMN "HasPassword" BOOLEAN NOT NULL DEFAULT FALSE;
ALTER TABLE "Users" ADD COLUMN "ResetToken" VARCHAR(255);
ALTER TABLE "Users" ADD COLUMN "ResetExpiry" TIMESTAMP WITH TIME ZONE;

-- Also update existing admin user to have HasPassword = true
UPDATE "Users" SET "HasPassword" = TRUE WHERE "Role" = 'Admin';

-- Verify the columns were added
SELECT column_name, data_type, is_nullable 
FROM information_schema.columns 
WHERE table_name = 'Users' 
ORDER BY ordinal_position;
