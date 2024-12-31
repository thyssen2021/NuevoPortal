--USE Portal_2_0
GO
IF object_id(N'BG_IHS_rel_regiones',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[BG_IHS_rel_regiones]
      PRINT '<<< BG_IHS_rel_regiones en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los BG_IHS_rel_regiones
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/08/17
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [BG_IHS_rel_regiones](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_ihs_version] [int] NOT NULL,
	[id_bg_ihs_region][int] NULL,
	[id_bg_ihs_plant][int] NOT NULL,
 CONSTRAINT [PK_BG_IHS_rel_regiones] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

 -- restriccion de clave foranea
alter table [BG_IHS_rel_regiones]
 add constraint FK_BG_IHS_rel_regiones_id_ihs_version
  foreign key (id_ihs_version)
  references BG_IHS_versiones(id);

-- restriccion de clave foranea
alter table [BG_IHS_rel_regiones]
 add constraint FK_BG_IHS_rel_regiones_id_ihs_region
  foreign key (id_bg_ihs_region)
  references BG_IHS_regiones(id);

  -- restriccion de clave foranea
alter table [BG_IHS_rel_regiones]
 add constraint FK_BG_IHS_rel_regiones_id_bg_ihs_plant
  foreign key (id_bg_ihs_plant)
  references BG_IHS_plantas(id);

  --agregar los elementos a esta tabla denámicamente
 -- SET IDENTITY_INSERT [BG_IHS_rel_regiones] ON 

	--INSERT [BG_IHS_rel_regiones] ([id], [id_bg_ihs_region], [production_plant]) VALUES (1, 1, N'Aguascalientes (A1)')
	--INSERT [BG_IHS_rel_regiones] ([id], [id_bg_ihs_region], [production_plant]) VALUES (2, 1, N'Aguascalientes (A2)')
	--INSERT [BG_IHS_rel_regiones] ([id], [id_bg_ihs_region], [production_plant]) VALUES (3, 1, N'Celaya')
	--INSERT [BG_IHS_rel_regiones] ([id], [id_bg_ihs_region], [production_plant]) VALUES (4, 2, N'Cuautitlan')
	--INSERT [BG_IHS_rel_regiones] ([id], [id_bg_ihs_region], [production_plant]) VALUES (5, 2, N'Cuernavaca #1')
	--INSERT [BG_IHS_rel_regiones] ([id], [id_bg_ihs_region], [production_plant]) VALUES (6, 2, N'Cuernavaca #2')
	--INSERT [BG_IHS_rel_regiones] ([id], [id_bg_ihs_region], [production_plant]) VALUES (7, 1, N'El Salto')
	--INSERT [BG_IHS_rel_regiones] ([id], [id_bg_ihs_region], [production_plant]) VALUES (8, 3, N'Hermosillo')
	--INSERT [BG_IHS_rel_regiones] ([id], [id_bg_ihs_region], [production_plant]) VALUES (9, 4, N'Monterrey')
	--INSERT [BG_IHS_rel_regiones] ([id], [id_bg_ihs_region], [production_plant]) VALUES (10, 2, N'Puebla #1')
	--INSERT [BG_IHS_rel_regiones] ([id], [id_bg_ihs_region], [production_plant]) VALUES (11, 4, N'Ramos Arizpe #2')
	--INSERT [BG_IHS_rel_regiones] ([id], [id_bg_ihs_region], [production_plant]) VALUES (12, 1, N'Salamanca')
	--INSERT [BG_IHS_rel_regiones] ([id], [id_bg_ihs_region], [production_plant]) VALUES (13, 1, N'Guanajuato')
	--INSERT [BG_IHS_rel_regiones] ([id], [id_bg_ihs_region], [production_plant]) VALUES (14, 4, N'Saltillo Truck')
	--INSERT [BG_IHS_rel_regiones] ([id], [id_bg_ihs_region], [production_plant]) VALUES (15, 4, N'Saltillo Van')
	--INSERT [BG_IHS_rel_regiones] ([id], [id_bg_ihs_region], [production_plant]) VALUES (16, 2, N'San Jose Chiapa')
	--INSERT [BG_IHS_rel_regiones] ([id], [id_bg_ihs_region], [production_plant]) VALUES (17, 1, N'San Luis Potosi')
	--INSERT [BG_IHS_rel_regiones] ([id], [id_bg_ihs_region], [production_plant]) VALUES (18, 1, N'Silao')
	--INSERT [BG_IHS_rel_regiones] ([id], [id_bg_ihs_region], [production_plant]) VALUES (19, 3, N'Tijuana')
	--INSERT [BG_IHS_rel_regiones] ([id], [id_bg_ihs_region], [production_plant]) VALUES (20, 2, N'Toluca')
	--INSERT [BG_IHS_rel_regiones] ([id], [id_bg_ihs_region], [production_plant]) VALUES (21, 2, N'Sahagun')
	--INSERT [BG_IHS_rel_regiones] ([id], [id_bg_ihs_region], [production_plant]) VALUES (22, 1, N'Aguascalientes')
	--INSERT [BG_IHS_rel_regiones] ([id], [id_bg_ihs_region], [production_plant]) VALUES (23, 5, N'Sterling Heights')
	--INSERT [BG_IHS_rel_regiones] ([id], [id_bg_ihs_region], [production_plant]) VALUES (24, 5, N'Ingersoll #2')
	--INSERT [BG_IHS_rel_regiones] ([id], [id_bg_ihs_region], [production_plant]) VALUES (25, 5, N'Fremont')
	--INSERT [BG_IHS_rel_regiones] ([id], [id_bg_ihs_region], [production_plant]) VALUES (26, 5, N'Ridgeville')
	--INSERT [BG_IHS_rel_regiones] ([id], [id_bg_ihs_region], [production_plant]) VALUES (27, 5, N'Louisville')
	--INSERT [BG_IHS_rel_regiones] ([id], [id_bg_ihs_region], [production_plant]) VALUES (28, 5, N'Chattanooga')
	--INSERT [BG_IHS_rel_regiones] ([id], [id_bg_ihs_region], [production_plant]) VALUES (29, 5, N'Spartanburg North')
	--INSERT [BG_IHS_rel_regiones] ([id], [id_bg_ihs_region], [production_plant]) VALUES (30, 5, N'Warren Truck')
	--INSERT [BG_IHS_rel_regiones] ([id], [id_bg_ihs_region], [production_plant]) VALUES (31, 5, N'Cami')
	--INSERT [BG_IHS_rel_regiones] ([id], [id_bg_ihs_region], [production_plant]) VALUES (32, 5, N'Charleston')
	--INSERT [BG_IHS_rel_regiones] ([id], [id_bg_ihs_region], [production_plant]) VALUES (33, 5, N'Tuscaloosa #1')
	--INSERT [BG_IHS_rel_regiones] ([id], [id_bg_ihs_region], [production_plant]) VALUES (34, 5, N'Gigafactory Texas')
	--INSERT [BG_IHS_rel_regiones] ([id], [id_bg_ihs_region], [production_plant]) VALUES (35, 5, N'Orion')
	--INSERT [BG_IHS_rel_regiones] ([id], [id_bg_ihs_region], [production_plant]) VALUES (36, 5, N'Spartanburg')
	--INSERT [BG_IHS_rel_regiones] ([id], [id_bg_ihs_region], [production_plant]) VALUES (37, 5, N'CAMI #2')
	--INSERT [BG_IHS_rel_regiones] ([id], [id_bg_ihs_region], [production_plant]) VALUES (38, 5, N'Gigafactory Texas #2')
	--INSERT [BG_IHS_rel_regiones] ([id], [id_bg_ihs_region], [production_plant]) VALUES (39, 5, N'Gigafactory Texas #1')
	--INSERT [BG_IHS_rel_regiones] ([id], [id_bg_ihs_region], [production_plant]) VALUES (40, 5, N'Gigafactory Texas #1')
	--INSERT [BG_IHS_rel_regiones] ([id], [id_bg_ihs_region], [production_plant]) VALUES (41, 5, N'Gigafactory Texas #1')
	--INSERT [BG_IHS_rel_regiones] ([id], [id_bg_ihs_region], [production_plant]) VALUES (42, 5, N'Fremont #1')
	--INSERT [BG_IHS_rel_regiones] ([id], [id_bg_ihs_region], [production_plant]) VALUES (43, 5, N'Fremont #2')
	--INSERT [BG_IHS_rel_regiones] ([id], [id_bg_ihs_region], [production_plant]) VALUES (44, 5, N'Chattanooga #1')

	--SET IDENTITY_INSERT [BG_IHS_rel_regiones] OFF
GO


 	  
IF object_id(N'BG_IHS_rel_regiones',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< BG_IHS_rel_regiones en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla BG_IHS_rel_regiones  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
