GO
IF object_id(N'IM_cat_area',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IM_cat_area]
      PRINT '<<< IM_cat_area en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los IM_cat_impacto
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2021/12/09
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE IM_cat_area(
	[id] [int] IDENTITY(1,1) NOT NULL,
	[activo] [bit] NOT NULL,
	[descripcion] [varchar](50) NOT NULL,
	[listaCorreoElectronico] [varchar](50) NOT NULL,
	[id_planta] [int] NULL,
 CONSTRAINT [PK_IM_cat_Area] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

--Agrega FK
  -- restriccion de clave foranea
alter table [dbo].IM_cat_area
 add constraint FK_IM_cat_area_id_planta
  foreign key (id_planta)
  references plantas(clave);


SET IDENTITY_INSERT IM_cat_area ON 
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (1, 1, N'Programacion', N'ProgramacionPuebla@correo.com;', 1)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (2, 1, N'Jefe almacen', N'JefealmacenPuebla@correo.com;', 1)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (3, 1, N'Ventas', N'VentasPuebla@correo.com;', 1)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (4, 1, N'Controlling', N'ControllingPuebla@correo.com;', 1)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (5, 1, N'Master Data', N'MasterDataPuebla@correo.com;', 1)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (6, 1, N'Calidad', N'CalidadPuebla@correo.com;', 1)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (7, 1, N'Programacion', N'ProgramacionSilao@correo.com;', 2)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (8, 1, N'Jefe almacen', N'JefealmacenSilao@correo.com;', 2)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (9, 1, N'Ventas', N'VentasSilao@correo.com;', 2)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (10, 1, N'Controlling', N'ControllingSilao@correo.com;', 2)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (11, 1, N'Master Data', N'MasterDataSilao@correo.com;', 2)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (12, 1, N'Calidad', N'CalidadSilao@correo.com;', 2)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (13, 1, N'Programacion', N'ProgramacionSaltillo@correo.com;', 3)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (14, 1, N'Jefe almacen', N'JefealmacenSaltillo@correo.com;', 3)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (15, 1, N'Ventas', N'VentasSaltillo@correo.com;', 3)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (16, 1, N'Controlling', N'ControllingSaltillo@correo.com;', 3)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (17, 1, N'Master Data', N'MasterDataSaltillo@correo.com;', 3)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (18, 1, N'Calidad', N'CalidadSaltillo@correo.com;', 3)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (19, 1, N'Sistema', N'sistemas@correo.com;', 1)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (20, 1, N'Sistema', N'sistemasSilao@correo.com;', 2)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (128, 1, N'TODA LA PLANTA', N'NA', 2)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (129, 1, N'PRODUCCION ', N'NA', 2)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (130, 1, N'CINCINNATI', N'NA', 2)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (131, 1, N'BLK 1', N'NA', 2)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (132, 1, N'ALMACEN DE REFACCIONES', N'NA', 2)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (133, 1, N'BLK 2', N'NA', 2)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (134, 1, N'ALMACENES', N'NA', 2)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (135, 1, N'BLK 3', N'NA', 2)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (136, 1, N'MATRICERIA', N'NA', 2)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (137, 1, N'EMBARQUES', N'NA', 2)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (138, 1, N'RECIBO', N'NA', 2)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (139, 1, N'LOBBY', N'NA', 2)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (140, 1, N'CONTENEDORES', N'NA', 2)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (141, 1, N'VOLTEADOR', N'NA', 2)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (142, 1, N'RECURSOS HUMANOS', N'NA', 2)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (143, 1, N'MANTENIMIENTO', N'NA', 2)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (144, 1, N'COMEDOR', N'NA', 2)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (145, 1, N'ALMACEN Y EMBARQUES', N'NA', 1)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (146, 1, N'PRODUCCION', N'NA', 1)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (147, 1, N'MATRICERIA ', N'NA', 1)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (148, 1, N'MANTENIMIENTO', N'NA', 1)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (149, 1, N'HSE', N'NA', 1)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (150, 1, N'PROGRAMACION', N'NA', 5)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (151, 1, N'JEFE ALMACEN', N'NA', 5)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (152, 1, N'VENTAS', N'NA', 5)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (153, 1, N'CONTROLLING', N'NA', 5)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (154, 1, N'MASTER DATA', N'NA', 5)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (155, 1, N'CALIDAD', N'NA', 5)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (156, 1, N'SISTEMA', N'NA', 5)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (157, 1, N'ALMACEN Y EMBARQUES', N'NA', 5)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (158, 1, N'PRODUCCION', N'NA', 5)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (159, 1, N'MATRICERIA ', N'NA', 5)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (160, 1, N'MANTENIMIENTO', N'NA', 5)
GO
INSERT IM_cat_area ([id], [activo], [descripcion], [listaCorreoElectronico], [id_planta]) VALUES (161, 1, N'HSE', N'NA', 5)
GO
SET IDENTITY_INSERT IM_cat_area OFF
GO



IF object_id(N'IM_cat_area',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IM_cat_area en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IM_cat_area  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
