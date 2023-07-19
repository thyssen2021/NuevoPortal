--USE Portal_2_0
GO
IF object_id(N'SCDM_cat_peso_recubrimiento',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[SCDM_cat_peso_recubrimiento]
      PRINT '<<< SCDM_cat_peso_recubrimiento en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los SCDM_cat_peso_recubrimiento
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[SCDM_cat_peso_recubrimiento](
	[id] [int] IDENTITY(1,1) NOT NULL,	
	[descripcion] [varchar](50) NOT NULL,
	[activo] [bit] NOT NULL,
 CONSTRAINT [PK_SCDM_cat_peso_recubrimiento_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

--Inserta valores

SET IDENTITY_INSERT [dbo].[SCDM_cat_peso_recubrimiento] ON 

GO

 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (1, 1, N'45g/45g') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (2, 1, N'50g/50g') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (3, 1, N'60g/60g') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (4, 1, N'60g/0') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (5, 1, N'70g/70g') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (6, 1, N'70g/0') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (7, 1, N'75g/75g') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (8, 1, N'90g/90g') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (9, 1, N'98g/98g') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (10, 1, N'T125') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (11, 1, N'7.5 micrometer/7.5 micrometer') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (12, 1, N'6M44A') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (13, 1, N'20g/20g') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (14, 1, N'30g/30g') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (15, 1, N'T140') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (16, 1, N'98g/60g') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (17, 1, N'90g/60g') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (18, 1, N'60g/90g') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (19, 1, N'45g/75g') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (20, 1, N'75g/0') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (21, 1, N'44g/44g') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (22, 1, N'55g/55g') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (23, 1, N'25g/25g') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (24, 1, N'53g/53g') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (25, 1, N'36g/36g') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (26, 1, N'Z100') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (27, 1, N'Z75') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (28, 1, N'100g/100g') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (29, 1, N'72/72') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (30, 1, N'001,12 / 001,12') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (31, 1, N'001,12 / 002,80') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (32, 1, N'001,12 / 004,48') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (33, 1, N'002,20 / 001,10') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (34, 1, N'002,20 / 002,20') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (35, 1, N'002,24 / 001,12') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (36, 1, N'002,24 / 002,24') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (37, 1, N'002,24 / 005,60') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (38, 1, N'002,80 / 002,80') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (39, 1, N'002,80 / 004,48') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (40, 1, N'003,92 / 003,92') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (41, 1, N'004,00 / 004,00') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (42, 1, N'005,60 / 005,60') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (43, 1, N'008,40 / 002,80') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (44, 1, N'008,40 / 008,40') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (45, 1, N'011,20 / 011,20') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (46, 1, N'.20/.30') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (47, 1, N'.20/0') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (48, 1, N'.20/.15') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (49, 1, N'.10/.20') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (50, 1, N'.25/.10') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (51, 1, N'.35/.35') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (52, 1, N'56/56') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (53, 1, N'.40/.10') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (54, 1, N'.25/.75') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (55, 1, N'.50/.50') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (56, 1, N'.75/.25') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (57, 1, N'20/20') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (58, 1, N'.25/.25') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (59, 1, N'.25/.40') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (60, 1, N'.75/.75') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (61, 1, N'C-5 Coated') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (62, 1, N'.10/.10') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (63, 1, N'1.0/1.0') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (64, 1, N'.20/.50') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (65, 1, N'.50/.20') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (66, 1, N'5.0/5.0') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (67, 1, N'80G/80G') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (68, 1, N'C-6 Coated') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion]) VALUES (69, 1, N'120/120') 
 


GO

SET IDENTITY_INSERT [dbo].[SCDM_cat_peso_recubrimiento] OFF
	  
IF object_id(N'SCDM_cat_peso_recubrimiento',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< SCDM_cat_peso_recubrimiento en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla SCDM_cat_peso_recubrimiento  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
