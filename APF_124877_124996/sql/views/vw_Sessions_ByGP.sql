IF OBJECT_ID('vw_Sessions_ByGP', 'V') IS NOT NULL
    DROP VIEW vw_Sessions_ByGP;
GO
CREATE VIEW vw_Sessions_ByGP AS
SELECT 
    NomeSessão,
    Estado,
    CondiçõesPista,
    NomeGP
FROM Sessões;