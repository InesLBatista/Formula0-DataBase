CREATE VIEW vw_Results_Detailed AS
SELECT 
    r.ID_Resultado,
    r.PosiçãoGrid,
    r.TempoFinal,
    r.PosiçãoFinal,
    r.Status,
    r.Pontos,
    r.NomeSessão,
    r.ID_Piloto,
    p.NumeroPermanente,
    p.Abreviação AS DriverCode,
    m.Nome AS DriverName,
    e.Nome AS TeamName,
    ISNULL(r.NomeGP, s.NomeGP) AS GrandPrix
FROM Resultados r
INNER JOIN Piloto p ON r.ID_Piloto = p.ID_Piloto
LEFT JOIN Membros_da_Equipa m ON p.ID_Membro = m.ID_Membro
LEFT JOIN Equipa e ON p.ID_Equipa = e.ID_Equipa
LEFT JOIN Sessões s ON r.NomeSessão = s.NomeSessão 
    AND (r.NomeGP = s.NomeGP OR r.NomeGP IS NULL);