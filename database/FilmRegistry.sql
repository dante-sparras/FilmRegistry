-- Creates the FilmRegistry database, Genre and Movie, and the sample rows.
-- Run this in SQL Server Management Studio or with sqlcmd.

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

IF NOT EXISTS (SELECT 1 FROM dbo.Genre)
BEGIN
    INSERT INTO dbo.Genre (Name)
    VALUES
        (N'Sci-Fi'),
        (N'Horror'),
        (N'Action');
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Movie)
BEGIN
    INSERT INTO dbo.Movie (Title, ReleaseYear, GenreId)
    VALUES
        (N'Alien', 1979, (SELECT Id FROM dbo.Genre WHERE Name = N'Sci-Fi')),
        (N'The Shining', 1980, (SELECT Id FROM dbo.Genre WHERE Name = N'Horror')),
        (N'Die Hard', 1988, (SELECT Id FROM dbo.Genre WHERE Name = N'Action'));
END
GO
