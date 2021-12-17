CREATE DATABASE MusicFlow
USE MusicFlow
--DROP TABLE Users

CREATE TABLE Users (
    id INT NOT NULL IDENTITY(1,1),
    email VARCHAR(255) UNIQUE,
    username VARCHAR(255) UNIQUE,
    password BINARY(32), -- sha256 salted hashed password
    --salt BINARY(4) salt for each user (harder to bruteforce)
    --avatar VARBINARY(max), -- IMAGE was deprecated in ms sql
    avatar VARCHAR(255), -- id of avatar path
    role TINYINT DEFAULT 0, -- 0 user; 1 staff
    PRIMARY KEY (id)
)

--INSERT INTO Users(email, username, password) OUTPUT inserted.id VALUES('', '', '')

--SELECT * FROM Users
--SELECT TOP 100 u.* FROM Users u