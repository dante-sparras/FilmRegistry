-- The console app creates this database and sample data on startup when they are missing.
-- You can also run this script in SQL Server Management Studio or with sqlcmd.

IF DB_ID(N'FilmRegistry') IS NULL
    CREATE DATABASE FilmRegistry;
GO

USE FilmRegistry;
GO

IF OBJECT_ID(N'dbo.Genre', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Genre
    (
        Id int IDENTITY(1, 1) NOT NULL CONSTRAINT PK_Genre PRIMARY KEY,
        Name nvarchar(100) NOT NULL CONSTRAINT UQ_Genre_Name UNIQUE
    );
END
GO

IF OBJECT_ID(N'dbo.Movie', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Movie
    (
        Id int IDENTITY(1, 1) NOT NULL CONSTRAINT PK_Movie PRIMARY KEY,
        Title nvarchar(200) NOT NULL,
        ReleaseYear int NOT NULL,
        GenreId int NOT NULL CONSTRAINT FK_Movie_Genre FOREIGN KEY REFERENCES dbo.Genre (Id)
    );
END
GO

INSERT INTO dbo.Genre (Name)
SELECT Seed.Name
FROM (VALUES
    (N'Sci-Fi'),
    (N'Horror'),
    (N'Action'),
    (N'Comedy')
) AS Seed (Name)
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.Genre
    WHERE Genre.Name = Seed.Name
);
GO

INSERT INTO dbo.Movie (Title, ReleaseYear, GenreId)
SELECT Seed.Title, Seed.ReleaseYear, Genre.Id
FROM (VALUES
    (N'Alien', 1979, N'Sci-Fi'),
    (N'Blade Runner', 1982, N'Sci-Fi'),
    (N'The Matrix', 1999, N'Sci-Fi'),
    (N'Children of Men', 2006, N'Sci-Fi'),
    (N'Ex Machina', 2014, N'Sci-Fi'),
    (N'Interstellar', 2014, N'Sci-Fi'),
    (N'Arrival', 2016, N'Sci-Fi'),
    (N'The Exorcist', 1973, N'Horror'),
    (N'Halloween', 1978, N'Horror'),
    (N'The Shining', 1980, N'Horror'),
    (N'A Nightmare on Elm Street', 1984, N'Horror'),
    (N'Get Out', 2017, N'Horror'),
    (N'Hereditary', 2018, N'Horror'),
    (N'Raiders of the Lost Ark', 1981, N'Action'),
    (N'Die Hard', 1988, N'Action'),
    (N'Terminator 2: Judgment Day', 1991, N'Action'),
    (N'The Raid', 2011, N'Action'),
    (N'John Wick', 2014, N'Action'),
    (N'Mad Max: Fury Road', 2015, N'Action'),
    (N'Some Like It Hot', 1959, N'Comedy'),
    (N'Monty Python and the Holy Grail', 1975, N'Comedy'),
    (N'Groundhog Day', 1993, N'Comedy'),
    (N'Superbad', 2007, N'Comedy'),
    (N'Bridesmaids', 2011, N'Comedy'),
    (N'The Grand Budapest Hotel', 2014, N'Comedy')
) AS Seed (Title, ReleaseYear, GenreName)
INNER JOIN dbo.Genre ON Genre.Name = Seed.GenreName
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.Movie
    WHERE Movie.Title = Seed.Title
);
GO
