IF OBJECT_ID('sp_InsertContract', 'P') IS NOT NULL
    DROP PROCEDURE sp_InsertContract;
GO
CREATE PROCEDURE sp_InsertContract
    @AnoInicio INT,
    @AnoFim INT = NULL,
    @Funcao NVARCHAR(100) = NULL,
    @Salario DECIMAL(18,2),
    @Genero NVARCHAR(10) = NULL,
    @ID_Membro INT
AS
BEGIN
    INSERT INTO Contrato (AnoInicio, AnoFim, Função, Salário, Género, ID_Membro)
    VALUES (@AnoInicio, @AnoFim, @Funcao, @Salario, @Genero, @ID_Membro);
END;