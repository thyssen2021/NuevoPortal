--USE Portal_2_0
GO
IF object_id(N'SCDM_cat_departamentos_asignacion',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[SCDM_cat_departamentos_asignacion]
      PRINT '<<< SCDM_cat_departamentos_asignacion en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los SCDM_cat_departamentos_asignacion
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[SCDM_cat_departamentos_asignacion](
	[id] [int] IDENTITY(1,1) NOT NULL,	
	[id_planta][int] NOT NULL,
	[descripcion] [varchar](50) NOT NULL,
	[activo] [bit]  NOT NULL DEFAULT 1,	
 CONSTRAINT [PK_SCDM_cat_departamentos_asignacion_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

  -- restriccion de clave foranea
alter table [dbo].[SCDM_cat_departamentos_asignacion]
 add constraint FK_SCDM_cat_departamentos_asignacion_id_planta
  foreign key (id_planta)
  references plantas(clave);

SET IDENTITY_INSERT [dbo].[SCDM_cat_departamentos_asignacion] ON 

GO


--puebla
INSERT [dbo].[SCDM_cat_departamentos_asignacion] ([id], [id_planta], [activo], [descripcion]) VALUES (1, 1, 1, N'Facturación')  
INSERT [dbo].[SCDM_cat_departamentos_asignacion] ([id], [id_planta], [activo], [descripcion]) VALUES (2, 1, 1, N'Compras')  
INSERT [dbo].[SCDM_cat_departamentos_asignacion] ([id], [id_planta], [activo], [descripcion]) VALUES (3, 1, 1, N'Controlling')  
INSERT [dbo].[SCDM_cat_departamentos_asignacion] ([id], [id_planta], [activo], [descripcion]) VALUES (4, 1, 1, N'Ingeniería')  
INSERT [dbo].[SCDM_cat_departamentos_asignacion] ([id], [id_planta], [activo], [descripcion]) VALUES (5, 1, 1, N'Calidad')  
INSERT [dbo].[SCDM_cat_departamentos_asignacion] ([id], [id_planta], [activo], [descripcion]) VALUES (6, 1, 1, N'Compras MRO')  
INSERT [dbo].[SCDM_cat_departamentos_asignacion] ([id], [id_planta], [activo], [descripcion]) VALUES (7, 1, 1, N'Disposición')  
INSERT [dbo].[SCDM_cat_departamentos_asignacion] ([id], [id_planta], [activo], [descripcion]) VALUES (8, 1, 1, N'Ventas')  

--silao
INSERT [dbo].[SCDM_cat_departamentos_asignacion] ([id], [id_planta], [activo], [descripcion]) VALUES (9, 2, 1, N'Facturación')  
INSERT [dbo].[SCDM_cat_departamentos_asignacion] ([id], [id_planta], [activo], [descripcion]) VALUES (10, 2, 1, N'Compras')  
INSERT [dbo].[SCDM_cat_departamentos_asignacion] ([id], [id_planta], [activo], [descripcion]) VALUES (11, 2, 1, N'Controlling')  
INSERT [dbo].[SCDM_cat_departamentos_asignacion] ([id], [id_planta], [activo], [descripcion]) VALUES (12, 2, 1, N'Ingeniería')  
INSERT [dbo].[SCDM_cat_departamentos_asignacion] ([id], [id_planta], [activo], [descripcion]) VALUES (13, 2, 1, N'Calidad')  
INSERT [dbo].[SCDM_cat_departamentos_asignacion] ([id], [id_planta], [activo], [descripcion]) VALUES (14, 2, 1, N'Compras MRO')  
INSERT [dbo].[SCDM_cat_departamentos_asignacion] ([id], [id_planta], [activo], [descripcion]) VALUES (15, 2, 1, N'Disposición')  
INSERT [dbo].[SCDM_cat_departamentos_asignacion] ([id], [id_planta], [activo], [descripcion]) VALUES (16, 2, 1, N'Ventas')  

--saltillo
INSERT [dbo].[SCDM_cat_departamentos_asignacion] ([id], [id_planta], [activo], [descripcion]) VALUES (17, 3, 1, N'Facturación')  
INSERT [dbo].[SCDM_cat_departamentos_asignacion] ([id], [id_planta], [activo], [descripcion]) VALUES (18, 3, 1, N'Compras')  
INSERT [dbo].[SCDM_cat_departamentos_asignacion] ([id], [id_planta], [activo], [descripcion]) VALUES (19, 3, 1, N'Controlling')  
INSERT [dbo].[SCDM_cat_departamentos_asignacion] ([id], [id_planta], [activo], [descripcion]) VALUES (20, 3, 1, N'Ingeniería')  
INSERT [dbo].[SCDM_cat_departamentos_asignacion] ([id], [id_planta], [activo], [descripcion]) VALUES (21, 3, 1, N'Calidad')  
INSERT [dbo].[SCDM_cat_departamentos_asignacion] ([id], [id_planta], [activo], [descripcion]) VALUES (22, 3, 1, N'Compras MRO')  
INSERT [dbo].[SCDM_cat_departamentos_asignacion] ([id], [id_planta], [activo], [descripcion]) VALUES (23, 3, 1, N'Disposición')  
INSERT [dbo].[SCDM_cat_departamentos_asignacion] ([id], [id_planta], [activo], [descripcion]) VALUES (24, 3, 1, N'Ventas')  



SET IDENTITY_INSERT [dbo].[SCDM_cat_departamentos_asignacion] OFF

	  
IF object_id(N'SCDM_cat_departamentos_asignacion',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< SCDM_cat_departamentos_asignacion en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla SCDM_cat_departamentos_asignacion  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
