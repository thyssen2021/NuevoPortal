USE Portal_2_0
GO
IF object_id(N'IT_carpetas_red',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IT_carpetas_red]
      PRINT '<<< IT_carpetas_red en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los IT_carpetas_red
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/05/10
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [IT_carpetas_red](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[descripcion] [varchar](40) NOT NULL,
	[aplica_descripcion] [bit] NOT NULL,
	[activo] [bit] NOT NULL	
 CONSTRAINT [PK_IT_carpetas_red] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restricción default
ALTER TABLE [IT_carpetas_red] ADD  CONSTRAINT [DF_IT_carpetas_red_activo]  DEFAULT (1) FOR [activo]
GO


SET IDENTITY_INSERT [IT_carpetas_red] ON 

INSERT [IT_carpetas_red] ([id], [descripcion],[aplica_descripcion], [activo]) VALUES (1, N'Almacén',0, 1)
INSERT [IT_carpetas_red] ([id], [descripcion],[aplica_descripcion], [activo]) VALUES (2, N'Calidad',0, 1)
INSERT [IT_carpetas_red] ([id], [descripcion],[aplica_descripcion], [activo]) VALUES (3, N'Contabilidad',0, 1)
INSERT [IT_carpetas_red] ([id], [descripcion],[aplica_descripcion], [activo]) VALUES (4, N'Compras',0, 1)
INSERT [IT_carpetas_red] ([id], [descripcion],[aplica_descripcion], [activo]) VALUES (5, N'Comercio Exterior',0, 1)
INSERT [IT_carpetas_red] ([id], [descripcion],[aplica_descripcion], [activo]) VALUES (6, N'Controlling',0, 1)
INSERT [IT_carpetas_red] ([id], [descripcion],[aplica_descripcion], [activo]) VALUES (7, N'Dirección',0, 1)
INSERT [IT_carpetas_red] ([id], [descripcion],[aplica_descripcion], [activo]) VALUES (8, N'Disposición',0, 1)
INSERT [IT_carpetas_red] ([id], [descripcion],[aplica_descripcion], [activo]) VALUES (9, N'Ingeniería',0, 1)
INSERT [IT_carpetas_red] ([id], [descripcion],[aplica_descripcion], [activo]) VALUES (10, N'IT',0, 1)
INSERT [IT_carpetas_red] ([id], [descripcion],[aplica_descripcion], [activo]) VALUES (11, N'Mantenimiento',0, 1)
INSERT [IT_carpetas_red] ([id], [descripcion],[aplica_descripcion], [activo]) VALUES (12, N'Matricería',0, 1)
INSERT [IT_carpetas_red] ([id], [descripcion],[aplica_descripcion], [activo]) VALUES (13, N'Operaciones',0, 1)
INSERT [IT_carpetas_red] ([id], [descripcion],[aplica_descripcion], [activo]) VALUES (14, N'Programación',0, 1)
INSERT [IT_carpetas_red] ([id], [descripcion],[aplica_descripcion], [activo]) VALUES (15, N'RH',0, 1)
INSERT [IT_carpetas_red] ([id], [descripcion],[aplica_descripcion], [activo]) VALUES (16, N'Ventas',0, 1)
INSERT [IT_carpetas_red] ([id], [descripcion],[aplica_descripcion], [activo]) VALUES (17, N'Seguridad',0, 1)
INSERT [IT_carpetas_red] ([id], [descripcion],[aplica_descripcion], [activo]) VALUES (18, N'Otro (especificar)',1, 1)

SET IDENTITY_INSERT [IT_carpetas_red] OFF
GO


 	  
IF object_id(N'IT_carpetas_red',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IT_carpetas_red en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IT_carpetas_red  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
