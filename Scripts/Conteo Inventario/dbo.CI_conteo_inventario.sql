--USE Portal_2_0
GO
IF object_id(N'CI_conteo_inventario',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[CI_conteo_inventario]
      PRINT '<<< CI_conteo_inventario en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los CI_conteo_inventario
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/04/18
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [CI_conteo_inventario](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[plant] [varchar](4) NOT NULL,
	[storage_location] [varchar](4) NOT NULL,
	[storage_bin] [varchar](20) NULL,
	[batch] [varchar](4) NULL,
	[ship_to_number] [varchar](12) NULL,
	[material] [varchar](12) NULL,
	[material_description] [varchar](40) NULL,
	[ihs_number] [varchar](120) NULL,
	[pieces] [int] NULL,
	[unrestricted] [int] NULL,
	[blocked] [int] NULL,
	[in_quality] [int] NULL,
	[value_stock][float] NULL,
	[base_unit_measure] [varchar](10) NULL,
	[gauge][float] NULL,
	[gauge_min][float] NULL,
	[gauge_max][float] NULL,
	[altura][float] NULL,
	[espesor][float] NULL,
	[num_tarima][int] NULL,
	
 CONSTRAINT [PK_CI_conteo_inventario] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restricción default
--ALTER TABLE [CI_conteo_inventario] ADD  CONSTRAINT [DF_CI_conteo_inventario_activo]  DEFAULT (1) FOR [activo]
GO


SET IDENTITY_INSERT [CI_conteo_inventario] ON 

SET IDENTITY_INSERT [CI_conteo_inventario] OFF
 	  
IF object_id(N'CI_conteo_inventario',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< CI_conteo_inventario en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla CI_conteo_inventario  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
