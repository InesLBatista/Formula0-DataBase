IF OBJECT_ID('sp_UpdateTeamDetails', 'P') IS NOT NULL
    DROP PROCEDURE sp_UpdateTeamDetails;
GO
CREATE PROCEDURE sp_UpdateTeamDetails
    @ID_Equipa INT,
    @Nome NVARCHAR(100),
    @Nacionalidade NVARCHAR(100) = NULL,
    @Base NVARCHAR(100) = NULL,
    @ChefeEquipa NVARCHAR(100) = NULL,
    @ChefeTécnico NVARCHAR(100) = NULL,
    @AnoEstreia INT = NULL,
    @ModeloChassis NVARCHAR(100) = NULL,
    @Power_Unit NVARCHAR(100) = NULL,
    @PilotosReserva NVARCHAR(100) = NULL
AS
BEGIN
    UPDATE Equipa SET
        Nome = @Nome,
        Nacionalidade = @Nacionalidade,
        Base = @Base,
        ChefeEquipa = @ChefeEquipa,
        ChefeTécnico = @ChefeTécnico,
        AnoEstreia = @AnoEstreia,
        ModeloChassis = @ModeloChassis,
        Power_Unit = @Power_Unit,
        PilotosReserva = @PilotosReserva
    WHERE ID_Equipa = @ID_Equipa;
END;