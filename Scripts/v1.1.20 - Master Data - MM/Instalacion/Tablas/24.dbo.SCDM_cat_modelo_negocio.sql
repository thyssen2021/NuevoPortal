--USE Portal_2_0
GO
IF object_id(N'SCDM_cat_modelo_negocio',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[SCDM_cat_modelo_negocio]
      PRINT '<<< SCDM_cat_modelo_negocio en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los SCDM_cat_modelo_negocio
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[SCDM_cat_modelo_negocio](
	[id] [int] IDENTITY(1,1) NOT NULL,	
	[descripcion] [varchar](250) NOT NULL,
	[activo] [bit] NOT NULL,
 CONSTRAINT [PK_SCDM_cat_modelo_negocio_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

--Inserta valores

SET IDENTITY_INSERT [dbo].[SCDM_cat_modelo_negocio] ON 

GO

INSERT [dbo].[SCDM_cat_modelo_negocio] ([id], [activo], [descripcion]) VALUES (1, 1, N'Blanking') 

INSERT [dbo].[SCDM_cat_modelo_negocio] ([id], [activo], [descripcion]) VALUES (2, 1, N'Blanks Package Turnover') 

INSERT [dbo].[SCDM_cat_modelo_negocio] ([id], [activo], [descripcion]) VALUES (3, 1, N'C&B') 

INSERT [dbo].[SCDM_cat_modelo_negocio] ([id], [activo], [descripcion]) VALUES (4, 1, N'Coil to Coil  (warehousing)') 

INSERT [dbo].[SCDM_cat_modelo_negocio] ([id], [activo], [descripcion]) VALUES (5, 1, N'Rewind') 

INSERT [dbo].[SCDM_cat_modelo_negocio] ([id], [activo], [descripcion]) VALUES (6, 1, N'Slitter') 

INSERT [dbo].[SCDM_cat_modelo_negocio] ([id], [activo], [descripcion]) VALUES (7, 1, N'Slitter + Blanking') 

INSERT [dbo].[SCDM_cat_modelo_negocio] ([id], [activo], [descripcion]) VALUES (8, 1, N'Shearing (Rollo+Platina)') 

INSERT [dbo].[SCDM_cat_modelo_negocio] ([id], [activo], [descripcion]) VALUES (9, 1, N'Shearing (Rollo+Cinta+Platina)') 

INSERT [dbo].[SCDM_cat_modelo_negocio] ([id], [activo], [descripcion]) VALUES (10, 1, N'Weight Division (desbaste)') 

INSERT [dbo].[SCDM_cat_modelo_negocio] ([id], [activo], [descripcion]) VALUES (11, 1, N'Weld (Rollo+Platina)') 

INSERT [dbo].[SCDM_cat_modelo_negocio] ([id], [activo], [descripcion]) VALUES (12, 1, N'Weld (Rollo+Cinta+Platina)') 


GO

SET IDENTITY_INSERT [dbo].[SCDM_cat_modelo_negocio] OFF
	  
IF object_id(N'SCDM_cat_modelo_negocio',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< SCDM_cat_modelo_negocio en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla SCDM_cat_modelo_negocio  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
