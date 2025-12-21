IF OBJECT_ID('sp_InsertStaff', 'P') IS NOT NULL
    DROP PROCEDURE sp_InsertStaff;
GO
CREATE PROCEDURE sp_InsertStaff
    @Username NVARCHAR(50),
    @Password NVARCHAR(50),
    @NomeCompleto NVARCHAR(100),
    @Role NVARCHAR(20)
AS
BEGIN
    INSERT INTO Staff (Username, Password, NomeCompleto, Role)
    VALUES (@Username, @Password, @NomeCompleto, @Role);
END;