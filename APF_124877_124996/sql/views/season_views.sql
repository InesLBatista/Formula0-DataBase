USE p3g9;
GO

-- View: Season Summary with GP Count
IF OBJECT_ID('vw_SeasonSummary', 'V') IS NOT NULL
    DROP VIEW vw_SeasonSummary;
GO

CREATE VIEW vw_SeasonSummary AS
SELECT 
    t.Ano,
    ISNULL(gp.GPCount, 0) AS NumCorridas
FROM Temporada t
LEFT JOIN (
    SELECT Ano_Temporada, COUNT(*) AS GPCount
    FROM Grande_Prémio
    GROUP BY Ano_Temporada
) gp ON t.Ano = gp.Ano_Temporada;
GO

PRINT 'Views de Temporadas criadas com sucesso!';
