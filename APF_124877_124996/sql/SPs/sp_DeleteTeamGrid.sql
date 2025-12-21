IF OBJECT_ID('sp_DeleteTeamGrid', 'P') IS NOT NULL
    DROP PROCEDURE sp_DeleteTeamGrid;
GO
CREATE PROCEDURE sp_DeleteTeamGrid
    @ID_Equipa INT
AS
BEGIN
    DELETE FROM Equipa WHERE ID_Equipa = @ID_Equipa;
END;