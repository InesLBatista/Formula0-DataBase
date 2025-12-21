CREATE PROCEDURE sp_UpdateGrandPrix
    @NomeGP NVARCHAR(100),
    @DataCorrida DATE,
    @ID_Circuito INT,
    @Season INT
AS
BEGIN
    UPDATE Grande_Prémio
    SET DataCorrida = @DataCorrida,
        ID_Circuito = @ID_Circuito,
        Ano_Temporada = @Season
    WHERE NomeGP = @NomeGP;
END