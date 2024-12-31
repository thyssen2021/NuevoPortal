--USE Portal_2_0
GO
IF object_id(N'BG_IHS_item',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[BG_IHS_item]
      PRINT '<<< BG_IHS_item en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los datos BG_IHS_item
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/09/16
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [BG_IHS_item](
	[id] [int] IDENTITY(1,1) NOT NULL,	
	[id_ihs_version] [int] NOT NULL,
	[core_nameplate_region_mnemonic][VARCHAR](15)  NULL,
	[core_nameplate_plant_mnemonic][VARCHAR](15)  NULL,
	[mnemonic_vehicle][VARCHAR](15)	 NULL,
	[mnemonic_vehicle_plant][VARCHAR](15) NOT NULL,  --UNIQUE
	[mnemonic_platform][VARCHAR](15)  NULL,
	[mnemonic_plant][VARCHAR](15)  NULL,
	[region][VARCHAR](25)  NULL,
	[market][VARCHAR](10)  NULL,
	[country_territory][VARCHAR](30)  NULL,
	[production_plant][VARCHAR](50)  NULL,
	[city][VARCHAR](50)  NULL,
	[plant_state_province][VARCHAR](50)  NULL,
	[source_plant][VARCHAR](60)  NULL,
	[source_plant_country_territory][VARCHAR](30)  NULL,
	[source_plant_region][VARCHAR](30)  NULL,
	[design_parent][VARCHAR](50)  NULL,
	[engineering_group][VARCHAR](30)  NULL,
	[manufacturer_group][VARCHAR](30)  NULL,
	[manufacturer][VARCHAR](30)  NULL,
	[sales_parent][VARCHAR](30)	 NULL,
	[production_brand][VARCHAR](30)  NULL,
	[platform_design_owner][VARCHAR](30)  NULL,
	[architecture][VARCHAR](30)	 NULL,
	[platform][VARCHAR](40)  NULL,
	[program][VARCHAR](30)  NULL,
	[production_nameplate][VARCHAR](30)	 NULL,
	[sop_start_of_production][DATETIME]	 NULL,
	[eop_end_of_production][DATETIME]  NULL,
	[lifecycle_time][INT]  NULL,
	[vehicle][VARCHAR](100)	 NULL,
	[assembly_type][VARCHAR](40)  NULL,
	[strategic_group][VARCHAR](40)  NULL,
	[sales_group][VARCHAR](40)	 NULL,
	[global_nameplate][VARCHAR](40)	 NULL,
	[primary_design_center][VARCHAR](40) NULL,
	[primary_design_country_territory][VARCHAR](40)	 NULL,
	[primary_design_region][VARCHAR](40)  NULL,
	[secondary_design_center][VARCHAR](40)  NULL,
	[secondary_design_country_territory][VARCHAR](40)  NULL,
	[secondary_design_region][VARCHAR](40)	 NULL,
	[gvw_rating][VARCHAR](30)  NULL,
	[gvw_class][VARCHAR](30)  NULL,
	[car_truck][VARCHAR](30)  NULL,
	[production_type][VARCHAR](10) NULL,
	[global_production_segment][VARCHAR](30)  NULL,
	[regional_sales_segment][VARCHAR](30)  NULL,
	[global_production_price_class][VARCHAR](30) NULL,
	[global_sales_segment][VARCHAR](10)	 NULL,
	[global_sales_sub_segment][VARCHAR](15)	 NULL,
	[global_sales_price_class][INT]  NULL,
	[short_term_risk_rating][INT]  NULL,
	[long_term_risk_rating][INT]  NULL,
	[origen][VARCHAR](8) NOT NULL,	
	[porcentaje_scrap][decimal](5,2) NOT NULL,
 CONSTRAINT [PK_BG_IHS_item] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

  -- restriccion unique
 alter table [BG_IHS_item]
  add constraint UQ_BG_IHS_item_mnemonic_vehicle_plant
  unique (id_ihs_version, mnemonic_vehicle_plant, vehicle, sop_start_of_production); -- ver cual será el campo unico para elemento de IHS personalizado
 
 -- restriccion de clave foranea
alter table [BG_IHS_item]
 add constraint FK_BG_BG_IHS_item_id_ihs_version
  foreign key (id_ihs_version)
  references BG_IHS_versiones(id);

-- restricion check
ALTER TABLE [BG_IHS_item] ADD CONSTRAINT CK_BG_IHS_item_tipo_viaje CHECK ([origen] IN 
('IHS', 'USER')
)

 	  
IF object_id(N'BG_IHS_item',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< BG_IHS_item en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla BG_IHS_item  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO

