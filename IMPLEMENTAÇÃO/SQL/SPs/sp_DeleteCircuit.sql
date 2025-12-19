CREATE PROCEDURE sp_DeleteCircuit
    @ID_Circuito INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM Circuito WHERE ID_Circuito = @ID_Circuito;
END
