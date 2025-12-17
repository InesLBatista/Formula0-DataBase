/*
 * Script DML (Data Manipulation Language) com dados de exemplo.
 * A ordem das inserções é importante para respeitar as Foreign Keys.
 */
/*
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
*/
-- 2. Inserir Temporada (sem dependências)
INSERT INTO Temporada (Ano, NumCorridas)
VALUES (2024, 0); -- NumCorridas pode ser atualizado por um trigger, por exemplo
/*
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
*/
INSERT INTO Membros_da_Equipa (Nome, Nacionalidade, DataNascimento, Género, Função, ID_Equipa)
VALUES
-- 1. Alpine (ID_Equipa = 1)
('Pierre Gasly', 'French', '07-02-1996', 'M', 'Driver', 1),
('Franco Colapinto', 'Argentine', '27-05-2003', 'M', 'Driver', 1),
('Jack Doohan', 'Australian', '20-01-2003', 'M', 'Reserve Driver', 1),
('Paul Aron', 'Estonian', '04-02-2004', 'M', 'Reserve Driver', 1),
('Kush Maini', 'Indian', '09-01-2000', 'M', 'Reserve Driver', 1),
('Flavio Briatore', 'Italian', '12-04-1950', 'M', 'Team Chief', 1),
('David Sanchez', 'French', '15-03-1980', 'M', 'Technical Chief', 1),

-- 2. Aston Martin (ID_Equipa = 2)
('Fernando Alonso', 'Spanish', '29-07-1981', 'M', 'Driver', 2),
('Lance Stroll', 'Canadian', '29-10-1998', 'M', 'Driver', 2),
('Stoffel Vandoorne', 'Belgian', '26-03-1992', 'M', 'Reserve Driver', 2),
('Andy Cowell', 'British', '12-02-1969', 'M', 'Team Chief', 2),
('Enrico Cardile', 'Italian', '05-04-1975', 'M', 'Technical Chief', 2),

-- 3. Ferrari (ID_Equipa = 3)
('Charles Leclerc', 'Monegasque', '16-10-1997', 'M', 'Driver', 3),
('Lewis Hamilton', 'British', '07-01-1985', 'M', 'Driver', 3),
('Zhou Guanyu', 'Chinese', '30-05-1999', 'M', 'Reserve Driver', 3),
('Antonio Giovinazzi', 'Italian', '14-12-1993', 'M', 'Reserve Driver', 3),
('Frédéric Vasseur', 'French', '28-05-1968', 'M', 'Team Chief', 3),
('Loic Serra', 'French', '20-09-1972', 'M', 'Technical Chief', 3), --Chasis
('Enrico Gualtieri', 'Italian', '21-02-1975', 'M', 'Technical Chief', 3), --Power Unit


-- 4. Red Bull Racing (ID_Equipa = 4)
('Max Verstappen', 'Dutch', '30-09-1997', 'M', 'Driver', 4),
('Yuki Tsunoda', 'Japanese', '11-05-2000', 'M', 'Driver', 4),
('Laurent Mekies', 'French', '28-04-1977', 'M', 'Team Chief', 4),
('Pierre Waché', 'French', '10-12-1974', 'M', 'Technical Chief', 4),

-- 5. Mercedes-AMG (ID_Equipa = 5)
('George Russell', 'British', '15-02-1998', 'M', 'Driver', 5),
('Kimi Antonelli', 'Italian', '25-08-2006', 'M', 'Driver', 5),
('Valtteri Bottas', 'Finnish', '28-08-1989', 'M', 'Reserve Driver', 5),
('Toto Wolff', 'Austrian', '12-01-1972', 'M', 'Team Principal', 5),
('James Allison', 'British', '22-02-1968', 'M', 'Technical Director', 5),

-- 6. McLaren (ID_Equipa = 6)
('Lando Norris', 'British', '13-11-1999', 'M', 'Driver', 6),
('Oscar Piastri', 'Australian', '06-04-2001', 'M', 'Driver', 6),
('Pato O’Ward', 'Mexican', '06-05-1999', 'M', 'Reserve Driver', 6),
('Andrea Stella', 'Italian', '22-02-1971', 'M', 'Team Principal', 6),
('Rob Marshall', 'British', '14-04-1968', 'M', 'Chief Designer', 6),

-- 7. Williams (ID_Equipa = 7)
('Alexander Albon', 'Thai', '23-03-1996', 'M', 'Driver', 7),
('Carlos Sainz', 'Spanish', '01-09-1994', 'M', 'Driver', 7),
('James Vowles', 'British', '20-06-1979', 'M', 'Team Principal', 7),
('Pat Fry', 'British', '17-03-1964', 'M', 'Technical Director', 7),

-- 8. Haas (ID_Equipa = 8)
('Esteban Ocon', 'French', '17-09-1996', 'M', 'Driver', 8),
('Oliver Bearman', 'British', '08-05-2005', 'M', 'Driver', 8),
('Ayao Komatsu', 'Japanese', '28-01-1976', 'M', 'Team Principal', 8),

