
CREATE OR ALTER PROCEDURE sp_AddDriver
    @Nome NVARCHAR(100),
    @Nacionalidade NVARCHAR(100),
    @DataNascimento DATE,
    @Género CHAR(1),
    @ID_Equipa INT,
    @NumeroPermanente INT,
    @Abreviação CHAR(3)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @ID_Membro INT;
    
    BEGIN TRANSACTION;
    BEGIN TRY
        -- 1. Inserir em Membros_da_Equipa
        INSERT INTO Membros_da_Equipa (Nome, Nacionalidade, DataNascimento, Género, Função, ID_Equipa)
        VALUES (@Nome, @Nacionalidade, @DataNascimento, @Género, 'Driver', @ID_Equipa);
        
        SET @ID_Membro = SCOPE_IDENTITY();
        
        -- 2. Inserir automaticamente em Piloto
        INSERT INTO Piloto (NumeroPermanente, Abreviação, ID_Equipa, ID_Membro)
        VALUES (@NumeroPermanente, @Abreviação, @ID_Equipa, @ID_Membro);
        
        COMMIT TRANSACTION;
        
        PRINT 'Piloto ' + @Nome + ' inserido com sucesso! (ID_Membro: ' + CAST(@ID_Membro AS NVARCHAR) + ')';
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        THROW 50000, @ErrorMessage, 1;
    END CATCH
END;
GO

