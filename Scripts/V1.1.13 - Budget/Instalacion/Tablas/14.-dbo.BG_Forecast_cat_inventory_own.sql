--USE Portal_2_0_budget
GO
IF object_id(N'BG_Forecast_cat_inventory_own',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[BG_Forecast_cat_inventory_own]
      PRINT '<<< BG_Forecast_cat_inventory_own en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los datos BG_Forecast_cat_inventory_own
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/11/09
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [BG_Forecast_cat_inventory_own](
	[id] [int] IDENTITY(1,1) NOT NULL,	
	[id_bg_forecast_reporte][int] NOT NULL,
	[orden][int] NOT NULL,
	[mes][int] NOT NULL,
	[cantidad][FLOAT] NOT NULL,

 CONSTRAINT [PK_BG_Forecast_cat_inventory_own] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

  -- restriccion de clave foranea
alter table [BG_Forecast_cat_inventory_own]
 add constraint FK_BG_Forecast_cat_inventory_own_id_bg_forecast_reporte
  foreign key (id_bg_forecast_reporte)
  references BG_Forecast_reporte(id); 	  

 	  
IF object_id(N'BG_Forecast_cat_inventory_own',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< BG_Forecast_cat_inventory_own en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla BG_Forecast_cat_inventory_own  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
