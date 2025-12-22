
-- 1. Inserir Circuitos 
INSERT INTO Circuito (Nome, Cidade, Pais, Comprimento_km, NumCurvas)
VALUES 
('Albert Park Circuit', 'Melbourne', 'Australia', 5.278, 16),
('Autódromo Hermanos Rodríguez', 'Mexico City', 'Mexico', 4.304, 17),
('Autodromo Internazionale Enzo e Dino Ferrari', 'Imola', 'Italy', 4.909, 19),
('Autodromo José Carlos Pace', 'São Paulo', 'Brazil', 4.309, 15),
('Autodromo Nazionale di Monza', 'Monza', 'Italy', 5.793, 11),
('Bahrain International Circuit', 'Sakhir', 'Bahrain', 5.412, 15),
('Baku City Circuit', 'Baku', 'Azerbaijan', 6.003, 20),
('Circuit de Barcelona-Catalunya', 'Montmeló', 'Spain', 4.657, 14),
('Circuit de Monaco', 'Monte Carlo', 'Monaco', 3.337, 19),
('Circuit de Spa-Francorchamps', 'Stavelot', 'Belgium', 7.004, 20),
('Circuit Gilles Villeneuve', 'Montreal', 'Canada', 4.361, 14),
('Circuit of the Americas', 'Austin', 'USA', 5.513, 20),
('Circuit Zandvoort', 'Zandvoort', 'Netherlands', 4.259, 14),
('Hungaroring', 'Mogyoród', 'Hungary', 4.381, 14),
('Jeddah Street Circuit', 'Jeddah', 'Saudi Arabia', 6.174, 27),
('Las Vegas Street Circuit', 'Las Vegas', 'USA', 6.201, 17),
('Lusail International Circuit', 'Lusail', 'Qatar', 5.419, 16),
('Marina Bay Street Circuit', 'Marina Bay','Singapore',4.940,19),
('Miami International Autodrome', 'Miami Gardens', 'USA', 5.412, 19),
('Red Bull Ring', 'Spielberg', 'Austria', 4.318, 10),
('Shanghai International Circuit', 'Shanghai', 'China', 5.451, 16),
('Silverstone Circuit', 'Silverstone', 'UK', 5.891, 18),
('Suzuka International Racing Course', 'Suzuka', 'Japan', 5.807, 18),
('Yas Marina Circuit', 'Abu Dhabi', 'UAE', 5.281, 16);

-- 2. Inserir Temporada (sem dependências)
INSERT INTO Temporada (Ano, NumCorridas)
VALUES (2024, 0); -- NumCorridas pode ser atualizado por um trigger, por exemplo

-- 3. Inserir Equipas (sem dependências)
INSERT INTO Equipa (Nome, Nacionalidade, Base, ChefeEquipa, ChefeTécnico, AnoEstreia, ModeloChassis, Power_Unit)
VALUES 
('Alpine', 'French', 'Enstone, United Kingdom', 'Flavio Briatore', 'David Sanchez', '1986', 'A525', 'Renault'),
('Aston Martin', 'British', 'Silverstone, England','Andy Cowell', 'Enrico Cardile','2018', 'AMR25', 'Mercedes'),
('Scuderia Ferrari', 'Italian', 'Maranello, Italy', 'Frédéric Vasseur', 'Loic Serra / Enrico Gualtieri', '1950', 'SF-25', 'Ferrari'),
('McLaren', 'British','Woking, United Kingdom', 'Andrea Stella', 'Peter Prodromou / Neil Houldey', '1966', 'MCL39', 'Mercedes'),
('Mercedes', 'German', 'Brackley, United Kingdom', 'Toto Wolff', 'James Allison', '1970', 'W16', 'Mercedes'),
('Red Bull Racing', 'Austrian', 'Milton Keynes, United Kingdom', 'Laurent Mekies', 'Pierre Waché', '1997', 'RB21', 'Honda RBPT'),
('Williams', 'British', 'Grove, United Kingdom', 'James Vowles', 'Pat Fry', '1978', 'FW47', 'Mercedes'),
('Racing Bulls', 'Italian', 'Faenza, Italy', 'Alan Permane', 'Tim Goss', '1985', 'VCARB 02', 'Honda RBPT'),
('Haas', 'American', 'Kannapolis, USA', 'Ayao Komatsu', 'Andrea De Zordo', '2016', 'VF-25', 'Ferrari'),
('Kick Sauber', 'Swiss', 'Hinwil, Switzerland', 'Jonathan Wheatley', 'James Key', '1993', 'C45', 'Ferrari');
-- 4. Inserir Membros da Equipa (depende de Equipa)
-- Assumimos que o ID_Equipa 1 é 'Alpine', 2 é 'Aston Martin' e 3 é 'Ferrari'

INSERT INTO Membros_da_Equipa (Nome, Nacionalidade, DataNascimento, Género, Função, ID_Equipa)
VALUES
-- 1. Alpine (ID_Equipa = 1)
('Pierre Gasly', 'French', '1996-02-07', 'M', 'Driver', 1),
('Franco Colapinto', 'Argentine', '2003-05-27', 'M', 'Driver', 1),
('Jack Doohan', 'Australian', '2003-01-20', 'M', 'Reserve Driver', 1),
('Paul Aron', 'Estonian', '2004-02-04', 'M', 'Reserve Driver', 1),
('Kush Maini', 'Indian', '2000-01-09', 'M', 'Reserve Driver', 1),
('Flavio Briatore', 'Italian', '1950-04-12', 'M', 'Team Chief', 1),
('David Sanchez', 'French', '1980-03-15', 'M', 'Technical Chief', 1),

-- 2. Aston Martin (ID_Equipa = 2)
('Fernando Alonso', 'Spanish', '1981-07-29', 'M', 'Driver', 2),
('Lance Stroll', 'Canadian', '1998-10-29', 'M', 'Driver', 2),
('Stoffel Vandoorne', 'Belgian', '1992-03-26', 'M', 'Reserve Driver', 2),
('Andy Cowell', 'British', '1969-02-12', 'M', 'Team Chief', 2),
('Enrico Cardile', 'Italian', '1975-04-05', 'M', 'Technical Chief', 2),

-- 3. Ferrari (ID_Equipa = 3)
('Charles Leclerc', 'Monegasque', '1997-10-16', 'M', 'Driver', 3),
('Lewis Hamilton', 'British', '1985-01-07', 'M', 'Driver', 3),
('Zhou Guanyu', 'Chinese', '1999-05-30', 'M', 'Reserve Driver', 3),
('Antonio Giovinazzi', 'Italian', '1993-12-14', 'M', 'Reserve Driver', 3),
('Frédéric Vasseur', 'French', '1968-05-28', 'M', 'Team Chief', 3),
('Loic Serra', 'French', '1972-09-20', 'M', 'Technical Chief', 3),
('Enrico Gualtieri', 'Italian', '1975-02-21', 'M', 'Technical Chief', 3),

-- 4. McLaren (ID_Equipa = 4)
('Lando Norris', 'British', '1999-11-13', 'M', 'Driver', 4),
('Oscar Piastri', 'Australian', '2001-04-06', 'M', 'Driver', 4),
('Andrea Stella', 'Italian', '1971-02-22', 'M', 'Team Chief', 4),
('Peter Prodromou', 'British', '1969-01-14', 'M', 'Technical Chief', 4),
('Neil Houldey', 'British', '1976-03-14', 'M', 'Technical Chief', 4),

-- 5. Mercedes-AMG (ID_Equipa = 5)
('George Russell', 'British', '1998-02-15', 'M', 'Driver', 5),
('Kimi Antonelli', 'Italian', '2006-08-25', 'M', 'Driver', 5),
('Valtteri Bottas', 'Finnish', '1989-08-28', 'M', 'Reserve Driver', 5),
('Frederik Vesti', 'Danish', '2002-01-13', 'M', 'Reserve Driver', 5),
('Toto Wolff', 'Austrian', '1972-01-12', 'M', 'Team Chief', 5),
('James Allison', 'British', '1968-02-22', 'M', 'Technical Chief', 5),

-- 6. Red Bull Racing (ID_Equipa = 6)
('Max Verstappen', 'Dutch', '1997-09-30', 'M', 'Driver', 6),
('Yuki Tsunoda', 'Japanese', '2000-05-11', 'M', 'Driver', 6),
('Laurent Mekies', 'French', '1977-04-28', 'M', 'Team Chief', 6),
('Pierre Waché', 'French', '1974-12-10', 'M', 'Technical Chief', 6),

