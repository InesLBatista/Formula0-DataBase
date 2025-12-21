IF OBJECT_ID('sp_DeleteSession', 'P') IS NOT NULL
    DROP PROCEDURE sp_DeleteSession;
GO
CREATE PROCEDURE sp_DeleteSession
    @NomeSessao NVARCHAR(100),
    @NomeGP NVARCHAR(100)
AS
BEGIN
    DELETE FROM Sessões WHERE NomeSessão = @NomeSessao AND NomeGP = @NomeGP
END
GO
