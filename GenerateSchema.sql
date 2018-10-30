-- See README.md

USE videotheque
GO

DROP TABLE dbo.Customers
GO

CREATE TABLE dbo.Customers
(ID int IDENTITY(1,1) PRIMARY KEY,
  Name varchar(100) NOT NULL)
GO

-- vim:sw=2:ts=2:et:fileformat=dos
