
IF OBJECT_ID('trg_UpdateSeasonRaceCount', 'TR') IS NOT NULL
    DROP TRIGGER trg_UpdateSeasonRaceCount;
GO

CREATE TRIGGER trg_UpdateSeasonRaceCount
ON Grande_Prémio
AFTER INSERT, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE t
    SET t.NumCorridas = (
        SELECT COUNT(*) 
        FROM Grande_Prémio gp 
        WHERE gp.Ano_Temporada = t.Ano
    )
    FROM Temporada t
    WHERE t.Ano IN (
        SELECT DISTINCT Ano_Temporada FROM inserted
        UNION
        SELECT DISTINCT Ano_Temporada FROM deleted
    );
END;
GO

PRINT 'Trigger trg_UpdateSeasonRaceCount criado com sucesso!';
