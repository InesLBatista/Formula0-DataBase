CREATE PROCEDURE sp_InsertDriver
    @NumeroPermanente INT,
    @Abreviacao NVARCHAR(3),
    @ID_Equipa INT,
    @ID_Membro INT
AS
BEGIN
    INSERT INTO Piloto (NumeroPermanente, Abreviação, ID_Equipa, ID_Membro)
    VALUES (@NumeroPermanente, @Abreviacao, @ID_Equipa, @ID_Membro);
END