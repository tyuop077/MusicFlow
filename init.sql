DROP TABLE Users

CREATE TABLE Users (
    id INT NOT NULL IDENTITY(1,1),
    email VARCHAR(255) UNIQUE,
    username VARCHAR(255) UNIQUE,
    password UNIQUEIDENTIFIER, -- hashed password
    PRIMARY KEY (id)
)