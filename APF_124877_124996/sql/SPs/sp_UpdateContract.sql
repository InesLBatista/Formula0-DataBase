IF OBJECT_ID('sp_UpdateContract', 'P') IS NOT NULL
    DROP PROCEDURE sp_UpdateContract;
GO
CREATE PROCEDURE sp_UpdateContract
    @ContractID INT,
    @AnoInicio INT,
    @AnoFim INT = NULL,
    @Funcao NVARCHAR(100) = NULL,
    @Salario DECIMAL(18,2),
    @Genero NVARCHAR(10) = NULL
AS
BEGIN
    UPDATE Contrato 
    SET AnoInicio = @AnoInicio, 
        AnoFim = @AnoFim, 
        Função = @Funcao, 
        Salário = @Salario, 
        Género = @Genero
    WHERE ID_Contrato = @ContractID;
END;