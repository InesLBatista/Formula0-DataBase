-- =============================================
-- UDF: Obter nome completo do piloto
-- =============================================

IF OBJECT_ID('dbo.fn_GetDriverFullName', 'FN') IS NOT NULL
    DROP FUNCTION dbo.fn_GetDriverFullName;
GO

CREATE FUNCTION dbo.fn_GetDriverFullName(@DriverID INT)
RETURNS NVARCHAR(100)
AS
BEGIN
    DECLARE @FullName NVARCHAR(100);
    
    SELECT @FullName = m.Nome
    FROM Piloto p
    INNER JOIN Membros_da_Equipa m ON p.ID_Membro = m.ID_Membro
    WHERE p.ID_Piloto = @DriverID;
    
    RETURN ISNULL(@FullName, 'Unknown');
END;
GO

PRINT 'UDF fn_GetDriverFullName criada com sucesso!';

-- Exemplo de uso:
-- SELECT dbo.fn_GetDriverFullName(1) AS NomeCompleto;
