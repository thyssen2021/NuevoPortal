GO
IF object_id(N'area',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[area]
      PRINT '<<< area en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los areas más relaciones
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2021/09/27
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


GO
CREATE TABLE [Area](
	[clave] [int] IDENTITY(1,1) NOT NULL,
	[activo] [bit] NOT NULL,
	[descripcion] [varchar](80) NOT NULL,
	[listaCorreoElectronico] [varchar](100) NULL,
	[plantaClave] [int] NULL,
 CONSTRAINT [PK_Area] PRIMARY KEY CLUSTERED 
(
	[clave] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

--agregar FK
-- restriccion de clave foranea
alter table [Area]
 add constraint FK_areas_planta_clave
  foreign key (plantaClave)
  references plantas(clave);

  -- restricción default
ALTER TABLE [Area] ADD  CONSTRAINT [DF_area_activo]  DEFAULT (1) FOR [activo]
GO


GO
SET IDENTITY_INSERT [Area] ON 

INSERT [Area] ([clave], [activo], [descripcion], [listaCorreoElectronico], [plantaClave]) VALUES (1, 1, N'Programacion', N'ProgramacionPuebla@correo.com;', 1)
INSERT [Area] ([clave], [activo], [descripcion], [listaCorreoElectronico], [plantaClave]) VALUES (2, 1, N'Jefe almacen', N'JefealmacenPuebla@correo.com;', 1)
INSERT [Area] ([clave], [activo], [descripcion], [listaCorreoElectronico], [plantaClave]) VALUES (3, 1, N'Ventas', N'VentasPuebla@correo.com;', 1)
INSERT [Area] ([clave], [activo], [descripcion], [listaCorreoElectronico], [plantaClave]) VALUES (4, 1, N'Controlling', N'ControllingPuebla@correo.com;', 1)
INSERT [Area] ([clave], [activo], [descripcion], [listaCorreoElectronico], [plantaClave]) VALUES (5, 1, N'Master Data', N'MasterDataPuebla@correo.com;', 1)
INSERT [Area] ([clave], [activo], [descripcion], [listaCorreoElectronico], [plantaClave]) VALUES (6, 1, N'Calidad', N'CalidadPuebla@correo.com;', 1)
INSERT [Area] ([clave], [activo], [descripcion], [listaCorreoElectronico], [plantaClave]) VALUES (7, 1, N'Programacion', N'ProgramacionSilao@correo.com;', 2)
INSERT [Area] ([clave], [activo], [descripcion], [listaCorreoElectronico], [plantaClave]) VALUES (8, 1, N'Jefe almacen', N'JefealmacenSilao@correo.com;', 2)
INSERT [Area] ([clave], [activo], [descripcion], [listaCorreoElectronico], [plantaClave]) VALUES (9, 1, N'Ventas', N'VentasSilao@correo.com;', 2)
INSERT [Area] ([clave], [activo], [descripcion], [listaCorreoElectronico], [plantaClave]) VALUES (10, 1, N'Controlling', N'ControllingSilao@correo.com;', 2)
INSERT [Area] ([clave], [activo], [descripcion], [listaCorreoElectronico], [plantaClave]) VALUES (11, 1, N'Master Data', N'MasterDataSilao@correo.com;', 2)
INSERT [Area] ([clave], [activo], [descripcion], [listaCorreoElectronico], [plantaClave]) VALUES (12, 1, N'Calidad', N'CalidadSilao@correo.com;', 2)
INSERT [Area] ([clave], [activo], [descripcion], [listaCorreoElectronico], [plantaClave]) VALUES (13, 1, N'Programacion', N'ProgramacionSaltillo@correo.com;', 3)
INSERT [Area] ([clave], [activo], [descripcion], [listaCorreoElectronico], [plantaClave]) VALUES (14, 1, N'Jefe almacen', N'JefealmacenSaltillo@correo.com;', 3)
INSERT [Area] ([clave], [activo], [descripcion], [listaCorreoElectronico], [plantaClave]) VALUES (15, 1, N'Ventas', N'VentasSaltillo@correo.com;', 3)
INSERT [Area] ([clave], [activo], [descripcion], [listaCorreoElectronico], [plantaClave]) VALUES (16, 1, N'Controlling', N'ControllingSaltillo@correo.com;', 3)
INSERT [Area] ([clave], [activo], [descripcion], [listaCorreoElectronico], [plantaClave]) VALUES (17, 1, N'Master Data', N'MasterDataSaltillo@correo.com;', 3)
INSERT [Area] ([clave], [activo], [descripcion], [listaCorreoElectronico], [plantaClave]) VALUES (18, 1, N'Calidad', N'CalidadSaltillo@correo.com;', 3)
INSERT [Area] ([clave], [activo], [descripcion], [listaCorreoElectronico], [plantaClave]) VALUES (19, 1, N'Sistema', N'sistemas@correo.com;', 1)
INSERT [Area] ([clave], [activo], [descripcion], [listaCorreoElectronico], [plantaClave]) VALUES (20, 1, N'Sistema', N'sistemasSilao@correo.com;', 2)
INSERT [Area] ([clave], [activo], [descripcion], [listaCorreoElectronico], [plantaClave]) VALUES (128, 1, N'TODA LA PLANTA', N'NA', 2)
INSERT [Area] ([clave], [activo], [descripcion], [listaCorreoElectronico], [plantaClave]) VALUES (129, 1, N'PRODUCCION ', N'NA', 2)
INSERT [Area] ([clave], [activo], [descripcion], [listaCorreoElectronico], [plantaClave]) VALUES (130, 1, N'CINCINNATI', N'NA', 2)
INSERT [Area] ([clave], [activo], [descripcion], [listaCorreoElectronico], [plantaClave]) VALUES (131, 1, N'BLK 1', N'NA', 2)
INSERT [Area] ([clave], [activo], [descripcion], [listaCorreoElectronico], [plantaClave]) VALUES (132, 1, N'ALMACEN DE REFACCIONES', N'NA', 2)
INSERT [Area] ([clave], [activo], [descripcion], [listaCorreoElectronico], [plantaClave]) VALUES (133, 1, N'BLK 2', N'NA', 2)
INSERT [Area] ([clave], [activo], [descripcion], [listaCorreoElectronico], [plantaClave]) VALUES (134, 1, N'ALMACENES', N'NA', 2)
INSERT [Area] ([clave], [activo], [descripcion], [listaCorreoElectronico], [plantaClave]) VALUES (135, 1, N'BLK 3', N'NA', 2)
INSERT [Area] ([clave], [activo], [descripcion], [listaCorreoElectronico], [plantaClave]) VALUES (136, 1, N'MATRICERIA', N'NA', 2)
INSERT [Area] ([clave], [activo], [descripcion], [listaCorreoElectronico], [plantaClave]) VALUES (137, 1, N'EMBARQUES', N'NA', 2)
INSERT [Area] ([clave], [activo], [descripcion], [listaCorreoElectronico], [plantaClave]) VALUES (138, 1, N'RECIBO', N'NA', 2)
INSERT [Area] ([clave], [activo], [descripcion], [listaCorreoElectronico], [plantaClave]) VALUES (139, 1, N'LOBBY', N'NA', 2)
INSERT [Area] ([clave], [activo], [descripcion], [listaCorreoElectronico], [plantaClave]) VALUES (140, 1, N'CONTENEDORES', N'NA', 2)
INSERT [Area] ([clave], [activo], [descripcion], [listaCorreoElectronico], [plantaClave]) VALUES (141, 1, N'VOLTEADOR', N'NA', 2)
INSERT [Area] ([clave], [activo], [descripcion], [listaCorreoElectronico], [plantaClave]) VALUES (142, 1, N'RECURSOS HUMANOS', N'NA', 2)
INSERT [Area] ([clave], [activo], [descripcion], [listaCorreoElectronico], [plantaClave]) VALUES (143, 1, N'MANTENIMIENTO', N'NA', 2)
INSERT [Area] ([clave], [activo], [descripcion], [listaCorreoElectronico], [plantaClave]) VALUES (144, 1, N'COMEDOR', N'NA', 2)
INSERT [Area] ([clave], [activo], [descripcion], [listaCorreoElectronico], [plantaClave]) VALUES (145, 1, N'ALMACEN Y EMBARQUES', N'NA', 1)
INSERT [Area] ([clave], [activo], [descripcion], [listaCorreoElectronico], [plantaClave]) VALUES (146, 1, N'PRODUCCION', N'NA', 1)
INSERT [Area] ([clave], [activo], [descripcion], [listaCorreoElectronico], [plantaClave]) VALUES (147, 1, N'MATRICERIA ', N'NA', 1)
INSERT [Area] ([clave], [activo], [descripcion], [listaCorreoElectronico], [plantaClave]) VALUES (148, 1, N'MANTENIMIENTO', N'NA', 1)
INSERT [Area] ([clave], [activo], [descripcion], [listaCorreoElectronico], [plantaClave]) VALUES (149, 1, N'HSE', N'NA', 1)
SET IDENTITY_INSERT [Area] OFF
GO


 	  
IF object_id(N'area',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< area en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla area  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
