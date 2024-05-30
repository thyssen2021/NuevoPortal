--USE Portal_2_0
GO
IF object_id(N'SCDM_cat_po_condiciones_pago',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[SCDM_cat_po_condiciones_pago]
      PRINT '<<< SCDM_cat_po_condiciones_pago en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los SCDM_cat_po_condiciones_pago
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[SCDM_cat_po_condiciones_pago](
	[id] [int] IDENTITY(1,1) NOT NULL,	
	[codigo][varchar](3) NOT NULL,
	[descripcion] [varchar](100) NOT NULL,
	[activo] [bit] NOT NULL,
 CONSTRAINT [PK_SCDM_cat_po_condiciones_pago_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

--Inserta valores

SET IDENTITY_INSERT [dbo].[SCDM_cat_po_condiciones_pago] ON 

GO
INSERT [dbo].[SCDM_cat_po_condiciones_pago] ([id], [activo],[codigo], [descripcion]) VALUES (1, 1, N'001',N'Due Immediately') 
INSERT [dbo].[SCDM_cat_po_condiciones_pago] ([id], [activo],[codigo], [descripcion]) VALUES (2, 1, N'002',N'Net 10 Days') 
INSERT [dbo].[SCDM_cat_po_condiciones_pago] ([id], [activo],[codigo], [descripcion]) VALUES (3, 1, N'003',N'Net 8 Days') 
INSERT [dbo].[SCDM_cat_po_condiciones_pago] ([id], [activo],[codigo], [descripcion]) VALUES (4, 1, N'010',N'Net 30 Days') 
INSERT [dbo].[SCDM_cat_po_condiciones_pago] ([id], [activo],[codigo], [descripcion]) VALUES (5, 1, N'011',N'Net 45 Days') 
INSERT [dbo].[SCDM_cat_po_condiciones_pago] ([id], [activo],[codigo], [descripcion]) VALUES (6, 1, N'012',N'Within 60 days Due net') 
INSERT [dbo].[SCDM_cat_po_condiciones_pago] ([id], [activo],[codigo], [descripcion]) VALUES (7, 1, N'013',N'Within 10 days 0.5 % cash discount Within 30 days Due net') 
INSERT [dbo].[SCDM_cat_po_condiciones_pago] ([id], [activo],[codigo], [descripcion]) VALUES (8, 1, N'014',N'Within 15 days 2 % cash discount Within 60 days Due net') 
INSERT [dbo].[SCDM_cat_po_condiciones_pago] ([id], [activo],[codigo], [descripcion]) VALUES (9, 1, N'015',N'Payable immediately Due net Baseline date on 30 of next month') 
INSERT [dbo].[SCDM_cat_po_condiciones_pago] ([id], [activo],[codigo], [descripcion]) VALUES (10, 1, N'016',N'Within 90 days Due net') 
INSERT [dbo].[SCDM_cat_po_condiciones_pago] ([id], [activo],[codigo], [descripcion]) VALUES (11, 1, N'017',N'Within 150 days Due net') 
INSERT [dbo].[SCDM_cat_po_condiciones_pago] ([id], [activo],[codigo], [descripcion]) VALUES (12, 1, N'018',N'Within 75 days Due Net') 
INSERT [dbo].[SCDM_cat_po_condiciones_pago] ([id], [activo],[codigo], [descripcion]) VALUES (13, 1, N'019',N'Within 80 days Due Net') 
INSERT [dbo].[SCDM_cat_po_condiciones_pago] ([id], [activo],[codigo], [descripcion]) VALUES (14, 1, N'020',N'Payable immediately Due net Baseline date on 15 of next month') 
INSERT [dbo].[SCDM_cat_po_condiciones_pago] ([id], [activo],[codigo], [descripcion]) VALUES (15, 1, N'021',N'Within 21 days 5 % cash discount Within 30 days Due net') 
INSERT [dbo].[SCDM_cat_po_condiciones_pago] ([id], [activo],[codigo], [descripcion]) VALUES (16, 1, N'022',N'Within 120 days Due net') 
INSERT [dbo].[SCDM_cat_po_condiciones_pago] ([id], [activo],[codigo], [descripcion]) VALUES (17, 1, N'023',N'Within 10 days 2 % cash discount Within 30 days Due net') 
INSERT [dbo].[SCDM_cat_po_condiciones_pago] ([id], [activo],[codigo], [descripcion]) VALUES (18, 1, N'024',N'second day  to the second month') 
INSERT [dbo].[SCDM_cat_po_condiciones_pago] ([id], [activo],[codigo], [descripcion]) VALUES (19, 1, N'025',N'Net 20 days due net') 
INSERT [dbo].[SCDM_cat_po_condiciones_pago] ([id], [activo],[codigo], [descripcion]) VALUES (20, 1, N'026',N'Net 15 days due net') 
INSERT [dbo].[SCDM_cat_po_condiciones_pago] ([id], [activo],[codigo], [descripcion]) VALUES (21, 1, N'027',N'Net 5 days due net') 
INSERT [dbo].[SCDM_cat_po_condiciones_pago] ([id], [activo],[codigo], [descripcion]) VALUES (22, 1, N'028',N'Net 7 days due net') 
INSERT [dbo].[SCDM_cat_po_condiciones_pago] ([id], [activo],[codigo], [descripcion]) VALUES (23, 1, N'029',N'Net 135 days due net') 
INSERT [dbo].[SCDM_cat_po_condiciones_pago] ([id], [activo],[codigo], [descripcion]) VALUES (24, 1, N'030',N'Net 180 days due net') 
INSERT [dbo].[SCDM_cat_po_condiciones_pago] ([id], [activo],[codigo], [descripcion]) VALUES (25, 1, N'031',N'Mondey to  each week') 
INSERT [dbo].[SCDM_cat_po_condiciones_pago] ([id], [activo],[codigo], [descripcion]) VALUES (26, 1, N'032',N'50% prepayment 50% delivery') 
INSERT [dbo].[SCDM_cat_po_condiciones_pago] ([id], [activo],[codigo], [descripcion]) VALUES (27, 1, N'033',N'Day 15  after 3 months') 
INSERT [dbo].[SCDM_cat_po_condiciones_pago] ([id], [activo],[codigo], [descripcion]) VALUES (28, 1, N'034',N'Within 15 days 1.917 % cash discount') 
INSERT [dbo].[SCDM_cat_po_condiciones_pago] ([id], [activo],[codigo], [descripcion]) VALUES (29, 1, N'035',N'Within 10 days 0.25 % cash discount Within 30 days Due net') 
INSERT [dbo].[SCDM_cat_po_condiciones_pago] ([id], [activo],[codigo], [descripcion]) VALUES (30, 1, N'036',N'Within 10 days 1 % cash discount Within 30 days Due net') 
INSERT [dbo].[SCDM_cat_po_condiciones_pago] ([id], [activo],[codigo], [descripcion]) VALUES (31, 1, N'037',N'Within 15 days 1.167 % cash discount') 
INSERT [dbo].[SCDM_cat_po_condiciones_pago] ([id], [activo],[codigo], [descripcion]) VALUES (32, 1, N'038',N'Within 15 days 2 % cash discount Within 30 days Due net') 
INSERT [dbo].[SCDM_cat_po_condiciones_pago] ([id], [activo],[codigo], [descripcion]) VALUES (33, 1, N'039',N'Within 55 days 1 % cash discount') 
INSERT [dbo].[SCDM_cat_po_condiciones_pago] ([id], [activo],[codigo], [descripcion]) VALUES (34, 1, N'040',N'Net 25th, Prox') 
INSERT [dbo].[SCDM_cat_po_condiciones_pago] ([id], [activo],[codigo], [descripcion]) VALUES (35, 1, N'041',N'By 15th, 10th of following month, Aft 15th, 25th By 15th, 10th of following month, Aft 15th, 25th') 
INSERT [dbo].[SCDM_cat_po_condiciones_pago] ([id], [activo],[codigo], [descripcion]) VALUES (36, 1, N'042',N'Net 82 days due net') 
INSERT [dbo].[SCDM_cat_po_condiciones_pago] ([id], [activo],[codigo], [descripcion]) VALUES (37, 1, N'043',N'Within 10 days 1 % cash discount Within 60 days Due net') 
INSERT [dbo].[SCDM_cat_po_condiciones_pago] ([id], [activo],[codigo], [descripcion]) VALUES (38, 1, N'044',N'After 15th, 31st in 2 months After 15th, 31st in 2Before 15 in 2 months Due net') 
INSERT [dbo].[SCDM_cat_po_condiciones_pago] ([id], [activo],[codigo], [descripcion]) VALUES (39, 1, N'045',N'Within 15 days 6 % cash discount Within 30 days Due net') 
INSERT [dbo].[SCDM_cat_po_condiciones_pago] ([id], [activo],[codigo], [descripcion]) VALUES (40, 1, N'046',N'33% 30 d, 33% 60d, 34% 90 days') 
INSERT [dbo].[SCDM_cat_po_condiciones_pago] ([id], [activo],[codigo], [descripcion]) VALUES (41, 1, N'050',N'Net 50 days') 

GO

SET IDENTITY_INSERT [dbo].[SCDM_cat_po_condiciones_pago] OFF
	  
IF object_id(N'SCDM_cat_po_condiciones_pago',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< SCDM_cat_po_condiciones_pago en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla SCDM_cat_po_condiciones_pago  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
