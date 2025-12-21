CREATE PROCEDURE sp_InsertResult
    @PosicaoGrid INT,
    @TempoFinal TIME = NULL,
    @PosicaoFinal INT,
    @Status NVARCHAR(50),
    @Pontos DECIMAL(10,2),
    @NomeSessao NVARCHAR(100),
    @NomeGP NVARCHAR(100),
    @ID_Piloto INT
AS
BEGIN
    INSERT INTO Resultados (PosiçãoGrid, TempoFinal, PosiçãoFinal, Status, Pontos, NomeSessão, NomeGP, ID_Piloto)
    VALUES (@PosicaoGrid, @TempoFinal, @PosicaoFinal, @Status, @Pontos, @NomeSessao, @NomeGP, @ID_Piloto)
END
GO
