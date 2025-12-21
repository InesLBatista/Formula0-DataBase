IF OBJECT_ID('sp_UpdateSession', 'P') IS NOT NULL
    DROP PROCEDURE sp_UpdateSession;
GO
CREATE PROCEDURE sp_UpdateSession
    @NomeSessao NVARCHAR(100),
    @Estado NVARCHAR(50),
    @CondicoesPista NVARCHAR(100),
    @NomeGP NVARCHAR(100)
AS
BEGIN
    UPDATE Sessões
    SET Estado = @Estado,
        CondiçõesPista = @CondicoesPista
    WHERE NomeSessão = @NomeSessao AND NomeGP = @NomeGP
END
GO
