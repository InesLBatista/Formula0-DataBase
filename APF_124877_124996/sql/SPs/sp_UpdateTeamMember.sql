CREATE OR ALTER PROCEDURE sp_UpdateTeamMember
    @ID_Membro INT,
    @Nome NVARCHAR(100),
    @Nacionalidade NVARCHAR(100),
    @DataNascimento DATE,
    @Género NVARCHAR(10),
    @Função NVARCHAR(100),
    @ID_Equipa INT
AS
BEGIN
    UPDATE Membros_da_Equipa
    SET Nome = @Nome,
        Nacionalidade = @Nacionalidade,
        DataNascimento = @DataNascimento,
        Género = @Género,
        Função = @Função,
        ID_Equipa = @ID_Equipa
    WHERE ID_Membro = @ID_Membro;
END
