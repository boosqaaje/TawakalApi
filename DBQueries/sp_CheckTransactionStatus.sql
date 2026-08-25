USE TawakalDb;
GO

-- 1. Check Transaction Status Stored Procedure
CREATE OR ALTER PROCEDURE sp_CheckTransactionStatus
    @ReferenceId NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        ReferenceId,
        PartnerUsername,
        Amount,
        Currency,
        ServiceCode,
        Status,
        CreatedAt
    FROM Transactions
    WHERE ReferenceId = @ReferenceId;
END;
GO