
IF OBJECT_ID('trg_ValidateResultPoints', 'TR') IS NOT NULL
    DROP TRIGGER trg_ValidateResultPoints;
GO

CREATE TRIGGER trg_ValidateResultPoints
ON Resultados
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    IF EXISTS (SELECT 1 FROM inserted WHERE Pontos < 0)
    BEGIN
        RAISERROR('Os pontos não podem ser negativos!', 16, 1);
        ROLLBACK TRANSACTION;
    END
    
    IF EXISTS (SELECT 1 FROM inserted WHERE PosiçãoFinal < 0)
    BEGIN
        RAISERROR('A posição final não pode ser negativa!', 16, 1);
        ROLLBACK TRANSACTION;
    END
END;
GO

PRINT 'Trigger trg_ValidateResultPoints criado com sucesso!';
