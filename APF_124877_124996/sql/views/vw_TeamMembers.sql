CREATE OR ALTER VIEW vw_TeamMembers AS
SELECT
    m.ID_Membro,
    m.Nome,
    m.Nacionalidade,
    m.DataNascimento,
    m.Género,
    m.Função,
    m.ID_Equipa,
    e.Nome AS TeamName
FROM Membros_da_Equipa m
INNER JOIN Equipa e ON m.ID_Equipa = e.ID_Equipa;
