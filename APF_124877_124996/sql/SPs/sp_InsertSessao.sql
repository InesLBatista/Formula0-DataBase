-- INSERT Sessão
GO
CREATE OR ALTER PROCEDURE sp_InsertSessao
    @NomeSessao NVARCHAR(100),
    @Estado NVARCHAR(50),
    @CondicoesPista NVARCHAR(50),
    @NomeGP NVARCHAR(100)
AS
BEGIN
    INSERT INTO Sessões (NomeSessão, Estado, CondiçõesPista, NomeGP)
    VALUES (@NomeSessao, @Estado, @CondicoesPista, @NomeGP)
END
GO
