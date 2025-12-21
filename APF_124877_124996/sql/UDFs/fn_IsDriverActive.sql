-- =============================================
-- UDF: Verificar se um piloto está ativo (tem resultados recentes)
-- =============================================

IF OBJECT_ID('dbo.fn_IsDriverActive', 'FN') IS NOT NULL
    DROP FUNCTION dbo.fn_IsDriverActive;
GO

CREATE FUNCTION dbo.fn_IsDriverActive(@DriverID INT)
RETURNS BIT
AS
BEGIN
    DECLARE @IsActive BIT = 0;
    
    IF EXISTS (
        SELECT 1 
        FROM Resultados r
        INNER JOIN Grande_Prémio gp ON r.NomeGP = gp.NomeGP
        WHERE r.ID_Piloto = @DriverID 
          AND gp.Ano_Temporada >= YEAR(GETDATE()) - 1
    )
        SET @IsActive = 1;
    
    RETURN @IsActive;
END;
GO

PRINT 'UDF fn_IsDriverActive criada com sucesso!';

-- Exemplo de uso:
-- SELECT dbo.fn_IsDriverActive(1) AS EstaAtivo;