-- 9. Racing Bulls (RB) (ID_Equipa = 9)
('Yuki Tsunoda', 'Japanese', '11-05-2000', 'M', 'Driver', 9),
('Isack Hadjar', 'French', '28-09-2004', 'M', 'Driver', 9),
('Laurent Mekies', 'French', '28-04-1977', 'M', 'Team Principal', 9),

-- 10. Sauber / Audi (ID_Equipa = 10)
('Nico Hülkenberg', 'German', '19-08-1987', 'M', 'Driver', 10),
('Gabriel Bortoleto', 'Brazilian', '14-10-2004', 'M', 'Driver', 10),
('Mattia Binotto', 'Italian', '03-11-1969', 'M', 'CTO/Leader', 10),
('Jonathan Wheatley', 'British', '07-05-1967', 'M', 'Team Principal', 10);

-- 5. Inserir Contratos (depende de Membros_da_Equipa)
INSERT INTO Contrato (AnoInicio, AnoFim, Função, Salário, Género, ID_Membro)
VALUES
-- Alpine
(2023, 2026, 'Piloto', 8000000.00, 'M', 1), -- Pierre Gasly
(2025, 2027, 'Piloto', 2500000.00, 'M', 2), -- Jack Doohan
(2024, 2025, 'Reserve Driver', 500000.00, 'M', 3), -- Paul Aron
(2024, 2026, 'Engenheiro Chefe', 1200000.00, 'M', 4), -- David Sanchez

-- Aston Martin
(2023, 2026, 'Piloto', 18000000.00, 'M', 5), -- Fernando Alonso
(2022, 2025, 'Piloto', 12000000.00, 'M', 6), -- Lance Stroll
(2024, 2025, 'Reserve Driver', 600000.00, 'M', 7), -- Stoffel Vandoorne
(2024, 2025, 'Reserve Driver', 550000.00, 'M', 8), -- Felipe Drugovich
(2024, 2025, 'Reserve Driver', 450000.00, 'M', 9), -- Jak Crawford
(2024, 2026, 'Engenheiro Chefe', 1500000.00, 'M', 10), -- Luca Furbatto

-- Ferrari
(2019, 2029, 'Piloto', 12000000.00, 'M', 11), -- Charles Leclerc
(2025, 2027, 'Piloto', 45000000.00, 'M', 12), -- Lewis Hamilton
(2024, 2025, 'Reserve Driver', 400000.00, 'M', 13), -- Oliver Bearman
(2024, 2026, 'Engenheiro Chefe', 1800000.00, 'M', 14); -- Loic Serra

-- 6. Inserir Pilotos (depende de Equipa e Membros_da_Equipa)
-- Nota: Reserve Drivers NÃO têm entrada em Piloto até correrem
INSERT INTO Piloto (NumeroPermanente, Abreviação, ID_Equipa, ID_Membro)
VALUES
-- Alpine
(10, 'GAS', 1, 1), -- Pierre Gasly
(61, 'DOO', 1, 2), -- Jack Doohan

-- Aston Martin
(14, 'ALO', 2, 5), -- Fernando Alonso
(18, 'STR', 2, 6), -- Lance Stroll

-- Ferrari
(16, 'LEC', 3, 11), -- Charles Leclerc
(44, 'HAM', 3, 12); -- Lewis Hamilton

-- 7. Inserir Grande Prémio (depende de Circuito e Temporada)
INSERT INTO Grande_Prémio (Nome, DataCorrida, NumeroVoltas, ID_Circuito, Ano_Temporada)
VALUES ('Australian Grand Prix', '2024-03-24', 58, 1, 2024);

-- 8. Inserir Sessões (depende de Grande_Prémio)
INSERT INTO Sessões (NomeSessão, NomeGP, Estado, CondiçõesPista)
VALUES
('Practice 1', 'Australian Grand Prix', 'Concluída', 'Seco'),
('Qualifying', 'Australian Grand Prix', 'Concluída', 'Seco'),
('Race', 'Australian Grand Prix', 'Concluída', 'Seco');

-- 9. Inserir Resultados (depende de Sessões e Piloto)
INSERT INTO Resultados (NomeSessão, NomeGP, ID_Piloto, PosiçãoGrid, PosiçãoFinal, Pontos, Status)
VALUES
-- Race results - Australian GP
('Race', 'Australian Grand Prix', 1, 5, 3, 15, 'Terminou'), -- Gasly P3
('Race', 'Australian Grand Prix', 2, 12, 10, 1, 'Terminou'), -- Doohan P10
('Race', 'Australian Grand Prix', 3, 8, 6, 8, 'Terminou'), -- Alonso P6
('Race', 'Australian Grand Prix', 4, 14, 12, 0, 'Terminou'), -- Stroll P12
('Race', 'Australian Grand Prix', 5, 1, 1, 25, 'Terminou'), -- Leclerc P1
('Race', 'Australian Grand Prix', 6, 2, 2, 18, 'Terminou'); -- Hamilton P2

PRINT 'Dados de exemplo inseridos com sucesso!';
