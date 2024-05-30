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
	[descripcion] [varchar](50) NOT NULL,
	[activo] [bit]  NOT NULL DEFAULT 1,	
 CONSTRAINT [PK_SCDM_cat_departamentos_asignacion_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


GO

--delete from [dbo].[SCDM_cat_departamentos_asignacion]; DBCC CHECKIDENT ('SCDM_cat_departamentos_asignacion', RESEED, 0);

--puebla
SET IDENTITY_INSERT [dbo].[SCDM_cat_departamentos_asignacion] ON 

INSERT [dbo].[SCDM_cat_departamentos_asignacion] ([id], [activo], [descripcion]) VALUES (1, 1, N'Facturación')  
INSERT [dbo].[SCDM_cat_departamentos_asignacion] ([id], [activo], [descripcion]) VALUES (2, 1, N'Compras')  
INSERT [dbo].[SCDM_cat_departamentos_asignacion] ([id], [activo], [descripcion]) VALUES (3, 1, N'Controlling')  
INSERT [dbo].[SCDM_cat_departamentos_asignacion] ([id], [activo], [descripcion]) VALUES (4, 1, N'Ingeniería')  
INSERT [dbo].[SCDM_cat_departamentos_asignacion] ([id], [activo], [descripcion]) VALUES (5, 1, N'Calidad')  
INSERT [dbo].[SCDM_cat_departamentos_asignacion] ([id], [activo], [descripcion]) VALUES (6, 1, N'Compras MRO')  
INSERT [dbo].[SCDM_cat_departamentos_asignacion] ([id], [activo], [descripcion]) VALUES (7, 1, N'Disposición')  
INSERT [dbo].[SCDM_cat_departamentos_asignacion] ([id], [activo], [descripcion]) VALUES (8, 1, N'Ventas') 
INSERT [dbo].[SCDM_cat_departamentos_asignacion] ([id], [activo], [descripcion]) VALUES (9, 1, N'SCDM') 

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
