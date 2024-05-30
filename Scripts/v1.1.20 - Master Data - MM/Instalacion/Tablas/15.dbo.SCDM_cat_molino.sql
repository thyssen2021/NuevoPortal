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
	[clave][varchar](3) NOT NULL,
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

 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (1, 1, N'TKS', '01') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (2, 1, N'Others', '02') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (3, 1, N'Foreign', '03') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (4, 1, N'US Steel-Gary', '04') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (5, 1, N'US Steel-Midwest', '05') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (6, 1, N'US Steel-Protec', '06') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (7, 1, N'AK Steel', '07') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (8, 1, N'Inland Steel', '08') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (9, 1, N'Severstal - Rouge', '09') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (10, 1, N'ISG - Bethlehem', '10') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (11, 1, N'US Steel-Great Lakes', '11') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (12, 1, N'US Steel-Fairless Hills', '12') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (13, 1, N'Posco', '13') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (14, 1, N'AHMSA', '14') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (15, 1, N'APM', '15') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (16, 1, N'Arcelor', '16') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (17, 1, N'Galvak', '17') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (18, 1, N'IMSA', '18') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (19, 1, N'USIMINAS', '19') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (20, 1, N'GALVSUD', '20') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (21, 1, N'TAGAL', '21') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (22, 1, N'NIPPON', '22') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (23, 1, N'JFE', '23') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (24, 1, N'KOBE', '24') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (25, 1, N'Metalone', '25') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (26, 1, N'Marubeni', '26') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (27, 1, N'Richburg', '27') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (28, 1, N'Mill Steel Company, Grand Rapi', '28') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (29, 1, N'US Steel -  Processed Products', '29') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (30, 1, N'China Steel Corporation', '30') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (31, 1, N'Ansteel', '31') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (32, 1, N'Corus', '32') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (33, 1, N'AM USA Inc. (IH-E) - ZMITTAL', '33') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (34, 1, N'AM Kote Inc - ZMITTAL', '34') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (35, 1, N'AM Burns Harbor - ZMITTAL', '35') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (36, 1, N'Ternium', '36') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (37, 1, N'AM USA Inc.(IH-E)', '37') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (38, 1, N'AM Kote', '38') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (39, 1, N'AM Burns Harbor', '39') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (40, 1, N'ArcelorMittal Indiana Harbor', '40') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (41, 1, N'ArcelorMittal Lackawanna LLC', '41') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (42, 1, N'GM', '42') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (43, 1, N'GM C/O Nucor', '43') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (44, 1, N'GM C/O Severstal', '44') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (45, 1, N'GM C/O U.S. Steel', '45') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (46, 1, N'GM C/O Mittal', '46') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (47, 1, N'GM C/O AK Steel', '47') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (48, 1, N'TKSNA', '48') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (49, 1, N'Buderus', '49') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (50, 1, N'HYSCO', '50') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (51, 1, N'TKS USA LLC', '51') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (52, 1, N'NUCOR', '52') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (53, 1, N'HHO', '53') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (54, 1, N'TKRAS', '54') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (55, 1, N'TKES', '55') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (56, 1, N'Posco-Mexico', '56') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (57, 1, N'AM/NS Calvert: Supplier 553735', '57') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (58, 1, N'SSM', '58') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (59, 1, N'Tenigal', '59') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (60, 1, N'Bao Steel', '60') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (61, 1, N'Shougang', '61') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (62, 1, N'Panhua Group', '62') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (63, 1, N'Dofasco', '63') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (64, 1, N'Salzgitter', '64') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (65, 1, N'Horizon Steel', '65') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (66, 1, N'Ken Mac', '66') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (67, 1, N'Copper & Brass', '67') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (68, 1, N'TK Schulte', '68') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (69, 1, N'CSN', '69') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (70, 1, N'TATA STEEL', '70') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (71, 1, N'LEE STEEL', '71') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (72, 1, N'HTMX', '72') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (73, 1, N'US Steel-Spartan', '73') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (74, 1, N'VENTURE', '74') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (75, 1, N'HOA SEN', '75') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (76, 1, N'Voestalpine', '76') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (77, 1, N'SSAB', '77') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (78, 1, N'ALERIS', '78') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (79, 1, N'NOVELIS', '79') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (80, 1, N'Hydro Extrusion', '80') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (81, 1, N'REA Magnet', '81') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (82, 1, N'SAPA', '82') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (83, 1, N'Tecnofil', '83') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (84, 1, N'TKMNA', '84') 
 INSERT [dbo].[SCDM_cat_molino] ([id], [activo], [descripcion], [clave]) VALUES (85, 1, N'Novelis Korea', '85') 


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
