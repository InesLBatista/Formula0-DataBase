IF OBJECT_ID('vw_Team_Details', 'V') IS NOT NULL
    DROP VIEW vw_Team_Details;
GO
CREATE VIEW vw_Team_Details AS
SELECT 
    e.ID_Equipa,
    e.Nome,
    e.Nacionalidade,
    e.Base,
    e.ChefeEquipa,
    ChefeTecnico = e.ChefeTécnico,
    e.AnoEstreia,
    e.ModeloChassis,
    e.Power_Unit,
    e.PilotosReserva,
    ReserveDrivers = (
        SELECT STRING_AGG(m.Nome, ', ')
        FROM Membros_da_Equipa m
        WHERE m.ID_Equipa = e.ID_Equipa
          AND m.Função = 'Reserve Driver'
    )
FROM Equipa e;
