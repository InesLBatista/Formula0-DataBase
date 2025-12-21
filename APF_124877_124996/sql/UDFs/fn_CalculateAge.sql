-- =============================================
-- UDF: Calcular idade de um membro
-- =============================================

IF OBJECT_ID('dbo.fn_CalculateAge', 'FN') IS NOT NULL
    DROP FUNCTION dbo.fn_CalculateAge;
GO

CREATE FUNCTION dbo.fn_CalculateAge(@BirthDate DATE)
RETURNS INT
AS
BEGIN
    DECLARE @Age INT;
    
    SET @Age = DATEDIFF(YEAR, @BirthDate, GETDATE()) - 
               CASE 
                   WHEN (MONTH(@BirthDate) > MONTH(GETDATE())) OR 
                        (MONTH(@BirthDate) = MONTH(GETDATE()) AND DAY(@BirthDate) > DAY(GETDATE()))
                   THEN 1 
                   ELSE 0 
               END;
    
    RETURN @Age;
END;
GO

PRINT 'UDF fn_CalculateAge criada com sucesso!';

-- Exemplo de uso:
-- SELECT dbo.fn_CalculateAge('1990-01-15') AS Idade;
