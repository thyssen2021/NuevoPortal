--USE Portal_2_0
GO
IF object_id(N'SCDM_cat_incoterm',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[SCDM_cat_incoterm]
      PRINT '<<< SCDM_cat_incoterm en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los SCDM_cat_incoterm
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[SCDM_cat_incoterm](
	[id] [int] IDENTITY(1,1) NOT NULL,	
	[codigo][varchar](3) NOT NULL,
	[descripcion] [varchar](50) NOT NULL,
	[activo] [bit] NOT NULL,
 CONSTRAINT [PK_SCDM_cat_incoterm_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

--Inserta valores

SET IDENTITY_INSERT [dbo].[SCDM_cat_incoterm] ON 

GO
INSERT [dbo].[SCDM_cat_incoterm] ([id], [activo],[codigo], [descripcion]) VALUES (1, 1, N'CFR',N'Cost Freight') 
INSERT [dbo].[SCDM_cat_incoterm] ([id], [activo],[codigo], [descripcion]) VALUES (2, 1, N'CIF',N'Cost Insurance Freight') 
INSERT [dbo].[SCDM_cat_incoterm] ([id], [activo],[codigo], [descripcion]) VALUES (3, 1, N'CIP',N'Carriage and Insurance Paid To') 
INSERT [dbo].[SCDM_cat_incoterm] ([id], [activo],[codigo], [descripcion]) VALUES (4, 1, N'CPT',N'Carriage Paid To') 
INSERT [dbo].[SCDM_cat_incoterm] ([id], [activo],[codigo], [descripcion]) VALUES (5, 1, N'DAP',N'Delivery at Place') 
INSERT [dbo].[SCDM_cat_incoterm] ([id], [activo],[codigo], [descripcion]) VALUES (6, 1, N'DAT',N'Delivery at terminal') 
INSERT [dbo].[SCDM_cat_incoterm] ([id], [activo],[codigo], [descripcion]) VALUES (7, 1, N'DDP',N'Duty Delivery Paid') 
INSERT [dbo].[SCDM_cat_incoterm] ([id], [activo],[codigo], [descripcion]) VALUES (8, 1, N'DDU',N'Duty Delivery ') 
INSERT [dbo].[SCDM_cat_incoterm] ([id], [activo],[codigo], [descripcion]) VALUES (9, 1, N'EXW',N'Ex-fábrica') 
INSERT [dbo].[SCDM_cat_incoterm] ([id], [activo],[codigo], [descripcion]) VALUES (10, 1, N'FCA',N'Free Carrier') 
INSERT [dbo].[SCDM_cat_incoterm] ([id], [activo],[codigo], [descripcion]) VALUES (11, 1, N'DAF',N'Delivery at Frontier') 

GO

SET IDENTITY_INSERT [dbo].[SCDM_cat_incoterm] OFF
	  
IF object_id(N'SCDM_cat_incoterm',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< SCDM_cat_incoterm en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla SCDM_cat_incoterm  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
