-- =============================================
-- SP: Get Driver Standings for a specific season
-- =============================================

IF OBJECT_ID('sp_GetDriverStandingsBySeason', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetDriverStandingsBySeason;
GO

CREATE PROCEDURE sp_GetDriverStandingsBySeason
    @Season INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        ROW_NUMBER() OVER (ORDER BY ISNULL(SUM(r.Pontos), 0) DESC, 
                                    COUNT(CASE WHEN r.PosiçãoFinal = 1 THEN 1 END) DESC) AS Position,
        ISNULL(m.Nome, 'Unknown Driver') AS Driver,
        ISNULL(e.Nome, 'No Team') AS Team,
        ISNULL(SUM(r.Pontos), 0) AS TotalPoints,
        COUNT(CASE WHEN r.PosiçãoFinal = 1 THEN 1 END) AS Wins,
        COUNT(CASE WHEN r.PosiçãoFinal <= 3 THEN 1 END) AS Podiums
    FROM Piloto p
    INNER JOIN Membros_da_Equipa m ON p.ID_Membro = m.ID_Membro
    INNER JOIN Equipa e ON p.ID_Equipa = e.ID_Equipa
    INNER JOIN Resultados r ON p.ID_Piloto = r.ID_Piloto
    INNER JOIN Grande_Prémio gp ON r.NomeGP = gp.NomeGP
    WHERE r.NomeSessão = 'Race' AND gp.Ano_Temporada = @Season
    GROUP BY p.ID_Piloto, m.Nome, e.Nome
    HAVING ISNULL(SUM(r.Pontos), 0) > 0
    ORDER BY TotalPoints DESC, Wins DESC;
END;
GO

PRINT 'Stored Procedure sp_GetDriverStandingsBySeason criada com sucesso!';

-- Exemplo de uso:
-- EXEC sp_GetDriverStandingsBySeason @Season = 2024;
