CREATE VIEW vw_GPList_ByCircuit AS
SELECT 
    NomeGP AS [Grand Prix Name],
    DataCorrida AS [Race Date],
    Ano_Temporada AS Season,
    ID_Circuito
FROM Grande_Prémio;