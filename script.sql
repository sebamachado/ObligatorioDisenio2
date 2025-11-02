
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
    CONSTRAINT FK_Mensajes_Usuarios   FOREIGN KEY (RemitenteUsername) REFERENCES Usuarios (Username),
    CONSTRAINT FK_Mensajes_Categorias FOREIGN KEY (CategoriaCod)      REFERENCES Categorias (Codigo),
    
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

--creacion de usuario IIS para que el sitio pueda acceder a la bd-------------------------------------------
USE master
GO

CREATE LOGIN [IIS APPPOOL\DefaultAppPool] FROM WINDOWS 
GO

USE BiosMessenger
GO

CREATE USER [IIS APPPOOL\DefaultAppPool] FOR LOGIN [IIS APPPOOL\DefaultAppPool]
GO

exec sys.sp_addrolemember 'db_owner', [IIS APPPOOL\DefaultAppPool]
go



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


---------------------------------------------------------------
-- 1) USUARIOS
---------------------------------------------------------------
INSERT INTO dbo.Usuarios (Username, Pass, NombreCompleto, FechaNacimiento, Email) VALUES
('IMVLXOL1','AAA111..','Aroa Valbuena Trillo','2005-01-31','ayalamaura@yahoo.com'),
('DIEIJBJ1','AAA111..','Juan Antonio Viana Mínguez','1985-08-02','atienzajovita@palomares.com'),
('EOULZXJ1','AAA111..','Andrea Corral Peñalver','1990-09-01','demetriocerdan@gmail.com'),
('MLXEJLR1','AAA111..','Mireia Cabrero Soler','2004-12-05','morilloeli@gmail.com'),
('AMYVWGH1','AAA111..','José Luis Urrutia-Calderón','1981-06-24','famor@roda.com'),
('BHYAGCA1','AAA111..','Alonso del Miralles','1973-07-07','coral79@hotmail.com'),
('EKJBZPH1','AAA111..','Gastón Arribas Ballesteros','1972-09-21','yurena@hotmail.com'),
('WHWCDRM1','AAA111..','Aurelio Juliá Santamaria','2002-08-18','cesar34@hotmail.com'),
('CXXDZJN1','AAA111..','Manu Portillo Gilabert','2006-02-16','maria-luisamascaro@polo-taboada.com'),
('AZJVRCA1','AAA111..','Brígida Rivera Garay','1992-09-02','bernardanadal@yahoo.com'),
('RKLRMPN1','AAA111..','Zacarías Montesinos','2004-05-05','rodriguezprimitivo@pardo.es'),
('DSILZQO1','AAA111..','Pablo Trujillo Aguilar','1984-11-29','cerezomarcelo@yahoo.com'),
('UIBIMDM1','AAA111..','Alma Nogués Fernandez','2006-04-04','lauravalles@yanez.es'),
('ABWRJYC1','AAA111..','Ileana Bustamante Conde','1969-11-02','pulidoflavia@yahoo.com'),
('DUXONVF1','AAA111..','Julieta Fonseca-Martorell','2002-01-20','guardiolacebrian@prado.com'),
('OMMLREN1','AAA111..','Tania Tejera Céspedes','2000-08-12','arcesoraya@amores.es'),
('KKIVRGQ1','AAA111..','Bienvenida Águeda Mas Seco','2002-07-13','vilmalosa@hotmail.com'),
('ONCSQKB1','AAA111..','Gabriel Carnero Sola','1974-04-05','eduardo02@hotmail.com'),
('AKVOAGG1','AAA111..','Odalis Elorza','1979-02-24','maxiestrada@cuadrado.es'),
('TSEPFGE1','AAA111..','Demetrio Sierra Morell','2001-09-07','gpellicer@yahoo.com'),
('SXMOBTE1','AAA111..','Rómulo Campos Contreras','2001-06-30','folchchuy@cueto.org'),
('YDKHUIY1','AAA111..','Judith Barrios','1999-02-24','bernabe22@blasco.com'),
('UXDDXUO1','AAA111..','Socorro de Toledo','1981-07-30','loboaura@roma.net'),
('ZEFYYUR1','AAA111..','Nydia Escolano Castejón','2003-01-27','blancarevilla@gmail.com'),
('ZAJVBHG1','AAA111..','Noemí Mayol-Colom','1982-06-13','cloegarces@manso.net'),
('TSSFVJF1','AAA111..','Jacobo Rolando Moreno Fonseca','1991-04-07','saavedravasco@yahoo.com'),
('ZZPWPHZ1','AAA111..','Mariano Feijoo Barriga','1988-12-26','clotilde96@nieto-blasco.org'),
('ZQCUVCU1','AAA111..','Piedad Domingo Campo','1994-09-30','nestor15@rodriguez.es'),
('SRSLURE1','AAA111..','Trinidad del Figuerola','1988-08-07','isabelaespejo@yahoo.com'),
('RWCVILW1','AAA111..','Jennifer Garzón Botella','2004-05-28','drubio@yahoo.com'),
('ILABOZO1','AAA111..','Jose Francisco del Isern','1999-11-30','andresdominguez@villar.com'),
('LAJRNOZ1','AAA111..','Ruben Barba Zamorano','1969-09-05','mercedesquintero@aliaga.es'),
('TQPYMGA1','AAA111..','Adela Romero Jiménez','1984-12-23','madridelias@gmail.com'),
('ZBPREYW1','AAA111..','Fermín Atienza Espejo','1992-12-08','galabayon@hotmail.com'),
('RJDTXYI1','AAA111..','Iris Manso-Escalona','1990-04-24','armida81@vilar.es'),
('TNRGNMF1','AAA111..','Tristán Vilanova','1987-09-24','leongabaldon@rosell.org'),
('QTORAPN1','AAA111..','Guiomar Escolano','1985-07-07','adolfo26@hotmail.com'),
('BFXONVR1','AAA111..','Camilo Cabello','2001-06-22','blasacevedo@aguado.com'),
('HABYCQO1','AAA111..','Xiomara Menéndez Pineda','1965-05-01','uriapepita@hotmail.com'),
('DVJEWHP1','AAA111..','Emilia Perez','1981-08-11','hernandorosa-maria@prieto.com'),
('UUOKMWS1','AAA111..','Santos Gonzalez Gras','1970-04-30','artemioespada@hotmail.com'),
('KWPMBLC1','AAA111..','Alba Guillén Talavera','1965-04-28','cerrochuy@adan.com'),
('FSKXSXI1','AAA111..','Gaspar Luna Baeza','2004-03-09','gamezelias@hotmail.com'),
('KIIBYOG1','AAA111..','Laura Casals Salas','1989-05-01','carmelita93@gmail.com'),
('PIXYQTI1','AAA111..','Asdrubal Peláez Feijoo','1999-05-07','jblazquez@ortuno.com'),
('BMLNKUQ1','AAA111..','Ciriaco Gil Roldan','1983-07-05','secoamor@perez.net'),
('JDFPLIT1','AAA111..','Lina Otero-Murcia','1985-07-12','fidel53@yahoo.com'),
('OWDHQYN1','AAA111..','Lara Vall-Checa','1971-09-26','carbajohernando@yahoo.com'),
('SNNCDFJ1','AAA111..','Lucio Lozano Díaz','2000-12-12','jimenezfrancisca@bermudez.es'),
('SAHEAEE1','AAA111..','Aránzazu Cáceres Criado','1989-06-24','isoriano@gmail.com');

