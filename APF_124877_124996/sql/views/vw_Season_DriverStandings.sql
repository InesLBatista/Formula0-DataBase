-- vw_Season_DriverStandings.sql
-- View for driver standings by season
go

CREATE OR ALTER VIEW vw_Season_DriverStandings AS
SELECT 
    ROW_NUMBER() OVER (ORDER BY ISNULL(SUM(r.Pontos), 0) DESC, COUNT(CASE WHEN r.PosiçãoFinal = 1 THEN 1 END) DESC) AS Position,
    ISNULL(m.Nome, 'Unknown Driver') AS Driver,
    ISNULL(e.Nome, 'No Team') AS Team,
    ISNULL(SUM(r.Pontos), 0) AS TotalPoints,
    COUNT(CASE WHEN r.PosiçãoFinal = 1 THEN 1 END) AS Wins,
    COUNT(CASE WHEN r.PosiçãoFinal <= 3 THEN 1 END) AS Podiums,
    gp.Ano_Temporada AS Year
FROM Piloto p
INNER JOIN Membros_da_Equipa m ON p.ID_Membro = m.ID_Membro
INNER JOIN Equipa e ON p.ID_Equipa = e.ID_Equipa
INNER JOIN Resultados r ON p.ID_Piloto = r.ID_Piloto
INNER JOIN Grande_Prémio gp ON r.NomeGP = gp.NomeGP
WHERE r.NomeSessão = 'Race'
GROUP BY p.ID_Piloto, m.Nome, e.Nome, gp.Ano_Temporada
HAVING ISNULL(SUM(r.Pontos), 0) > 0;
