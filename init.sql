CREATE DATABASE MusicFlow
USE MusicFlow

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

--SELECT TOP 100 * FROM Users

CREATE TABLE ForumThreads (
    tid INT NOT NULL IDENTITY(1,1) PRIMARY KEY, -- thread id
    topic VARCHAR(64) NOT NULL, -- DEFAULT 'Untitled thread'
    oid INT NOT NULL, -- thread owner id
    pinned BIT NOT NULL DEFAULT 0,
    locked BIT NOT NULL DEFAULT 0,
    CONSTRAINT FTOwnerById FOREIGN KEY (oid) REFERENCES Users(id) -- ON DELETE CASCADE ON UPDATE CASCADE
)

CREATE TABLE ThreadsContents (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY, -- message id
    tid INT NOT NULL,
    oid INT NOT NULL, -- message owner id
    content VARCHAR(4096) NOT NULL,
    rid INT, -- reply id
    CONSTRAINT TCOwnerById FOREIGN KEY (oid) REFERENCES Users(id)
    --INDEX ThreadsSortedMessages (id, tid ASC)
)

CREATE TABLE MusicContents (
    --id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY ROWGUIDCOL, -- guid to prevent private music fetch via id by going through all ids
    --data VARBINARY(47185920) FILESTREAM -- max size - 45mb
    id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
    data VARBINARY(MAX)
)

CREATE TABLE UserLibrary (
    oid INT NOT NULL, -- owner id
    id UNIQUEIDENTIFIER NOT NULL, -- music content id
    name VARCHAR(256) NOT NULL,
    artists VARCHAR(256) NOT NULL,
    FOREIGN KEY (id) REFERENCES MusicContents(id),
    FOREIGN KEY (oid) REFERENCES Users(id),
    PRIMARY KEY (oid, id)
)