-- 7. Williams (ID_Equipa = 7)
('Alexander Albon', 'Thai', '1996-03-23', 'M', 'Driver', 7),
('Carlos Sainz', 'Spanish', '1994-09-01', 'M', 'Driver', 7),
('James Vowles', 'British', '1979-06-20', 'M', 'Team Chief', 7),
('Pat Fry', 'British', '1964-03-17', 'M', 'Technical Chief', 7),

-- 8. Haas (ID_Equipa = 8)
('Esteban Ocon', 'French', '1996-09-17', 'M', 'Driver', 8),
('Oliver Bearman', 'British', '2005-05-08', 'M', 'Driver', 8),
('Ryo Hirakawa', 'Japanese', '1994-03-07', 'M', 'Reserve Driver', 8), --
('Ayao Komatsu', 'Japanese', '1976-01-28', 'M', 'Team Chief', 8), --
('Andrea De Zordo', 'Italian', '1974-05-01', 'M', 'Technical Chief', 8), --

-- 9. Racing Bulls (VCARB) (ID_Equipa = 9)
('Liam Lawson', 'New Zealander', '2000-05-11', 'M', 'Driver', 9),
('Isack Hadjar', 'French', '2004-09-28', 'M', 'Driver', 9),
('Ayumu Iwasa', 'Japanese', '2001-09-22', 'M', 'Reserve Driver', 9),
('Alan Permane', 'British', '1967-02-09', 'M', 'Team Chief', 9),
('Tim Goss', 'British', '1963-02-28', 'M', 'Technical Chief', 9),

-- 10. Sauber / Audi (ID_Equipa = 10)
('Nico Hülkenberg', 'German', '1987-08-19', 'M', 'Driver', 10), --
('Gabriel Bortoleto', 'Brazilian', '2004-10-14', 'M', 'Driver', 10), --
('Jonathan Wheatley', 'British', '1967-05-07', 'M', 'Team Chief', 10), --
('James Key', 'British', '1972-01-14', 'M', 'Technical Chief', 10); --

