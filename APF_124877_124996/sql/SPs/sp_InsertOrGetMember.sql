IF OBJECT_ID('sp_InsertOrGetMember', 'P') IS NOT NULL
    DROP PROCEDURE sp_InsertOrGetMember;
GO
CREATE PROCEDURE sp_InsertOrGetMember
    @Nome NVARCHAR(100),
    @Nacionalidade NVARCHAR(100) = NULL,
    @DataNascimento DATE = NULL,
    @Genero NVARCHAR(10) = NULL,
    @Funcao NVARCHAR(100) = NULL,
    @ID_Equipa INT = NULL,
    @ID_Membro INT OUTPUT
AS
BEGIN
    IF (@Funcao = 'Driver' AND @ID_Equipa IS NULL)
    BEGIN
        ;THROW 50001, 'Para criar um Driver tem de indicar a equipa (ID_Equipa).', 1;
    END

    IF NOT EXISTS (SELECT 1 FROM Membros_da_Equipa WHERE Nome = @Nome)
    BEGIN
        INSERT INTO Membros_da_Equipa (Nome, Nacionalidade, DataNascimento, Género, Função, ID_Equipa)
        VALUES (@Nome, @Nacionalidade, @DataNascimento, @Genero, @Funcao, @ID_Equipa);
    END
    ELSE
    BEGIN
        UPDATE Membros_da_Equipa
        SET Nacionalidade = COALESCE(@Nacionalidade, Nacionalidade),
            DataNascimento = COALESCE(@DataNascimento, DataNascimento),
            Género = COALESCE(@Genero, Género),
            Função = COALESCE(@Funcao, Função),
            ID_Equipa = COALESCE(@ID_Equipa, ID_Equipa)
        WHERE Nome = @Nome;
    END

    SELECT @ID_Membro = ID_Membro FROM Membros_da_Equipa WHERE Nome = @Nome;
END;
