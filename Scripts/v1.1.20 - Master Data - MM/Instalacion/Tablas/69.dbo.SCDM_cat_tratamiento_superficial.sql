--USE Portal_2_0
GO
IF object_id(N'SCDM_cat_tratamiento_superficial',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[SCDM_cat_tratamiento_superficial]
      PRINT '<<< SCDM_cat_tratamiento_superficial en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los SCDM_cat_tratamiento_superficial
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[SCDM_cat_tratamiento_superficial](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[clave][varchar](4) NOT NULL,
	[descripcion] [varchar](100) NOT NULL,
	[activo] [bit] NOT NULL,
 CONSTRAINT [PK_SCDM_cat_tratamiento_superficial_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
--Inserta valores

SET IDENTITY_INSERT [dbo].[SCDM_cat_tratamiento_superficial] ON 

GO
INSERT [dbo].[SCDM_cat_tratamiento_superficial] ([id],[clave], [descripcion], [activo]) VALUES (1, N'1', N'dry', 1)
INSERT [dbo].[SCDM_cat_tratamiento_superficial] ([id],[clave], [descripcion], [activo]) VALUES (2, N'2', N'oil', 1)
INSERT [dbo].[SCDM_cat_tratamiento_superficial] ([id],[clave], [descripcion], [activo]) VALUES (3, N'3', N'prelube', 1)
INSERT [dbo].[SCDM_cat_tratamiento_superficial] ([id],[clave], [descripcion], [activo]) VALUES (4, N'4', N'prephos', 1)
INSERT [dbo].[SCDM_cat_tratamiento_superficial] ([id],[clave], [descripcion], [activo]) VALUES (5, N'5', N'Bonazinc 1 side', 1)
INSERT [dbo].[SCDM_cat_tratamiento_superficial] ([id],[clave], [descripcion], [activo]) VALUES (6, N'6', N'Bonazinc 2 side', 1)
INSERT [dbo].[SCDM_cat_tratamiento_superficial] ([id],[clave], [descripcion], [activo]) VALUES (7, N'7', N'Granacoat 1 sided', 1)
INSERT [dbo].[SCDM_cat_tratamiento_superficial] ([id],[clave], [descripcion], [activo]) VALUES (8, N'8', N'Granacoat 2 sided', 1)
INSERT [dbo].[SCDM_cat_tratamiento_superficial] ([id],[clave], [descripcion], [activo]) VALUES (9, N'9', N'LC 0000 / 0044', 1)
INSERT [dbo].[SCDM_cat_tratamiento_superficial] ([id],[clave], [descripcion], [activo]) VALUES (10, N'10', N'LC 0000 / 0244', 1)
INSERT [dbo].[SCDM_cat_tratamiento_superficial] ([id],[clave], [descripcion], [activo]) VALUES (11, N'11', N'LC 0044 / 0044', 1)
INSERT [dbo].[SCDM_cat_tratamiento_superficial] ([id],[clave], [descripcion], [activo]) VALUES (12, N'12', N'LC 0202 / 1011', 1)
INSERT [dbo].[SCDM_cat_tratamiento_superficial] ([id],[clave], [descripcion], [activo]) VALUES (13, N'13', N'LC 0244 / 0244', 1)
INSERT [dbo].[SCDM_cat_tratamiento_superficial] ([id],[clave], [descripcion], [activo]) VALUES (14, N'14', N'LC 1011 / 0000', 1)
INSERT [dbo].[SCDM_cat_tratamiento_superficial] ([id],[clave], [descripcion], [activo]) VALUES (15, N'15', N'LC 1011 / 0202', 1)
INSERT [dbo].[SCDM_cat_tratamiento_superficial] ([id],[clave], [descripcion], [activo]) VALUES (16, N'16', N'LC 1011 / 2003', 1)
INSERT [dbo].[SCDM_cat_tratamiento_superficial] ([id],[clave], [descripcion], [activo]) VALUES (17, N'17', N'LC 2002 / 2003', 1)
INSERT [dbo].[SCDM_cat_tratamiento_superficial] ([id],[clave], [descripcion], [activo]) VALUES (18, N'18', N'LC 2002 / 2602', 1)
INSERT [dbo].[SCDM_cat_tratamiento_superficial] ([id],[clave], [descripcion], [activo]) VALUES (19, N'19', N'LC 2999 / 2999', 1)
INSERT [dbo].[SCDM_cat_tratamiento_superficial] ([id],[clave], [descripcion], [activo]) VALUES (20, N'20', N'phosphate free', 1)

SET IDENTITY_INSERT [dbo].[SCDM_cat_tratamiento_superficial] OFF
GO

IF object_id(N'SCDM_cat_tratamiento_superficial',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< SCDM_cat_tratamiento_superficial en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla SCDM_cat_tratamiento_superficial  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO

