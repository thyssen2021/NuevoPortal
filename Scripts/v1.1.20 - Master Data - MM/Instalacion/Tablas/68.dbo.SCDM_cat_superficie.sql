--USE Portal_2_0
GO
IF object_id(N'SCDM_cat_superficie',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[SCDM_cat_superficie]
      PRINT '<<< SCDM_cat_superficie en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los SCDM_cat_superficie
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[SCDM_cat_superficie](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[clave][varchar](4) NOT NULL,
	[descripcion] [varchar](100) NOT NULL,
	[activo] [bit] NOT NULL,
 CONSTRAINT [PK_SCDM_cat_superficie_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
--Inserta valores

SET IDENTITY_INSERT [dbo].[SCDM_cat_superficie] ON 

GO

INSERT [dbo].[SCDM_cat_superficie] ([id],[clave], [descripcion], [activo]) VALUES (1, N'1', N'exposed', 1)
INSERT [dbo].[SCDM_cat_superficie] ([id],[clave], [descripcion], [activo]) VALUES (2, N'2', N'semi-exposed', 1)
INSERT [dbo].[SCDM_cat_superficie] ([id],[clave], [descripcion], [activo]) VALUES (3, N'3', N'unexposed', 1)
INSERT [dbo].[SCDM_cat_superficie] ([id],[clave], [descripcion], [activo]) VALUES (4, N'4', N'black', 1)
INSERT [dbo].[SCDM_cat_superficie] ([id],[clave], [descripcion], [activo]) VALUES (5, N'5', N'Shotblasted & primered', 1)
INSERT [dbo].[SCDM_cat_superficie] ([id],[clave], [descripcion], [activo]) VALUES (6, N'6', N'blank RA max. 0,20', 1)
INSERT [dbo].[SCDM_cat_superficie] ([id],[clave], [descripcion], [activo]) VALUES (7, N'7', N'matt RA  0,90 - 1,50', 1)
INSERT [dbo].[SCDM_cat_superficie] ([id],[clave], [descripcion], [activo]) VALUES (8, N'8', N'SF RA 0,25-0,45 konv', 1)
INSERT [dbo].[SCDM_cat_superficie] ([id],[clave], [descripcion], [activo]) VALUES (9, N'9', N'SF RA 0,30-0,56 DR', 1)
INSERT [dbo].[SCDM_cat_superficie] ([id],[clave], [descripcion], [activo]) VALUES (10, N'10', N'SF RA 0,30-0,56 konv', 1)
INSERT [dbo].[SCDM_cat_superficie] ([id],[clave], [descripcion], [activo]) VALUES (11, N'11', N'210', 1)
INSERT [dbo].[SCDM_cat_superficie] ([id],[clave], [descripcion], [activo]) VALUES (12, N'12', N'211', 1)
INSERT [dbo].[SCDM_cat_superficie] ([id],[clave], [descripcion], [activo]) VALUES (13, N'13', N'221', 1)
INSERT [dbo].[SCDM_cat_superficie] ([id],[clave], [descripcion], [activo]) VALUES (14, N'14', N'222', 1)
INSERT [dbo].[SCDM_cat_superficie] ([id],[clave], [descripcion], [activo]) VALUES (15, N'15', N'261', 1)
INSERT [dbo].[SCDM_cat_superficie] ([id],[clave], [descripcion], [activo]) VALUES (16, N'16', N'266', 1)
INSERT [dbo].[SCDM_cat_superficie] ([id],[clave], [descripcion], [activo]) VALUES (17, N'17', N'SF 7C', 1)
INSERT [dbo].[SCDM_cat_superficie] ([id],[clave], [descripcion], [activo]) VALUES (18, N'18', N'Matte 5B', 1)
INSERT [dbo].[SCDM_cat_superficie] ([id],[clave], [descripcion], [activo]) VALUES (19, N'19', N'Stone Finish', 1)
INSERT [dbo].[SCDM_cat_superficie] ([id],[clave], [descripcion], [activo]) VALUES (20, N'20', N'Stone Finish Fine', 1)
INSERT [dbo].[SCDM_cat_superficie] ([id],[clave], [descripcion], [activo]) VALUES (21, N'21', N'Bright Finish', 1)
INSERT [dbo].[SCDM_cat_superficie] ([id],[clave], [descripcion], [activo]) VALUES (22, N'22', N'Matte 5c', 1)
INSERT [dbo].[SCDM_cat_superficie] ([id],[clave], [descripcion], [activo]) VALUES (23, N'23', N'531', 1)
INSERT [dbo].[SCDM_cat_superficie] ([id],[clave], [descripcion], [activo]) VALUES (24, N'24', N'5C', 1)
INSERT [dbo].[SCDM_cat_superficie] ([id],[clave], [descripcion], [activo]) VALUES (25, N'25', N'Matte Finish', 1)



SET IDENTITY_INSERT [dbo].[SCDM_cat_superficie] OFF
GO

IF object_id(N'SCDM_cat_superficie',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< SCDM_cat_superficie en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla SCDM_cat_superficie  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
