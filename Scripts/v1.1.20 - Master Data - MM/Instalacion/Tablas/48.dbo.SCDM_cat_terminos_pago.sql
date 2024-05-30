--USE Portal_2_0
GO
IF object_id(N'SCDM_cat_terminos_pago',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[SCDM_cat_terminos_pago]
      PRINT '<<< SCDM_cat_terminos_pago en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los SCDM_cat_terminos_pago
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[SCDM_cat_terminos_pago](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[clave][varchar](4) NOT NULL,
	[descripcion] [varchar](100) NOT NULL,
	[activo] [bit] NOT NULL,
 CONSTRAINT [PK_SCDM_cat_terminos_pago_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
--Inserta valores

SET IDENTITY_INSERT [dbo].[SCDM_cat_terminos_pago] ON 

GO
INSERT [dbo].[SCDM_cat_terminos_pago] ([id],[clave], [descripcion], [activo]) VALUES (1, N'0001', N'Due Immediately', 1)
INSERT [dbo].[SCDM_cat_terminos_pago] ([id],[clave], [descripcion], [activo]) VALUES (2, N'0002', N'Net 10 Days', 1)
INSERT [dbo].[SCDM_cat_terminos_pago] ([id],[clave], [descripcion], [activo]) VALUES (3, N'0003', N'Net 8 Days', 1)
INSERT [dbo].[SCDM_cat_terminos_pago] ([id],[clave], [descripcion], [activo]) VALUES (4, N'0010', N'Net 30 Days', 1)
INSERT [dbo].[SCDM_cat_terminos_pago] ([id],[clave], [descripcion], [activo]) VALUES (5, N'0011', N'Net 45 Days', 1)
INSERT [dbo].[SCDM_cat_terminos_pago] ([id],[clave], [descripcion], [activo]) VALUES (6, N'0012', N'Within 60 days Due net', 1)
INSERT [dbo].[SCDM_cat_terminos_pago] ([id],[clave], [descripcion], [activo]) VALUES (7, N'0013', N'Within 10 days 0.5 % cash discount Within 30 days Due net', 1)
INSERT [dbo].[SCDM_cat_terminos_pago] ([id],[clave], [descripcion], [activo]) VALUES (8, N'0014', N'Within 15 days 2 % cash discount Within 60 days Due net', 1)
INSERT [dbo].[SCDM_cat_terminos_pago] ([id],[clave], [descripcion], [activo]) VALUES (9, N'0015', N'Payable immediately Due net Baseline date on 30 of next month', 1)
INSERT [dbo].[SCDM_cat_terminos_pago] ([id],[clave], [descripcion], [activo]) VALUES (10, N'0016', N'Within 90 days Due net', 1)
INSERT [dbo].[SCDM_cat_terminos_pago] ([id],[clave], [descripcion], [activo]) VALUES (11, N'0017', N'Within 150 days Due net', 1)
INSERT [dbo].[SCDM_cat_terminos_pago] ([id],[clave], [descripcion], [activo]) VALUES (12, N'0018', N'Within 75 days Due Net', 1)
INSERT [dbo].[SCDM_cat_terminos_pago] ([id],[clave], [descripcion], [activo]) VALUES (13, N'0019', N'Within 80 days Due Net', 1)
INSERT [dbo].[SCDM_cat_terminos_pago] ([id],[clave], [descripcion], [activo]) VALUES (14, N'0020', N'Payable immediately Due net Baseline date on 15 of next month', 1)
INSERT [dbo].[SCDM_cat_terminos_pago] ([id],[clave], [descripcion], [activo]) VALUES (15, N'0021', N'Within 21 days 5 % cash discount Within 30 days Due net', 1)
INSERT [dbo].[SCDM_cat_terminos_pago] ([id],[clave], [descripcion], [activo]) VALUES (16, N'0022', N'Within 120 days Due net', 1)
INSERT [dbo].[SCDM_cat_terminos_pago] ([id],[clave], [descripcion], [activo]) VALUES (17, N'0023', N'Within 10 days 2 % cash discount Within 30 days Due net', 1)
INSERT [dbo].[SCDM_cat_terminos_pago] ([id],[clave], [descripcion], [activo]) VALUES (18, N'0024', N'second day  to the second month', 1)
INSERT [dbo].[SCDM_cat_terminos_pago] ([id],[clave], [descripcion], [activo]) VALUES (19, N'0025', N'Net 20 days due net', 1)
INSERT [dbo].[SCDM_cat_terminos_pago] ([id],[clave], [descripcion], [activo]) VALUES (20, N'0026', N'Net 15 days due net', 1)
INSERT [dbo].[SCDM_cat_terminos_pago] ([id],[clave], [descripcion], [activo]) VALUES (21, N'0027', N'Net 5 days due net', 1)
INSERT [dbo].[SCDM_cat_terminos_pago] ([id],[clave], [descripcion], [activo]) VALUES (22, N'0028', N'Net 7 days due net', 1)
INSERT [dbo].[SCDM_cat_terminos_pago] ([id],[clave], [descripcion], [activo]) VALUES (23, N'0029', N'Net 135 days due net', 1)
INSERT [dbo].[SCDM_cat_terminos_pago] ([id],[clave], [descripcion], [activo]) VALUES (24, N'0030', N'Net 180 days due net', 1)
INSERT [dbo].[SCDM_cat_terminos_pago] ([id],[clave], [descripcion], [activo]) VALUES (25, N'0031', N'Mondey to  each week', 1)
INSERT [dbo].[SCDM_cat_terminos_pago] ([id],[clave], [descripcion], [activo]) VALUES (26, N'0032', N'50% prepayment 50% delivery', 1)
INSERT [dbo].[SCDM_cat_terminos_pago] ([id],[clave], [descripcion], [activo]) VALUES (27, N'0033', N'Day 15  after 3 months', 1)
INSERT [dbo].[SCDM_cat_terminos_pago] ([id],[clave], [descripcion], [activo]) VALUES (28, N'0034', N'Within 15 days 1.917 % cash discount', 1)
INSERT [dbo].[SCDM_cat_terminos_pago] ([id],[clave], [descripcion], [activo]) VALUES (29, N'0035', N'Within 10 days 0.25 % cash discount Within 30 days Due net', 1)
INSERT [dbo].[SCDM_cat_terminos_pago] ([id],[clave], [descripcion], [activo]) VALUES (30, N'0036', N'Within 10 days 1 % cash discount Within 30 days Due net', 1)
INSERT [dbo].[SCDM_cat_terminos_pago] ([id],[clave], [descripcion], [activo]) VALUES (31, N'0037', N'Within 15 days 1.167 % cash discount', 1)
INSERT [dbo].[SCDM_cat_terminos_pago] ([id],[clave], [descripcion], [activo]) VALUES (32, N'0038', N'Within 15 days 2 % cash discount Within 30 days Due net', 1)
INSERT [dbo].[SCDM_cat_terminos_pago] ([id],[clave], [descripcion], [activo]) VALUES (33, N'0039', N'Within 55 days 1 % cash discount', 1)
INSERT [dbo].[SCDM_cat_terminos_pago] ([id],[clave], [descripcion], [activo]) VALUES (34, N'0040', N'Net 25th, Prox', 1)
INSERT [dbo].[SCDM_cat_terminos_pago] ([id],[clave], [descripcion], [activo]) VALUES (35, N'0041', N'By 15th, 10th of following month, Aft 15th, 25th By 15th, 10th of following month, Aft 15th, 25th', 1)
INSERT [dbo].[SCDM_cat_terminos_pago] ([id],[clave], [descripcion], [activo]) VALUES (36, N'0042', N'Net 82 days due net', 1)
INSERT [dbo].[SCDM_cat_terminos_pago] ([id],[clave], [descripcion], [activo]) VALUES (37, N'0043', N'Within 10 days 1 % cash discount Within 60 days Due net', 1)
INSERT [dbo].[SCDM_cat_terminos_pago] ([id],[clave], [descripcion], [activo]) VALUES (38, N'0044', N'After 15th, 31st in 2 months After 15th, 31st in 2Before 15 in 2 months Due net', 1)
INSERT [dbo].[SCDM_cat_terminos_pago] ([id],[clave], [descripcion], [activo]) VALUES (39, N'0045', N'Within 15 days 6 % cash discount Within 30 days Due net', 1)
INSERT [dbo].[SCDM_cat_terminos_pago] ([id],[clave], [descripcion], [activo]) VALUES (40, N'0046', N'33% 30 d, 33% 60d, 34% 90 days', 1)


SET IDENTITY_INSERT [dbo].[SCDM_cat_terminos_pago] OFF
GO


	  
IF object_id(N'SCDM_cat_terminos_pago',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< SCDM_cat_terminos_pago en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla SCDM_cat_terminos_pago  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