-- 5. Inserir Contratos (depende de Membros_da_Equipa)
INSERT INTO Contrato (AnoInicio, AnoFim, Função, Salário, Género, ID_Membro)
VALUES
-- 1. Alpine
(2023, 2026, 'Driver', 8000000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Pierre Gasly' AND ID_Equipa = 1)),
(2025, 2025, 'Driver', 1500000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Franco Colapinto' AND ID_Equipa = 1)),
(2025, 2025, 'Reserve Driver', 400000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Jack Doohan' AND ID_Equipa = 1)),
(2025, 2025, 'Reserve Driver', 250000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Paul Aron' AND ID_Equipa = 1)),
(2025, 2025, 'Reserve Driver', 200000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Kush Maini' AND ID_Equipa = 1)),
(2024, 2027, 'Team Chief', 5000000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Flavio Briatore' AND ID_Equipa = 1)),
(2024, 2027, 'Technical Chief', 2800000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'David Sanchez' AND ID_Equipa = 1)),

-- 2. Aston Martin
(2023, 2026, 'Driver', 20000000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Fernando Alonso' AND ID_Equipa = 2)),
(2017, 2026, 'Driver', 10000000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Lance Stroll' AND ID_Equipa = 2)),
(2024, 2025, 'Reserve Driver', 600000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Stoffel Vandoorne' AND ID_Equipa = 2)),
(2024, 2028, 'Team Chief', 6500000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Andy Cowell' AND ID_Equipa = 2)),
(2025, 2028, 'Technical Chief', 3500000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Enrico Cardile' AND ID_Equipa = 2)),

-- 3. Ferrari
(2019, 2029, 'Driver', 30000000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Charles Leclerc' AND ID_Equipa = 3)),
(2025, 2027, 'Driver', 50000000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Lewis Hamilton' AND ID_Equipa = 3)),
(2025, 2025, 'Reserve Driver', 600000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Zhou Guanyu' AND ID_Equipa = 3)),
(2021, 2025, 'Reserve Driver', 500000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Antonio Giovinazzi' AND ID_Equipa = 3)),
(2023, 2026, 'Team Chief', 4500000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Frédéric Vasseur' AND ID_Equipa = 3)),
(2024, 2027, 'Technical Chief', 3000000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Loic Serra' AND ID_Equipa = 3)),
(2024, 2026, 'Technical Chief', 2500000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Enrico Gualtieri' AND ID_Equipa = 3)),

-- 4. McLaren
(2022, 2027, 'Driver', 20000000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Lando Norris' AND ID_Equipa = 4)),
(2023, 2026, 'Driver', 8000000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Oscar Piastri' AND ID_Equipa = 4)),
(2023, 2027, 'Team Chief', 3500000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Andrea Stella' AND ID_Equipa = 4)),
(2014, 2026, 'Technical Chief', 2500000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Peter Prodromou' AND ID_Equipa = 4)),
(2022, 2026, 'Technical Chief', 2200000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Neil Houldey' AND ID_Equipa = 4)),

-- 5. Mercedes-AMG
(2022, 2026, 'Driver', 15000000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'George Russell' AND ID_Equipa = 5)),
(2025, 2027, 'Driver', 1000000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Kimi Antonelli' AND ID_Equipa = 5)),
(2025, 2026, 'Reserve Driver', 2000000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Valtteri Bottas' AND ID_Equipa = 5)),
(2023, 2025, 'Reserve Driver', 300000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Frederik Vesti' AND ID_Equipa = 5)),
(2013, 2026, 'Team Chief', 16000000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Toto Wolff' AND ID_Equipa = 5)),
(2023, 2026, 'Technical Chief', 4000000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'James Allison' AND ID_Equipa = 5)),

-- 6. Red Bull Racing
(2022, 2028, 'Driver', 55000000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Max Verstappen' AND ID_Equipa = 6)),
(2021, 2025, 'Driver', 3000000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Yuki Tsunoda' AND ID_Equipa = 6)),
(2024, 2026, 'Team Chief', 3500000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Laurent Mekies' AND ID_Equipa = 6)),
(2023, 2027, 'Technical Chief', 5000000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Pierre Waché' AND ID_Equipa = 6)),

-- 7. Williams
(2022, 2026, 'Driver', 3000000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Alexander Albon' AND ID_Equipa = 7)),
(2025, 2028, 'Driver', 12000000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Carlos Sainz' AND ID_Equipa = 7)),
(2023, 2027, 'Team Chief', 3000000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'James Vowles' AND ID_Equipa = 7)),
(2023, 2026, 'Technical Chief', 2500000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Pat Fry' AND ID_Equipa = 7)),

-- 8. Haas
(2025, 2027, 'Driver', 6000000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Esteban Ocon' AND ID_Equipa = 8)),
(2025, 2026, 'Driver', 800000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Oliver Bearman' AND ID_Equipa = 8)),
(2025, 2025, 'Reserve Driver', 400000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Ryo Hirakawa' AND ID_Equipa = 8)),
(2024, 2026, 'Team Chief', 2000000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Ayao Komatsu' AND ID_Equipa = 8)),
(2024, 2026, 'Technical Chief', 1500000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Andrea De Zordo' AND ID_Equipa = 8)),

-- 9. Racing Bulls
(2024, 2026, 'Driver', 1200000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Liam Lawson' AND ID_Equipa = 9)),
(2025, 2025, 'Driver', 800000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Isack Hadjar' AND ID_Equipa = 9)),
(2024, 2025, 'Reserve Driver', 300000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Ayumu Iwasa' AND ID_Equipa = 9)),
(2024, 2026, 'Team Chief', 2500000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Alan Permane' AND ID_Equipa = 9)),
(2024, 2027, 'Technical Chief', 2200000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Tim Goss' AND ID_Equipa = 9)),

-- 10. Sauber / Audi
(2025, 2027, 'Driver', 5000000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Nico Hülkenberg' AND ID_Equipa = 10)),
(2025, 2026, 'Driver', 1000000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Gabriel Bortoleto' AND ID_Equipa = 10)),
(2025, 2028, 'Team Chief', 4500000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Jonathan Wheatley' AND ID_Equipa = 10)),
(2023, 2026, 'Technical Chief', 3000000.00, 'M', (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'James Key' AND ID_Equipa = 10));

-- 6. Inserir Pilotos (depende de Equipa e Membros_da_Equipa)
-- Todos os membros com Função = 'Driver' são automaticamente pilotos
INSERT INTO Piloto (NumeroPermanente, Abreviação, ID_Equipa, ID_Membro)
VALUES
-- Alpine
(10, 'GAS', 1, (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Pierre Gasly')),
(43, 'COL', 1, (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Franco Colapinto')),

-- Aston Martin
(14, 'ALO', 2, (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Fernando Alonso')),
(18, 'STR', 2, (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Lance Stroll')),

-- Ferrari
(16, 'LEC', 3, (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Charles Leclerc')),
(44, 'HAM', 3, (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Lewis Hamilton')),

-- McLaren
(4, 'NOR', 4, (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Lando Norris')),
(81, 'PIA', 4, (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Oscar Piastri')),

-- Mercedes
(63, 'RUS', 5, (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'George Russell')),
(12, 'ANT', 5, (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Kimi Antonelli')),

-- Red Bull Racing
(1, 'VER', 6, (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Max Verstappen')),
(22, 'TSU', 6, (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Yuki Tsunoda')),

-- Williams
(23, 'ALB', 7, (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Alexander Albon')),
(55, 'SAI', 7, (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Carlos Sainz')),

-- Haas
(31, 'OCO', 8, (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Esteban Ocon')),
(87, 'BEA', 8, (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Oliver Bearman')),

-- Racing Bulls (VCARB)
(30, 'LAW', 9, (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Liam Lawson')),
(6, 'HAD', 9, (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Isack Hadjar')),

-- Sauber / Audi
(27, 'HUL', 10, (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Nico Hülkenberg')),
(5, 'BOR', 10, (SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = 'Gabriel Bortoleto'));
-- Inserir as temporadas de 2024 e 2025
-- A ordem é importante: a tabela Temporada deve ter dados antes da tabela Grande_Prémio
IF NOT EXISTS (SELECT 1 FROM Temporada WHERE Ano = 2024)
    INSERT INTO Temporada (Ano, NumCorridas) VALUES (2024, 24);

IF NOT EXISTS (SELECT 1 FROM Temporada WHERE Ano = 2025)
    INSERT INTO Temporada (Ano, NumCorridas) VALUES (2025, 24);

-- Verificar se foram inseridas corretamente
SELECT * FROM Temporada;

-- 7. Inserir Grande Prémio (depende de Circuito e Temporada)
INSERT INTO Grande_Prémio (NomeGP, DataCorrida, ID_Circuito, Ano_Temporada)
VALUES 
-- Australian Grand Prix
('Australian Grand Prix 2024', '2024-03-24', (SELECT ID_Circuito FROM Circuito WHERE Nome = 'Albert Park Circuit'), 2024),
('Australian Grand Prix 2025', '2025-03-16', (SELECT ID_Circuito FROM Circuito WHERE Nome = 'Albert Park Circuit'), 2025),

-- Bahrain Grand Prix
('Bahrain Grand Prix 2024', '2024-03-02', (SELECT ID_Circuito FROM Circuito WHERE Nome = 'Bahrain International Circuit'), 2024),
('Bahrain Grand Prix 2025', '2025-04-13', (SELECT ID_Circuito FROM Circuito WHERE Nome = 'Bahrain International Circuit'), 2025),

-- Saudi Arabian Grand Prix
('Saudi Arabian Grand Prix 2024', '2024-03-09', (SELECT ID_Circuito FROM Circuito WHERE Nome = 'Jeddah Street Circuit'), 2024),
('Saudi Arabian Grand Prix 2025', '2025-04-20', (SELECT ID_Circuito FROM Circuito WHERE Nome = 'Jeddah Street Circuit'), 2025),
-- Chinese Grand Prix
('Chinese Grand Prix 2024', '2024-04-21', (SELECT ID_Circuito FROM Circuito WHERE Nome = 'Shanghai International Circuit'), 2024),
('Chinese Grand Prix 2025', '2025-03-23', (SELECT ID_Circuito FROM Circuito WHERE Nome = 'Shanghai International Circuit'), 2025),

-- Japanese Grand Prix
('Japanese Grand Prix 2024', '2024-04-07', (SELECT ID_Circuito FROM Circuito WHERE Nome = 'Suzuka International Racing Course'), 2024),
('Japanese Grand Prix 2025', '2025-04-06', (SELECT ID_Circuito FROM Circuito WHERE Nome = 'Suzuka International Racing Course'), 2025),

-- Miami Grand Prix
('Miami Grand Prix 2024', '2024-05-05', (SELECT ID_Circuito FROM Circuito WHERE Nome = 'Miami International Autodrome'), 2024),
('Miami Grand Prix 2025', '2025-05-04', (SELECT ID_Circuito FROM Circuito WHERE Nome = 'Miami International Autodrome'), 2025),
-- Emilia Romagna Grand Prix
('Emilia Romagna Grand Prix 2024', '2024-05-19', (SELECT ID_Circuito FROM Circuito WHERE Nome = 'Autodromo Internazionale Enzo e Dino Ferrari'), 2024),
('Emilia Romagna Grand Prix 2025', '2025-05-18', (SELECT ID_Circuito FROM Circuito WHERE Nome = 'Autodromo Internazionale Enzo e Dino Ferrari'), 2025),

-- Monaco Grand Prix
('Monaco Grand Prix 2024', '2024-05-26', (SELECT ID_Circuito FROM Circuito WHERE Nome = 'Circuit de Monaco'), 2024),
('Monaco Grand Prix 2025', '2025-05-25', (SELECT ID_Circuito FROM Circuito WHERE Nome = 'Circuit de Monaco'), 2025),

-- Spanish Grand Prix
('Spanish Grand Prix 2024', '2024-06-23', (SELECT ID_Circuito FROM Circuito WHERE Nome = 'Circuit de Barcelona-Catalunya'), 2024),
('Spanish Grand Prix 2025', '2025-06-01', (SELECT ID_Circuito FROM Circuito WHERE Nome = 'Circuit de Barcelona-Catalunya'), 2025),
-- Canadian Grand Prix
('Canadian Grand Prix 2024', '2024-06-09', (SELECT ID_Circuito FROM Circuito WHERE Nome = 'Circuit Gilles Villeneuve'), 2024),
('Canadian Grand Prix 2025', '2025-06-15', (SELECT ID_Circuito FROM Circuito WHERE Nome = 'Circuit Gilles Villeneuve'), 2025),

-- Austrian Grand Prix
('Austrian Grand Prix 2024', '2024-06-30', (SELECT ID_Circuito FROM Circuito WHERE Nome = 'Red Bull Ring'), 2024),
('Austrian Grand Prix 2025', '2025-06-29', (SELECT ID_Circuito FROM Circuito WHERE Nome = 'Red Bull Ring'), 2025),

-- British Grand Prix
('British Grand Prix 2024', '2024-07-07', (SELECT ID_Circuito FROM Circuito WHERE Nome = 'Silverstone Circuit'), 2024),
('British Grand Prix 2025', '2025-07-06', (SELECT ID_Circuito FROM Circuito WHERE Nome = 'Silverstone Circuit'), 2025),
-- Hungarian Grand Prix
('Hungarian Grand Prix 2024', '2024-07-21', (SELECT ID_Circuito FROM Circuito WHERE Nome = 'Hungaroring'), 2024),
('Hungarian Grand Prix 2025', '2025-08-03', (SELECT ID_Circuito FROM Circuito WHERE Nome = 'Hungaroring'), 2025),

-- Belgian Grand Prix
('Belgian Grand Prix 2024', '2024-07-28', (SELECT ID_Circuito FROM Circuito WHERE Nome = 'Circuit de Spa-Francorchamps'), 2024),
('Belgian Grand Prix 2025', '2025-07-27', (SELECT ID_Circuito FROM Circuito WHERE Nome = 'Circuit de Spa-Francorchamps'), 2025),
-- Dutch Grand Prix
('Dutch Grand Prix 2024', '2024-08-25', (SELECT ID_Circuito FROM Circuito WHERE Nome = 'Circuit Zandvoort'), 2024),
('Dutch Grand Prix 2025', '2025-08-31', (SELECT ID_Circuito FROM Circuito WHERE Nome = 'Circuit Zandvoort'), 2025),

-- Italian Grand Prix
('Italian Grand Prix 2024', '2024-09-01', (SELECT ID_Circuito FROM Circuito WHERE Nome = 'Autodromo Nazionale di Monza'), 2024),
('Italian Grand Prix 2025', '2025-09-07', (SELECT ID_Circuito FROM Circuito WHERE Nome = 'Autodromo Nazionale di Monza'), 2025),
-- Azerbaijan Grand Prix
('Azerbaijan Grand Prix 2024', '2024-09-15', (SELECT ID_Circuito FROM Circuito WHERE Nome = 'Baku City Circuit'), 2024),
('Azerbaijan Grand Prix 2025', '2025-09-21', (SELECT ID_Circuito FROM Circuito WHERE Nome = 'Baku City Circuit'), 2025),

-- Singapore Grand Prix
('Singapore Grand Prix 2024', '2024-09-22', (SELECT ID_Circuito FROM Circuito WHERE Nome = 'Marina Bay Street Circuit'), 2024),
('Singapore Grand Prix 2025', '2025-10-05', (SELECT ID_Circuito FROM Circuito WHERE Nome = 'Marina Bay Street Circuit'), 2025),

-- United States Grand Prix
('United States Grand Prix 2024', '2024-10-20', (SELECT ID_Circuito FROM Circuito WHERE Nome = 'Circuit of the Americas'), 2024),
('United States Grand Prix 2025', '2025-10-19', (SELECT ID_Circuito FROM Circuito WHERE Nome = 'Circuit of the Americas'), 2025),
-- Mexico City Grand Prix
('Mexico City Grand Prix 2024', '2024-10-27', (SELECT ID_Circuito FROM Circuito WHERE Nome = 'Autódromo Hermanos Rodríguez'), 2024),
('Mexico City Grand Prix 2025', '2025-10-26', (SELECT ID_Circuito FROM Circuito WHERE Nome = 'Autódromo Hermanos Rodríguez'), 2025),

-- São Paulo Grand Prix
('São Paulo Grand Prix 2024', '2024-11-03', (SELECT ID_Circuito FROM Circuito WHERE Nome = 'Autodromo José Carlos Pace'), 2024),
('São Paulo Grand Prix 2025', '2025-11-09', (SELECT ID_Circuito FROM Circuito WHERE Nome = 'Autodromo José Carlos Pace'), 2025),
-- Las Vegas Grand Prix
('Las Vegas Grand Prix 2024', '2024-11-23', (SELECT ID_Circuito FROM Circuito WHERE Nome = 'Las Vegas Street Circuit'), 2024),
('Las Vegas Grand Prix 2025', '2025-11-22', (SELECT ID_Circuito FROM Circuito WHERE Nome = 'Las Vegas Street Circuit'), 2025),

-- Qatar Grand Prix
('Qatar Grand Prix 2024', '2024-12-01', (SELECT ID_Circuito FROM Circuito WHERE Nome = 'Lusail International Circuit'), 2024),
('Qatar Grand Prix 2025', '2025-11-30', (SELECT ID_Circuito FROM Circuito WHERE Nome = 'Lusail International Circuit'), 2025),

-- Abu Dhabi Grand Prix
('Abu Dhabi Grand Prix 2024', '2024-12-08', (SELECT ID_Circuito FROM Circuito WHERE Nome = 'Yas Marina Circuit'), 2024),
('Abu Dhabi Grand Prix 2025', '2025-12-07', (SELECT ID_Circuito FROM Circuito WHERE Nome = 'Yas Marina Circuit'), 2025);

-- 8. Inserir Sessões (depende de Grande_Prémio)
INSERT INTO Sessões (NomeSessão, NomeGP, Estado, CondiçõesPista)
VALUES
-- Australian Grand Prix 2025
('Free Practice 1', 'Australian Grand Prix 2025', 'Completed', 'Dry'),
('Free Practice 2', 'Australian Grand Prix 2025', 'Completed', 'Dry'),
('Free Practice 3', 'Australian Grand Prix 2025', 'Completed', 'Dry'),
('Qualifying', 'Australian Grand Prix 2025', 'Completed', 'Dry'),
('Race', 'Australian Grand Prix 2025', 'Completed', 'Dry'),

-- Chinese Grand Prix 2025
('Free Practice 1', 'Chinese Grand Prix 2025', 'Completed', 'Dry'),
('Sprint Qualifying', 'Chinese Grand Prix 2025', 'Completed', 'Dry'),
('Sprint Race', 'Chinese Grand Prix 2025', 'Completed', 'Wet'),
('Qualifying', 'Chinese Grand Prix 2025', 'Completed', 'Dry'),
('Race', 'Chinese Grand Prix 2025', 'Completed', 'Dry'),

-- Japanese Grand Prix 2025
('Free Practice 1', 'Japanese Grand Prix 2025', 'Completed', 'Dry'),
('Free Practice 2', 'Japanese Grand Prix 2025', 'Completed', 'Dry'),
('Free Practice 3', 'Japanese Grand Prix 2025', 'Completed', 'Dry'),
('Qualifying', 'Japanese Grand Prix 2025', 'Completed', 'Dry'),
('Race', 'Japanese Grand Prix 2025', 'Completed', 'Dry'),

-- Bahrain Grand Prix 2025
('Free Practice 1', 'Bahrain Grand Prix 2025', 'Completed', 'Dry'),
('Free Practice 2', 'Bahrain Grand Prix 2025', 'Completed', 'Dry'),
('Free Practice 3', 'Bahrain Grand Prix 2025', 'Completed', 'Dry'),
('Qualifying', 'Bahrain Grand Prix 2025', 'Completed', 'Dry'),
('Race', 'Bahrain Grand Prix 2025', 'Completed', 'Dry'),

-- Saudi Arabian Grand Prix 2025
('Free Practice 1', 'Saudi Arabian Grand Prix 2025', 'Completed', 'Dry'),
('Free Practice 2', 'Saudi Arabian Grand Prix 2025', 'Completed', 'Dry'),
('Free Practice 3', 'Saudi Arabian Grand Prix 2025', 'Completed', 'Dry'),
('Qualifying', 'Saudi Arabian Grand Prix 2025', 'Completed', 'Dry'),
('Race', 'Saudi Arabian Grand Prix 2025', 'Completed', 'Dry'),

-- Miami Grand Prix 2025 (Sprint Format)
('Free Practice 1', 'Miami Grand Prix 2025', 'Completed', 'Dry'),
('Sprint Qualifying', 'Miami Grand Prix 2025', 'Completed', 'Dry'),
('Sprint Race', 'Miami Grand Prix 2025', 'Completed', 'Dry'),
('Qualifying', 'Miami Grand Prix 2025', 'Completed', 'Dry'),
('Race', 'Miami Grand Prix 2025', 'Completed', 'Dry'),

-- Emilia Romagna Grand Prix 2025
('Free Practice 1', 'Emilia Romagna Grand Prix 2025', 'Completed', 'Dry'),
('Free Practice 2', 'Emilia Romagna Grand Prix 2025', 'Completed', 'Dry'),
('Free Practice 3', 'Emilia Romagna Grand Prix 2025', 'Completed', 'Dry'),
('Qualifying', 'Emilia Romagna Grand Prix 2025', 'Completed', 'Dry'),
('Race', 'Emilia Romagna Grand Prix 2025', 'Completed', 'Dry'),

-- Monaco Grand Prix 2025
('Free Practice 1', 'Monaco Grand Prix 2025', 'Completed', 'Dry'),
('Free Practice 2', 'Monaco Grand Prix 2025', 'Completed', 'Dry'),
('Free Practice 3', 'Monaco Grand Prix 2025', 'Completed', 'Dry'),
('Qualifying', 'Monaco Grand Prix 2025', 'Completed', 'Dry'),
('Race', 'Monaco Grand Prix 2025', 'Completed', 'Dry'),

-- Spanish Grand Prix 2025
('Free Practice 1', 'Spanish Grand Prix 2025', 'Completed', 'Dry'),
('Free Practice 2', 'Spanish Grand Prix 2025', 'Completed', 'Dry'),
('Free Practice 3', 'Spanish Grand Prix 2025', 'Completed', 'Dry'),
('Qualifying', 'Spanish Grand Prix 2025', 'Completed', 'Dry'),
('Race', 'Spanish Grand Prix 2025', 'Completed', 'Dry'),

-- Canadian Grand Prix 2025
('Free Practice 1', 'Canadian Grand Prix 2025', 'Completed', 'Wet'),
('Free Practice 2', 'Canadian Grand Prix 2025', 'Completed', 'Dry'),
('Free Practice 3', 'Canadian Grand Prix 2025', 'Completed', 'Dry'),
('Qualifying', 'Canadian Grand Prix 2025', 'Completed', 'Dry'),
('Race', 'Canadian Grand Prix 2025', 'Completed', 'Dry'),

-- Austrian Grand Prix 2025 (Sprint Format)
('Free Practice 1', 'Austrian Grand Prix 2025', 'Completed', 'Dry'),
('Sprint Qualifying', 'Austrian Grand Prix 2025', 'Completed', 'Dry'),
('Sprint Race', 'Austrian Grand Prix 2025', 'Completed', 'Dry'),
('Qualifying', 'Austrian Grand Prix 2025', 'Completed', 'Dry'),
('Race', 'Austrian Grand Prix 2025', 'Completed', 'Dry'),

-- British Grand Prix 2025
('Free Practice 1', 'British Grand Prix 2025', 'Completed', 'Wet'),
('Free Practice 2', 'British Grand Prix 2025', 'Completed', 'Dry'),
('Free Practice 3', 'British Grand Prix 2025', 'Completed', 'Wet'),
('Qualifying', 'British Grand Prix 2025', 'Completed', 'Wet'),
('Race', 'British Grand Prix 2025', 'Completed', 'Dry'),

-- Belgian Grand Prix 2025 (Sprint Format)
('Free Practice 1', 'Belgian Grand Prix 2025', 'Completed', 'Wet'),
('Sprint Qualifying', 'Belgian Grand Prix 2025', 'Completed', 'Wet'),
('Sprint Race', 'Belgian Grand Prix 2025', 'Completed', 'Dry'),
('Qualifying', 'Belgian Grand Prix 2025', 'Completed', 'Dry'),
('Race', 'Belgian Grand Prix 2025', 'Completed', 'Dry'),

-- Hungarian Grand Prix 2025
('Free Practice 1', 'Hungarian Grand Prix 2025', 'Completed', 'Dry'),
('Free Practice 2', 'Hungarian Grand Prix 2025', 'Completed', 'Dry'),
('Free Practice 3', 'Hungarian Grand Prix 2025', 'Completed', 'Dry'),
('Qualifying', 'Hungarian Grand Prix 2025', 'Completed', 'Dry'),
('Race', 'Hungarian Grand Prix 2025', 'Completed', 'Dry'),

-- Dutch Grand Prix 2025
('Free Practice 1', 'Dutch Grand Prix 2025', 'Completed', 'Dry'),
('Free Practice 2', 'Dutch Grand Prix 2025', 'Completed', 'Dry'),
('Free Practice 3', 'Dutch Grand Prix 2025', 'Completed', 'Dry'),
('Qualifying', 'Dutch Grand Prix 2025', 'Completed', 'Dry'),
('Race', 'Dutch Grand Prix 2025', 'Completed', 'Dry'),

-- Italian Grand Prix 2025
('Free Practice 1', 'Italian Grand Prix 2025', 'Completed', 'Dry'),
('Free Practice 2', 'Italian Grand Prix 2025', 'Completed', 'Dry'),
('Free Practice 3', 'Italian Grand Prix 2025', 'Completed', 'Dry'),
('Qualifying', 'Italian Grand Prix 2025', 'Completed', 'Dry'),
('Race', 'Italian Grand Prix 2025', 'Completed', 'Dry'),

-- Azerbaijan Grand Prix 2025
('Free Practice 1', 'Azerbaijan Grand Prix 2025', 'Completed', 'Dry'),
('Free Practice 2', 'Azerbaijan Grand Prix 2025', 'Completed', 'Dry'),
('Free Practice 3', 'Azerbaijan Grand Prix 2025', 'Completed', 'Dry'),
('Qualifying', 'Azerbaijan Grand Prix 2025', 'Completed', 'Dry'),
('Race', 'Azerbaijan Grand Prix 2025', 'Completed', 'Dry'),

-- Singapore Grand Prix 2025
('Free Practice 1', 'Singapore Grand Prix 2025', 'Completed', 'Dry'),
('Free Practice 2', 'Singapore Grand Prix 2025', 'Completed', 'Dry'),
('Free Practice 3', 'Singapore Grand Prix 2025', 'Completed', 'Dry'),
('Qualifying', 'Singapore Grand Prix 2025', 'Completed', 'Dry'),
('Race', 'Singapore Grand Prix 2025', 'Completed', 'Dry'),

-- United States Grand Prix 2025 (Sprint Format)
('Free Practice 1', 'United States Grand Prix 2025', 'Completed', 'Dry'),
('Sprint Qualifying', 'United States Grand Prix 2025', 'Completed', 'Dry'),
('Sprint Race', 'United States Grand Prix 2025', 'Completed', 'Dry'),
('Qualifying', 'United States Grand Prix 2025', 'Completed', 'Dry'),
('Race', 'United States Grand Prix 2025', 'Completed', 'Dry'),

-- Mexico City Grand Prix 2025
('Free Practice 1', 'Mexico City Grand Prix 2025', 'Completed', 'Dry'),
('Free Practice 2', 'Mexico City Grand Prix 2025', 'Completed', 'Dry'),
('Free Practice 3', 'Mexico City Grand Prix 2025', 'Completed', 'Dry'),
('Qualifying', 'Mexico City Grand Prix 2025', 'Completed', 'Dry'),
('Race', 'Mexico City Grand Prix 2025', 'Completed', 'Dry'),

-- São Paulo Grand Prix 2025 (Sprint Format)
('Free Practice 1', 'São Paulo Grand Prix 2025', 'Completed', 'Wet'),
('Sprint Qualifying', 'São Paulo Grand Prix 2025', 'Completed', 'Wet'),
('Sprint Race', 'São Paulo Grand Prix 2025', 'Completed', 'Dry'),
('Qualifying', 'São Paulo Grand Prix 2025', 'Completed', 'Dry'),
('Race', 'São Paulo Grand Prix 2025', 'Completed', 'Wet'),

-- Las Vegas Grand Prix 2025
('Free Practice 1', 'Las Vegas Grand Prix 2025', 'Completed', 'Dry'),
('Free Practice 2', 'Las Vegas Grand Prix 2025', 'Completed', 'Dry'),
('Free Practice 3', 'Las Vegas Grand Prix 2025', 'Completed', 'Dry'),
('Qualifying', 'Las Vegas Grand Prix 2025', 'Completed', 'Dry'),
('Race', 'Las Vegas Grand Prix 2025', 'Completed', 'Dry'),

-- Qatar Grand Prix 2025 (Sprint Format)
('Free Practice 1', 'Qatar Grand Prix 2025', 'Completed', 'Dry'),
('Sprint Qualifying', 'Qatar Grand Prix 2025', 'Completed', 'Dry'),
('Sprint Race', 'Qatar Grand Prix 2025', 'Completed', 'Dry'),
('Qualifying', 'Qatar Grand Prix 2025', 'Completed', 'Dry'),
('Race', 'Qatar Grand Prix 2025', 'Completed', 'Dry'),

-- Abu Dhabi Grand Prix 2025
('Free Practice 1', 'Abu Dhabi Grand Prix 2025', 'Completed', 'Dry'),
('Free Practice 2', 'Abu Dhabi Grand Prix 2025', 'Completed', 'Dry'),
('Free Practice 3', 'Abu Dhabi Grand Prix 2025', 'Completed', 'Dry'),
('Qualifying', 'Abu Dhabi Grand Prix 2025', 'Completed', 'Dry'),
('Race', 'Abu Dhabi Grand Prix 2025', 'Completed', 'Dry'),

-- Australian Grand Prix 2024
('Free Practice 1', 'Australian Grand Prix 2024', 'Completed', 'Dry'),
('Free Practice 2', 'Australian Grand Prix 2024', 'Completed', 'Dry'),
('Free Practice 3', 'Australian Grand Prix 2024', 'Completed', 'Dry'),
('Qualifying', 'Australian Grand Prix 2024', 'Completed', 'Dry'),
('Race', 'Australian Grand Prix 2024', 'Completed', 'Dry'),

-- Chinese Grand Prix 2024 (Sprint Format)
('Free Practice 1', 'Chinese Grand Prix 2024', 'Completed', 'Dry'),
('Sprint Qualifying', 'Chinese Grand Prix 2024', 'Completed', 'Dry'),
('Sprint Race', 'Chinese Grand Prix 2024', 'Completed', 'Wet'),
('Qualifying', 'Chinese Grand Prix 2024', 'Completed', 'Dry'),
('Race', 'Chinese Grand Prix 2024', 'Completed', 'Dry'),

-- Japanese Grand Prix 2024
('Free Practice 1', 'Japanese Grand Prix 2024', 'Completed', 'Dry'),
('Free Practice 2', 'Japanese Grand Prix 2024', 'Completed', 'Dry'),
('Free Practice 3', 'Japanese Grand Prix 2024', 'Completed', 'Dry'),
('Qualifying', 'Japanese Grand Prix 2024', 'Completed', 'Dry'),
('Race', 'Japanese Grand Prix 2024', 'Completed', 'Dry'),

-- Bahrain Grand Prix 2024
('Free Practice 1', 'Bahrain Grand Prix 2024', 'Completed', 'Dry'),
('Free Practice 2', 'Bahrain Grand Prix 2024', 'Completed', 'Dry'),
('Free Practice 3', 'Bahrain Grand Prix 2024', 'Completed', 'Dry'),
('Qualifying', 'Bahrain Grand Prix 2024', 'Completed', 'Dry'),
('Race', 'Bahrain Grand Prix 2024', 'Completed', 'Dry'),

-- Saudi Arabian Grand Prix 2024
('Free Practice 1', 'Saudi Arabian Grand Prix 2024', 'Completed', 'Dry'),
('Free Practice 2', 'Saudi Arabian Grand Prix 2024', 'Completed', 'Dry'),
('Free Practice 3', 'Saudi Arabian Grand Prix 2024', 'Completed', 'Dry'),
('Qualifying', 'Saudi Arabian Grand Prix 2024', 'Completed', 'Dry'),
('Race', 'Saudi Arabian Grand Prix 2024', 'Completed', 'Dry'),

-- Miami Grand Prix 2024 (Sprint Format)
('Free Practice 1', 'Miami Grand Prix 2024', 'Completed', 'Dry'),
('Sprint Qualifying', 'Miami Grand Prix 2024', 'Completed', 'Dry'),
('Sprint Race', 'Miami Grand Prix 2024', 'Completed', 'Dry'),
('Qualifying', 'Miami Grand Prix 2024', 'Completed', 'Dry'),
('Race', 'Miami Grand Prix 2024', 'Completed', 'Dry'),

-- Emilia Romagna Grand Prix 2024
('Free Practice 1', 'Emilia Romagna Grand Prix 2024', 'Completed', 'Dry'),
('Free Practice 2', 'Emilia Romagna Grand Prix 2024', 'Completed', 'Dry'),
('Free Practice 3', 'Emilia Romagna Grand Prix 2024', 'Completed', 'Dry'),
('Qualifying', 'Emilia Romagna Grand Prix 2024', 'Completed', 'Dry'),
('Race', 'Emilia Romagna Grand Prix 2024', 'Completed', 'Dry'),

-- Monaco Grand Prix 2024
('Free Practice 1', 'Monaco Grand Prix 2024', 'Completed', 'Dry'),
('Free Practice 2', 'Monaco Grand Prix 2024', 'Completed', 'Dry'),
('Free Practice 3', 'Monaco Grand Prix 2024', 'Completed', 'Dry'),
('Qualifying', 'Monaco Grand Prix 2024', 'Completed', 'Dry'),
('Race', 'Monaco Grand Prix 2024', 'Completed', 'Dry'),

-- Spanish Grand Prix 2024
('Free Practice 1', 'Spanish Grand Prix 2024', 'Completed', 'Dry'),
('Free Practice 2', 'Spanish Grand Prix 2024', 'Completed', 'Dry'),
('Free Practice 3', 'Spanish Grand Prix 2024', 'Completed', 'Dry'),
('Qualifying', 'Spanish Grand Prix 2024', 'Completed', 'Dry'),
('Race', 'Spanish Grand Prix 2024', 'Completed', 'Dry'),

-- Canadian Grand Prix 2024
('Free Practice 1', 'Canadian Grand Prix 2024', 'Completed', 'Wet'),
('Free Practice 2', 'Canadian Grand Prix 2024', 'Completed', 'Dry'),
('Free Practice 3', 'Canadian Grand Prix 2024', 'Completed', 'Dry'),
('Qualifying', 'Canadian Grand Prix 2024', 'Completed', 'Dry'),
('Race', 'Canadian Grand Prix 2024', 'Completed', 'Dry'),

-- Austrian Grand Prix 2024 (Sprint Format)
('Free Practice 1', 'Austrian Grand Prix 2024', 'Completed', 'Dry'),
('Sprint Qualifying', 'Austrian Grand Prix 2024', 'Completed', 'Dry'),
('Sprint Race', 'Austrian Grand Prix 2024', 'Completed', 'Dry'),
('Qualifying', 'Austrian Grand Prix 2024', 'Completed', 'Dry'),
('Race', 'Austrian Grand Prix 2024', 'Completed', 'Dry'),

-- British Grand Prix 2024
('Free Practice 1', 'British Grand Prix 2024', 'Completed', 'Wet'),
('Free Practice 2', 'British Grand Prix 2024', 'Completed', 'Dry'),
('Free Practice 3', 'British Grand Prix 2024', 'Completed', 'Wet'),
('Qualifying', 'British Grand Prix 2024', 'Completed', 'Wet'),
('Race', 'British Grand Prix 2024', 'Completed', 'Dry'),

-- Belgian Grand Prix 2024 (Standard Format in 2024)
('Free Practice 1', 'Belgian Grand Prix 2024', 'Completed', 'Wet'),
('Free Practice 2', 'Belgian Grand Prix 2024', 'Completed', 'Dry'),
('Free Practice 3', 'Belgian Grand Prix 2024', 'Completed', 'Dry'),
('Qualifying', 'Belgian Grand Prix 2024', 'Completed', 'Dry'),
('Race', 'Belgian Grand Prix 2024', 'Completed', 'Dry'),

-- Hungarian Grand Prix 2024
('Free Practice 1', 'Hungarian Grand Prix 2024', 'Completed', 'Dry'),
('Free Practice 2', 'Hungarian Grand Prix 2024', 'Completed', 'Dry'),
('Free Practice 3', 'Hungarian Grand Prix 2024', 'Completed', 'Dry'),
('Qualifying', 'Hungarian Grand Prix 2024', 'Completed', 'Dry'),
('Race', 'Hungarian Grand Prix 2024', 'Completed', 'Dry'),

-- Dutch Grand Prix 2024
('Free Practice 1', 'Dutch Grand Prix 2024', 'Completed', 'Dry'),
('Free Practice 2', 'Dutch Grand Prix 2024', 'Completed', 'Dry'),
('Free Practice 3', 'Dutch Grand Prix 2024', 'Completed', 'Dry'),
('Qualifying', 'Dutch Grand Prix 2024', 'Completed', 'Dry'),
('Race', 'Dutch Grand Prix 2024', 'Completed', 'Dry'),

-- Italian Grand Prix 2024
('Free Practice 1', 'Italian Grand Prix 2024', 'Completed', 'Dry'),
('Free Practice 2', 'Italian Grand Prix 2024', 'Completed', 'Dry'),
('Free Practice 3', 'Italian Grand Prix 2024', 'Completed', 'Dry'),
('Qualifying', 'Italian Grand Prix 2024', 'Completed', 'Dry'),
('Race', 'Italian Grand Prix 2024', 'Completed', 'Dry'),

-- Azerbaijan Grand Prix 2024
('Free Practice 1', 'Azerbaijan Grand Prix 2024', 'Completed', 'Dry'),
('Free Practice 2', 'Azerbaijan Grand Prix 2024', 'Completed', 'Dry'),
('Free Practice 3', 'Azerbaijan Grand Prix 2024', 'Completed', 'Dry'),
('Qualifying', 'Azerbaijan Grand Prix 2024', 'Completed', 'Dry'),
('Race', 'Azerbaijan Grand Prix 2024', 'Completed', 'Dry'),

-- Singapore Grand Prix 2024
('Free Practice 1', 'Singapore Grand Prix 2024', 'Completed', 'Dry'),
('Free Practice 2', 'Singapore Grand Prix 2024', 'Completed', 'Dry'),
('Free Practice 3', 'Singapore Grand Prix 2024', 'Completed', 'Dry'),
('Qualifying', 'Singapore Grand Prix 2024', 'Completed', 'Dry'),
('Race', 'Singapore Grand Prix 2024', 'Completed', 'Dry'),

-- United States Grand Prix 2024 (Sprint Format)
('Free Practice 1', 'United States Grand Prix 2024', 'Completed', 'Dry'),
('Sprint Qualifying', 'United States Grand Prix 2024', 'Completed', 'Dry'),
('Sprint Race', 'United States Grand Prix 2024', 'Completed', 'Dry'),
('Qualifying', 'United States Grand Prix 2024', 'Completed', 'Dry'),
('Race', 'United States Grand Prix 2024', 'Completed', 'Dry'),

-- Mexico City Grand Prix 2024
('Free Practice 1', 'Mexico City Grand Prix 2024', 'Completed', 'Dry'),
('Free Practice 2', 'Mexico City Grand Prix 2024', 'Completed', 'Dry'),
('Free Practice 3', 'Mexico City Grand Prix 2024', 'Completed', 'Dry'),
('Qualifying', 'Mexico City Grand Prix 2024', 'Completed', 'Dry'),
('Race', 'Mexico City Grand Prix 2024', 'Completed', 'Dry'),

-- São Paulo Grand Prix 2024 (Sprint Format)
('Free Practice 1', 'São Paulo Grand Prix 2024', 'Completed', 'Wet'),
('Sprint Qualifying', 'São Paulo Grand Prix 2024', 'Completed', 'Wet'),
('Sprint Race', 'São Paulo Grand Prix 2024', 'Completed', 'Dry'),
('Qualifying', 'São Paulo Grand Prix 2024', 'Completed', 'Dry'),
('Race', 'São Paulo Grand Prix 2024', 'Completed', 'Wet'),

-- Las Vegas Grand Prix 2024
('Free Practice 1', 'Las Vegas Grand Prix 2024', 'Completed', 'Dry'),
('Free Practice 2', 'Las Vegas Grand Prix 2024', 'Completed', 'Dry'),
('Free Practice 3', 'Las Vegas Grand Prix 2024', 'Completed', 'Dry'),
('Qualifying', 'Las Vegas Grand Prix 2024', 'Completed', 'Dry'),
('Race', 'Las Vegas Grand Prix 2024', 'Completed', 'Dry'),

-- Qatar Grand Prix 2024 (Sprint Format)
('Free Practice 1', 'Qatar Grand Prix 2024', 'Completed', 'Dry'),
('Sprint Qualifying', 'Qatar Grand Prix 2024', 'Completed', 'Dry'),
('Sprint Race', 'Qatar Grand Prix 2024', 'Completed', 'Dry'),
('Qualifying', 'Qatar Grand Prix 2024', 'Completed', 'Dry'),
('Race', 'Qatar Grand Prix 2024', 'Completed', 'Dry'),

-- Abu Dhabi Grand Prix 2024
('Free Practice 1', 'Abu Dhabi Grand Prix 2024', 'Completed', 'Dry'),
('Free Practice 2', 'Abu Dhabi Grand Prix 2024', 'Completed', 'Dry'),
('Free Practice 3', 'Abu Dhabi Grand Prix 2024', 'Completed', 'Dry'),
('Qualifying', 'Abu Dhabi Grand Prix 2024', 'Completed', 'Dry'),
('Race', 'Abu Dhabi Grand Prix 2024', 'Completed', 'Dry');

-- 9. Inserir Resultados (depende de Sessões e Piloto)
-- NOTA: IDs começam em 3 porque já existiam dados anteriores
INSERT INTO Resultados (NomeSessão, NomeGP, ID_Piloto, PosiçãoGrid, TempoFinal, PosiçãoFinal, Pontos, Status)
VALUES
-- Qualifying: Lap times para cada piloto (formato TIME)
('Qualifying', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 44), 0, '00:01:15.915', 1, 0, 'Completed'),
('Qualifying', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 1),  0, '00:01:16.024', 2, 0, 'Completed'), -- VER
('Qualifying', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 16), 0, '00:01:16.157', 3, 0, 'Completed'), -- LEC
('Qualifying', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 81), 0, '00:01:16.298', 4, 0, 'Completed'), -- PIA
('Qualifying', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 63), 0, '00:01:16.412', 5, 0, 'Completed'), -- RUS
('Qualifying', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 10), 0, '00:01:16.543', 6, 0, 'Completed'), -- GAS
('Qualifying', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 55), 0, '00:01:16.687', 7, 0, 'Completed'), -- SAI
('Qualifying', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 14), 0, '00:01:16.794', 8, 0, 'Completed'), -- ALO
('Qualifying', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 12), 0, '00:01:16.921', 9, 0, 'Completed'), -- ANT
('Qualifying', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 23), 0, '00:01:17.045', 10, 0, 'Completed'),-- ALB
('Qualifying', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 43), 0, '00:01:17.189', 11, 0, 'Completed'),-- COL
('Qualifying', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 22), 0, '00:01:17.312', 12, 0, 'Completed'),-- TSU
('Qualifying', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 31), 0, '00:01:17.456', 13, 0, 'Completed'),-- OCO
('Qualifying', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 27), 0, '00:01:17.587', 14, 0, 'Completed'),-- HUL
('Qualifying', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 18), 0, '00:01:17.723', 15, 0, 'Completed'),-- STR
('Qualifying', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 87), 0, '00:01:17.891', 16, 0, 'Completed'),-- BEA
('Qualifying', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 30), 0, '00:01:18.034', 17, 0, 'Completed'),-- LAW
('Qualifying', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 5),  0, '00:01:18.197', 18, 0, 'Completed'),-- BOR
('Qualifying', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 6),  0, '00:01:18.365', 19, 0, 'Completed'),-- HAD
('Qualifying', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 4),  0, '00:01:18.521', 20, 0, 'Completed'),

-- Race: Tempos finais de corrida (tempo total da corrida)
('Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 44), 1, '01:28:50.123', 1, 25, 'Finished'), 
('Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 16), 3, '01:28:52.456', 2, 18, 'Finished'), -- LEC +2.333s
('Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 1), 2, '01:28:55.789', 3, 15, 'Finished'), -- VER +5.666s
('Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 63), 5, '01:29:02.134', 4, 12, 'Finished'), -- RUS +12.011s
('Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 81), 4, '01:29:08.567', 5, 10, 'Finished'), -- PIA +18.444s
('Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 10), 6, '01:29:15.234', 6, 8, 'Finished'), -- GAS +25.111s
('Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 55), 7, '01:29:22.678', 7, 6, 'Finished'), -- SAI +32.555s
('Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 14), 8, '01:29:28.912', 8, 4, 'Finished'), -- ALO +38.789s
('Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 43), 11, '01:29:35.345', 9, 2, 'Finished'), -- COL +45.222s
('Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 12), 9, '01:29:41.678', 10, 1, 'Finished'), -- ANT +51.555s
('Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 23), 10, '01:29:48.234', 11, 0, 'Finished'), -- ALB +58.111s
('Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 22), 12, '01:29:54.567', 12, 0, 'Finished'), -- TSU +1:04.444
('Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 31), 13, '01:30:01.123', 13, 0, 'Finished'), -- OCO +1:11.000
('Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 27), 14, '01:30:07.456', 14, 0, 'Finished'), -- HUL +1:17.333
('Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 18), 15, '01:30:13.789', 15, 0, 'Finished'), -- STR +1:23.666
('Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 87), 16, '01:30:20.234', 16, 0, 'Finished'), -- BEA +1:30.111
('Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 30), 17, '01:30:26.567', 17, 0, 'Finished'), -- LAW +1:36.444
('Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 5), 18, '01:30:32.912', 18, 0, 'Finished'), -- BOR +1:42.789
('Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 6), 19, '01:30:39.345', 19, 0, 'Finished'), -- HAD +1:49.222
('Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 4), 20, '01:30:45.678', 20, 0, 'Finished'); -- NOR +1:55.555

-- 10. Inserir Pitstops (depende de Piloto e Sessões)
-- Australian Grand Prix 2025 - Race Pitstops
INSERT INTO Pitstop (NumeroVolta, DuraçãoParagem, DuraçãoPitlane, NomeSessão, NomeGP, ID_Piloto)
VALUES
-- Hamilton (44) - 2 stops strategy
(15, '00:00:02.234', '00:00:21.456', 'Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 44)),
(38, '00:00:02.189', '00:00:20.987', 'Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 44)),

-- Leclerc (16) - 2 stops strategy
(16, '00:00:02.567', '00:00:22.134', 'Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 16)),
(39, '00:00:02.401', '00:00:21.789', 'Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 16)),

-- Verstappen (1) - 2 stops strategy
(17, '00:00:02.890', '00:00:23.567', 'Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 1)),
(40, '00:00:02.123', '00:00:21.234', 'Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 1)),

-- Russell (63) - 2 stops strategy
(18, '00:00:02.345', '00:00:22.456', 'Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 63)),
(41, '00:00:02.678', '00:00:22.890', 'Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 63)),

-- Piastri (81) - 2 stops strategy (home race)
(14, '00:00:02.112', '00:00:21.234', 'Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 81)),
(36, '00:00:02.890', '00:00:23.123', 'Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 81)),

-- Gasly (10) - 2 stops strategy
(19, '00:00:02.456', '00:00:22.678', 'Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 10)),
(42, '00:00:02.234', '00:00:21.890', 'Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 10)),

-- Sainz (55) - 2 stops strategy
(20, '00:00:02.567', '00:00:23.012', 'Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 55)),
(43, '00:00:02.389', '00:00:22.234', 'Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 55)),

-- Alonso (14) - 1 stop strategy (veteran!)
(28, '00:00:02.678', '00:00:22.456', 'Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 14)),

-- Colapinto (43) - 2 stops strategy
(21, '00:00:02.901', '00:00:23.567', 'Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 43)),
(44, '00:00:02.456', '00:00:22.678', 'Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 43)),

-- Antonelli (12) - 2 stops strategy (rookie)
(22, '00:00:03.123', '00:00:24.234', 'Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 12)),
(45, '00:00:02.789', '00:00:23.456', 'Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 12)),

-- Albon (23) - 1 stop strategy
(30, '00:00:02.567', '00:00:22.890', 'Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 23)),

-- Tsunoda (22) - 2 stops strategy
(23, '00:00:02.456', '00:00:22.567', 'Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 22)),
(46, '00:00:02.678', '00:00:23.123', 'Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 22)),

-- Ocon (31) - 2 stops strategy
(24, '00:00:02.890', '00:00:23.789', 'Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 31)),
(47, '00:00:02.345', '00:00:22.012', 'Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 31)),

-- Hülkenberg (27) - 1 stop strategy
(32, '00:00:02.678', '00:00:23.234', 'Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 27)),

-- Stroll (18) - 2 stops strategy
(25, '00:00:03.234', '00:00:24.567', 'Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 18)),
(48, '00:00:02.890', '00:00:23.890', 'Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 18)),

-- Bearman (87) - 2 stops strategy (rookie)
(26, '00:00:03.456', '00:00:25.123', 'Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 87)),
(49, '00:00:03.012', '00:00:24.456', 'Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 87)),

-- Lawson (30) - 1 stop strategy
(33, '00:00:02.789', '00:00:23.567', 'Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 30)),

-- Bortoleto (5) - 2 stops strategy (rookie)
(27, '00:00:03.567', '00:00:25.678', 'Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 5)),
(50, '00:00:03.234', '00:00:24.890', 'Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 5)),

-- Hadjar (6) - 2 stops strategy (rookie)
(28, '00:00:03.890', '00:00:26.234', 'Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 6)),
(51, '00:00:03.456', '00:00:25.567', 'Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 6)),

-- Norris (4) - 3 stops strategy (disaster race - had to recover)
(12, '00:00:02.345', '00:00:22.123', 'Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 4)),
(29, '00:00:02.567', '00:00:22.890', 'Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 4)),
(52, '00:00:02.789', '00:00:23.456', 'Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 4));

-- 11. Inserir Penalizações (depende de Piloto e Sessões)
-- Australian Grand Prix 2025 - Penalties
INSERT INTO Penalizações (TipoPenalização, Motivo, NomeSessão, NomeGP, ID_Piloto)
VALUES
-- QUALIFYING PENALTIES
-- Norris - Grid penalty for impeding in qualifying (manifests in race starting grid)
('Grid Penalty (-3 positions)', 'Impeding another driver during Qualifying', 'Qualifying', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 4)),

-- Tsunoda - Warning for track limits in qualifying
('Warning', 'Exceeding track limits multiple times (Turns 9 and 10)', 'Qualifying', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 22)),

-- RACE PENALTIES
-- Verstappen - 5s time penalty for unsafe release
('5 Second Time Penalty', 'Unsafe release from pit box on lap 17', 'Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 1)),

-- Stroll - 5s time penalty for forcing another driver off track
('5 Second Time Penalty', 'Forcing Bearman off track at Turn 3', 'Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 18)),

-- Hadjar - 10s time penalty for causing collision
('10 Second Time Penalty', 'Causing collision with Bortoleto at Turn 1', 'Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 6)),

-- Bearman - Warning for pit lane speeding
('Warning', 'Exceeding pit lane speed limit by 2 km/h', 'Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 87)),

-- Antonelli - 5s time penalty for gaining advantage off track
('5 Second Time Penalty', 'Gaining lasting advantage by leaving track at Turn 6', 'Race', 'Australian Grand Prix 2025', (SELECT ID_Piloto FROM Piloto WHERE NumeroPermanente = 12));


-- 12. Inserir Staff (administradores do sistema + todos os membros da equipa com username e password)
-- Verificar se já existem para evitar duplicados
IF NOT EXISTS (SELECT 1 FROM Staff WHERE Username = 'admin')
BEGIN
    INSERT INTO Staff (Username, Password, NomeCompleto, Role)
    VALUES
    -- Administradores do sistema
    ('admin', 'admin123', 'System Administrator', 'Staff'),
    ('testadmin', 'test123', 'Test Administrator', 'Staff'),
    
    -- 1. Alpine
    ('pgasly', 'alpine2025', 'Pierre Gasly', 'Staff'),
('fcolapinto', 'alpine2025', 'Franco Colapinto', 'Staff'),
('jdoohan', 'alpine2025', 'Jack Doohan', 'Staff'),
('paron', 'alpine2025', 'Paul Aron', 'Staff'),
('kmaini', 'alpine2025', 'Kush Maini', 'Staff'),
('fbriatore', 'alpine2025', 'Flavio Briatore', 'Staff'),
('dsanchez', 'alpine2025', 'David Sanchez', 'Staff'),

-- 2. Aston Martin
('falonso', 'aston2025', 'Fernando Alonso', 'Staff'),
('lstroll', 'aston2025 ', 'Lance Stroll', 'Staff'),
('svandoorne', 'aston2025', 'Stoffel Vandoorne', 'Staff'),
('acowell', 'aston2025', 'Andy Cowell', 'Staff'),
('ecardile', 'aston2025', 'Enrico Cardile', 'Staff'),

-- 3. Ferrari
('cleclerc', 'ferrari2025', 'Charles Leclerc', 'Staff'),
('lhamilton', 'ferrari2025', 'Lewis Hamilton', 'Staff'),
('zguanyu', 'ferrari2025', 'Zhou Guanyu', 'Staff'),
('agiovinazzi', 'ferrari2025', 'Antonio Giovinazzi', 'Staff'),
('fvasseur', 'ferrari2025', 'Frédéric Vasseur', 'Staff'),
('lserra', 'ferrari2025', 'Loic Serra', 'Staff'),
('egualtieri', 'ferrari2025', 'Enrico Gualtieri', 'Staff'),

-- 4. Red Bull Racing
('mverstappen', 'redbull2025', 'Max Verstappen', 'Staff'),
('ytsunoda', 'redbull2025', 'Yuki Tsunoda', 'Staff'),
('lmekies', 'redbull2025', 'Laurent Mekies', 'Staff'),
('pwache', 'redbull2025', 'Pierre Waché', 'Staff'),

-- 5. Mercedes-AMG
('grussell', 'merc2025', 'George Russell', 'Staff'),
('kantonelli', 'merc2025', 'Kimi Antonelli', 'Staff'),
('vbottas', 'merc2025', 'Valtteri Bottas', 'Staff'),
('fvesti', 'merc2025', 'Frederik Vesti', 'Staff'),
('twolff', 'merc2025', 'Toto Wolff', 'Staff'),
('jallison', 'merc2025', 'James Allison', 'Staff'),

-- 6. McLaren
('lnorris', 'mclaren2025', 'Lando Norris', 'Staff'),
('opiastri', 'mclaren2025', 'Oscar Piastri', 'Staff'),
('astella', 'mclaren2025', 'Andrea Stella', 'Staff'),
('pprodromou', 'mclaren2025', 'Peter Prodromou', 'Staff'),
('nhouldey', 'mclaren2025', 'Neil Houldey', 'Staff'),

-- 7. Williams
('aalbon', 'williams2025', 'Alexander Albon', 'Staff'),
('csainz', 'williams2025', 'Carlos Sainz', 'Staff'),
('jvowles', 'williams2025', 'James Vowles', 'Staff'),
('pfry', 'williams2025', 'Pat Fry', 'Staff'),

-- 8. Haas
('eocon', 'haas2025', 'Esteban Ocon', 'Staff'),
('obearman', 'haas2025', 'Oliver Bearman', 'Staff'),
('rhirakawa', 'haas2025', 'Ryo Hirakawa', 'Staff'),
('akomatsu', 'haas2025', 'Ayao Komatsu', 'Staff'),
('adezordo', 'haas2025', 'Andrea De Zordo', 'Staff'),

-- 9. Racing Bulls (VCARB)
('llawson', 'vcarb2025', 'Liam Lawson', 'Staff'),
('ihadjar', 'vcarb2025', 'Isack Hadjar', 'Staff'),
('aiwasa', 'vcarb2025', 'Ayumu Iwasa', 'Staff'),
('apermane', 'vcarb2025', 'Alan Permane', 'Staff'),
('tgoss', 'vcarb2025', 'Tim Goss', 'Staff'),

-- 10. Sauber / Audi
    ('nhulkenberg', 'sauber2025', 'Nico Hülkenberg', 'Staff'),
    ('gbortoleto', 'sauber2025', 'Gabriel Bortoleto', 'Staff'),
    ('jwheatley', 'sauber2025', 'Jonathan Wheatley', 'Staff'),
    ('jkey', 'sauber2025', 'James Key', 'Staff');
END



