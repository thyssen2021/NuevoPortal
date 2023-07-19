--USE Portal_2_0
GO
IF object_id(N'SCDM_cat_molino',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[SCDM_cat_molino]
      PRINT '<<< SCDM_cat_molino en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los SCDM_cat_molino
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[SCDM_cat_molino](
	[id] [int] IDENTITY(1,1) NOT NULL,	
	[descripcion] [varchar](50) NOT NULL,
	[activo] [bit] NOT NULL,
 CONSTRAINT [PK_SCDM_cat_molino_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

--Inserta valores

SET IDENTITY_INSERT [dbo].[SCDM_cat_molino] ON 

GO
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (1, 1, N'TKS') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (2, 1, N'Others') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (3, 1, N'Foreign') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (4, 1, N'US Steel-Gary') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (5, 1, N'US Steel-Midwest') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (6, 1, N'US Steel-Protec') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (7, 1, N'AK Steel') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (8, 1, N'Inland Steel') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (9, 1, N'Severstal - Rouge') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (10, 1, N'ISG - Bethlehem') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (11, 1, N'US Steel-Great Lakes') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (12, 1, N'US Steel-Fairless Hills') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (13, 1, N'Posco') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (14, 1, N'AHMSA') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (15, 1, N'APM') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (16, 1, N'Arcelor') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (17, 1, N'Galvak') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (18, 1, N'IMSA') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (19, 1, N'USIMINAS') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (20, 1, N'GALVSUD') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (21, 1, N'TAGAL') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (22, 1, N'NIPPON') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (23, 1, N'JFE') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (24, 1, N'KOBE') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (25, 1, N'Metalone') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (26, 1, N'Marubeni') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (27, 1, N'Richburg') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (28, 1, N'Mill Steel Company, Grand Rapi') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (29, 1, N'US Steel -  Processed Products') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (30, 1, N'China Steel Corporation') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (31, 1, N'Ansteel') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (32, 1, N'Corus') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (33, 1, N'AM USA Inc. (IH-E) - ZMITTAL') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (34, 1, N'AM Kote Inc - ZMITTAL') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (35, 1, N'AM Burns Harbor - ZMITTAL') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (36, 1, N'Ternium') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (37, 1, N'AM USA Inc.(IH-E)') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (38, 1, N'AM Kote') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (39, 1, N'AM Burns Harbor') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (40, 1, N'ArcelorMittal Indiana Harbor') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (41, 1, N'ArcelorMittal Lackawanna LLC') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (42, 1, N'GM') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (43, 1, N'GM C/O Nucor') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (44, 1, N'GM C/O Severstal') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (45, 1, N'GM C/O U.S. Steel') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (46, 1, N'GM C/O Mittal') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (47, 1, N'GM C/O AK Steel') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (48, 1, N'TKSNA') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (49, 1, N'Buderus') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (50, 1, N'HYSCO') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (51, 1, N'TKS USA LLC') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (52, 1, N'NUCOR') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (53, 1, N'HHO') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (54, 1, N'TKRAS') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (55, 1, N'TKES') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (56, 1, N'Posco-Mexico') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (57, 1, N'AM/NS Calvert: Supplier 553735') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (58, 1, N'SSM') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (59, 1, N'Tenigal') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (60, 1, N'Bao Steel') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (61, 1, N'Shougang') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (62, 1, N'Panhua Group') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (63, 1, N'Dofasco') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (64, 1, N'Salzgitter') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (65, 1, N'Horizon Steel') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (66, 1, N'Ken Mac') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (67, 1, N'Copper & Brass') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (68, 1, N'TK Schulte') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (69, 1, N'CSN') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (70, 1, N'TATA STEEL') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (71, 1, N'LEE STEEL') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (72, 1, N'HTMX') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (73, 1, N'US Steel-Spartan') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (74, 1, N'VENTURE') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (75, 1, N'HOA SEN') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (76, 1, N'Voestalpine') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (77, 1, N'SSAB') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (78, 1, N'ALERIS') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (79, 1, N'NOVELIS') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (80, 1, N'Hydro Extrusion') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (81, 1, N'REA Magnet') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (82, 1, N'SAPA') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (83, 1, N'Tecnofil') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (84, 1, N'TKMNA') 
 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion]) VALUES (85, 1, N'Novelis Korea') 
 

GO

SET IDENTITY_INSERT [dbo].[SCDM_cat_molino] OFF
	  
IF object_id(N'SCDM_cat_molino',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< SCDM_cat_molino en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla SCDM_cat_molino  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
