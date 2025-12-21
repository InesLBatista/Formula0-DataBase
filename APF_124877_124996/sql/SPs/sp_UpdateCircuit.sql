CREATE PROCEDURE sp_UpdateCircuit
    @ID_Circuito INT,
    @Nome NVARCHAR(100),
    @Cidade NVARCHAR(100),
    @Pais NVARCHAR(100),
    @Comprimento_km FLOAT,
    @NumCurvas INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Circuito
    SET Nome = @Nome,
        Cidade = @Cidade,
        Pais = @Pais,
        Comprimento_km = @Comprimento_km,
        NumCurvas = @NumCurvas
    WHERE ID_Circuito = @ID_Circuito;
END
