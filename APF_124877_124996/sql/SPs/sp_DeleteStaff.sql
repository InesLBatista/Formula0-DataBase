CREATE OR ALTER PROCEDURE sp_DeleteStaff
    @StaffID INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ID_Membro INT;

    -- Tentar mapear o Staff para o membro correspondente
    SELECT TOP 1 @ID_Membro = m.ID_Membro
    FROM Staff s
    LEFT JOIN Membros_da_Equipa m ON m.Nome = s.NomeCompleto
    WHERE s.StaffID = @StaffID;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF @ID_Membro IS NOT NULL
        BEGIN
            EXEC sp_DeleteTeamMember @ID_Membro;
        END

        DELETE FROM Staff WHERE StaffID = @StaffID;

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
END;
