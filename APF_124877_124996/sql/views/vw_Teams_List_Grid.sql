IF OBJECT_ID('vw_Teams_List_Grid', 'V') IS NOT NULL
    DROP VIEW vw_Teams_List_Grid;
GO
CREATE VIEW vw_Teams_List_Grid AS
SELECT ID_Equipa, Nome, Nacionalidade, AnoEstreia FROM Equipa;