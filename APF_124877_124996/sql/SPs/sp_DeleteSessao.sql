-- DELETE Sessão
GO
CREATE OR ALTER PROCEDURE sp_DeleteSessao
    @NomeSessao NVARCHAR(100),
    @NomeGP NVARCHAR(100)
AS
BEGIN
    DELETE FROM Sessões WHERE NomeSessão = @NomeSessao AND NomeGP = @NomeGP
END
GO
