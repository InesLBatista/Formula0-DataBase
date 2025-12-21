-- =============================================
-- UDF: Obter total de pontos de um piloto
-- =============================================

IF OBJECT_ID('dbo.fn_GetDriverTotalPoints', 'FN') IS NOT NULL
    DROP FUNCTION dbo.fn_GetDriverTotalPoints;
GO

CREATE FUNCTION dbo.fn_GetDriverTotalPoints(@DriverID INT)
RETURNS INT
AS
BEGIN
    DECLARE @TotalPoints INT;
    
    SELECT @TotalPoints = ISNULL(SUM(r.Pontos), 0)
    FROM Resultados r
    WHERE r.ID_Piloto = @DriverID AND r.NomeSessão = 'Race';
    
    RETURN @TotalPoints;
END;
GO

PRINT 'UDF fn_GetDriverTotalPoints criada com sucesso!';

-- Exemplo de uso:
-- SELECT dbo.fn_GetDriverTotalPoints(1) AS TotalPontos;
