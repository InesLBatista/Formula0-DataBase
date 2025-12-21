CREATE PROCEDURE sp_AuthenticateStaff
    @StaffID INT,
    @Password NVARCHAR(100)
AS
BEGIN
    SELECT COUNT(1) AS IsValid
    FROM Staff
    WHERE StaffID = @StaffID AND Password = @Password;
END