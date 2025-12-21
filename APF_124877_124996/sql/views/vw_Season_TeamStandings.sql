-- vw_Season_TeamStandings.sql
-- View or query for team standings by season
go

CREATE OR ALTER VIEW vw_Season_TeamStandings AS
SELECT 
    ROW_NUMBER() OVER (ORDER BY ISNULL(SUM(r.Pontos), 0) DESC, COUNT(CASE WHEN r.PosiçãoFinal = 1 THEN 1 END) DESC) AS Position,
    e.Nome AS Team,
    ISNULL(SUM(r.Pontos), 0) AS TotalPoints,
    COUNT(CASE WHEN r.PosiçãoFinal = 1 THEN 1 END) AS Wins,
    COUNT(CASE WHEN r.PosiçãoFinal <= 3 THEN 1 END) AS Podiums,
    gp.Ano_Temporada AS Year
FROM Equipa e
INNER JOIN Piloto p ON e.ID_Equipa = p.ID_Equipa
INNER JOIN Resultados r ON p.ID_Piloto = r.ID_Piloto
INNER JOIN Grande_Prémio gp ON r.NomeGP = gp.NomeGP
WHERE r.NomeSessão = 'Race'
GROUP BY e.ID_Equipa, e.Nome, gp.Ano_Temporada
HAVING ISNULL(SUM(r.Pontos), 0) > 0;
