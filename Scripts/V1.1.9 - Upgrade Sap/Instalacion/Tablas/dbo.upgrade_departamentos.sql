USE Portal_2_0
GO
IF object_id(N'upgrade_departamentos',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[upgrade_departamentos]
      PRINT '<<< upgrade_departamentos en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los upgrade_departamentos
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/02/23
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [upgrade_departamentos](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_upgrade_planta][int] NOT NULL,
	[descripcion] [varchar](40) NOT NULL,
	[activo] [bit] NOT NULL	
 CONSTRAINT [PK_upgrade_departamentos] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


-- restriccion de clave foranea
  alter table [upgrade_departamentos]
 add constraint FK_upgrade_departamentos_id_planta
  foreign key (id_upgrade_planta)
  references upgrade_plantas(id);

-- restricción default
ALTER TABLE [upgrade_departamentos] ADD  CONSTRAINT [DF_upgrade_departamentos_activo]  DEFAULT (1) FOR [activo]
GO


--SET IDENTITY_INSERT [upgrade_departamentos] ON 

GO
INSERT INTO [dbo].[upgrade_departamentos] ([id_upgrade_planta],[descripcion],[activo]) VALUES (1,N'Almacén',1)
INSERT INTO [dbo].[upgrade_departamentos] ([id_upgrade_planta],[descripcion],[activo]) VALUES (1,N'Almacén de Refacciones',1)
INSERT INTO [dbo].[upgrade_departamentos] ([id_upgrade_planta],[descripcion],[activo]) VALUES (1,N'Almacén y Embarques',1)
INSERT INTO [dbo].[upgrade_departamentos] ([id_upgrade_planta],[descripcion],[activo]) VALUES (1,N'Calidad',1)
INSERT INTO [dbo].[upgrade_departamentos] ([id_upgrade_planta],[descripcion],[activo]) VALUES (1,N'Compras',1)
INSERT INTO [dbo].[upgrade_departamentos] ([id_upgrade_planta],[descripcion],[activo]) VALUES (1,N'Compras Materia Prima',1)
INSERT INTO [dbo].[upgrade_departamentos] ([id_upgrade_planta],[descripcion],[activo]) VALUES (1,N'Disposición',1)
INSERT INTO [dbo].[upgrade_departamentos] ([id_upgrade_planta],[descripcion],[activo]) VALUES (1,N'Finanzas',1)
INSERT INTO [dbo].[upgrade_departamentos] ([id_upgrade_planta],[descripcion],[activo]) VALUES (1,N'Ingenieria',1)
INSERT INTO [dbo].[upgrade_departamentos] ([id_upgrade_planta],[descripcion],[activo]) VALUES (1,N'Producción',1)
INSERT INTO [dbo].[upgrade_departamentos] ([id_upgrade_planta],[descripcion],[activo]) VALUES (1,N'Programación Producción',1)
INSERT INTO [dbo].[upgrade_departamentos] ([id_upgrade_planta],[descripcion],[activo]) VALUES (1,N'Purchasing',0)
INSERT INTO [dbo].[upgrade_departamentos] ([id_upgrade_planta],[descripcion],[activo]) VALUES (1,N'Recepción facturas',1)
INSERT INTO [dbo].[upgrade_departamentos] ([id_upgrade_planta],[descripcion],[activo]) VALUES (1,N'Ventas',1)
INSERT INTO [dbo].[upgrade_departamentos] ([id_upgrade_planta],[descripcion],[activo]) VALUES (3,N'Almacén',1)
INSERT INTO [dbo].[upgrade_departamentos] ([id_upgrade_planta],[descripcion],[activo]) VALUES (3,N'C&B',1)
INSERT INTO [dbo].[upgrade_departamentos] ([id_upgrade_planta],[descripcion],[activo]) VALUES (2,N'Almacén de Refacciones',1)
INSERT INTO [dbo].[upgrade_departamentos] ([id_upgrade_planta],[descripcion],[activo]) VALUES (2,N'Almacen Y Embarques',1)
INSERT INTO [dbo].[upgrade_departamentos] ([id_upgrade_planta],[descripcion],[activo]) VALUES (2,N'Calidad',1)
INSERT INTO [dbo].[upgrade_departamentos] ([id_upgrade_planta],[descripcion],[activo]) VALUES (2,N'Ingenieria',1)
INSERT INTO [dbo].[upgrade_departamentos] ([id_upgrade_planta],[descripcion],[activo]) VALUES (2,N'Producción',1)
INSERT INTO [dbo].[upgrade_departamentos] ([id_upgrade_planta],[descripcion],[activo]) VALUES (2,N'Programacion Y Control De Produccion',1)
-- Se agregan deptos de IT
INSERT INTO [dbo].[upgrade_departamentos] ([id_upgrade_planta],[descripcion],[activo]) VALUES (2,N'IT',1)
INSERT INTO [dbo].[upgrade_departamentos] ([id_upgrade_planta],[descripcion],[activo]) VALUES (1,N'IT',1)
INSERT INTO [dbo].[upgrade_departamentos] ([id_upgrade_planta],[descripcion],[activo]) VALUES (2,N'W&S',1)



GO


--SET IDENTITY_INSERT [upgrade_departamentos] OFF
GO
 	  
IF object_id(N'upgrade_departamentos',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< upgrade_departamentos en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla upgrade_departamentos  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
