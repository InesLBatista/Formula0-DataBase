CREATE PROCEDURE sp_InsertTeamMember
    @Nome NVARCHAR(100),
    @Nacionalidade NVARCHAR(100),
    @DataNascimento DATE,
    @Genero NVARCHAR(1),
    @ID_Equipa INT,
    @ID_Membro INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Membros_da_Equipa (Nome, Nacionalidade, DataNascimento, Género, Função, ID_Equipa)
    VALUES (@Nome, @Nacionalidade, @DataNascimento, @Genero, 'Driver', @ID_Equipa);
    
    SET @ID_Membro = SCOPE_IDENTITY();
END
