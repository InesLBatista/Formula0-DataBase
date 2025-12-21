IF OBJECT_ID('vw_Season_GPCount', 'V') IS NOT NULL
	DROP VIEW vw_Season_GPCount;
GO
CREATE VIEW vw_Season_GPCount AS
SELECT Ano_Temporada AS Year, COUNT(*) AS GPCount
FROM Grande_Prémio
GROUP BY Ano_Temporada;