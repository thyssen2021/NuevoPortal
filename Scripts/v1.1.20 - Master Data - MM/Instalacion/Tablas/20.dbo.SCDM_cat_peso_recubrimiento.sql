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
	[clave][varchar](3) NOT NULL,
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

 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (1, 1, N'45g/45g', '01') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (2, 1, N'50g/50g', '02') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (3, 1, N'60g/60g', '03') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (4, 1, N'60g/0', '04') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (5, 1, N'70g/70g', '05') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (6, 1, N'70g/0', '06') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (7, 1, N'75g/75g', '07') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (8, 1, N'90g/90g', '08') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (9, 1, N'98g/98g', '09') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (10, 1, N'T125', '10') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (11, 1, N'7.5 micrometer/7.5 micrometer', '11') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (12, 1, N'6M44A', '12') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (13, 1, N'20g/20g', '13') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (14, 1, N'30g/30g', '14') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (15, 1, N'T140', '15') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (16, 1, N'98g/60g', '16') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (17, 1, N'90g/60g', '17') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (18, 1, N'60g/90g', '18') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (19, 1, N'45g/75g', '19') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (20, 1, N'75g/0', '20') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (21, 1, N'44g/44g', '21') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (22, 1, N'55g/55g', '22') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (23, 1, N'25g/25g', '23') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (24, 1, N'53g/53g', '24') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (25, 1, N'36g/36g', '25') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (26, 1, N'Z100', '26') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (27, 1, N'Z75', '27') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (28, 1, N'100g/100g', '28') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (29, 1, N'72/72', '29') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (30, 1, N'001,12 / 001,12', '30') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (31, 1, N'001,12 / 002,80', '31') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (32, 1, N'001,12 / 004,48', '32') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (33, 1, N'002,20 / 001,10', '33') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (34, 1, N'002,20 / 002,20', '34') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (35, 1, N'002,24 / 001,12', '35') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (36, 1, N'002,24 / 002,24', '36') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (37, 1, N'002,24 / 005,60', '37') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (38, 1, N'002,80 / 002,80', '38') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (39, 1, N'002,80 / 004,48', '39') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (40, 1, N'003,92 / 003,92', '40') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (41, 1, N'004,00 / 004,00', '41') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (42, 1, N'005,60 / 005,60', '42') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (43, 1, N'008,40 / 002,80', '43') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (44, 1, N'008,40 / 008,40', '44') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (45, 1, N'011,20 / 011,20', '45') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (46, 1, N'.20/.30', '46') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (47, 1, N'.20/0', '47') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (48, 1, N'.20/.15', '48') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (49, 1, N'.10/.20', '49') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (50, 1, N'.25/.10', '50') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (51, 1, N'.35/.35', '51') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (52, 1, N'56/56', '52') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (53, 1, N'.40/.10', '53') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (54, 1, N'.25/.75', '54') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (55, 1, N'.50/.50', '55') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (56, 1, N'.75/.25', '56') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (57, 1, N'20/20', '57') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (58, 1, N'.25/.25', '58') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (59, 1, N'.25/.40', '59') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (60, 1, N'.75/.75', '60') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (61, 1, N'C-5 Coated', '61') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (62, 1, N'.10/.10', '62') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (63, 1, N'1.0/1.0', '63') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (64, 1, N'.20/.50', '64') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (65, 1, N'.50/.20', '65') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (66, 1, N'5.0/5.0', '66') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (67, 1, N'80G/80G', '67') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (68, 1, N'C-6 Coated', '68') 
 
 INSERT [dbo].[SCDM_cat_peso_recubrimiento] ([id], [activo], [descripcion], [clave]) VALUES (69, 1, N'120/120', '69') 
 


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
