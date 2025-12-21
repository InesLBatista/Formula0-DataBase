-- =============================================
-- UDF: Obter total de vitórias de um piloto
-- =============================================

IF OBJECT_ID('dbo.fn_GetDriverWins', 'FN') IS NOT NULL
    DROP FUNCTION dbo.fn_GetDriverWins;
GO

CREATE FUNCTION dbo.fn_GetDriverWins(@DriverID INT)
RETURNS INT
AS
BEGIN
    DECLARE @Wins INT;
    
    SELECT @Wins = COUNT(*)
    FROM Resultados r
    WHERE r.ID_Piloto = @DriverID 
      AND r.NomeSessão = 'Race' 
      AND r.PosiçãoFinal = 1;
    
    RETURN @Wins;
END;
GO

PRINT 'UDF fn_GetDriverWins criada com sucesso!';

-- Exemplo de uso:
-- SELECT dbo.fn_GetDriverWins(1) AS Vitorias;
