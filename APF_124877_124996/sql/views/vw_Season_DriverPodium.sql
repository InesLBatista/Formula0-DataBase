IF OBJECT_ID('vw_Season_DriverPodium', 'V') IS NOT NULL
    DROP VIEW vw_Season_DriverPodium;
GO
CREATE VIEW vw_Season_DriverPodium AS
SELECT 
    gp.Ano_Temporada AS Year,
    p.Abreviação,
    m.Nome AS DriverName,
    ISNULL(SUM(r.Pontos), 0) AS TotalPoints
FROM Grande_Prémio gp
INNER JOIN Resultados r ON r.NomeGP = gp.NomeGP
INNER JOIN Piloto p ON r.ID_Piloto = p.ID_Piloto
INNER JOIN Membros_da_Equipa m ON p.ID_Membro = m.ID_Membro
WHERE r.NomeSessão = 'Race'
GROUP BY gp.Ano_Temporada, p.ID_Piloto, p.Abreviação, m.Nome, p.NumeroPermanente;