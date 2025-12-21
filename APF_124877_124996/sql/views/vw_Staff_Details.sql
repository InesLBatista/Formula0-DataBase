IF OBJECT_ID('vw_Staff_Details', 'V') IS NOT NULL
    DROP VIEW vw_Staff_Details;
GO
CREATE VIEW vw_Staff_Details AS
SELECT 
    s.StaffID,
    s.Username,
    s.Password,
    s.NomeCompleto,
    s.Role,
    c.ID_Contrato,
    c.AnoInicio,
    c.AnoFim,
    c.Função,
    c.Salário,
    c.Género,
    c.ID_Membro
FROM Staff s
LEFT JOIN Membros_da_Equipa m ON s.NomeCompleto = m.Nome
LEFT JOIN Contrato c ON m.ID_Membro = c.ID_Membro;