CREATE VIEW vw_GrandPrixDetails AS
SELECT 
    gp.NomeGP,
    gp.DataCorrida,
    c.Nome AS Circuit,
    gp.ID_Circuito,
    gp.Ano_Temporada AS Season
FROM Grande_Prémio gp
INNER JOIN Circuito c ON gp.ID_Circuito = c.ID_Circuito;