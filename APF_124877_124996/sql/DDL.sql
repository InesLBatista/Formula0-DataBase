IF OBJECT_ID('Pitstop', 'U') IS NOT NULL DROP TABLE Pitstop;
IF OBJECT_ID('Penalizações', 'U') IS NOT NULL DROP TABLE Penalizações;
IF OBJECT_ID('Resultados', 'U') IS NOT NULL DROP TABLE Resultados;
IF OBJECT_ID('Sessões', 'U') IS NOT NULL DROP TABLE Sessões;
IF OBJECT_ID('Grande_Prémio', 'U') IS NOT NULL DROP TABLE Grande_Prémio;
IF OBJECT_ID('Piloto', 'U') IS NOT NULL DROP TABLE Piloto;
IF OBJECT_ID('Contrato', 'U') IS NOT NULL DROP TABLE Contrato;
IF OBJECT_ID('Membros_da_Equipa', 'U') IS NOT NULL DROP TABLE Membros_da_Equipa;
IF OBJECT_ID('Equipa', 'U') IS NOT NULL DROP TABLE Equipa;
IF OBJECT_ID('Temporada', 'U') IS NOT NULL DROP TABLE Temporada;
IF OBJECT_ID('Circuito', 'U') IS NOT NULL DROP TABLE Circuito;

CREATE TABLE Circuito (
	ID_Circuito INT PRIMARY KEY IDENTITY(1,1),
	Nome NVARCHAR(100) NOT NULL,
	Cidade NVARCHAR(100),
	Pais NVARCHAR(100),
	Comprimento_km DECIMAL(10, 3) NOT NULL,
	NumCurvas INT
);

CREATE TABLE Equipa (
	ID_Equipa INT PRIMARY KEY IDENTITY(1,1),
	Nome NVARCHAR(100) NOT NULL,
	Nacionalidade NVARCHAR(100) NOT NULL,
	Base NVARCHAR(100) NOT NULL,
	ChefeEquipa NVARCHAR(100) NOT NULL,
	ChefeTécnico NVARCHAR(100) NOT NULL,
	AnoEstreia INT NOT NULL,
	ModeloChassis NVARCHAR(100) NOT NULL,
	Power_Unit NVARCHAR(100) NOT NULL,
	PilotosReserva INT
);

CREATE TABLE Membros_da_Equipa (
	ID_Membro INT PRIMARY KEY IDENTITY(1,1),
	Nome NVARCHAR(100) NOT NULL,
	Nacionalidade NVARCHAR(100) NOT NULL,
	DataNascimento DATE NOT NULL,
	Género CHAR(1) NOT NULL,
	Função NVARCHAR(100) NOT NULL,
	ID_Equipa INT NOT NULL,
	CONSTRAINT FK_Membros_Equipa FOREIGN KEY (ID_Equipa) REFERENCES Equipa(ID_Equipa)
);

CREATE TABLE Contrato (
	ID_Contrato INT PRIMARY KEY IDENTITY(1,1),
	AnoInicio INT NOT NULL,
	AnoFim INT NOT NULL,
	Função NVARCHAR(100) NOT NULL,
	Salário DECIMAL(20, 4),
	Género CHAR(1),
	ID_Membro INT NOT NULL,
	CONSTRAINT FK_Contrato_Membro FOREIGN KEY (ID_Membro) REFERENCES Membros_da_Equipa(ID_Membro)
);

CREATE TABLE Piloto (
	ID_Piloto INT PRIMARY KEY IDENTITY(1,1),
	NumeroPermanente INT UNIQUE NOT NULL,
	Abreviação CHAR(3) UNIQUE NOT NULL,
	ID_Equipa INT NOT NULL,
	ID_Membro INT UNIQUE,
	CONSTRAINT FK_Piloto_Equipa FOREIGN KEY (ID_Equipa) REFERENCES Equipa(ID_Equipa),
	CONSTRAINT FK_Piloto_Membro FOREIGN KEY (ID_Membro) REFERENCES Membros_da_Equipa(ID_Membro)
);

