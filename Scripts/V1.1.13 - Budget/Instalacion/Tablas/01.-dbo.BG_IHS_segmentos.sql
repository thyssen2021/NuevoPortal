--USE Portal_2_0
GO
IF object_id(N'BG_IHS_segmentos',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[BG_IHS_segmentos]
      PRINT '<<< BG_IHS_segmentos en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los BG_IHS_segmentos
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/10/06
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [BG_IHS_segmentos](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_ihs_version] [int] NOT NULL,
	[global_production_segment] [varchar](30) NOT NULL,
	[flat_rolled_steel_usage] [int]  NULL,
	[blanks_usage_percent] [decimal](3,2)  NULL,
 CONSTRAINT [PK_BG_IHS_segmentos] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

 -- restriccion de clave foranea
alter table [BG_IHS_segmentos]
 add constraint FK_BG_IHS_segmentos_id_ihs_version
  foreign key (id_ihs_version)
  references BG_IHS_versiones(id);


--SET IDENTITY_INSERT [BG_IHS_segmentos] ON 

--INSERT [BG_IHS_segmentos] ([id], [global_production_segment], [flat_rolled_steel_usage], [blanks_usage_percent]) VALUES (1, N'A-Segment', 650,0.75)
--INSERT [BG_IHS_segmentos] ([id], [global_production_segment], [flat_rolled_steel_usage], [blanks_usage_percent]) VALUES (2, N'B-Segment', 720,0.75)
--INSERT [BG_IHS_segmentos] ([id], [global_production_segment], [flat_rolled_steel_usage], [blanks_usage_percent]) VALUES (3, N'Compact Full-Frame', 800,0.75)
--INSERT [BG_IHS_segmentos] ([id], [global_production_segment], [flat_rolled_steel_usage], [blanks_usage_percent]) VALUES (4, N'C-Segment', 800,0.75)
--INSERT [BG_IHS_segmentos] ([id], [global_production_segment], [flat_rolled_steel_usage], [blanks_usage_percent]) VALUES (5, N'D-Segment', 850,0.75)
--INSERT [BG_IHS_segmentos] ([id], [global_production_segment], [flat_rolled_steel_usage], [blanks_usage_percent]) VALUES (6, N'E-Segment', 900,0.75)
--INSERT [BG_IHS_segmentos] ([id], [global_production_segment], [flat_rolled_steel_usage], [blanks_usage_percent]) VALUES (7, N'Full-Size Full-Frame', 1000,0.75)

--SET IDENTITY_INSERT [BG_IHS_segmentos] OFF
GO

 	  
IF object_id(N'BG_IHS_segmentos',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< BG_IHS_segmentos en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla BG_IHS_segmentos  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END	
SET ANSI_PADDING OFF
GO
