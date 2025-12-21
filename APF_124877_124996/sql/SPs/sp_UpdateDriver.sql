CREATE PROCEDURE sp_UpdateDriver
    @ID_Piloto INT,
    @NumeroPermanente INT,
    @Abreviacao NVARCHAR(3),
    @ID_Equipa INT,
    @ID_Membro INT
AS
BEGIN
    UPDATE Piloto
    SET NumeroPermanente = @NumeroPermanente,
        Abreviação = @Abreviacao,
        ID_Equipa = @ID_Equipa,
        ID_Membro = @ID_Membro
    WHERE ID_Piloto = @ID_Piloto;
END