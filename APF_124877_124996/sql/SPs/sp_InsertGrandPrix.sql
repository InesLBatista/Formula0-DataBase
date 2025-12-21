CREATE PROCEDURE sp_InsertGrandPrix
    @NomeGP NVARCHAR(100),
    @DataCorrida DATE,
    @ID_Circuito INT,
    @Season INT
AS
BEGIN
    INSERT INTO Grande_Prémio (NomeGP, DataCorrida, ID_Circuito, Ano_Temporada)
    VALUES (@NomeGP, @DataCorrida, @ID_Circuito, @Season);
END