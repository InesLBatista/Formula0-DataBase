CREATE OR ALTER PROCEDURE sp_GetTeamCareerPoints
AS
BEGIN
    SELECT * FROM vw_Team_CareerPoints ORDER BY TotalPoints DESC, TeamName ASC;
END