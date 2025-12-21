
USE p3g9;
GO

IF OBJECT_ID('vw_DriverStandings', 'V') IS NOT NULL
    DROP VIEW vw_DriverStandings;
GO

CREATE VIEW vw_DriverStandings AS
SELECT 
    ROW_NUMBER() OVER (ORDER BY ISNULL(SUM(r.Pontos), 0) DESC, 
                                COUNT(DISTINCT CASE WHEN r.PosiçãoFinal = 1 THEN r.ID_Resultado END) DESC) AS Position,
    p.ID_Piloto,
    ISNULL(p.NumeroPermanente, 0) AS Number,
    ISNULL(p.Abreviação, '---') AS Code,
    ISNULL(m.Nome, 'Unknown Driver') AS DriverName,
    ISNULL(m.Nacionalidade, 'Unknown') AS Nationality,
    ISNULL(e.Nome, 'No Team') AS Team,
    ISNULL(SUM(r.Pontos), 0) AS TotalPoints,
    COUNT(DISTINCT CASE WHEN r.PosiçãoFinal = 1 THEN r.ID_Resultado END) AS Wins,
    COUNT(DISTINCT CASE WHEN r.PosiçãoFinal <= 3 THEN r.ID_Resultado END) AS Podiums,
    COUNT(DISTINCT CASE WHEN r.NomeSessão = 'Race' THEN r.ID_Resultado END) AS Races
FROM Piloto p
INNER JOIN Membros_da_Equipa m ON p.ID_Membro = m.ID_Membro
INNER JOIN Equipa e ON p.ID_Equipa = e.ID_Equipa
INNER JOIN Resultados r ON p.ID_Piloto = r.ID_Piloto
WHERE r.NomeSessão = 'Race'
GROUP BY p.ID_Piloto, p.NumeroPermanente, p.Abreviação, m.Nome, m.Nacionalidade, e.Nome
HAVING ISNULL(SUM(r.Pontos), 0) > 0;
GO

-- View: Team Standings
IF OBJECT_ID('vw_TeamStandings', 'V') IS NOT NULL
    DROP VIEW vw_TeamStandings;
GO

CREATE VIEW vw_TeamStandings AS
SELECT 
    ROW_NUMBER() OVER (ORDER BY ISNULL(SUM(r.Pontos), 0) DESC, 
                                COUNT(DISTINCT CASE WHEN r.PosiçãoFinal = 1 THEN r.ID_Resultado END) DESC) AS Position,
    e.ID_Equipa,
    e.Nome AS Team,
    ISNULL(e.Nacionalidade, 'Unknown') AS Nationality,
    COUNT(DISTINCT p.ID_Piloto) AS Drivers,
    ISNULL(SUM(r.Pontos), 0) AS TotalPoints,
    COUNT(DISTINCT CASE WHEN r.PosiçãoFinal = 1 THEN r.ID_Resultado END) AS Wins,
    COUNT(DISTINCT CASE WHEN r.PosiçãoFinal <= 3 THEN r.ID_Resultado END) AS Podiums
FROM Equipa e
INNER JOIN Piloto p ON e.ID_Equipa = p.ID_Equipa
INNER JOIN Resultados r ON p.ID_Piloto = r.ID_Piloto
WHERE r.NomeSessão = 'Race'
GROUP BY e.ID_Equipa, e.Nome, e.Nacionalidade
HAVING ISNULL(SUM(r.Pontos), 0) > 0;
GO

-- Driver Standings by Season
IF OBJECT_ID('vw_DriverStandingsBySeason', 'V') IS NOT NULL
    DROP VIEW vw_DriverStandingsBySeason;
GO

CREATE VIEW vw_DriverStandingsBySeason AS
SELECT 
    gp.Ano_Temporada AS Season,
    ROW_NUMBER() OVER (PARTITION BY gp.Ano_Temporada 
                       ORDER BY ISNULL(SUM(r.Pontos), 0) DESC, 
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
WHERE r.NomeSessão = 'Race'
GROUP BY gp.Ano_Temporada, p.ID_Piloto, m.Nome, e.Nome
HAVING ISNULL(SUM(r.Pontos), 0) > 0;
GO

-- Team Standings by Season
IF OBJECT_ID('vw_TeamStandingsBySeason', 'V') IS NOT NULL
    DROP VIEW vw_TeamStandingsBySeason;
GO

CREATE VIEW vw_TeamStandingsBySeason AS
SELECT 
    gp.Ano_Temporada AS Season,
    ROW_NUMBER() OVER (PARTITION BY gp.Ano_Temporada 
                       ORDER BY ISNULL(SUM(r.Pontos), 0) DESC, 
                                COUNT(CASE WHEN r.PosiçãoFinal = 1 THEN 1 END) DESC) AS Position,
    ISNULL(e.Nome, 'Unknown Team') AS Team,
    ISNULL(SUM(r.Pontos), 0) AS TotalPoints,
    COUNT(CASE WHEN r.PosiçãoFinal = 1 THEN 1 END) AS Wins,
    COUNT(CASE WHEN r.PosiçãoFinal <= 3 THEN 1 END) AS Podiums
FROM Equipa e
INNER JOIN Piloto p ON e.ID_Equipa = p.ID_Equipa
INNER JOIN Resultados r ON p.ID_Piloto = r.ID_Piloto
INNER JOIN Grande_Prémio gp ON r.NomeGP = gp.NomeGP
WHERE r.NomeSessão = 'Race'
GROUP BY gp.Ano_Temporada, e.ID_Equipa, e.Nome
HAVING ISNULL(SUM(r.Pontos), 0) > 0;
GO

PRINT 'Views de Standings criadas com sucesso!';
