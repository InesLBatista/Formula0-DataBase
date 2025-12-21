IF OBJECT_ID('sp_InsertSession', 'P') IS NOT NULL
    DROP PROCEDURE sp_InsertSession;
GO
CREATE PROCEDURE sp_InsertSession
    @NomeSessao NVARCHAR(100),
    @Estado NVARCHAR(50),
    @CondicoesPista NVARCHAR(100),
    @NomeGP NVARCHAR(100)
AS
BEGIN
    INSERT INTO Sessões (NomeSessão, Estado, CondiçõesPista, NomeGP)
    VALUES (@NomeSessao, @Estado, @CondicoesPista, @NomeGP)
END
GO
