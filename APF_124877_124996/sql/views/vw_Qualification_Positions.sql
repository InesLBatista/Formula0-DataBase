CREATE VIEW vw_Qualification_Positions AS
SELECT r.ID_Piloto, r.PosiçãoFinal, s.NomeSessão, s.NomeGP
FROM Resultados r
INNER JOIN Sessões s ON r.NomeSessão = s.NomeSessão AND (r.NomeGP = s.NomeGP OR r.NomeGP IS NULL)
WHERE s.NomeSessão IN ('Qualification', 'Sprint Qualification');