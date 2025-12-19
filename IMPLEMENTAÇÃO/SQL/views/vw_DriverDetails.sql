CREATE OR ALTER VIEW vw_DriverDetails AS
SELECT 
    p.ID_Piloto,
    p.NumeroPermanente,
    p.Abreviação,
    p.ID_Equipa,
    e.Nome AS TeamName,
    p.ID_Membro,
    m.Nome AS DriverName
FROM Piloto p
LEFT JOIN Equipa e ON p.ID_Equipa = e.ID_Equipa
LEFT JOIN Membros_da_Equipa m ON p.ID_Membro = m.ID_Membro;
