USE master;
GO
IF DB_ID('BiosMessenger') IS NOT NULL
    DROP DATABASE BiosMessenger;
GO
CREATE DATABASE BiosMessenger;
GO
USE BiosMessenger;
GO

CREATE TABLE Usuarios (
    Username        CHAR(8)     NOT NULL PRIMARY KEY,
    Pass            CHAR(8)     NOT NULL,
    NombreCompleto  VARCHAR(50) NOT NULL,
    FechaNacimiento DATE        NOT NULL,
    Email           VARCHAR(100)NOT NULL
);
GO

ALTER TABLE Usuarios
ADD CONSTRAINT CK_Usuarios_FechaNacimiento_Pasado --RNE
CHECK (FechaNacimiento <= CAST(SYSDATETIME() AS date));

ALTER TABLE Usuarios
ADD CONSTRAINT CK_Usuarios_Username_8_NoSpaces --RNE
CHECK (
  LEN(TRIM(Username)) = 8
  AND Username NOT LIKE '% %'
);
GO

CREATE TABLE Categorias (
    Codigo CHAR(3)     NOT NULL PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL
);
GO

ALTER TABLE dbo.Categorias
ADD CONSTRAINT CK_Categorias_Codigo_3Letras --RNE
CHECK (Codigo LIKE '[A-Za-z][A-Za-z][A-Za-z]');
GO

CREATE TABLE Mensajes (
    Id                INT IDENTITY(1,1) PRIMARY KEY, --RNE
    Asunto            VARCHAR(50)  NOT NULL,
    Texto             VARCHAR(100) NOT NULL,
    FechaEnvio        DATETIME     NOT NULL DEFAULT(GETDATE()), --RNE
    RemitenteUsername CHAR(8)      NOT NULL REFERENCES Usuarios(Username),
	CategoriaCod	  CHAR(3)      NOT NULL REFERENCES Categorias(Codigo),
	FechaCaducidad    DATETIME  NOT NULL
);
GO

ALTER TABLE Mensajes
ADD CONSTRAINT CK_Mensajes_Caducidad_DMas1 --RNE
CHECK (DATEDIFF(DAY, CONVERT(date, FechaEnvio), CONVERT(date, FechaCaducidad)) >= 1
);

CREATE TABLE MensajeDestinatarios (
    MensajeId       INT NOT NULL REFERENCES Mensajes(Id),
    DestinoUsername CHAR(8) NOT NULL REFERENCES Usuarios(Username),
    PRIMARY KEY (MensajeId, DestinoUsername)
);
GO

CREATE PROCEDURE spUsuario_Baja
    @Username CHAR(8)
AS
BEGIN
    DELETE Usuarios
    WHERE Username = @Username;
END
GO

CREATE PROCEDURE spMensaje_Alta
    @Asunto				VARCHAR(50),
    @Texto				VARCHAR(100),
    @CategoriaCod		CHAR(3),
    @Remitente			CHAR(8),
	@FechaCaducidad     DATETIME,
    @Id           INT OUTPUT
AS
BEGIN
    INSERT INTO Mensajes (Asunto,Texto,FechaEnvio,RemitenteUsername,CategoriaCod,FechaCaducidad)
    VALUES (@Asunto,@Texto,GETDATE(),@Remitente,@CategoriaCod,@FechaCaducidad);
    SET @Id = SCOPE_IDENTITY();
END
GO

CREATE PROCEDURE spMensaje_AddDestinatario
    @IdMsg   INT,
    @Destino CHAR(8)
AS
	INSERT INTO MensajeDestinatarios (MensajeId,DestinoUsername)
	VALUES (@IdMsg,@Destino);
GO

