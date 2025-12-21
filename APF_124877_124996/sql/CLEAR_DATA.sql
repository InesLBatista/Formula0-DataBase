-- Apagar dados na ordem inversa das dependências
DELETE FROM Pitstop;
DELETE FROM Penalizações;
DELETE FROM Resultados;
DELETE FROM Sessões;
DELETE FROM Grande_Prémio;
DELETE FROM Piloto;
DELETE FROM Contrato;
DELETE FROM Membros_da_Equipa;
DELETE FROM Equipa;
DELETE FROM Temporada;
DELETE FROM Circuito;
DELETE FROM Staff;

-- Reset dos contadores IDENTITY 
DBCC CHECKIDENT ('Pitstop', RESEED, 0);
DBCC CHECKIDENT ('Penalizações', RESEED, 0);
DBCC CHECKIDENT ('Resultados', RESEED, 0);
DBCC CHECKIDENT ('Piloto', RESEED, 0);
DBCC CHECKIDENT ('Contrato', RESEED, 0);
DBCC CHECKIDENT ('Membros_da_Equipa', RESEED, 0);
DBCC CHECKIDENT ('Equipa', RESEED, 0);
DBCC CHECKIDENT ('Circuito', RESEED, 0);

PRINT 'Todos os dados foram apagados! Podes executar o DML.sql novamente.';
