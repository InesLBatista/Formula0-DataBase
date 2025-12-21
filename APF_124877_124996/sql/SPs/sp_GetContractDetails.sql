IF OBJECT_ID('sp_GetContractDetails', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetContractDetails;
GO
CREATE PROCEDURE sp_GetContractDetails
    @ContractID INT
AS
BEGIN
    SELECT c.*, m.Nome, m.Nacionalidade
    FROM Contrato c
    INNER JOIN Membros_da_Equipa m ON c.ID_Membro = m.ID_Membro
    WHERE c.ID_Contrato = @ContractID;
END;