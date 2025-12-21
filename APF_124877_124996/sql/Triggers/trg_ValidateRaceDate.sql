
IF OBJECT_ID('trg_ValidateRaceDate', 'TR') IS NOT NULL
    DROP TRIGGER trg_ValidateRaceDate;
GO

CREATE TRIGGER trg_ValidateRaceDate
ON Grande_Prémio
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    IF EXISTS (
        SELECT 1 FROM inserted 
        WHERE DataCorrida > DATEADD(YEAR, 2, GETDATE())
    )
    BEGIN
        RAISERROR('A data da corrida não pode ser mais de 2 anos no futuro!', 16, 1);
        ROLLBACK TRANSACTION;
    END
END;
GO

PRINT 'Trigger trg_ValidateRaceDate criado com sucesso!';
