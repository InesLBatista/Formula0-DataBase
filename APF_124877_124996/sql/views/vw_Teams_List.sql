IF OBJECT_ID('vw_Teams_List', 'V') IS NOT NULL
    DROP VIEW vw_Teams_List;
GO
CREATE VIEW vw_Teams_List AS
SELECT ID_Equipa, Nome FROM Equipa;