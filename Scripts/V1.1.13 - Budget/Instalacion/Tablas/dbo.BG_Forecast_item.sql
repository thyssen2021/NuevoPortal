USE Portal_2_0
GO
IF object_id(N'BG_Forecast_item',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[BG_Forecast_item]
      PRINT '<<< BG_Forecast_item en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los datos BG_Forecast_item
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/11/09
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [BG_Forecast_item](
	[id] [int] IDENTITY(1,1) NOT NULL,	
	[id_bg_forecast_reporte][int] NOT NULL,
	[business_and_plant][VARCHAR](30) NULL,
	[calculo_activo][bit] NOT NULL DEFAULT 1,
	[sap_invoice_code][VARCHAR](12) NULL,
	[invoiced_to][VARCHAR](100) NULL,
	[number_sap_client][VARCHAR](8) NULL,
	[shipped_to][VARCHAR](100) NULL,
	[own_cm][VARCHAR](5) NULL,  --puede ser automático
	[route][VARCHAR](15) NULL,
	[plant][VARCHAR](4) NULL,
	[external_process][varchar](25) NULL DEFAULT 'NO',
	[mill][VARCHAR](100) NULL,
	[sap_master_coil][VARCHAR](12) NULL,
	[part_description][VARCHAR](100) NULL,
	[part_number][VARCHAR](100) NULL,
	[production_line][VARCHAR](15) NULL,
	[vehicle][VARCHAR](150) NULL,
	[parts_auto][DECIMAL](5,2) NULL,
	[strokes_auto][DECIMAL](5,2) NULL,
	[material_type][VARCHAR](20) NULL,
	[shape][VARCHAR](5) NULL,
	[initial_weight_part][DECIMAL](8,3) NULL,
	[net_weight_part][decimal](8,3) NULL,
	--[eng_scrap_part][DECIMAL](8,3) NULL,  --Se puede calcular 
	[scrap_consolidation][BIT] NOT NULL DEFAULT 0,
	[ventas_part][DECIMAL](6,2) NULL,
	[material_cost_part][DECIMAL](6,2) NULL,
	[cost_of_outside_processor][DECIMAL](6,2) NULL,
	--[vas_part][DECIMAL](6,2) NULL, --Se puede calcular
	[additional_material_cost_part][DECIMAL](6,2) NULL,
	[outgoing_freight_part][DECIMAL](6,2) NULL,
	[freights_income][VARCHAR](100) NULL,
	[outgoing freight][VARCHAR](100) NULL,
	[cat_1][VARCHAR](20) NULL, --PO in hand
	[cat_2][VARCHAR](20) NULL,
	[cat_3][VARCHAR](20) NULL,
	[cat_4][VARCHAR](20) NULL
 CONSTRAINT [PK_BG_Forecast_item] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

  -- restriccion de clave foranea
alter table [BG_Forecast_item]
 add constraint FK_BG_IHS_rel_combinacion_id_bg_forecast_reporte
  foreign key (id_bg_forecast_reporte)
  references BG_Forecast_reporte(id);
 	  

 -- restricion check
ALTER TABLE [BG_Forecast_item] ADD CONSTRAINT CK_BG_Forecast_item_own_cm CHECK ([own_cm] IN 
('OWN', 'CM')
)


 	  
IF object_id(N'BG_Forecast_item',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< BG_Forecast_item en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla BG_Forecast_item  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO

