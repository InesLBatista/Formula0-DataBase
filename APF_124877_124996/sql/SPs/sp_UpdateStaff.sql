IF OBJECT_ID('sp_UpdateStaff', 'P') IS NOT NULL
    DROP PROCEDURE sp_UpdateStaff;
GO
CREATE PROCEDURE sp_UpdateStaff
    @StaffID INT,
    @Username NVARCHAR(50),
    @Password NVARCHAR(50),
    @NomeCompleto NVARCHAR(100),
    @Role NVARCHAR(20)
AS
BEGIN
    UPDATE Staff 
    SET Username = @Username, 
        Password = @Password, 
        NomeCompleto = @NomeCompleto,
        Role = @Role
    WHERE StaffID = @StaffID;
END;