---------------------------------------------------------------
-- 2) CATEGORÍAS
---------------------------------------------------------------
INSERT INTO dbo.Categorias (Codigo, Nombre) VALUES
('TRB','Trabajo'),
('FAM','Familia'),
('AMI','Amigos'),
('EST','Estudios'),
('SAL','Salud'),
('FIN','Finanzas'),
('HOG','Hogar'),
('VIA','Viajes'),
('DEP','Deportes'),
('TEC','Tecnología'),
('EDU','Educación'),
('REC','Recordatorios'),
('COM','Compras'),
('CIT','Citas'),
('EVE','Eventos'),
('REU','Reuniones'),
('PRO','Proyectos'),
('LEG','Legal'),
('MAN','Mantenimiento'),
('URG','Urgente');

---------------------------------------------------------------
-- 3) MENSAJES
---------------------------------------------------------------
SET IDENTITY_INSERT dbo.Mensajes ON;

INSERT INTO dbo.Mensajes (Id, Asunto, Texto, RemitenteUsername, CategoriaCod, FechaCaducidad)
SELECT v.Id,
       v.Asunto,
       v.Texto,
       v.RemitenteUsername,
       CASE v.Tipo WHEN 'P' THEN 'PRO' WHEN 'R' THEN 'REU' WHEN 'C' THEN 'COM' END AS CategoriaCod,
       DATEADD(DAY, 7, GETDATE()) AS FechaCaducidad
