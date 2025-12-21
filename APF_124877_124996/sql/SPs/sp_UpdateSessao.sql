-- UPDATE Sessão
GO
CREATE OR ALTER PROCEDURE sp_UpdateSessao
    @Estado NVARCHAR(50),
    @CondicoesPista NVARCHAR(50),
    @NomeSessao NVARCHAR(100),
    @NomeGP NVARCHAR(100)
AS
BEGIN
    UPDATE Sessões
    SET Estado = @Estado, CondiçõesPista = @CondicoesPista
    WHERE NomeSessão = @NomeSessao AND NomeGP = @NomeGP
END
GO
