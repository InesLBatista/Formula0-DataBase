IF OBJECT_ID('sp_DeleteResult', 'P') IS NOT NULL
DROP PROCEDURE sp_DeleteResult;
GO
CREATE PROCEDURE sp_DeleteResult
    @ID_Resultado INT
AS
BEGIN
    DELETE FROM Resultados WHERE ID_Resultado = @ID_Resultado
END
GO\
