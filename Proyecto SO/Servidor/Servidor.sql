
DROP DATABASE IF EXISTS M9_BBDDServidor;
CREATE DATABASE M9_BBDDServidor;
USE M9_BBDDServidor;

CREATE TABLE Player ( Name VARCHAR(20) NOT NULL PRIMARY KEY,
					 Password TEXT NOT NULL
					 ) ENGINE=InnoDB;

CREATE TABLE Game (
				   Identifier INTEGER PRIMARY KEY NOT NULL,
				   EndDateTime DATETIME NOT NULL,
				   Duration TIME NOT NULL,
				   Winner TEXT NOT NULL
				   ) ENGINE=InnoDB;

CREATE TABLE Participation (
							Player VARCHAR(20) NOT NULL,
							Game INTEGER NOT NULL,
							Position INTEGER NOT NULL,
							FOREIGN KEY (Player) REFERENCES Player (Name),
							FOREIGN KEY (Game) REFERENCES Game (Identifier)
							) ENGINE=InnoDB;

INSERT INTO Player (Name, Password) VALUES ('Alice', 'pass123');
INSERT INTO Player (Name, Password) VALUES ('Bob', 'secure456');
INSERT INTO Player (Name, Password) VALUES ('Charlie', 'mysecret789');


INSERT INTO Game (Identifier, EndDateTime, Duration, Winner) VALUES (1, '2024-09-29 15:30:00', '01:20:05', 'Alice');
INSERT INTO Game (Identifier, EndDateTime, Duration, Winner) VALUES (2, '2024-09-29 16:30:00', '03:43:01', 'Bob');
INSERT INTO Game (Identifier, EndDateTime, Duration, Winner) VALUES (3, '2024-09-29 17:30:00', '02:11:05', 'Charlie');

INSERT INTO Participation (Player, Game, Position) VALUES ('Alice', 1, 1);
INSERT INTO Participation (Player, Game, Position) VALUES ('Bob', 2, 2);
INSERT INTO Participation (Player, Game, Position) VALUES ('Charlie', 3, 1);
INSERT INTO Participation (Player, Game, Position) VALUES ('Alice', 2, 1);
INSERT INTO Participation (Player, Game, Position) VALUES ('Bob', 1, 2);   
INSERT INTO Participation (Player, Game, Position) VALUES ('Charlie', 2, 1);
INSERT INTO Participation (Player, Game, Position) VALUES ('Alice', 3, 1);  
INSERT INTO Participation (Player, Game, Position) VALUES ('Charlie', 1, 2);
					


SELECT Game.Identifier, Game.EndDateTime
	FROM Player
	JOIN Participation ON Player.Name = Participation.Player
	JOIN Game ON Participation.Game = Game.Identifier

SET @prev_winner := NULL;
SET @streak := 0;

SELECT Winner, MAX(Streak) AS MaxStreak
FROM (
    SELECT 
        Winner,
        EndDateTime,
        CASE 
            WHEN @prev_winner = Winner AND DATE(@prev_date) = DATE(EndDateTime - INTERVAL 1 DAY)
            THEN @streak := @streak + 1
            ELSE @streak := 1
        END AS Streak,
        @prev_winner := Winner,
        @prev_date := EndDateTime
    FROM Game
    WHERE Winner = 'Charlie'
    ORDER BY EndDateTime
) AS StreakTable;
