-- 1. Partners Table (To validate incoming usernames)
CREATE TABLE T_a_partners (
    Id                     INT            IDENTITY (1, 1) PRIMARY KEY,
    ClientId               NVARCHAR (100) NOT NULL UNIQUE,
    PartnerName            NVARCHAR (150) NOT NULL,
    CurrentSecretHash      TEXT           NOT NULL,
    NextSecretHash         TEXT          ,
    IsActive               BIT            DEFAULT 1 NOT NULL,
    CurrentSecretCreatedAt DATETIME2      DEFAULT GETDATE(),
    NextSecretCreatedAt    DATETIME2      NULL
);

SELECT *
FROM   T_a_partners;

UPDATE T_a_partners
SET    PartnerUsername = 'choice-remit';

TRUNCATE TABLE T_a_partners;

ALTER TABLE T_a_partners ALTER COLUMN PartnerUsername NVARCHAR NOT NULL;

-- 2. Services Table (For service lookups from Postman collection)
CREATE TABLE Services (
    Id          INT            IDENTITY (1, 1) PRIMARY KEY,
    ServiceCode NVARCHAR (50)  NOT NULL UNIQUE,
    ServiceName NVARCHAR (150) NOT NULL,
    IsActive    BIT            DEFAULT 1 NOT NULL
);

SELECT *
FROM   Services;

-- 3. Transactions Table (To store pushed transactions)
CREATE TABLE Transactions (
    Id              BIGINT          IDENTITY (1, 1) PRIMARY KEY,
    PartnerUsername NVARCHAR (100)  NOT NULL,
    ReferenceId     NVARCHAR (100)  NOT NULL UNIQUE,
    Amount          DECIMAL (18, 2) NOT NULL,
    Currency        NVARCHAR (10)   NOT NULL,
    ServiceCode     NVARCHAR (50)   NOT NULL,
    Status          NVARCHAR (50)   DEFAULT 'PENDING' NOT NULL,
    CreatedAt       DATETIME2       DEFAULT GETDATE()
);

SELECT *
FROM   Transactions;

SELECT *
FROM   [dbo].[T_a_portal_users];

delete from T_a_portal_users where Email <> 'boos@gmail.com';

TRUNCATE TABLE T_a_portal_users;

UPDATE Transactions
SET    [Status] = 'SUCCESS'
WHERE  ReferenceId = 'PR400';

-- Insert a test partner for local development testing
INSERT  INTO T_a_partners (
    ClientId,
    PartnerName,
    CurrentSecretHash,
    IsActive
)
VALUES                   ('propaid', 'ProPaid Norway Inc', 'hashed_secret_here', 1);