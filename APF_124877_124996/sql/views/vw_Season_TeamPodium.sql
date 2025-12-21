IF OBJECT_ID('vw_Season_TeamPodium', 'V') IS NOT NULL
    DROP VIEW vw_Season_TeamPodium;
GO
CREATE VIEW vw_Season_TeamPodium AS
SELECT 
    gp.Ano_Temporada AS Year,
    e.Nome AS TeamName,
    ISNULL(SUM(r.Pontos), 0) AS TotalPoints
FROM Grande_Prémio gp
INNER JOIN Resultados r ON r.NomeGP = gp.NomeGP
INNER JOIN Piloto p ON r.ID_Piloto = p.ID_Piloto
INNER JOIN Equipa e ON p.ID_Equipa = e.ID_Equipa
WHERE r.NomeSessão = 'Race'
GROUP BY gp.Ano_Temporada, e.ID_Equipa, e.Nome;