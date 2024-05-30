--USE Portal_2_0
GO
IF object_id(N'SCDM_cat_secciones',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[SCDM_cat_secciones]
      PRINT '<<< SCDM_cat_secciones en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los SCDM_cat_secciones
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[SCDM_cat_secciones](
	[id] [int] IDENTITY(1,1) NOT NULL,	
	[descripcion] [varchar](50) NOT NULL,
	[aplica_solicitud] [bit] NOT NULL,
	[activo] [bit] NOT NULL,
 CONSTRAINT [PK_SCDM_cat_secciones_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

--Inserta valores

SET IDENTITY_INSERT [dbo].[SCDM_cat_secciones] ON 

GO

INSERT [dbo].[SCDM_cat_secciones] ([id], [activo], [aplica_solicitud], [descripcion]) VALUES (1, 1, 1, N'Lista Técnica') 
INSERT [dbo].[SCDM_cat_secciones] ([id], [activo], [aplica_solicitud], [descripcion]) VALUES (2, 1, 1, N'Formato Compra') 
INSERT [dbo].[SCDM_cat_secciones] ([id], [activo], [aplica_solicitud], [descripcion]) VALUES (3, 1, 1, N'Rollo') 
INSERT [dbo].[SCDM_cat_secciones] ([id], [activo], [aplica_solicitud], [descripcion]) VALUES (4, 1, 1, N'Cinta') 
INSERT [dbo].[SCDM_cat_secciones] ([id], [activo], [aplica_solicitud], [descripcion]) VALUES (5, 1, 1, N'Platina') 
INSERT [dbo].[SCDM_cat_secciones] ([id], [activo], [aplica_solicitud], [descripcion]) VALUES (6, 1, 1, N'Shearing') 
INSERT [dbo].[SCDM_cat_secciones] ([id], [activo], [aplica_solicitud], [descripcion]) VALUES (7, 1, 1, N'Platina Soldada') 
INSERT [dbo].[SCDM_cat_secciones] ([id], [activo], [aplica_solicitud], [descripcion]) VALUES (8, 1, 1, N'C&B') 
INSERT [dbo].[SCDM_cat_secciones] ([id], [activo], [aplica_solicitud], [descripcion]) VALUES (9, 1, 1, N'Cambio Descripción') 
INSERT [dbo].[SCDM_cat_secciones] ([id], [activo], [aplica_solicitud], [descripcion]) VALUES (10, 1, 1, N'Cambio Ingeniería') 
INSERT [dbo].[SCDM_cat_secciones] ([id], [activo], [aplica_solicitud], [descripcion]) VALUES (11, 1, 1, N'Creación con Referencia') 
INSERT [dbo].[SCDM_cat_secciones] ([id], [activo], [aplica_solicitud], [descripcion]) VALUES (12, 1, 1, N'Cambio de Estatus') 

GO

SET IDENTITY_INSERT [dbo].[SCDM_cat_secciones] OFF
	  
IF object_id(N'SCDM_cat_secciones',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< SCDM_cat_secciones en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla SCDM_cat_secciones  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
