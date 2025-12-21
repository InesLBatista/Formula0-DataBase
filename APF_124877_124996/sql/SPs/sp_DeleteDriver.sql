CREATE OR ALTER PROCEDURE sp_DeleteDriver
    @ID_Piloto INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DELETE FROM Pitstop WHERE ID_Piloto = @ID_Piloto;
        DELETE FROM Penalizações WHERE ID_Piloto = @ID_Piloto;
        DELETE FROM Resultados WHERE ID_Piloto = @ID_Piloto;

        DELETE FROM Piloto WHERE ID_Piloto = @ID_Piloto;

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
