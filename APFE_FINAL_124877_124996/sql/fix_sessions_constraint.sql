-- Script to fix Sessões table PRIMARY KEY constraint
-- This allows the same session name (e.g., "Race") to exist in different GPs

PRINT 'Starting constraint fix...';

-- Step 1: Add NomeGP column to Resultados if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Resultados') AND name = 'NomeGP')
BEGIN
    PRINT 'Adding NomeGP column to Resultados table...';
    ALTER TABLE Resultados ADD NomeGP NVARCHAR(100) NULL;
    PRINT 'Column added successfully.';
    
    -- Populate NomeGP using dynamic SQL to avoid compilation errors
    PRINT 'Populating NomeGP from Sessões...';
    EXEC('UPDATE r SET r.NomeGP = s.NomeGP FROM Resultados r INNER JOIN Sessões s ON r.NomeSessão = s.NomeSessão');
    PRINT 'NomeGP populated.';
    
    -- Make it NOT NULL after populating
    ALTER TABLE Resultados ALTER COLUMN NomeGP NVARCHAR(100) NOT NULL;
    PRINT 'NomeGP set to NOT NULL.';
END

-- Step 2: Do the same for Penalizações if it exists
IF OBJECT_ID('Penalizações', 'U') IS NOT NULL 
   AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Penalizações') AND name = 'NomeGP')
BEGIN
    PRINT 'Adding NomeGP column to Penalizações table...';
    ALTER TABLE Penalizações ADD NomeGP NVARCHAR(100) NULL;
    
    PRINT 'Populating NomeGP in Penalizações...';
    EXEC('UPDATE p SET p.NomeGP = s.NomeGP FROM Penalizações p INNER JOIN Sessões s ON p.NomeSessão = s.NomeSessão');
    
    ALTER TABLE Penalizações ALTER COLUMN NomeGP NVARCHAR(100) NOT NULL;
    PRINT 'Penalizações updated.';
END

-- Step 3: Drop foreign keys that reference Sessões
PRINT 'Dropping old foreign keys...';
IF OBJECT_ID('FK_Resultado_Sessao', 'F') IS NOT NULL
    ALTER TABLE Resultados DROP CONSTRAINT FK_Resultado_Sessao;

IF OBJECT_ID('FK_Penalizacao_Sessao', 'F') IS NOT NULL
    ALTER TABLE Penalizações DROP CONSTRAINT FK_Penalizacao_Sessao;

-- Step 4: Drop the current PRIMARY KEY constraint on Sessões
PRINT 'Dropping old PRIMARY KEY...';
DECLARE @ConstraintName nvarchar(200)
SELECT @ConstraintName = Name FROM SYS.KEY_CONSTRAINTS 
WHERE PARENT_OBJECT_ID = OBJECT_ID('Sessões') AND type = 'PK'
IF @ConstraintName IS NOT NULL
BEGIN
    EXEC('ALTER TABLE Sessões DROP CONSTRAINT ' + @ConstraintName)
    PRINT 'Dropped constraint: ' + @ConstraintName;
END

-- Step 5: Add new composite PRIMARY KEY (NomeGP, NomeSessão)
PRINT 'Adding new composite PRIMARY KEY...';
ALTER TABLE Sessões 
ADD CONSTRAINT PK_Sessoes PRIMARY KEY (NomeGP, NomeSessão);

-- Step 6: Recreate foreign keys with composite key
PRINT 'Recreating foreign keys...';
ALTER TABLE Resultados 
ADD CONSTRAINT FK_Resultado_Sessao 
FOREIGN KEY (NomeGP, NomeSessão) 
REFERENCES Sessões(NomeGP, NomeSessão);

IF OBJECT_ID('Penalizações', 'U') IS NOT NULL
BEGIN
    ALTER TABLE Penalizações 
    ADD CONSTRAINT FK_Penalizacao_Sessao 
    FOREIGN KEY (NomeGP, NomeSessão) 
    REFERENCES Sessões(NomeGP, NomeSessão);
END

PRINT '✓ SUCCESS: Sessões table PRIMARY KEY updated!';
PRINT '✓ Sessions can now have the same name in different Grand Prix events.';
PRINT '✓ Example: Both "Monaco GP" and "British GP" can have a "Race" session.';

