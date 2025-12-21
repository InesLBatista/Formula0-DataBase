CREATE VIEW vw_Seasons_Overview AS
SELECT 
    t.Ano,
    ISNULL(gp.GPCount, 0) AS NumCorridas,
    ld.DriverName AS LeaderDriver,
    lt.TeamName AS LeaderTeam
FROM Temporada t
LEFT JOIN (
    SELECT Ano_Temporada, COUNT(*) AS GPCount
    FROM Grande_Prémio
    GROUP BY Ano_Temporada
) gp ON t.Ano = gp.Ano_Temporada
OUTER APPLY (
    SELECT TOP 1
        m.Nome AS DriverName,
        SUM(r.Pontos) AS TotalPoints
    FROM Grande_Prémio gp2
    INNER JOIN Resultados r ON r.NomeGP = gp2.NomeGP
    INNER JOIN Piloto p ON r.ID_Piloto = p.ID_Piloto
    INNER JOIN Membros_da_Equipa m ON p.ID_Membro = m.ID_Membro
    WHERE gp2.Ano_Temporada = t.Ano AND r.NomeSessão = 'Race'
    GROUP BY m.Nome, p.NumeroPermanente
    ORDER BY TotalPoints DESC, p.NumeroPermanente ASC
) ld
OUTER APPLY (
    SELECT TOP 1
        e.Nome AS TeamName,
        SUM(r.Pontos) AS TotalPoints
    FROM Grande_Prémio gp3
    INNER JOIN Resultados r ON r.NomeGP = gp3.NomeGP
    INNER JOIN Piloto p ON r.ID_Piloto = p.ID_Piloto
    INNER JOIN Equipa e ON p.ID_Equipa = e.ID_Equipa
    WHERE gp3.Ano_Temporada = t.Ano AND r.NomeSessão = 'Race'
    GROUP BY e.Nome
    ORDER BY TotalPoints DESC, e.Nome ASC
) lt
ORDER BY t.Ano DESC;
