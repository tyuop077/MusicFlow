CREATE DATABASE MusicFlow
USE MusicFlow
--DROP TABLE Users

CREATE TABLE Users (
    id INT NOT NULL IDENTITY(1,1),
    email VARCHAR(255) UNIQUE,
    username VARCHAR(255) UNIQUE,
    password BINARY(32), -- sha256 salted hashed password
    avatar VARCHAR(255), -- id of avatar path
    PRIMARY KEY (id)
)

--INSERT INTO Users(email, username, password) OUTPUT inserted.id VALUES('', '', '')

--SELECT * FROM Users
--SELECT TOP 100 u.* FROM Users u