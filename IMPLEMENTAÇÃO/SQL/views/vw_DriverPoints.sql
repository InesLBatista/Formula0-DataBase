CREATE OR ALTER VIEW vw_DriverPoints AS
SELECT TOP 15
    p.Abreviação,
    m.Nome AS DriverName,
    e.Nome AS TeamName,
    ISNULL(SUM(r.Pontos), 0) AS TotalPoints
FROM Piloto p
INNER JOIN Membros_da_Equipa m ON p.ID_Membro = m.ID_Membro
LEFT JOIN Equipa e ON p.ID_Equipa = e.ID_Equipa
LEFT JOIN Resultados r ON p.ID_Piloto = r.ID_Piloto
GROUP BY p.Abreviação, m.Nome, p.NumeroPermanente, e.Nome
ORDER BY TotalPoints DESC, p.NumeroPermanente ASC;