FROM (VALUES
(1,  'Possimus excepturi officia.','Mollitia ratione voluptas provident qui ad neque provident.','UIBIMDM1','P'),
(2,  'Vero delectus.','Fuga quam dolorem harum incidunt voluptatem. Provident iusto illum provident.','OWDHQYN1','P'),
(3,  'Iste repellendus.','Nulla minus occaecati deleniti optio aliquid dolorum.','TNRGNMF1','P'),
(4,  'Dignissimos perferendis.','Rerum omnis ipsam asperiores distinctio sequi occaecati.','DVJEWHP1','P'),
(5,  'Earum inventore.','Doloribus totam quibusdam. Consequatur ipsum facilis temporibus voluptate eius.','TSSFVJF1','P'),
(6,  'Perspiciatis occaecati.','Iusto aspernatur dolorem asperiores.','ZBPREYW1','P'),
(7,  'Fugit sunt.','Aliquid odit incidunt iste ipsum culpa.','LAJRNOZ1','P'),
(8,  'Omnis.','Numquam aliquid numquam quidem tenetur quaerat quia.','BMLNKUQ1','P'),
(9,  'Ipsa.','Consectetur voluptates ratione. Et perferendis sed amet.','EOULZXJ1','P'),
(10, 'Veniam exercitationem magni.','Pariatur qui iure architecto quia ut ad. Neque iste deserunt mollitia iusto.','CXXDZJN1','P'),
(11, 'Quas pariatur.','Voluptates doloribus tempora reprehenderit minus.','TNRGNMF1','P'),
(12, 'Eum ut eius.','Quibusdam odio deleniti ex quisquam dignissimos.','WHWCDRM1','P'),
(13, 'Distinctio quam ab.','Veniam quae rerum. Pariatur illo atque consequatur quas minima possimus.','AKVOAGG1','P'),
(14, 'Tempora ea eveniet.','Vero distinctio a. Ipsam quisquam a consequatur quibusdam.','SAHEAEE1','P'),
(15, 'Et neque.','Iste libero praesentium molestias magnam mollitia.','EOULZXJ1','P'),
(16, 'Cumque impedit.','Voluptas quibusdam voluptate laboriosam quam. Rem aspernatur repellat saepe.','ZZPWPHZ1','P'),
(17, 'Voluptatibus perspiciatis.','Incidunt accusantium inventore asperiores.','EOULZXJ1','P'),
(18, 'Repellat impedit.','Accusamus dolorem in qui ducimus reprehenderit ipsa.','UXDDXUO1','P'),
(19, 'Adipisci quidem.','Repellendus vel nesciunt quisquam architecto.','BMLNKUQ1','P'),
(20, 'Vel saepe quo.','Incidunt eos odio accusamus nemo voluptas.','SNNCDFJ1','P'),
(21, 'Consequuntur.','Quas beatae odit reiciendis ab perferendis quia doloremque.','DSILZQO1','P'),
(22, 'Atque molestias.','Aperiam labore ab vel doloremque repudiandae sed.','DVJEWHP1','P'),
(23, 'Blanditiis.','Sequi excepturi voluptatibus asperiores quam accusamus laboriosam.','KIIBYOG1','P'),
(24, 'Tenetur officiis delectus.','Delectus quae cumque iste suscipit modi sint.','AMYVWGH1','P'),
(25, 'Doloribus officia temporibus.','Quaerat animi nostrum accusantium quae nisi. Cum molestias ea eaque.','LAJRNOZ1','P'),
(26, 'Nemo doloribus.','Odio vitae magni adipisci. Voluptatum eos consectetur.','SXMOBTE1','P'),
(27, 'Tenetur voluptate.','Provident est ullam beatae quasi. Labore possimus facilis.','KWPMBLC1','P'),
(28, 'Dicta reiciendis.','Culpa voluptates quis aliquam ratione illo nesciunt eaque.','SXMOBTE1','P'),
(29, 'Ad unde quia.','Dolore libero deleniti dignissimos blanditiis ullam sint.','RWCVILW1','P'),
(30, 'Cumque at.','Adipisci nisi aliquam repudiandae quam deserunt. Magni amet tempora facilis.','AKVOAGG1','P'),
(31, 'Doloribus possimus.','Quae molestiae sapiente temporibus quas praesentium.','RJDTXYI1','R'),
(32, 'Minus quibusdam dolore.','Iste facilis numquam magni.','OMMLREN1','R'),
(33, 'Rem itaque.','Molestias non laborum adipisci architecto qui soluta sequi.','RJDTXYI1','R'),
(34, 'Ullam quasi quaerat.','Doloremque aperiam officiis. Libero minus nostrum deserunt sed voluptatibus.','TSEPFGE1','R'),
(35, 'Cum reprehenderit.','In corrupti eius aut. Ex odit eius ipsa accusamus.','HABYCQO1','R'),
(36, 'Natus temporibus.','Architecto sed natus sint iusto. Soluta vero vitae vel id facere.','MLXEJLR1','R'),
(37, 'Illo assumenda nam.','Aliquam adipisci dolorem error. Corrupti laborum deleniti velit.','TQPYMGA1','R'),
(38, 'Porro.','In repellat quaerat nulla doloribus similique inventore.','KKIVRGQ1','R'),
(39, 'Esse illum fugiat.','Error ab exercitationem mollitia sed.','DSILZQO1','R'),
(40, 'Doloribus doloribus fugit quia.','Natus ducimus qui quidem quos sed. Soluta ex nobis.','OWDHQYN1','R'),
(41, 'Excepturi eaque.','Consectetur aut perferendis rerum. Quam ratione dolores sunt amet.','SAHEAEE1','R'),
(42, 'Similique dolorum rem.','Laborum adipisci adipisci. Sequi harum magni quisquam reiciendis dicta ipsam.','RJDTXYI1','R'),
(43, 'Officiis laudantium laboriosam.','Ad dicta natus consequatur tempore iure quas. Qui hic reprehenderit.','DSILZQO1','R'),
(44, 'Consequatur totam porro veritatis.','Iste expedita sed eligendi impedit vero ipsa. Maiores debitis occaecati.','DSILZQO1','R'),
(45, 'At consectetur.','Recusandae omnis tenetur laudantium ipsam blanditiis corporis.','DVJEWHP1','R'),
(46, 'Nemo vitae.','Sapiente non explicabo quisquam. Id unde libero eius explicabo dolorum.','AZJVRCA1','R'),
(47, 'Fuga provident harum provident.','Aliquam quidem tempora ratione quibusdam sit voluptatibus.','EOULZXJ1','R'),
(48, 'Ipsam pariatur.','Consequatur at ex laboriosam. Molestiae voluptatum libero ut mollitia.','ZZPWPHZ1','R'),
(49, 'Nulla reiciendis nostrum.','Molestiae nostrum aspernatur itaque quibusdam deserunt.','MLXEJLR1','R'),
(50, 'Explicabo illum nam.','Eos enim alias nostrum fugiat autem aspernatur.','MLXEJLR1','R'),
(51, 'Reprehenderit hic.','Quos iste quibusdam nisi est voluptatem nesciunt maiores.','BFXONVR1','R'),
(52, 'A officia.','Voluptates asperiores maiores ullam. Doloremque doloribus voluptas dolores.','SAHEAEE1','R'),
(53, 'Cumque illo.','Deleniti voluptatem dolor saepe accusantium rerum.','HABYCQO1','R'),
(54, 'Repudiandae porro doloremque.','Tempore beatae id nesciunt vero quos.','QTORAPN1','R'),
(55, 'Id.','Iste error exercitationem magnam modi.','SAHEAEE1','R'),
(56, 'Consequatur expedita.','Aut consequatur ex autem eum.','UUOKMWS1','R'),
(57, 'Veniam laudantium.','Corporis quae nesciunt sit eligendi. Tempora hic dolor exercitationem cum.','BFXONVR1','R'),
(58, 'Saepe tempore accusamus.','Expedita ex omnis tempore. Minus tempore distinctio veniam architecto dolorum.','JDFPLIT1','R'),
(59, 'Unde facere nisi.','Ad neque in. Hic occaecati excepturi facilis distinctio nemo nostrum.','TSSFVJF1','R'),
(60, 'Praesentium quod.','Dolorum alias sequi qui perferendis eligendi necessitatibus.','OWDHQYN1','R'),
(61, 'Id occaecati quam voluptates.','Iure inventore consequuntur distinctio expedita reprehenderit facilis.','SRSLURE1','C'),
(62, 'Error dolores.','Ex maxime voluptatum quibusdam. Officiis vel est nisi eum.','WHWCDRM1','C'),
(63, 'Recusandae alias.','Pariatur est illo voluptatem nihil rem. Aut in sed.','AMYVWGH1','C'),
(64, 'Eligendi nostrum voluptatem.','Quisquam ex tempora. Eaque dicta quae consectetur quasi aspernatur nulla.','BFXONVR1','C'),
(65, 'Similique eum.','Consequuntur quis veniam commodi deserunt. Sunt et sunt doloremque quasi.','BFXONVR1','C'),
(66, 'Accusamus facilis quas.','Ipsa ducimus occaecati ratione. Omnis cumque exercitationem optio.','AMYVWGH1','C'),
(67, 'Aliquam quod.','Reprehenderit ullam eum eius. Natus totam ducimus doloribus sit facilis.','AMYVWGH1','C'),
(68, 'Reiciendis placeat.','Debitis nemo atque adipisci dolorum. Est explicabo soluta.','AMYVWGH1','C'),
(69, 'Dolorem doloremque.','Sint a cum aut autem veniam error.','ONCSQKB1','C'),
(70, 'Perferendis molestiae.','Delectus ab animi porro consequatur. Soluta perspiciatis quos.','RJDTXYI1','C'),
(71, 'Impedit laboriosam quaerat.','Vel natus perferendis. Eos perferendis eos illo sit qui.','QTORAPN1','C'),
(72, 'Rerum nihil est.','Dicta non atque odio itaque. Harum eaque molestias consectetur nostrum.','OMMLREN1','C'),
(73, 'Deleniti dolores.','Pariatur nostrum ea esse velit asperiores. Assumenda illo ut fugit.','UIBIMDM1','C'),
(74, 'Explicabo illum.','Veritatis quisquam dicta ratione asperiores. Culpa autem nam enim magni.','FSKXSXI1','C'),
(75, 'Eum harum illo.','Itaque suscipit amet. Ipsum architecto excepturi omnis ea ipsa mollitia.','ZQCUVCU1','C'),
(76, 'Modi sint.','Ea labore maxime ab placeat. Corporis dicta accusamus quidem rem assumenda.','JDFPLIT1','C'),
(77, 'Ipsum laborum.','Temporibus nisi ab numquam. Minus eos est harum. Quidem nulla quasi amet.','KWPMBLC1','C'),
(78, 'Esse possimus.','Molestiae molestiae labore aperiam. Eum velit quis ex quasi quasi.','TSSFVJF1','C'),
(79, 'Doloribus alias numquam.','Quis quisquam ut facere occaecati eos dolores a. Commodi eveniet minima.','OMMLREN1','C'),
(80, 'Labore.','Molestias alias maiores dolores perspiciatis id corporis.','RJDTXYI1','C'),
(81, 'Dolorum.','Molestias odit dolore exercitationem reprehenderit vero.','ZQCUVCU1','C'),
(82, 'Accusamus ipsa.','Maiores quidem rerum molestias dolores.','RWCVILW1','C'),
(83, 'Facere sequi.','Saepe eaque officiis officia esse iste excepturi ex.','SRSLURE1','C'),
(84, 'Cum nam voluptatum.','Laboriosam nam quia quidem.','MLXEJLR1','C'),
(85, 'Facere pariatur voluptate.','Laboriosam animi cum et vitae beatae nulla.','BHYAGCA1','C'),
(86, 'Sed deleniti.','Magni dignissimos corporis sint accusantium magni ipsam.','ZZPWPHZ1','C'),
(87, 'Aperiam nemo.','Nam saepe veniam nihil doloribus tempora atque. Cumque quis aliquam neque.','ABWRJYC1','C'),
(88, 'Neque.','Sint occaecati itaque corrupti possimus quidem totam.','RKLRMPN1','C'),
(89, 'Explicabo molestiae voluptates.','Illum pariatur accusamus labore. Sunt culpa excepturi rerum.','ZAJVBHG1','C'),
(90, 'Nemo totam architecto.','Suscipit laboriosam ipsum ipsam praesentium porro.','AKVOAGG1','C'),
(91, 'Explicabo tempora aliquam.','Sed vitae consectetur consequatur.','JDFPLIT1','C'),
(92, 'Tempora est.','Esse eos quibusdam ipsa nam provident. Dignissimos iste doloribus neque.','BMLNKUQ1','C'),
(93, 'Necessitatibus repellendus facere fugit.','Itaque adipisci omnis quasi accusantium incidunt repellendus.','UIBIMDM1','C'),
(94, 'Saepe ratione.','Id ab magni dolor vitae modi voluptates dolor. Dignissimos culpa mollitia.','MLXEJLR1','C'),
(95, 'Quae autem ab.','Impedit ducimus optio veniam. Quae quo aspernatur esse tempore.','RJDTXYI1','C'),
(96, 'In voluptatem nostrum.','Dolorem veniam quo rem a. Voluptatem error et repellendus perspiciatis.','SXMOBTE1','C'),
(97, 'Dicta corporis saepe.','Saepe tenetur omnis quasi.','BFXONVR1','C'),
(98, 'Dicta quasi.','Aliquid quidem doloribus quos consequatur.','ZBPREYW1','C'),
(99, 'Aut dicta.','Quasi voluptate quam expedita vel. Tempore in iste id tempora.','TQPYMGA1','C'),
(100,'Est aliquam officiis.','Excepturi tempora cumque reiciendis.','AMYVWGH1','C'),
(101,'Aut illo vel illum.','Consequuntur porro velit amet temporibus. Perferendis commodi et laudantium.','KIIBYOG1','C'),
(102,'Quibusdam corporis.','Saepe debitis sunt. Asperiores enim doloribus temporibus inventore rem.','QTORAPN1','C'),
(103,'Hic distinctio odio nulla.','Tempore accusamus nulla sit exercitationem maxime.','EOULZXJ1','C'),
(104,'Quidem mollitia.','Facilis molestiae doloribus quo praesentium ducimus.','FSKXSXI1','C'),
(105,'Dolores excepturi.','Quisquam sapiente aliquam. Doloremque quos facilis nulla explicabo.','SXMOBTE1','C'),
(106,'Incidunt ad sed.','Fuga at corporis incidunt. Quis odit veniam possimus.','BMLNKUQ1','C'),
(107,'Aut ipsa.','Corrupti laboriosam sunt dolor.','TSSFVJF1','C'),
(108,'Dolor ab impedit.','Suscipit inventore minus quam nostrum ipsam soluta. Dolorem iure eum harum.','TSEPFGE1','C'),
(109,'Ducimus quae.','Laborum voluptas minima cumque quibusdam maxime perspiciatis.','AMYVWGH1','C'),
(110,'Aut porro accusantium.','Nobis commodi voluptatem ipsam minus.','QTORAPN1','C'),
(111,'Nobis deserunt.','Ex beatae error deleniti illum molestias.','ABWRJYC1','C'),
(112,'Ratione.','Perspiciatis sint iste corporis est quaerat fuga.','UXDDXUO1','C'),
(113,'Consequatur exercitationem.','Quos dicta nam alias. Doloribus neque molestias aut dolore illum.','AKVOAGG1','C'),
(114,'Quasi quae.','Libero eligendi aliquid magnam reiciendis doloribus ipsa.','BMLNKUQ1','C'),
(115,'Nulla fugiat.','Cumque quidem consequatur recusandae sapiente.','ZBPREYW1','C'),
(116,'Fugiat quam.','Corrupti nulla fugit distinctio. Voluptates enim a eum delectus id.','TSEPFGE1','C'),
(117,'Suscipit quisquam qui.','Numquam tempore beatae iste nulla. Quidem odio eaque vitae sequi cupiditate.','WHWCDRM1','C'),
(118,'Ratione voluptates.','Beatae accusamus suscipit veritatis. Beatae necessitatibus dicta fuga quia.','AZJVRCA1','C'),
(119,'Nostrum voluptates expedita.','Laudantium quidem pariatur assumenda nam cupiditate esse assumenda.','ABWRJYC1','C'),
(120,'Nostrum illo quo.','At esse totam maiores sed.','UUOKMWS1','C'),
(121,'Laudantium debitis eum.','Consequatur quasi consectetur quo dignissimos eaque id.','KKIVRGQ1','C'),
(122,'Accusamus vel incidunt.','Doloribus aliquid blanditiis harum. Sint repellendus ad facere accusantium.','ONCSQKB1','C'),
(123,'Fugit aperiam quibusdam.','Labore atque corrupti magnam eius. Sapiente accusamus nulla recusandae.','UUOKMWS1','C'),
(124,'Quisquam placeat quas.','Veniam suscipit nesciunt.','TNRGNMF1','C'),
(125,'Ut qui rerum.','Minus ab provident sapiente perspiciatis provident voluptatibus.','AMYVWGH1','C'),
(126,'Fugit magnam suscipit reprehenderit.','Itaque ea repellendus rerum. Maxime aperiam vitae occaecati.','BFXONVR1','C'),
(127,'Dolore ipsa atque.','Ipsum architecto eaque. Doloremque nesciunt nesciunt facere.','EOULZXJ1','C'),
(128,'Laborum fuga.','Neque officia similique reiciendis. Quasi soluta ex officia.','ABWRJYC1','C'),
(129,'Libero dolorem repudiandae.','Nihil saepe est amet molestias cumque.','SAHEAEE1','C'),
(130,'Perspiciatis in.','Sed magni excepturi perferendis excepturi. Eaque veniam eveniet eos explicabo.','AZJVRCA1','C'),
(131,'Nihil fugit.','Quo earum culpa molestias dignissimos quos expedita.','DIEIJBJ1','C'),
(132,'Suscipit excepturi non.','Incidunt tempore sequi explicabo quo molestiae.','FSKXSXI1','C'),
(133,'Consequatur aliquam.','Aspernatur laboriosam reprehenderit. Odit ipsum in.','EKJBZPH1','C'),
(134,'Eaque eos.','Occaecati quos culpa itaque rerum voluptate.','UIBIMDM1','C'),
(135,'Explicabo enim.','Veritatis minus expedita rerum repellendus ad exercitationem.','DUXONVF1','C'),
(136,'Labore id ipsa.','In corporis veniam provident possimus alias.','YDKHUIY1','C'),
(137,'Adipisci quisquam beatae.','Cumque dicta sed aliquid.','KWPMBLC1','C'),
(138,'Inventore inventore reiciendis.','Rem iure porro. Culpa fugit dicta eveniet exercitationem doloribus.','YDKHUIY1','C'),
(139,'Necessitatibus nesciunt repellendus.','Quibusdam ex laborum excepturi.','BFXONVR1','C'),
(140,'Perspiciatis voluptates.','Adipisci id hic architecto accusamus autem illo.','ZQCUVCU1','C'),
(141,'Dolorum corrupti dolorem.','Incidunt quae et autem enim dolorem. Molestiae optio delectus.','HABYCQO1','C'),
(142,'Quis soluta deserunt.','Ratione laboriosam quasi magnam necessitatibus.','KKIVRGQ1','C'),
(143,'Aliquid esse vitae.','Velit repudiandae vitae quam incidunt fugiat tempora.','JDFPLIT1','C'),
(144,'Nobis placeat numquam.','Debitis eaque autem beatae aliquam sequi.','DVJEWHP1','C'),
(145,'Autem tenetur.','Dignissimos sequi consequuntur soluta magnam iure.','FSKXSXI1','C'),
(146,'Autem blanditiis.','Beatae doloremque qui rerum magni exercitationem.','TNRGNMF1','C'),
(147,'Pariatur.','Facere quia error. Magni earum ratione voluptates.','OWDHQYN1','C'),
(148,'Totam odio.','Nihil quis id quos consequuntur vitae alias expedita.','TSEPFGE1','C'),
(149,'Facere similique saepe.','Repellat dolor iusto quae. Maiores voluptates itaque soluta.','RWCVILW1','C'),
(150,'Ducimus architecto.','Laborum dolore harum qui dolor. Excepturi quod iste nemo.','OWDHQYN1','C'),
(151,'Debitis cum laboriosam.','Sunt quis harum eos quo vitae veniam.','FSKXSXI1','C'),
(152,'Recusandae.','Distinctio sit suscipit alias sit accusamus eum porro. Omnis iure nostrum.','UIBIMDM1','C'),
(153,'Quasi minima dignissimos.','Eligendi perferendis omnis delectus odit praesentium.','RWCVILW1','C'),
(154,'Vero dolor cumque.','Quae et aperiam corrupti. Labore doloribus libero quae.','LAJRNOZ1','C'),
(155,'Numquam iusto illum.','Inventore pariatur nemo ipsam cupiditate vel possimus.','IMVLXOL1','C'),
(156,'Aut modi autem illo.','Magnam sint voluptates accusantium quos nulla inventore.','MLXEJLR1','C'),
(157,'Quisquam minima.','Voluptatibus ut tenetur facere totam accusamus delectus.','ZBPREYW1','C'),
(158,'Iure veniam.','Quos quisquam nesciunt nostrum architecto. Velit omnis molestias accusantium.','JDFPLIT1','C'),
(159,'Nemo beatae sunt.','Nisi quaerat vero nesciunt aliquam quisquam. Fugit ullam sunt sint consequatur.','KKIVRGQ1','C'),
(160,'Soluta corrupti officia.','Vel repudiandae iure. Dolorem perspiciatis ullam cum occaecati amet reiciendis.','ONCSQKB1','C')
) AS v(Id,Asunto,Texto,RemitenteUsername,Tipo);

