-- =============================================
-- SP: Get Team Standings for a specific season
-- =============================================

IF OBJECT_ID('sp_GetTeamStandingsBySeason', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetTeamStandingsBySeason;
GO

CREATE PROCEDURE sp_GetTeamStandingsBySeason
    @Season INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        ROW_NUMBER() OVER (ORDER BY ISNULL(SUM(r.Pontos), 0) DESC, 
                                    COUNT(CASE WHEN r.PosiçãoFinal = 1 THEN 1 END) DESC) AS Position,
        e.Nome AS Team,
        ISNULL(SUM(r.Pontos), 0) AS TotalPoints,
        COUNT(CASE WHEN r.PosiçãoFinal = 1 THEN 1 END) AS Wins,
        COUNT(CASE WHEN r.PosiçãoFinal <= 3 THEN 1 END) AS Podiums
    FROM Equipa e
    INNER JOIN Piloto p ON e.ID_Equipa = p.ID_Equipa
    INNER JOIN Resultados r ON p.ID_Piloto = r.ID_Piloto
    INNER JOIN Grande_Prémio gp ON r.NomeGP = gp.NomeGP
    WHERE r.NomeSessão = 'Race' AND gp.Ano_Temporada = @Season
    GROUP BY e.ID_Equipa, e.Nome
    HAVING ISNULL(SUM(r.Pontos), 0) > 0
    ORDER BY TotalPoints DESC, Wins DESC;
END;
GO

PRINT 'Stored Procedure sp_GetTeamStandingsBySeason criada com sucesso!';

-- Exemplo de uso:
-- EXEC sp_GetTeamStandingsBySeason @Season = 2024;
