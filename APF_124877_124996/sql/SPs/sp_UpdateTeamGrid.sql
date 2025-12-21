IF OBJECT_ID('sp_UpdateTeamGrid', 'P') IS NOT NULL
    DROP PROCEDURE sp_UpdateTeamGrid;
GO
CREATE PROCEDURE sp_UpdateTeamGrid
    @ID_Equipa INT,
    @Nome NVARCHAR(100),
    @Nacionalidade NVARCHAR(100),
    @AnoEstreia INT
AS
BEGIN
    UPDATE Equipa SET
        Nome = @Nome,
        Nacionalidade = @Nacionalidade,
        AnoEstreia = @AnoEstreia
    WHERE ID_Equipa = @ID_Equipa;
END;