SET IDENTITY_INSERT dbo.Mensajes OFF;

---------------------------------------------------------------
-- 4) MENSAJE DESTINATARIOS
---------------------------------------------------------------
INSERT INTO dbo.MensajeDestinatarios (MensajeId, DestinoUsername) VALUES
(1, 'ZZPWPHZ1'),
(2, 'ONCSQKB1'),
(3, 'KIIBYOG1'),
(4, 'SRSLURE1'),
(5, 'JDFPLIT1'),
(6, 'DUXONVF1'),
(7, 'ZQCUVCU1'),
(8, 'HABYCQO1'),
(9, 'UXDDXUO1'),
(10,'SXMOBTE1'),
(11,'LAJRNOZ1'),
(12,'DSILZQO1'),
(13,'OWDHQYN1'),
(14,'AKVOAGG1'),
(15,'SNNCDFJ1'),
(16,'JDFPLIT1'),
(17,'ZQCUVCU1'),
(18,'LAJRNOZ1'),
(19,'KWPMBLC1'),
(20,'KIIBYOG1'),
(21,'AMYVWGH1'),
(22,'PIXYQTI1'),
(23,'ONCSQKB1'),
(24,'DIEIJBJ1'),
(25,'OWDHQYN1'),
(26,'ZEFYYUR1'),
(27,'AMYVWGH1'),
(28,'TNRGNMF1'),
(29,'DUXONVF1'),
(30,'RKLRMPN1'),
(31,'ABWRJYC1'),
(32,'SNNCDFJ1'),
(33,'TQPYMGA1'),
(34,'SAHEAEE1'),
(35,'KIIBYOG1'),
(36,'RWCVILW1'),
(37,'PIXYQTI1'),
(38,'AKVOAGG1'),
(39,'TSEPFGE1'),
(40,'DUXONVF1'),
(41,'RKLRMPN1'),
(42,'ZAJVBHG1'),
(43,'OMMLREN1'),
(44,'QTORAPN1'),
(45,'AKVOAGG1'),
(46,'BFXONVR1'),
(47,'ZQCUVCU1'),
(48,'OMMLREN1'),
(49,'DVJEWHP1'),
(50,'ZBPREYW1'),
(51,'AKVOAGG1'),
(52,'UIBIMDM1'),
(53,'DVJEWHP1'),
(54,'ILABOZO1'),
(55,'AMYVWGH1'),
(56,'ABWRJYC1'),
(57,'JDFPLIT1'),
(58,'ZEFYYUR1'),
(59,'OMMLREN1'),
(60,'DVJEWHP1'),
(61,'RWCVILW1'),
(62,'OMMLREN1'),
(63,'DIEIJBJ1'),
(64,'DUXONVF1'),
(65,'IMVLXOL1'),
(66,'OMMLREN1'),
(67,'UXDDXUO1'),
(68,'KKIVRGQ1'),
(69,'ABWRJYC1'),
(70,'OWDHQYN1'),
(71,'ILABOZO1'),
(72,'ZQCUVCU1'),
(73,'EKJBZPH1'),
(74,'UXDDXUO1'),
(75,'ILABOZO1'),
(76,'KIIBYOG1'),
(77,'MLXEJLR1'),
(78,'EKJBZPH1'),
(79,'UIBIMDM1'),
(80,'CXXDZJN1'),
(81,'ONCSQKB1'),
(82,'AMYVWGH1'),
(83,'EKJBZPH1'),
(84,'IMVLXOL1'),
(85,'DSILZQO1'),
(86,'LAJRNOZ1'),
(87,'MLXEJLR1'),
(88,'IMVLXOL1'),
(89,'ILABOZO1'),
(90,'BMLNKUQ1'),
(91,'FSKXSXI1'),
(92,'AZJVRCA1'),
(93,'DUXONVF1'),
(94,'SNNCDFJ1'),
(95,'SNNCDFJ1'),
(96,'MLXEJLR1'),
(97,'TQPYMGA1'),
(98,'MLXEJLR1'),
(99,'DSILZQO1'),
(100,'BHYAGCA1'),
(101,'TSSFVJF1'),
(101,'WHWCDRM1'),
(102,'HABYCQO1'),
(102,'DVJEWHP1'),
(103,'EKJBZPH1'),
(103,'ZQCUVCU1'),
(104,'QTORAPN1'),
(104,'ZBPREYW1'),
(105,'ABWRJYC1'),
(105,'KIIBYOG1'),
(106,'OMMLREN1'),
(106,'KKIVRGQ1'),
(107,'KIIBYOG1'),
(107,'FSKXSXI1'),
(108,'YDKHUIY1'),
(108,'SAHEAEE1'),
(109,'ILABOZO1'),
(109,'UUOKMWS1'),
(110,'AMYVWGH1'),
(110,'RJDTXYI1'),
(111,'ONCSQKB1'),
(111,'CXXDZJN1'),
(112,'OMMLREN1'),
(112,'ZAJVBHG1'),
(113,'RWCVILW1'),
(113,'TNRGNMF1'),
(114,'DVJEWHP1'),
(114,'KWPMBLC1'),
(115,'KIIBYOG1'),
(115,'QTORAPN1'),
(116,'CXXDZJN1'),
(116,'KKIVRGQ1'),
(117,'SNNCDFJ1'),
(117,'QTORAPN1'),
(118,'TSEPFGE1'),
(118,'DVJEWHP1'),
(119,'DUXONVF1'),
(119,'PIXYQTI1'),
(120,'TQPYMGA1'),
(120,'LAJRNOZ1'),
(121,'BHYAGCA1'),
(121,'KWPMBLC1'),
(121,'SRSLURE1'),
(122,'IMVLXOL1'),
(122,'UXDDXUO1'),
(122,'CXXDZJN1'),
(123,'RKLRMPN1'),
(123,'SNNCDFJ1'),
(123,'SRSLURE1'),
(124,'QTORAPN1'),
(124,'IMVLXOL1'),
(124,'WHWCDRM1'),
(125,'TNRGNMF1'),
(125,'EOULZXJ1'),
(125,'ZAJVBHG1'),
(126,'AZJVRCA1'),
(126,'ZQCUVCU1'),
(126,'CXXDZJN1'),
(127,'ZAJVBHG1'),
(127,'MLXEJLR1'),
(127,'ZEFYYUR1'),
(128,'KIIBYOG1'),
(128,'EKJBZPH1'),
(128,'ZEFYYUR1'),
(129,'ZZPWPHZ1'),
(129,'DVJEWHP1'),
(129,'OWDHQYN1'),
(130,'DSILZQO1'),
(130,'UIBIMDM1'),
(130,'ZQCUVCU1'),
(131,'SNNCDFJ1'),
(131,'UXDDXUO1'),
(131,'ZQCUVCU1'),
(132,'ONCSQKB1'),
(132,'RKLRMPN1'),
(132,'BMLNKUQ1'),
(133,'EOULZXJ1'),
(133,'LAJRNOZ1'),
(133,'OMMLREN1'),
(134,'ZEFYYUR1'),
(134,'SXMOBTE1'),
(134,'OMMLREN1'),
(135,'KIIBYOG1'),
(135,'UIBIMDM1'),
(135,'ZZPWPHZ1'),
(136,'AMYVWGH1'),
(136,'ONCSQKB1'),
(136,'ZEFYYUR1'),
(137,'TSSFVJF1'),
(137,'PIXYQTI1'),
(137,'RJDTXYI1'),
(138,'WHWCDRM1'),
(138,'KKIVRGQ1'),
(138,'DSILZQO1'),
(139,'EOULZXJ1'),
(139,'EKJBZPH1'),
(139,'DVJEWHP1'),
(140,'OWDHQYN1'),
(140,'SXMOBTE1'),
(140,'SRSLURE1'),
(141,'WHWCDRM1'),
(141,'ZAJVBHG1'),
(141,'QTORAPN1'),
(141,'UIBIMDM1'),
(142,'JDFPLIT1'),
(142,'SRSLURE1'),
(142,'IMVLXOL1'),
(142,'RJDTXYI1'),
(142,'TNRGNMF1'),
(142,'PIXYQTI1'),
(143,'ZEFYYUR1'),
(143,'ZQCUVCU1'),
(143,'AMYVWGH1'),
(143,'FSKXSXI1'),
(143,'YDKHUIY1'),
(144,'KIIBYOG1'),
(144,'WHWCDRM1'),
(144,'OWDHQYN1'),
(144,'TSEPFGE1'),
(144,'TQPYMGA1'),
(144,'JDFPLIT1'),
(145,'SXMOBTE1'),
(145,'TSSFVJF1'),
(145,'BMLNKUQ1'),
(145,'AKVOAGG1'),
(146,'UIBIMDM1'),
(146,'ZZPWPHZ1'),
(146,'KIIBYOG1'),
(146,'ZAJVBHG1'),
(146,'PIXYQTI1'),
(147,'DVJEWHP1'),
(147,'QTORAPN1'),
(147,'TSEPFGE1'),
(147,'TSSFVJF1'),
(147,'TNRGNMF1'),
(147,'IMVLXOL1'),
(148,'ABWRJYC1'),
(148,'SRSLURE1'),
(148,'HABYCQO1'),
(148,'DVJEWHP1'),
(148,'FSKXSXI1'),
(148,'YDKHUIY1'),
(149,'SRSLURE1'),
(149,'PIXYQTI1'),
(149,'ABWRJYC1'),
(149,'ZBPREYW1'),
(149,'LAJRNOZ1'),
(150,'FSKXSXI1'),
(150,'BHYAGCA1'),
(150,'AKVOAGG1'),
(150,'TQPYMGA1'),
(151,'YDKHUIY1'),
(151,'BHYAGCA1'),
(151,'OMMLREN1'),
(151,'PIXYQTI1'),
(151,'TSEPFGE1'),
(151,'DUXONVF1'),
(152,'DIEIJBJ1'),
(152,'EOULZXJ1'),
(152,'KKIVRGQ1'),
(152,'LAJRNOZ1'),
(152,'UUOKMWS1'),
(152,'AMYVWGH1'),
(153,'KWPMBLC1'),
(153,'BFXONVR1'),
(153,'UIBIMDM1'),
(153,'JDFPLIT1'),
(153,'BMLNKUQ1'),
(153,'ZAJVBHG1'),
(154,'OMMLREN1'),
(154,'AZJVRCA1'),
(154,'FSKXSXI1'),
(154,'BMLNKUQ1'),
(155,'SRSLURE1'),
(155,'OMMLREN1'),
(155,'UIBIMDM1'),
(155,'BMLNKUQ1'),
(155,'RJDTXYI1'),
(155,'ILABOZO1'),
(156,'KKIVRGQ1'),
(156,'CXXDZJN1'),
(156,'ILABOZO1'),
(156,'AZJVRCA1'),
(156,'KIIBYOG1'),
(157,'DVJEWHP1'),
(157,'SXMOBTE1'),
(157,'SAHEAEE1'),
(157,'SRSLURE1'),
(157,'UUOKMWS1'),
(158,'ZQCUVCU1'),
(158,'TNRGNMF1'),
(158,'SRSLURE1'),
(158,'RKLRMPN1'),
(158,'ILABOZO1'),
(158,'OWDHQYN1'),
(159,'KWPMBLC1'),
(159,'AKVOAGG1'),
(159,'RJDTXYI1'),
(159,'TQPYMGA1'),
(159,'SAHEAEE1'),
(159,'OMMLREN1'),
(160,'AMYVWGH1'),
(160,'JDFPLIT1'),
(160,'TSEPFGE1'),
(160,'OMMLREN1'),
(160,'AKVOAGG1');
