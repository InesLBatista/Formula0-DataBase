-- =============================================
-- Indexes para melhorar performance
-- =============================================
USE p3g9;
GO

-- Indexes para Resultados (tabela mais consultada)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Resultados_Piloto' AND object_id = OBJECT_ID('Resultados'))
    CREATE NONCLUSTERED INDEX IX_Resultados_Piloto ON Resultados(ID_Piloto);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Resultados_Session' AND object_id = OBJECT_ID('Resultados'))
    CREATE NONCLUSTERED INDEX IX_Resultados_Session ON Resultados(NomeSessão) INCLUDE (Pontos, PosiçãoFinal);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Resultados_GP' AND object_id = OBJECT_ID('Resultados'))
    CREATE NONCLUSTERED INDEX IX_Resultados_GP ON Resultados(NomeGP, NomeSessão);
GO

-- Indexes para Grande_Prémio
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_GP_Season' AND object_id = OBJECT_ID('Grande_Prémio'))
    CREATE NONCLUSTERED INDEX IX_GP_Season ON Grande_Prémio(Ano_Temporada) INCLUDE (NomeGP, DataCorrida);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_GP_Circuit' AND object_id = OBJECT_ID('Grande_Prémio'))
    CREATE NONCLUSTERED INDEX IX_GP_Circuit ON Grande_Prémio(ID_Circuito);
GO

-- Indexes para Piloto
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Piloto_Team' AND object_id = OBJECT_ID('Piloto'))
    CREATE NONCLUSTERED INDEX IX_Piloto_Team ON Piloto(ID_Equipa);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Piloto_Member' AND object_id = OBJECT_ID('Piloto'))
    CREATE NONCLUSTERED INDEX IX_Piloto_Member ON Piloto(ID_Membro);
GO

-- Indexes para Membros_da_Equipa
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Members_Team' AND object_id = OBJECT_ID('Membros_da_Equipa'))
    CREATE NONCLUSTERED INDEX IX_Members_Team ON Membros_da_Equipa(ID_Equipa);
GO

PRINT 'Indexes criados com sucesso!';
