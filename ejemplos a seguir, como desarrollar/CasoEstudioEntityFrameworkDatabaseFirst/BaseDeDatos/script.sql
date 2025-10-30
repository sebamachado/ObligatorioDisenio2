--creo la base de datos de ventas--------------------------------------------------------------------------
if exists(Select * FROM SysDataBases WHERE name='Ventas')
BEGIN
	DROP DATABASE Ventas
END
go

CREATE DATABASE Ventas

go

--pone en uso la bd----------------------------------------------------------------------------------------
USE Ventas
go

--creo las tablas------------------------------------------------------------------------------------------
CREATE TABLE Usuarios(
	Usulog varchar(15) not null PRIMARY KEY,
	PassLog varchar(5) not null
)
GO

CREATE TABLE Articulos(
	CodArt int NOT NULL PRIMARY KEY ,
	NomArt varchar(25) NOT NULL,
	PreArt int NOT NULL check (PreArt > 0)
) 
go

CREATE TABLE Facturas(
	NumFact int NOT NULL PRIMARY KEY ,
	FechaFact DATETIME NOT NULL DEFAULT getdate(),
	UsuLog varchar(15) NOT NULL FOREIGN KEY REFERENCES Usuarios (UsuLog)
)
go	

CREATE TABLE LINEAS (
	NumFact int NOT NULL FOREIGN KEY REFERENCES Facturas (NumFact),
	CodArt int NOT NULL FOREIGN KEY REFERENCES Articulos(CodArt),
	Cant int NOT NULL,
	Primary Key(NumFact, CodArt)
)
go



--creacion de usuario IIS para que el sitio pueda acceder a la bd-------------------------------------------
USE master
GO

CREATE LOGIN [IIS APPPOOL\DefaultAppPool] FROM WINDOWS 
GO

USE Ventas
GO

CREATE USER [IIS APPPOOL\DefaultAppPool] FOR LOGIN [IIS APPPOOL\DefaultAppPool]
GO

--esto es por el EF 
exec sys.sp_addrolemember 'db_owner', [IIS APPPOOL\DefaultAppPool]
go


----------------------------------------------------------------------------------------------------------
--el EF no reconoce el return value!!!!!
--cargamos en una variable de tipo output los numeros de error..... 
-- problema: cargar una variable output NO CORTA LA EJECUCION

--creo SP de alta
Create Procedure AltaArticulo @cod int, @nom varchar(20), @pre money, @ret int output AS
Begin
	if (exists(select * from Articulos where codArt = @cod))
		set @ret = -1
	else
	begin
		Insert Articulos(CodARt, NomArt, PreArt) Values (@cod, @nom, @pre)
		set @ret = 1
	end
end
go






--creo SP de Baja
Create Procedure BajaArticulo @cod int, @ret int output AS
Begin
	if (not exists(select * from Articulos where codArt = @cod))
	begin
		set @ret = -1
		return 
	end
	
	If (exists(select * from LINEAS where CodArt = @cod))
	begin
		set @ret = -2
		return 
	end
	
	
	Delete From Articulos where codArt = @cod
	set @ret = 1
	
end
go




--ingreso datos de prueba ----------------------------------------------------------------------------
INSERT Articulos Values(23, 'Licuadora', 2500)
INSERT Articulos Values(48, 'Maquina de Coser', 6700)
INSERT Articulos Values(514, 'Ventilador de Techo', 250)
go

INSERT Usuarios(Usulog, PassLog) Values('Usu1', '12345')
INSERT Usuarios(Usulog, PassLog) Values('Usu2', '54321')
go


--ingreso datos de prueba ----------------------------------------------------------------------------
INSERT INTO Articulos (CodArt, NomArt, PreArt) VALUES 
(1023, 'Heladera No Frost', 950),
(2489, 'Lavarropas', 850),
(3157, 'Cocina a Gas', 720),
(4210, 'Microondas 20L', 430),
(5378, 'Aire Acondicionado Split', 600),
(6041, 'Ventilador de Pie', 180),
(7123, 'Jarra Eléctrica', 95),
(8560, 'Procesadora', 125),
(9033, 'Horno Eléctrico', 380),
(9981, 'Calefon', 650)
go


INSERT Usuarios (Usulog, PassLog) VALUES('juan123', 'abc12') 
INSERT Usuarios (Usulog, PassLog) VALUES('maria89', 'qwe34')
INSERT Usuarios (Usulog, PassLog) VALUES('luis456', 'zxc56')
go


