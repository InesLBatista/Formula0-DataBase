CREATE PROCEDURE sp_InsertCircuit
    @Nome NVARCHAR(100),
    @Cidade NVARCHAR(100),
    @Pais NVARCHAR(100),
    @Comprimento_km FLOAT,
    @NumCurvas INT,
    @ID_Circuito INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Circuito (Nome, Cidade, Pais, Comprimento_km, NumCurvas)
    VALUES (@Nome, @Cidade, @Pais, @Comprimento_km, @NumCurvas);
    SET @ID_Circuito = SCOPE_IDENTITY();
END
