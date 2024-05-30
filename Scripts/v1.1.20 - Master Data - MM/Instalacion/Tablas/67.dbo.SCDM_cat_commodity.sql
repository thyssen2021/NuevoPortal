--USE Portal_2_0
GO
IF object_id(N'SCDM_cat_commodity',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[SCDM_cat_commodity]
      PRINT '<<< SCDM_cat_commodity en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los SCDM_cat_commodity
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[SCDM_cat_commodity](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[clave][varchar](4) NOT NULL,
	[descripcion] [varchar](100) NOT NULL,
	[activo] [bit] NOT NULL,
 CONSTRAINT [PK_SCDM_cat_commodity_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
--Inserta valores

SET IDENTITY_INSERT [dbo].[SCDM_cat_commodity] ON 

GO
INSERT [dbo].[SCDM_cat_commodity] ([id],[clave], [descripcion], [activo]) VALUES (1, N'AL', N'aluminzed', 1)
INSERT [dbo].[SCDM_cat_commodity] ([id],[clave], [descripcion], [activo]) VALUES (2, N'BN', N'Bondal', 1)
INSERT [dbo].[SCDM_cat_commodity] ([id],[clave], [descripcion], [activo]) VALUES (3, N'CR', N'Cold rolled', 1)
INSERT [dbo].[SCDM_cat_commodity] ([id],[clave], [descripcion], [activo]) VALUES (4, N'EG', N'Electro galvanized', 1)
INSERT [dbo].[SCDM_cat_commodity] ([id],[clave], [descripcion], [activo]) VALUES (5, N'GN', N'Galvannealed', 1)
INSERT [dbo].[SCDM_cat_commodity] ([id],[clave], [descripcion], [activo]) VALUES (6, N'GO', N'Grain Oriented Electrical', 1)
INSERT [dbo].[SCDM_cat_commodity] ([id],[clave], [descripcion], [activo]) VALUES (7, N'HD', N'Hot dipped', 1)
INSERT [dbo].[SCDM_cat_commodity] ([id],[clave], [descripcion], [activo]) VALUES (8, N'HP', N'Hot rolled P&O', 1)
INSERT [dbo].[SCDM_cat_commodity] ([id],[clave], [descripcion], [activo]) VALUES (9, N'HR', N'Hot rolled black', 1)
INSERT [dbo].[SCDM_cat_commodity] ([id],[clave], [descripcion], [activo]) VALUES (10, N'NG', N'Non Grain Oriented Electrical', 1)
INSERT [dbo].[SCDM_cat_commodity] ([id],[clave], [descripcion], [activo]) VALUES (11, N'PP', N'Plate', 1)
INSERT [dbo].[SCDM_cat_commodity] ([id],[clave], [descripcion], [activo]) VALUES (12, N'TP', N'Tin Plate', 1)
INSERT [dbo].[SCDM_cat_commodity] ([id],[clave], [descripcion], [activo]) VALUES (13, N'XD', N'GSP Cold Drawn', 1)
INSERT [dbo].[SCDM_cat_commodity] ([id],[clave], [descripcion], [activo]) VALUES (14, N'XE', N'GSP Extruded', 1)
INSERT [dbo].[SCDM_cat_commodity] ([id],[clave], [descripcion], [activo]) VALUES (15, N'XF', N'GSP Cold Formed', 1)
INSERT [dbo].[SCDM_cat_commodity] ([id],[clave], [descripcion], [activo]) VALUES (16, N'XP', N'GSP PLATE', 1)
INSERT [dbo].[SCDM_cat_commodity] ([id],[clave], [descripcion], [activo]) VALUES (17, N'XR', N'GSP Hot Rolled', 1)
INSERT [dbo].[SCDM_cat_commodity] ([id],[clave], [descripcion], [activo]) VALUES (18, N'XW', N'GSP Laser Weld', 1)

SET IDENTITY_INSERT [dbo].[SCDM_cat_commodity] OFF
GO

IF object_id(N'SCDM_cat_commodity',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< SCDM_cat_commodity en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla SCDM_cat_commodity  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
