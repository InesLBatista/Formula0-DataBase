IF OBJECT_ID('sp_InsertTeamGrid', 'P') IS NOT NULL
    DROP PROCEDURE sp_InsertTeamGrid;
GO
CREATE PROCEDURE sp_InsertTeamGrid
    @Nome NVARCHAR(100),
    @Nacionalidade NVARCHAR(100),
    @AnoEstreia INT
AS
BEGIN
    INSERT INTO Equipa (Nome, Nacionalidade, Base, ChefeEquipa, ChefeTécnico, AnoEstreia, ModeloChassis, Power_Unit)
    VALUES (@Nome, @Nacionalidade, 'TBD', 'TBD', 'TBD', @AnoEstreia, 'TBD', 'TBD');
END;