CREATE TABLE Temporada (
	Ano INT PRIMARY KEY,
	NumCorridas INT,
	PontosPiloto INT,
	PontosEquipa INT,
	PosiçãoPiloto INT,
	PosiçãoEquipa INT
);

CREATE TABLE Grande_Prémio (
	NomeGP NVARCHAR(100) PRIMARY KEY,
	DataCorrida DATE NOT NULL,
	ID_Circuito INT NOT NULL,
	Ano_Temporada INT NOT NULL,
	CONSTRAINT FK_GP_Circuito FOREIGN KEY (ID_Circuito) REFERENCES Circuito(ID_Circuito),
	CONSTRAINT FK_GP_Temporada FOREIGN KEY (Ano_Temporada) REFERENCES Temporada(Ano)
);

CREATE TABLE Sessões (
	NomeSessão NVARCHAR(100),
	Estado NVARCHAR(50),
	CondiçõesPista NVARCHAR(50),
	NomeGP NVARCHAR(100) NOT NULL,
	CONSTRAINT PK_Sessoes PRIMARY KEY (NomeGP, NomeSessão),
	CONSTRAINT FK_Sessao_GP FOREIGN KEY (NomeGP) REFERENCES Grande_Prémio(NomeGP)
);

-- CORRIGIDO: Adicionado NomeGP e FK composta
CREATE TABLE Resultados (
	ID_Resultado INT PRIMARY KEY IDENTITY(1,1),
	PosiçãoGrid INT,
	TempoFinal TIME,
	PosiçãoFinal INT,
	Status NVARCHAR(50),
	Pontos DECIMAL(5, 2),
	NomeSessão NVARCHAR(100) NOT NULL,
	NomeGP NVARCHAR(100) NOT NULL,
	ID_Piloto INT NOT NULL,
	CONSTRAINT FK_Resultado_Sessao FOREIGN KEY (NomeGP, NomeSessão) REFERENCES Sessões(NomeGP, NomeSessão),
	CONSTRAINT FK_Resultado_Piloto FOREIGN KEY (ID_Piloto) REFERENCES Piloto(ID_Piloto)
);

-- Adicionado NomeGP e FK composta
CREATE TABLE Penalizações (
	ID_Penalização INT PRIMARY KEY IDENTITY(1,1),
	TipoPenalização NVARCHAR(100) NOT NULL,
	Motivo NVARCHAR(255),
	NomeSessão NVARCHAR(100) NOT NULL,
	NomeGP NVARCHAR(100) NOT NULL,
	ID_Piloto INT NOT NULL,
	ID_Resultados INT,
	CONSTRAINT FK_Penalizacao_Sessao FOREIGN KEY (NomeGP, NomeSessão) REFERENCES Sessões(NomeGP, NomeSessão),
	CONSTRAINT FK_Penalizacao_Piloto FOREIGN KEY (ID_Piloto) REFERENCES Piloto(ID_Piloto),
	CONSTRAINT FK_Penalizacao_Resultado FOREIGN KEY (ID_Resultados) REFERENCES Resultados(ID_Resultado)
);

--  Adicionado NomeGP e FK composta
CREATE TABLE Pitstop (
	ID_Pitstop INT PRIMARY KEY IDENTITY(1,1),
	NumeroVolta INT NOT NULL,
	DuraçãoParagem TIME,
	DuraçãoPitlane TIME,
	NomeSessão NVARCHAR(100) NOT NULL,
	NomeGP NVARCHAR(100) NOT NULL,
	ID_Piloto INT NOT NULL,
	CONSTRAINT FK_Pitstop_Sessao FOREIGN KEY (NomeGP, NomeSessão) REFERENCES Sessões(NomeGP, NomeSessão),
	CONSTRAINT FK_Pitstop_Piloto FOREIGN KEY (ID_Piloto) REFERENCES Piloto(ID_Piloto)
);
