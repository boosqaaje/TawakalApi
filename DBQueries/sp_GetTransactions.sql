CREATE PROCEDURE sp_GetTransactions
@PartnerUsername NVARCHAR (50)
AS
BEGIN
    -- Prevents extra result sets from interfering with SELECT statements
    SET NOCOUNT ON;
    SELECT   Id,
             PartnerUsername,
             ReferenceId,
             Amount,
             Currency,
             ServiceCode,
             Status,
             CreatedAt
    FROM     Transactions
    WHERE    PartnerUsername = @PartnerUsername
    ORDER BY CreatedAt DESC;
END