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
    Username        VARCHAR(50)  NOT NULL,
    Pass            VARCHAR(8)   NOT NULL,
    NombreCompleto  VARCHAR(50)  NOT NULL,
    FechaNacimiento DATE         NOT NULL,
    Email           VARCHAR(100) NOT NULL,

    CONSTRAINT PK_Usuarios PRIMARY KEY (Username),

    -- No Fecha Nacimiento futura
    CONSTRAINT CK_Usuarios_FechaNacimiento
        CHECK (FechaNacimiento <= SYSDATETIME()),

    -- Username minimo 8 caracteres
    CONSTRAINT CK_Usuarios_Username
        CHECK (LEN(Username) >= 8),

    -- Password exactamente 8 caracteres con (3 letras,3 digitos, 2 simbolos)
    CONSTRAINT CK_Usuarios_Pass
        CHECK (
            LEN(Pass) = 8
            AND Pass LIKE '%[A-Za-z]%[A-Za-z]%[A-Za-z]%'   -- 3 letras
            AND Pass LIKE '%[0-9]%[0-9]%[0-9]%'             -- 3 digitos
            AND Pass LIKE '%[^A-Za-z0-9 ]%[^A-Za-z0-9 ]%'   -- 2 simbolos
        ),

    -- Email validación de formato
    CONSTRAINT CK_Usuarios_Email
        CHECK (
            Email NOT LIKE '% %'            -- sin espacios
            AND Email LIKE '_%@_%._%'       -- algo@algo.algo
            AND Email NOT LIKE '%@%@%'      -- una sola @
            AND Email NOT LIKE '@%'         -- no empieza con @
            AND Email NOT LIKE '%.@%'       -- no punto antes de @
            AND Email NOT LIKE '%@.%'       -- no punto despues de @
            AND Email NOT LIKE '%..%'       -- sin puntos consecutivos
        )
);
GO


CREATE TABLE Categorias (
    Codigo CHAR(3)     NOT NULL,
    Nombre VARCHAR(50) NOT NULL,
    CONSTRAINT PK_Categorias PRIMARY KEY (Codigo),
    
	--Codigo debe ser de tres letras
	CONSTRAINT CK_Categorias_Codigo
        CHECK (Codigo LIKE '[A-Za-z][A-Za-z][A-Za-z]')
);
GO

CREATE TABLE Mensajes (
    Id                INT IDENTITY(1,1) NOT NULL,
    Asunto            VARCHAR(50)   NOT NULL,
    Texto             VARCHAR(100)  NOT NULL,
    FechaEnvio        DATETIME      NOT NULL CONSTRAINT DF_Mensajes_FechaEnvio DEFAULT (GETDATE()), --RNE
    RemitenteUsername VARCHAR(50)   NOT NULL,
    CategoriaCod      CHAR(3)       NOT NULL,
    FechaCaducidad    DATETIME      NOT NULL,
    CONSTRAINT PK_Mensajes PRIMARY KEY (Id),
    CONSTRAINT FK_Mensajes_Usuarios   FOREIGN KEY (RemitenteUsername) REFERENCES dbo.Usuarios (Username),
    CONSTRAINT FK_Mensajes_Categorias FOREIGN KEY (CategoriaCod)      REFERENCES dbo.Categorias (Codigo),
    
	CONSTRAINT CK_Mensajes_Caducidad
        CHECK (FechaCaducidad >= DATEADD(DAY, 1, FechaEnvio))
);
GO

CREATE TABLE MensajeDestinatarios (
    MensajeId       INT NOT NULL REFERENCES Mensajes(Id),
    DestinoUsername VARCHAR(50) NOT NULL REFERENCES Usuarios(Username),
    PRIMARY KEY (MensajeId, DestinoUsername)
);
GO

Create Procedure spUsuario_Baja @Username VARCHAR(50), @Ret INT OUTPUT AS
Begin
    IF (NOT EXISTS(SELECT 1 FROM Usuarios WHERE Username = @Username))
    BEGIN
        SET @Ret = -1;
        RETURN;
    END

    IF (EXISTS(SELECT 1 FROM Mensajes WHERE RemitenteUsername = @Username))
    BEGIN
        SET @Ret = -2;
        RETURN;
    END

    IF (EXISTS(SELECT 1 FROM MensajeDestinatarios WHERE DestinoUsername = @Username))
    BEGIN
        SET @Ret = -3;
        RETURN;
    END

    DELETE FROM Usuarios WHERE Username = @Username;
    SET @Ret = 1;
END
GO

Create Procedure spMensaje_Alta @Asunto VARCHAR(50), @Texto VARCHAR(100), @CategoriaCod CHAR(3), @Remitente VARCHAR(50), @FechaCaducidad DATETIME, @Id INT OUTPUT, @Ret INT OUTPUT AS
Begin
    IF (NOT EXISTS(SELECT 1 FROM Usuarios WHERE Username = @Remitente))
    BEGIN
        SET @Ret = -1;
        RETURN;
    END

    IF (NOT EXISTS(SELECT 1 FROM Categorias WHERE Codigo = @CategoriaCod))
    BEGIN
        SET @Ret = -2;
        RETURN;
    END

    IF (@FechaCaducidad <= GETDATE())
    BEGIN
        SET @Ret = -3;
        RETURN;
    END

    INSERT INTO Mensajes (Asunto, Texto, FechaEnvio, RemitenteUsername, CategoriaCod, FechaCaducidad)
    VALUES (@Asunto, @Texto, GETDATE(), @Remitente, @CategoriaCod, @FechaCaducidad);

    SET @Id = SCOPE_IDENTITY();
    SET @Ret = 1;
END
GO

Create Procedure spMensaje_AddDestinatario @IdMsg INT, @Destino VARCHAR(50), @Ret INT OUTPUT AS
Begin
    IF (NOT EXISTS(SELECT 1 FROM Mensajes WHERE Id = @IdMsg))
    BEGIN
        SET @Ret = -1;
        RETURN;
    END

    IF (NOT EXISTS(SELECT 1 FROM Usuarios WHERE Username = @Destino))
    BEGIN
        SET @Ret = -2;
        RETURN;
    END

    IF (EXISTS(SELECT 1 FROM MensajeDestinatarios WHERE MensajeId = @IdMsg AND DestinoUsername = @Destino))
    BEGIN
        SET @Ret = -3;
        RETURN;
    END

    INSERT INTO MensajeDestinatarios (MensajeId, DestinoUsername)
    VALUES (@IdMsg, @Destino);

    SET @Ret = 1;
END
GO

