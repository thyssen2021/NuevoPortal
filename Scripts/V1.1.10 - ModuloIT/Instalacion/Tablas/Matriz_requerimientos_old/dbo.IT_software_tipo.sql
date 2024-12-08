USE Portal_2_0
GO
IF object_id(N'IT_software_tipo',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IT_software_tipo]
      PRINT '<<< IT_software_tipo en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los IT_software_tipo
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/05/10
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [IT_software_tipo](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[descripcion] [varchar](40) NOT NULL,
	[aplica_descripcion] [bit] NOT NULL,
	[activo] [bit] NOT NULL	
 CONSTRAINT [PK_IT_software_tipo] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restricción default
ALTER TABLE [IT_software_tipo] ADD  CONSTRAINT [DF_IT_software_tipo_activo]  DEFAULT (1) FOR [activo]
GO


SET IDENTITY_INSERT [IT_software_tipo] ON 

INSERT [IT_software_tipo] ([id], [descripcion],[aplica_descripcion], [activo]) VALUES (1, N'Usuario SAP (usuario referencia)',1, 1)
INSERT [IT_software_tipo] ([id], [descripcion],[aplica_descripcion], [activo]) VALUES (2, N'Autocad',0, 1)
INSERT [IT_software_tipo] ([id], [descripcion],[aplica_descripcion], [activo]) VALUES (3, N'Visores DWG, JIT, JT2G0',0, 1)
INSERT [IT_software_tipo] ([id], [descripcion],[aplica_descripcion], [activo]) VALUES (4, N'Project',0, 1)
INSERT [IT_software_tipo] ([id], [descripcion],[aplica_descripcion], [activo]) VALUES (5, N'Adobe PRO',0, 1)
INSERT [IT_software_tipo] ([id], [descripcion],[aplica_descripcion], [activo]) VALUES (6, N'Visio',0, 1)
INSERT [IT_software_tipo] ([id], [descripcion],[aplica_descripcion], [activo]) VALUES (7, N'MP9',0, 1)
INSERT [IT_software_tipo] ([id], [descripcion],[aplica_descripcion], [activo]) VALUES (8, N'Simens',0, 1)
INSERT [IT_software_tipo] ([id], [descripcion],[aplica_descripcion], [activo]) VALUES (9, N'COI',0, 1)
INSERT [IT_software_tipo] ([id], [descripcion],[aplica_descripcion], [activo]) VALUES (10, N'COFIDI',0, 1)
INSERT [IT_software_tipo] ([id], [descripcion],[aplica_descripcion], [activo]) VALUES (11, N'GoTo Meeting',0, 1)
INSERT [IT_software_tipo] ([id], [descripcion],[aplica_descripcion], [activo]) VALUES (12, N'Citrix',0, 1)
INSERT [IT_software_tipo] ([id], [descripcion],[aplica_descripcion], [activo]) VALUES (13, N'OEE',0, 1)
INSERT [IT_software_tipo] ([id], [descripcion],[aplica_descripcion], [activo]) VALUES (14, N'Otro (especificar)',1, 1)

SET IDENTITY_INSERT [IT_software_tipo] OFF
GO


 	  
IF object_id(N'IT_software_tipo',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IT_software_tipo en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IT_software_tipo  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
