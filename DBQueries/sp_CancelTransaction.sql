USE TawakalDb;
GO

-- 2. Cancel Transaction Stored Procedure
CREATE OR ALTER PROCEDURE sp_CancelTransaction
    @ReferenceId NVARCHAR(100),
    @Reason NVARCHAR(250),
    @ResponseCode INT OUTPUT,
    @ResponseMessage NVARCHAR(250) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if transaction exists
    DECLARE @CurrentStatus NVARCHAR(50);
    SELECT @CurrentStatus = Status FROM Transactions WHERE ReferenceId = @ReferenceId;

    IF @CurrentStatus IS NULL
    BEGIN
        SET @ResponseCode = 404;
        SET @ResponseMessage = 'Transaction not found.';
        RETURN;
    END

    -- Check if it can be cancelled (e.g., must be SUCCESS or PENDING)
    IF @CurrentStatus = 'CANCELLED'
    BEGIN
        SET @ResponseCode = 400;
        SET @ResponseMessage = 'Transaction is already cancelled.';
        RETURN;
    END

    BEGIN TRY
        -- Update the status to CANCELLED (you can also log the reason in an audit table if needed)
        UPDATE Transactions 
        SET Status = 'CANCELLED'
        WHERE ReferenceId = @ReferenceId;

        SET @ResponseCode = 0;
        SET @ResponseMessage = 'Transaction cancelled successfully.';
    END TRY
    BEGIN CATCH
        SET @ResponseCode = 500;
        SET @ResponseMessage = ERROR_MESSAGE();
    END CATCH
END;
GO