INSERT INTO Facturas (NumFact, FechaFact, UsuLog) VALUES
(1, '20240101', 'juan123'),
(2, '20250103', 'luis456'),
(3, '20250103', 'juan123'),
(4, '20250103', 'juan123'),
(5, '20250103', 'luis456'),
(6, '20230511', 'luis456'),
(7, '20240101', 'juan123'),
(8, '20250715', 'luis456'),
(9, '20250715', 'luis456'),
(10, '20250715', 'juan123'),
(11, '20250715', 'luis456'),
(12, '20230511', 'juan123'),
(13, '20230511', 'juan123'),
(14, '20240807', 'luis456'),
(15, '20250809', 'luis456'),
(16, '20250809', 'juan123'),
(17, '20230913', 'luis456'),
(18, '20250809', 'luis456'),
(19, '20250809', 'juan123'),
(20, '20250809', 'maria89'),
(21, '20251001', 'luis456'),
(22, '20251001', 'juan123'),
(23, '20251001', 'luis456'),
(24, '20251001', 'luis456'),
(25, '20230913', 'juan123'),
(26, '20230913', 'luis456'),
(27, '20250715', 'luis456'),
(28, '20240807', 'juan123'),
(29, '20250917', 'luis456'),
(30, '20250715', 'luis456'),
(31, '20250715', 'juan123'),
(32, '20250809', 'luis456'),
(33, '20240807', 'juan123'),
(34, '20240807', 'juan123'),
(35, '20230409', 'luis456'),
(36, '20230411', 'luis456'),
(37, '20240413', 'juan123'),
(38, '20240413', 'maria89'),
(39, '20240413', 'luis456'),
(40, '20220119', 'juan123'),
(41, '20210101', 'juan123'),
(42, '20220119', 'maria89'),
(43, '20210405', 'juan123'),
(44, '20220507', 'juan123'),
(45, '20220507', 'maria89'),
(46, '20220507', 'luis456'),
(47, '20220119', 'juan123'),
(48, '20210715', 'maria89'),
(49, '20220507', 'luis456'),
(50, '20220507', 'juan123'),
(51, '20240101', 'juan123'),
(52, '20250103', 'luis456'),
(53, '20250103', 'juan123'),
(54, '20250103', 'juan123'),
(55, '20250103', 'luis456'),
(56, '20230511', 'luis456'),
(57, '20240101', 'juan123'),
(58, '20250715', 'luis456'),
(59, '20250715', 'luis456'),
(60, '20250715', 'juan123')
go

--son 40 facturas de una lina
INSERT INTO LINEAS (NumFact, CodArt, Cant) VALUES
(1, 1023, 2),
(2, 2489, 1),
(3, 3157, 3),
(4, 4210, 1),
(5, 5378, 2),
(6, 6041, 4),
(7, 7123, 1),
(8, 8560, 2),
(9, 9033, 3),
(10, 9981, 1),
(11, 2489, 2),
(12, 2489, 1),
(13, 3157, 3),
(14, 4210, 2),
(15, 5378, 1),
(16, 7123, 4),
(17, 7123, 3),
(18, 8560, 1),
(19, 9033, 2),
(20, 23, 1),
(41, 2489, 2),
(42, 23, 1),
(43, 3157, 3),
(44, 4210, 2),
(45, 5378, 1),
(46, 7123, 4),
(47, 7123, 3),
(48, 8560, 1),
(49, 9033, 2),
(50, 9033, 1),
(51, 1023, 2),
(52, 2489, 1),
(53, 3157, 3),
(54, 4210, 1),
(55, 5378, 2),
(56, 6041, 4),
(57, 7123, 1),
(58, 8560, 2),
(59, 9033, 3),
(60, 9981, 1)
go


--son 20 facturas con dos lineas
INSERT INTO LINEAS (NumFact, CodArt, Cant) VALUES
(21, 2489, 2),
(21, 5378, 1),
(22, 3157, 3),
(22, 4210, 2),
(23, 5378, 1),
(23, 6041, 4),
(24, 7123, 2),
(24, 8560, 1),
(25, 9033, 3),
(25, 9981, 2),
(26, 1023, 1),
(26, 2489, 4),
(27, 3157, 3),
(27, 4210, 2),
(28, 5378, 1),
(28, 6041, 3),
(29, 7123, 2),
(29, 8560, 1),
(30, 9033, 4),
(30, 9981, 2),
(31, 2489, 3),
(31, 7123, 1),
(32, 3157, 2),
(32, 4210, 1),
(33, 5378, 2),
(33, 6041, 4),
(34, 7123, 1),
(34, 8560, 3),
(35, 9033, 2),
(35, 9981, 1),
(36, 1023, 4),
(36, 2489, 3),
(37, 3157, 1),
(37, 9033, 2),
(38, 5378, 3),
(38, 6041, 1),
(39, 7123, 2),
(39, 8560, 4),
(40, 9033, 1),
(40, 9981, 3)
go
