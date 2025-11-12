INSERT INTO Roles (Name, Description)
VALUES 
('manager', 'Administrador de liga'),
('player', 'Jugador de liga'),
('admin', 'Administrador global');

INSERT INTO Positions (Name, Abbreviation, Description)
VALUES
('Quarterback', 'QB', 'Mariscal de campo'),
('Running Back', 'RB', 'Corredor'),
('Wide Receiver', 'WR', 'Receptor abierto'),
('Tight End', 'TE', 'Ala cerrada'),
('Kicker', 'K', 'Pateador'),
('Defense/Special Teams', 'DEF', 'Defensa y equipos especiales'),
('Flex RB/WR', 'FLEX', 'Posición flexible: RB o WR'),
('Bench', 'BN', 'Banca, cualquier posición'),
('Injured Reserve', 'IR', 'Reserva por lesión');

INSERT INTO Scorings (Name, Abbreviation, Description, Points, Unit)
VALUES
('Passing Yards', 'PY', '1 punto cada 25 yardas por pase', 1, '25yds'),
('Passing Touchdowns', 'PTD', 'Touchdown por pase', 4, 'TD'),
('Interceptions Thrown', 'INTT', 'Intercepción lanzada', -2, 'INT'),
('Rushing Yards', 'RY', '1 punto cada 10 yardas por acarreo', 1, '10yds'),
('Receptions', 'REC', 'Recepción', 1, 'Reception'),
('Receiving Yards', 'REY', '1 punto cada 10 yardas por recepción', 1, '10yds'),
('Rush/Recv Touchdowns', 'RTD', 'Touchdown por acarreo o recepción', 6, 'TD'),
('Sacks', 'SACK', 'Captura defensiva', 1, 'Sack'),
('Interceptions', 'INT', 'Intercepción defensiva', 2, 'INT'),
('Fumbles Recovered', 'FR', 'Balón suelto recuperado', 2, 'Fumble'),
('Safeties', 'SAFE', 'Safety defensivo', 2, 'Safety'),
('Touchdowns', 'TD', 'Touchdown defensivo', 6, 'TD'),
('Team Def 2-point Return', 'DEF2PT', 'Retorno de 2 puntos defensivo', 2, '2ptReturn'),
('PAT Made', 'PAT', 'Punto extra', 1, 'PAT'),
('FG Made 0-50', 'FG', 'Gol de campo de 0-50 yardas', 3, 'FG0-50'),
('FG Made 50+', 'FG50', 'Gol de campo de 50+ yardas', 5, 'FG50+'),
('Points Allowed <=10', 'PA10', 'Puntos permitidos <=10', 5, 'PointsAllowed'),
('Points Allowed <=20', 'PA20', 'Puntos permitidos <=20', 2, 'PointsAllowed'),
('Points Allowed <=30', 'PA30', 'Puntos permitidos <=30', 0, 'PointsAllowed'),
('Points Allowed >30', 'PA30+', 'Puntos permitidos >30', -2, 'PointsAllowed');