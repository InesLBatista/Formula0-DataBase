CREATE VIEW vw_GPList_BySeason AS
SELECT 
    NomeGP AS [Grand Prix Name],
    DataCorrida AS [Race Date],
    Ano_Temporada AS Season
FROM Grande_Prémio;