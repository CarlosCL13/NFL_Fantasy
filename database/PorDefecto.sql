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
('Flex RB/WR', 'FLEX', 'Posici�n flexible: RB o WR'),
('Bench', 'BN', 'Banca, cualquier posici�n'),
('Injured Reserve', 'IR', 'Reserva por lesi�n');

INSERT INTO Scorings (Name, Abbreviation, Description, Points, Unit)
VALUES
('Passing Yards', 'PY', '1 punto cada 25 yardas por pase', 1, '25yds'),
('Passing Touchdowns', 'PTD', 'Touchdown por pase', 4, 'TD'),
('Interceptions Thrown', 'INTT', 'Intercepci�n lanzada', -2, 'INT'),
('Rushing Yards', 'RY', '1 punto cada 10 yardas por acarreo', 1, '10yds'),
('Receptions', 'REC', 'Recepci�n', 1, 'Reception'),
('Receiving Yards', 'REY', '1 punto cada 10 yardas por recepci�n', 1, '10yds'),
('Rush/Recv Touchdowns', 'RTD', 'Touchdown por acarreo o recepci�n', 6, 'TD'),
('Sacks', 'SACK', 'Captura defensiva', 1, 'Sack'),
('Interceptions', 'INT', 'Intercepci�n defensiva', 2, 'INT'),
('Fumbles Recovered', 'FR', 'Bal�n suelto recuperado', 2, 'Fumble'),
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

INSERT INTO Designaciones(Codigo, Descripcion)
VALUES 
('O', 'No jugaran'),
('D', 'Muy poco probable que juegue 25%'),
('Q', 'Cuestionable 50%'),
('P', 'Participacion plena'),
('IR', 'Reserva de lesiones'),
('PUP', 'Incapaz Físicamente de Jugar'),
('SUS', 'Suspendido');