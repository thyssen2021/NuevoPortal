--USE Portal_2_0
GO
IF object_id(N'SCDM_cat_material_referencia',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[SCDM_cat_material_referencia]
      PRINT '<<< SCDM_cat_material_referencia en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los SCDM_cat_material_referencia
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[SCDM_cat_material_referencia](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[clave][varchar](30) NOT NULL,
	[material_sap] [varchar](15) NOT NULL,
 CONSTRAINT [PK_SCDM_cat_material_referencia_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
--Inserta valores

SET IDENTITY_INSERT [dbo].[SCDM_cat_material_referencia] ON 

GO
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (1, N'R-I-AL-KG-Cinta', N'AL01872')
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (2, N'R-I-AL-PC-Platina', N'AL01873')
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (3, N'R-E-AL-KG-Cinta', N'AL01874')
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (4, N'R-E-AL-KG-Rollo', N'AL01875')
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (5, N'R-E-AL-PC-Platina', N'AL01876')
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (6, N'R-E-CM-PC-Platina', N'CM15260')
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (7, N'R-I-CM-PC-Platina', N'CM15261')
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (8, N'R-I-CM-KG-Cinta', N'CM15262')
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (9, N'R-E-CM-KG-Cinta', N'CM15263')
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (10, N'R-E-CM-KG-Rollo', N'CM15264')
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (11, N'R-E-CR-KG-Cinta', N'CR02960')
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (12, N'R-E-CR-KG-Rollo', N'CR02961')
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (13, N'R-E-CR-PC-Platina', N'CR02962')
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (14, N'R-I-CR-KG-Cinta', N'CR02963')
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (15, N'R-I-CR-PC-Platina', N'CR02964')
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (16, N'R-E-EG-KG-Rollo', N'EG06868')
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (17, N'R-E-EG-PC-Platina', N'EG06869')
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (18, N'R-I-EG-PC-Platina', N'EG06870')
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (19, N'R-I-EG-KG-Cinta', N'EG06871')
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (20, N'R-E-EG-KG-Cinta', N'EG06872')
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (21, N'R-E-GN-KG-Rollo', N'GN04231')
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (22, N'R-E-GN-PC-Platina', N'GN04232')
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (23, N'R-I-GN-PC-Platina', N'GN04233')
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (24, N'R-E-GN-KG-Cinta', N'GN04234')
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (25, N'R-I-GN-KG-Cinta', N'GN04235')
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (26, N'R-E-GO-KG-Cinta', N'GO00060')
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (27, N'R-I-GO-KG-Cinta', N'GO00061')
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (28, N'R-E-GO-KG-Rollo', N'GO00062')
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (29, N'R-E-GO-PC-Platina', N'GO00063')
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (30, N'R-I-GO-PC-Platina', N'GO00064')
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (31, N'R-E-HD-KG-Cinta', N'HD08911')
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (32, N'R-E-HD-KG-Rollo', N'HD08912')
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (33, N'R-E-HD-PC-Platina', N'HD08913')
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (34, N'R-I-HD-KG-Cinta', N'HD08914')
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (35, N'R-I-HD-PC-Platina', N'HD08915')
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (36, N'R-E-HP-KG-Cinta', N'HP02550')
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (37, N'R-E-HP-KG-Rollo', N'HP02551')
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (38, N'R-E-HP-PC-Platina', N'HP02552')
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (39, N'R-I-HP-KG-Cinta', N'HP02553')
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (40, N'R-I-HP-PC-Platina', N'HP02554')
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (41, N'R-E-HR-KG-Cinta', N'HR00820')
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (42, N'R-E-HR-KG-Rollo', N'HR00821')
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (43, N'R-E-HR-PC-Platina', N'HR00822')
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (44, N'R-I-HR-KG-Cinta', N'HR00823')
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (45, N'R-I-HR-PC-Platina', N'HR00824')
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (46, N'R-C&B-PC-PT-KIT', N'CB01725 ')
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (47, N'R-C&B-PC-PT-Pieza', N'CB01726')
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (48, N'R-C&B-KG-MP-Cobre', N'CB01367')
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (49, N'R-C&B-KG-MP-Aluminio', N'CB01367')
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (50, N'R-C&B-LB-MP-Cobre', N'CB03423')
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (51, N'R-C&B-LB-MP-Aluminio', N'CB03423')
INSERT [dbo].[SCDM_cat_material_referencia] ([id],[clave],[material_sap]) VALUES (52, N'R-C&B-LB-MP-Acero', N'CB03423')


SET IDENTITY_INSERT [dbo].[SCDM_cat_material_referencia] OFF
GO


	  
IF object_id(N'SCDM_cat_material_referencia',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< SCDM_cat_material_referencia en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla SCDM_cat_material_referencia  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
