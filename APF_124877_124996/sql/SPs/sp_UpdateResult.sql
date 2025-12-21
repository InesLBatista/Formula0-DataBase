CREATE PROCEDURE sp_UpdateResult
    @ID_Resultado INT,
    @PosicaoGrid INT,
    @TempoFinal TIME = NULL,
    @PosicaoFinal INT,
    @Status NVARCHAR(50),
    @Pontos DECIMAL(10,2),
    @NomeGP NVARCHAR(100),
    @ID_Piloto INT
AS
BEGIN
    UPDATE Resultados
    SET PosiçãoGrid = @PosicaoGrid,
        TempoFinal = @TempoFinal,
        PosiçãoFinal = @PosicaoFinal,
        Status = @Status,
        Pontos = @Pontos,
        NomeGP = @NomeGP,
        ID_Piloto = @ID_Piloto
    WHERE ID_Resultado = @ID_Resultado
END
GO
