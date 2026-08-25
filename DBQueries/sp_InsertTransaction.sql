CREATE OR ALTER PROCEDURE sp_InsertTransaction
@PartnerUsername NVARCHAR (100), 
@ReferenceId NVARCHAR (100), 
@Amount DECIMAL (18, 2), 
@Currency NVARCHAR (10), 
@ServiceCode NVARCHAR (50), 
@ResponseCode INT OUTPUT, 
@ResponseMessage NVARCHAR (250) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    -- 1. Check if partner is active
    IF NOT EXISTS (SELECT 1
                   FROM   T_a_partners
                   WHERE  PartnerUsername = @PartnerUsername
                          AND IsActive = 1)
        BEGIN
            SET @ResponseCode = 401;
            SET @ResponseMessage = 'Unauthorized or inactive partner username.';
            RETURN;
        END
    -- 2. Check for duplicate reference ID (Idempotency check)
    IF EXISTS (SELECT 1
               FROM   Transactions
               WHERE  ReferenceId = @ReferenceId)
        BEGIN
            SET @ResponseCode = 903;
            SET @ResponseMessage = 'Duplicate transaction reference ID.';
            RETURN;
        END
    -- 3. Insert the transaction
    BEGIN TRY
        INSERT  INTO Transactions (
            PartnerUsername,
            ReferenceId,
            Amount,
            Currency,
            ServiceCode,
            Status
        )
        VALUES                   (@PartnerUsername, @ReferenceId, @Amount, @Currency, @ServiceCode, 'PENDING');
        SET @ResponseCode = 200;
        SET @ResponseMessage = 'Transaction processed successfully.';
    END TRY
    BEGIN CATCH
        SET @ResponseCode = 500;
        SET @ResponseMessage = ERROR_MESSAGE();
    END CATCH
END