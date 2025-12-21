CREATE OR ALTER PROCEDURE sp_DeleteTeamMember
    @ID_Membro INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @PilotId INT;
        SELECT @PilotId = ID_Piloto FROM Piloto WHERE ID_Membro = @ID_Membro;

        IF @PilotId IS NOT NULL
        BEGIN
            -- Apagar tudo o que referencia o piloto antes de remover o registo do piloto
            DELETE FROM Pitstop WHERE ID_Piloto = @PilotId;
            DELETE FROM Penalizações WHERE ID_Piloto = @PilotId;
            DELETE FROM Resultados WHERE ID_Piloto = @PilotId;
            DELETE FROM Piloto WHERE ID_Piloto = @PilotId;
        END

        DELETE FROM Contrato WHERE ID_Membro = @ID_Membro;
        DELETE FROM Membros_da_Equipa WHERE ID_Membro = @ID_Membro;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
