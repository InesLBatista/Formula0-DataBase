CREATE VIEW vw_Drivers_List AS
SELECT p.ID_Piloto, p.NumeroPermanente, p.Abreviação, m.Nome AS DriverName, e.Nome AS Team
FROM Piloto p
LEFT JOIN Membros_da_Equipa m ON p.ID_Membro = m.ID_Membro
LEFT JOIN Equipa e ON p.ID_Equipa = e.ID_Equipa;