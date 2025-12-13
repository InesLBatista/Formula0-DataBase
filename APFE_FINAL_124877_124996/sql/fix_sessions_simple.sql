-- ========================================
-- SIMPLE FIX: Change Sessions PRIMARY KEY
-- ========================================

PRINT '1. Dropping old foreign keys...';
-- Drop FKs que referenciam Sessões
IF OBJECT_ID('FK_Resultado_Sessao', 'F') IS NOT NULL
    ALTER TABLE Resultados DROP CONSTRAINT FK_Resultado_Sessao;
IF OBJECT_ID('FK_Penalizacao_Sessao', 'F') IS NOT NULL
    ALTER TABLE Penalizações DROP CONSTRAINT FK_Penalizacao_Sessao;
IF OBJECT_ID('FK_Pitstop_Sessao', 'F') IS NOT NULL
    ALTER TABLE Pitstop DROP CONSTRAINT FK_Pitstop_Sessao;
PRINT '   ✓ Foreign keys dropped';

PRINT '2. Dropping old PRIMARY KEY...';
-- Encontrar e remover PK antiga
DECLARE @ConstraintName nvarchar(200);
SELECT @ConstraintName = Name FROM SYS.KEY_CONSTRAINTS 
WHERE PARENT_OBJECT_ID = OBJECT_ID('Sessões') AND type = 'PK';
IF @ConstraintName IS NOT NULL
BEGIN
    EXEC('ALTER TABLE Sessões DROP CONSTRAINT ' + @ConstraintName);
    PRINT '   ✓ Dropped: ' + @ConstraintName;
END

PRINT '3. Adding composite PRIMARY KEY...';
-- Adicionar nova PK composta
ALTER TABLE Sessões 
ADD CONSTRAINT PK_Sessoes PRIMARY KEY (NomeGP, NomeSessão);
PRINT '   ✓ New PRIMARY KEY: (NomeGP, NomeSessão)';

PRINT '4. Adding NomeGP to Resultados (if needed)...';
-- Adicionar NomeGP a Resultados se não existir
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Resultados') AND name = 'NomeGP')
BEGIN
    ALTER TABLE Resultados ADD NomeGP NVARCHAR(100) NULL;
    EXEC('UPDATE r SET r.NomeGP = s.NomeGP FROM Resultados r INNER JOIN Sessões s ON r.NomeSessão = s.NomeSessão');
    ALTER TABLE Resultados ALTER COLUMN NomeGP NVARCHAR(100) NOT NULL;
    PRINT '   ✓ NomeGP added to Resultados';
END
ELSE
    PRINT '   - NomeGP already exists in Resultados';

PRINT '5. Recreating foreign keys with composite key...';
-- Recriar FKs com chave composta
ALTER TABLE Resultados 
ADD CONSTRAINT FK_Resultado_Sessao 
FOREIGN KEY (NomeGP, NomeSessão) REFERENCES Sessões(NomeGP, NomeSessão);
PRINT '   ✓ FK_Resultado_Sessao recreated';

-- Fazer o mesmo para Penalizações se existir
IF OBJECT_ID('Penalizações', 'U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Penalizações') AND name = 'NomeGP')
    BEGIN
        ALTER TABLE Penalizações ADD NomeGP NVARCHAR(100) NULL;
        EXEC('UPDATE p SET p.NomeGP = s.NomeGP FROM Penalizações p INNER JOIN Sessões s ON p.NomeSessão = s.NomeSessão');
        ALTER TABLE Penalizações ALTER COLUMN NomeGP NVARCHAR(100) NOT NULL;
    END
    
    ALTER TABLE Penalizações 
    ADD CONSTRAINT FK_Penalizacao_Sessao 
    FOREIGN KEY (NomeGP, NomeSessão) REFERENCES Sessões(NomeGP, NomeSessão);
    PRINT '   ✓ FK_Penalizacao_Sessao recreated';
END

-- Fazer o mesmo para Pitstop se existir
IF OBJECT_ID('Pitstop', 'U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Pitstop') AND name = 'NomeGP')
    BEGIN
        ALTER TABLE Pitstop ADD NomeGP NVARCHAR(100) NULL;
        EXEC('UPDATE p SET p.NomeGP = s.NomeGP FROM Pitstop p INNER JOIN Sessões s ON p.NomeSessão = s.NomeSessão');
        ALTER TABLE Pitstop ALTER COLUMN NomeGP NVARCHAR(100) NOT NULL;
    END
    
    ALTER TABLE Pitstop 
    ADD CONSTRAINT FK_Pitstop_Sessao 
    FOREIGN KEY (NomeGP, NomeSessão) REFERENCES Sessões(NomeGP, NomeSessão);
    PRINT '   ✓ FK_Pitstop_Sessao recreated';
END

PRINT '';
PRINT '========================================';
PRINT '✓ SUCCESS! Database updated!';
PRINT '========================================';
PRINT 'Sessions can now have the same name in different GPs.';
PRINT 'Example: "Race" can exist in both Monaco GP and British GP';
PRINT '';
