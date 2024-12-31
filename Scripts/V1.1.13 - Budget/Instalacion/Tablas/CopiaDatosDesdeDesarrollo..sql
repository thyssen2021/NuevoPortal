-- Script completo para copiar datos de tablas BG_ entre bases de datos utilizando SELECT * para evitar omisiones

---------------------------------------
-- Tabla: BG_forecast_cat_clientes
DELETE FROM [Portal_2_0_budget].[dbo].[BG_forecast_cat_clientes];
DBCC CHECKIDENT ('[Portal_2_0_budget].[dbo].[BG_forecast_cat_clientes]', RESEED, 0);

-- Tabla: BG_Forecast_cat_defaults
DELETE FROM [Portal_2_0_budget].[dbo].[BG_Forecast_cat_defaults];
DBCC CHECKIDENT ('[Portal_2_0_budget].[dbo].[BG_Forecast_cat_defaults]', RESEED, 0);

-- Tabla: BG_Forecast_cat_historico_scrap
DELETE FROM [Portal_2_0_budget].[dbo].[BG_Forecast_cat_historico_scrap];
DBCC CHECKIDENT ('[Portal_2_0_budget].[dbo].[BG_Forecast_cat_historico_scrap]', RESEED, 0);

-- Tabla: BG_forecast_cat_historico_ventas
DELETE FROM [Portal_2_0_budget].[dbo].[BG_forecast_cat_historico_ventas];
DBCC CHECKIDENT ('[Portal_2_0_budget].[dbo].[BG_forecast_cat_historico_ventas]', RESEED, 0);

-- Tabla: BG_Forecast_cat_inventory_own
DELETE FROM [Portal_2_0_budget].[dbo].[BG_Forecast_cat_inventory_own];
DBCC CHECKIDENT ('[Portal_2_0_budget].[dbo].[BG_Forecast_cat_inventory_own]', RESEED, 0);

-- Tabla: BG_Forecast_cat_secciones_calculo
DELETE FROM [Portal_2_0_budget].[dbo].[BG_Forecast_cat_secciones_calculo];
DBCC CHECKIDENT ('[Portal_2_0_budget].[dbo].[BG_Forecast_cat_secciones_calculo]', RESEED, 0);

-- Tabla: BG_Forecast_item
DELETE FROM [Portal_2_0_budget].[dbo].[BG_Forecast_item];
DBCC CHECKIDENT ('[Portal_2_0_budget].[dbo].[BG_Forecast_item]', RESEED, 0);

-- Tabla: BG_Forecast_reporte
DELETE FROM [Portal_2_0_budget].[dbo].[BG_Forecast_reporte];
DBCC CHECKIDENT ('[Portal_2_0_budget].[dbo].[BG_Forecast_reporte]', RESEED, 0);

-- Tabla: BG_IHS_combinacion
DELETE FROM [Portal_2_0_budget].[dbo].[BG_IHS_combinacion];
DBCC CHECKIDENT ('[Portal_2_0_budget].[dbo].[BG_IHS_combinacion]', RESEED, 0);

-- Tabla: BG_IHS_custom_rel_demanda
DELETE FROM [Portal_2_0_budget].[dbo].[BG_IHS_custom_rel_demanda];
DBCC CHECKIDENT ('[Portal_2_0_budget].[dbo].[BG_IHS_custom_rel_demanda]', RESEED, 0);

-- Tabla: BG_IHS_division
DELETE FROM [Portal_2_0_budget].[dbo].[BG_IHS_division];
DBCC CHECKIDENT ('[Portal_2_0_budget].[dbo].[BG_IHS_division]', RESEED, 0);

-- Tabla: BG_IHS_item
DELETE FROM [Portal_2_0_budget].[dbo].[BG_IHS_item];
DBCC CHECKIDENT ('[Portal_2_0_budget].[dbo].[BG_IHS_item]', RESEED, 0);

-- Tabla: BG_IHS_plantas
DELETE FROM [Portal_2_0_budget].[dbo].[BG_IHS_plantas];
DBCC CHECKIDENT ('[Portal_2_0_budget].[dbo].[BG_IHS_plantas]', RESEED, 0);

-- Tabla: BG_IHS_regiones
DELETE FROM [Portal_2_0_budget].[dbo].[BG_IHS_regiones];
DBCC CHECKIDENT ('[Portal_2_0_budget].[dbo].[BG_IHS_regiones]', RESEED, 0);

-- Tabla: BG_IHS_rel_combinacion
DELETE FROM [Portal_2_0_budget].[dbo].[BG_IHS_rel_combinacion];
DBCC CHECKIDENT ('[Portal_2_0_budget].[dbo].[BG_IHS_rel_combinacion]', RESEED, 0);

-- Tabla: BG_IHS_rel_cuartos
DELETE FROM [Portal_2_0_budget].[dbo].[BG_IHS_rel_cuartos];
DBCC CHECKIDENT ('[Portal_2_0_budget].[dbo].[BG_IHS_rel_cuartos]', RESEED, 0);

-- Tabla: BG_IHS_rel_demanda
DELETE FROM [Portal_2_0_budget].[dbo].[BG_IHS_rel_demanda];
DBCC CHECKIDENT ('[Portal_2_0_budget].[dbo].[BG_IHS_rel_demanda]', RESEED, 0);

-- Tabla: BG_IHS_rel_division
DELETE FROM [Portal_2_0_budget].[dbo].[BG_IHS_rel_division];
DBCC CHECKIDENT ('[Portal_2_0_budget].[dbo].[BG_IHS_rel_division]', RESEED, 0);

-- Tabla: BG_IHS_rel_regiones
DELETE FROM [Portal_2_0_budget].[dbo].[BG_IHS_rel_regiones];
DBCC CHECKIDENT ('[Portal_2_0_budget].[dbo].[BG_IHS_rel_regiones]', RESEED, 0);

-- Tabla: BG_IHS_segmentos
DELETE FROM [Portal_2_0_budget].[dbo].[BG_IHS_segmentos];
DBCC CHECKIDENT ('[Portal_2_0_budget].[dbo].[BG_IHS_segmentos]', RESEED, 0);

-- Tabla: BG_ihs_vehicle_custom
DELETE FROM [Portal_2_0_budget].[dbo].[BG_ihs_vehicle_custom];
DBCC CHECKIDENT ('[Portal_2_0_budget].[dbo].[BG_ihs_vehicle_custom]', RESEED, 0);

-- Tabla: BG_IHS_versiones
DELETE FROM [Portal_2_0_budget].[dbo].[BG_IHS_versiones];
DBCC CHECKIDENT ('[Portal_2_0_budget].[dbo].[BG_IHS_versiones]', RESEED, 0);

---------------------------------------
-- Tabla: BG_forecast_cat_clientes
SET IDENTITY_INSERT [Portal_2_0_budget].[dbo].[BG_forecast_cat_clientes] ON;
INSERT INTO [Portal_2_0_budget].[dbo].[BG_forecast_cat_clientes] (id, descripcion, activo)
SELECT id, descripcion, activo
FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_forecast_cat_clientes];
SET IDENTITY_INSERT [Portal_2_0_budget].[dbo].[BG_forecast_cat_clientes] OFF;

-- Tabla: BG_Forecast_cat_defaults
SET IDENTITY_INSERT [Portal_2_0_budget].[dbo].[BG_Forecast_cat_defaults] ON;
INSERT INTO [Portal_2_0_budget].[dbo].[BG_Forecast_cat_defaults] (id, scrap_acero_valor_puebla, scrap_acero_ganancia_puebla, scrap_aluminio_valor_puebla, scrap_aluminio_ganancia_puebla, scrap_acero_valor_silao, scrap_acero_ganancia_silao, scrap_aluminio_valor_silao, scrap_aluminio_ganancia_silao)
SELECT id, scrap_acero_valor_puebla, scrap_acero_ganancia_puebla, scrap_aluminio_valor_puebla, scrap_aluminio_ganancia_puebla, scrap_acero_valor_silao, scrap_acero_ganancia_silao, scrap_aluminio_valor_silao, scrap_aluminio_ganancia_silao
FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_Forecast_cat_defaults];
SET IDENTITY_INSERT [Portal_2_0_budget].[dbo].[BG_Forecast_cat_defaults] OFF;

-- Tabla: BG_Forecast_cat_historico_scrap
SET IDENTITY_INSERT [Portal_2_0_budget].[dbo].[BG_Forecast_cat_historico_scrap] ON;
INSERT INTO [Portal_2_0_budget].[dbo].[BG_Forecast_cat_historico_scrap] (id, id_planta, tipo_metal, fecha, scrap, scrap_ganancia)
SELECT id, id_planta, tipo_metal, fecha, scrap, scrap_ganancia
FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_Forecast_cat_historico_scrap];
SET IDENTITY_INSERT [Portal_2_0_budget].[dbo].[BG_Forecast_cat_historico_scrap] OFF;

-- Tabla: BG_Forecast_cat_secciones_calculo
SET IDENTITY_INSERT [Portal_2_0_budget].[dbo].[BG_Forecast_cat_secciones_calculo] ON;
INSERT INTO [Portal_2_0_budget].[dbo].[BG_Forecast_cat_secciones_calculo] (id, descripcion, aplica_formula, formula, tipo_dato, decimales, activo)
SELECT id, descripcion, aplica_formula, formula, tipo_dato, decimales, activo
FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_Forecast_cat_secciones_calculo];
SET IDENTITY_INSERT [Portal_2_0_budget].[dbo].[BG_Forecast_cat_secciones_calculo] OFF;

-- Tabla: BG_IHS_versiones
SET IDENTITY_INSERT [Portal_2_0_budget].[dbo].[BG_IHS_versiones] ON;
INSERT INTO [Portal_2_0_budget].[dbo].[BG_IHS_versiones] (id, periodo, nombre, activo)
SELECT id, periodo, nombre, activo
FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_IHS_versiones];
SET IDENTITY_INSERT [Portal_2_0_budget].[dbo].[BG_IHS_versiones] OFF;


-- Tabla: BG_Forecast_reporte
SET IDENTITY_INSERT [Portal_2_0_budget].[dbo].[BG_Forecast_reporte] ON;
INSERT INTO [Portal_2_0_budget].[dbo].[BG_Forecast_reporte] (id, id_ihs_version, fecha, descripcion, activo)
SELECT id, id_ihs_version, fecha, descripcion, activo
FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_Forecast_reporte];
SET IDENTITY_INSERT [Portal_2_0_budget].[dbo].[BG_Forecast_reporte] OFF;

-- Tabla: BG_ihs_vehicle_custom
SET IDENTITY_INSERT [Portal_2_0_budget].[dbo].[BG_ihs_vehicle_custom] ON;
INSERT INTO [Portal_2_0_budget].[dbo].[BG_ihs_vehicle_custom] (Id, MnemonicVehiclePlant, ProductionPlant, ManufacturerGroup, sop_start_of_production, eop_end_of_production, Vehicle, AssemblyType, CreatedAt, UpdatedAt)
SELECT Id, MnemonicVehiclePlant, ProductionPlant, ManufacturerGroup, sop_start_of_production, eop_end_of_production, Vehicle, AssemblyType, CreatedAt, UpdatedAt
FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_ihs_vehicle_custom];
SET IDENTITY_INSERT [Portal_2_0_budget].[dbo].[BG_ihs_vehicle_custom] OFF;

-- Tabla: BG_IHS_item
SET IDENTITY_INSERT [Portal_2_0_budget].[dbo].[BG_IHS_item] ON;
INSERT INTO [Portal_2_0_budget].[dbo].[BG_IHS_item] (id, id_ihs_version, core_nameplate_region_mnemonic, core_nameplate_plant_mnemonic, mnemonic_vehicle, mnemonic_vehicle_plant, mnemonic_platform, mnemonic_plant, region, market, country_territory, production_plant, city, plant_state_province, source_plant, source_plant_country_territory, source_plant_region, design_parent, engineering_group, manufacturer_group, manufacturer, sales_parent, production_brand, platform_design_owner, architecture, platform, program, production_nameplate, sop_start_of_production, eop_end_of_production, lifecycle_time, vehicle, assembly_type, strategic_group, sales_group, global_nameplate, primary_design_center, primary_design_country_territory, primary_design_region, secondary_design_center, secondary_design_country_territory, secondary_design_region, gvw_rating, gvw_class, car_truck, production_type, global_production_segment, regional_sales_segment, global_production_price_class, global_sales_segment, global_sales_sub_segment, global_sales_price_class, short_term_risk_rating, long_term_risk_rating, origen, porcentaje_scrap)
SELECT id, id_ihs_version, core_nameplate_region_mnemonic, core_nameplate_plant_mnemonic, mnemonic_vehicle, mnemonic_vehicle_plant, mnemonic_platform, mnemonic_plant, region, market, country_territory, production_plant, city, plant_state_province, source_plant, source_plant_country_territory, source_plant_region, design_parent, engineering_group, manufacturer_group, manufacturer, sales_parent, production_brand, platform_design_owner, architecture, platform, program, production_nameplate, sop_start_of_production, eop_end_of_production, lifecycle_time, vehicle, assembly_type, strategic_group, sales_group, global_nameplate, primary_design_center, primary_design_country_territory, primary_design_region, secondary_design_center, secondary_design_country_territory, secondary_design_region, gvw_rating, gvw_class, car_truck, production_type, global_production_segment, regional_sales_segment, global_production_price_class, global_sales_segment, global_sales_sub_segment, global_sales_price_class, short_term_risk_rating, long_term_risk_rating, origen, porcentaje_scrap
FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_IHS_item];
SET IDENTITY_INSERT [Portal_2_0_budget].[dbo].[BG_IHS_item] OFF;

-- Tabla: BG_IHS_combinacion
SET IDENTITY_INSERT [Portal_2_0_budget].[dbo].[BG_IHS_combinacion] ON;
INSERT INTO [Portal_2_0_budget].[dbo].[BG_IHS_combinacion] (id, id_ihs_version, vehicle, production_brand, production_plant, manufacturer_group, sop_start_of_production, eop_end_of_production, comentario, porcentaje_scrap, activo, production_nameplate)
SELECT id, id_ihs_version, vehicle, production_brand, production_plant, manufacturer_group, sop_start_of_production, eop_end_of_production, comentario, porcentaje_scrap, activo, production_nameplate
FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_IHS_combinacion];
SET IDENTITY_INSERT [Portal_2_0_budget].[dbo].[BG_IHS_combinacion] OFF;

-- Tabla: BG_IHS_division
SET IDENTITY_INSERT [Portal_2_0_budget].[dbo].[BG_IHS_division] ON;
INSERT INTO [Portal_2_0_budget].[dbo].[BG_IHS_division] (id, id_ihs_version, id_ihs_item, comentario, porcentaje_scrap, activo)
SELECT id, id_ihs_version, id_ihs_item, comentario, porcentaje_scrap, activo
FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_IHS_division];
SET IDENTITY_INSERT [Portal_2_0_budget].[dbo].[BG_IHS_division] OFF;

-- Tabla: BG_IHS_rel_division
SET IDENTITY_INSERT [Portal_2_0_budget].[dbo].[BG_IHS_rel_division] ON;
INSERT INTO [Portal_2_0_budget].[dbo].[BG_IHS_rel_division] (id, id_ihs_division, vehicle, production_nameplate, porcentaje, activo)
SELECT id, id_ihs_division, vehicle, production_nameplate, porcentaje, activo
FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_IHS_rel_division];
SET IDENTITY_INSERT [Portal_2_0_budget].[dbo].[BG_IHS_rel_division] OFF;


-- Tabla: BG_Forecast_item
SET IDENTITY_INSERT [Portal_2_0_budget].[dbo].[BG_Forecast_item] ON;
INSERT INTO [Portal_2_0_budget].[dbo].[BG_Forecast_item] (id, id_bg_forecast_reporte, id_ihs_item, id_ihs_combinacion, id_ihs_rel_division, pos, business_and_plant, calculo_activo, sap_invoice_code, invoiced_to, number_sap_client, shipped_to, own_cm, route, plant, external_processor, mill, sap_master_coil, part_description, part_number, production_line, vehicle, parts_auto, strokes_auto, material_type, shape, initial_weight_part, net_weight_part, scrap_consolidation, ventas_part, material_cost_part, cost_of_outside_processor, additional_material_cost_part, outgoing_freight_part, freights_income, [outgoing freight], cat_1, cat_2, cat_3, cat_4, freights_income_usd_part, maniobras_usd_part, customs_expenses, previous_sap_invoice_code, mnemonic_vehicle_plant, propulsion_system_type, trans_silao_slp, wooden_pallet_usd_part, packaging_price_usd_part, neopreno_usd_part, id_ihs_custom)
SELECT id, id_bg_forecast_reporte, id_ihs_item, id_ihs_combinacion, id_ihs_rel_division, pos, business_and_plant, calculo_activo, sap_invoice_code, invoiced_to, number_sap_client, shipped_to, own_cm, route, plant, external_processor, mill, sap_master_coil, part_description, part_number, production_line, vehicle, parts_auto, strokes_auto, material_type, shape, initial_weight_part, net_weight_part, scrap_consolidation, ventas_part, material_cost_part, cost_of_outside_processor, additional_material_cost_part, outgoing_freight_part, freights_income, [outgoing freight], cat_1, cat_2, cat_3, cat_4, freights_income_usd_part, maniobras_usd_part, customs_expenses, previous_sap_invoice_code, mnemonic_vehicle_plant, propulsion_system_type, trans_silao_slp, wooden_pallet_usd_part, packaging_price_usd_part, neopreno_usd_part, id_ihs_custom
FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_Forecast_item];
SET IDENTITY_INSERT [Portal_2_0_budget].[dbo].[BG_Forecast_item] OFF;

-- Tabla: BG_IHS_custom_rel_demanda
SET IDENTITY_INSERT [Portal_2_0_budget].[dbo].[BG_IHS_custom_rel_demanda] ON;
INSERT INTO [Portal_2_0_budget].[dbo].[BG_IHS_custom_rel_demanda] (id, id_vehicle_custom, cantidad, fecha)
SELECT id, id_vehicle_custom, cantidad, fecha
FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_IHS_custom_rel_demanda];
SET IDENTITY_INSERT [Portal_2_0_budget].[dbo].[BG_IHS_custom_rel_demanda] OFF;

-- Tabla: BG_IHS_plantas
SET IDENTITY_INSERT [Portal_2_0_budget].[dbo].[BG_IHS_plantas] ON;
INSERT INTO [Portal_2_0_budget].[dbo].[BG_IHS_plantas] (id, id_ihs_version, mnemonic_plant, descripcion, activo)
SELECT id, id_ihs_version, mnemonic_plant, descripcion, activo
FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_IHS_plantas];
SET IDENTITY_INSERT [Portal_2_0_budget].[dbo].[BG_IHS_plantas] OFF;

-- Tabla: BG_IHS_regiones
SET IDENTITY_INSERT [Portal_2_0_budget].[dbo].[BG_IHS_regiones] ON;
INSERT INTO [Portal_2_0_budget].[dbo].[BG_IHS_regiones] (id, id_ihs_version, descripcion, activo)
SELECT id, id_ihs_version, descripcion, activo
FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_IHS_regiones];
SET IDENTITY_INSERT [Portal_2_0_budget].[dbo].[BG_IHS_regiones] OFF;

-- Tabla: BG_IHS_rel_combinacion
SET IDENTITY_INSERT [Portal_2_0_budget].[dbo].[BG_IHS_rel_combinacion] ON;
INSERT INTO [Portal_2_0_budget].[dbo].[BG_IHS_rel_combinacion] (id, id_ihs_combinacion, id_ihs_item)
SELECT id, id_ihs_combinacion, id_ihs_item
FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_IHS_rel_combinacion];
SET IDENTITY_INSERT [Portal_2_0_budget].[dbo].[BG_IHS_rel_combinacion] OFF;

-- Tabla: BG_IHS_rel_cuartos
SET IDENTITY_INSERT [Portal_2_0_budget].[dbo].[BG_IHS_rel_cuartos] ON;
INSERT INTO [Portal_2_0_budget].[dbo].[BG_IHS_rel_cuartos] (id, id_ihs_item, cuarto, anio, cantidad, fecha_carga)
SELECT id, id_ihs_item, cuarto, anio, cantidad, fecha_carga
FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_IHS_rel_cuartos];
SET IDENTITY_INSERT [Portal_2_0_budget].[dbo].[BG_IHS_rel_cuartos] OFF;

-- Tabla: BG_IHS_rel_demanda
SET IDENTITY_INSERT [Portal_2_0_budget].[dbo].[BG_IHS_rel_demanda] ON;
INSERT INTO [Portal_2_0_budget].[dbo].[BG_IHS_rel_demanda] (id, id_ihs_item, cantidad, fecha, fecha_carga, tipo)
SELECT id, id_ihs_item, cantidad, fecha, fecha_carga, tipo
FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_IHS_rel_demanda];
SET IDENTITY_INSERT [Portal_2_0_budget].[dbo].[BG_IHS_rel_demanda] OFF;

-- Tabla: BG_IHS_rel_regiones
SET IDENTITY_INSERT [Portal_2_0_budget].[dbo].[BG_IHS_rel_regiones] ON;
INSERT INTO [Portal_2_0_budget].[dbo].[BG_IHS_rel_regiones] (id, id_ihs_version, id_bg_ihs_region, id_bg_ihs_plant)
SELECT id, id_ihs_version, id_bg_ihs_region, id_bg_ihs_plant
FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_IHS_rel_regiones];
SET IDENTITY_INSERT [Portal_2_0_budget].[dbo].[BG_IHS_rel_regiones] OFF;

-- Tabla: BG_IHS_segmentos
SET IDENTITY_INSERT [Portal_2_0_budget].[dbo].[BG_IHS_segmentos] ON;
INSERT INTO [Portal_2_0_budget].[dbo].[BG_IHS_segmentos] (id, id_ihs_version, global_production_segment, flat_rolled_steel_usage, blanks_usage_percent)
SELECT id, id_ihs_version, global_production_segment, flat_rolled_steel_usage, blanks_usage_percent
FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_IHS_segmentos];
SET IDENTITY_INSERT [Portal_2_0_budget].[dbo].[BG_IHS_segmentos] OFF;

-- Tabla: BG_forecast_cat_historico_ventas
SET IDENTITY_INSERT [Portal_2_0_budget].[dbo].[BG_forecast_cat_historico_ventas] ON;
INSERT INTO [Portal_2_0_budget].[dbo].[BG_forecast_cat_historico_ventas] (id, id_cliente, id_seccion, fecha, valor)
SELECT id, id_cliente, id_seccion, fecha, valor
FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_forecast_cat_historico_ventas];
SET IDENTITY_INSERT [Portal_2_0_budget].[dbo].[BG_forecast_cat_historico_ventas] OFF;

-- Tabla: BG_Forecast_cat_inventory_own
SET IDENTITY_INSERT [Portal_2_0_budget].[dbo].[BG_Forecast_cat_inventory_own] ON;
INSERT INTO [Portal_2_0_budget].[dbo].[BG_Forecast_cat_inventory_own] (id, id_bg_forecast_reporte, orden, mes, cantidad)
SELECT id, id_bg_forecast_reporte, orden, mes, cantidad
FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_Forecast_cat_inventory_own];
SET IDENTITY_INSERT [Portal_2_0_budget].[dbo].[BG_Forecast_cat_inventory_own] OFF;
---------------------------------------
-- Tabla: BG_forecast_cat_clientes
SELECT TOP 5 * FROM [Portal_2_0_budget].[dbo].[BG_forecast_cat_clientes] ORDER BY id DESC;
SELECT TOP 5 * FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_forecast_cat_clientes] ORDER BY id DESC;

-- Tabla: BG_Forecast_cat_defaults
SELECT TOP 5 * FROM [Portal_2_0_budget].[dbo].[BG_Forecast_cat_defaults] ORDER BY id DESC;
SELECT TOP 5 * FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_Forecast_cat_defaults] ORDER BY id DESC;

-- Tabla: BG_Forecast_cat_historico_scrap
SELECT TOP 5 * FROM [Portal_2_0_budget].[dbo].[BG_Forecast_cat_historico_scrap] ORDER BY id DESC;
SELECT TOP 5 * FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_Forecast_cat_historico_scrap] ORDER BY id DESC;

-- Tabla: BG_Forecast_cat_secciones_calculo
SELECT TOP 5 * FROM [Portal_2_0_budget].[dbo].[BG_Forecast_cat_secciones_calculo] ORDER BY id DESC;
SELECT TOP 5 * FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_Forecast_cat_secciones_calculo] ORDER BY id DESC;

-- Tabla: BG_IHS_versiones
SELECT TOP 5 * FROM [Portal_2_0_budget].[dbo].[BG_IHS_versiones] ORDER BY id DESC;
SELECT TOP 5 * FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_IHS_versiones] ORDER BY id DESC;

-- Tabla: BG_Forecast_reporte
SELECT TOP 5 * FROM [Portal_2_0_budget].[dbo].[BG_Forecast_reporte] ORDER BY id DESC;
SELECT TOP 5 * FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_Forecast_reporte] ORDER BY id DESC;


-- Tabla: BG_ihs_vehicle_custom
SELECT TOP 5 * FROM [Portal_2_0_budget].[dbo].[BG_ihs_vehicle_custom] ORDER BY id DESC;
SELECT TOP 5 * FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_ihs_vehicle_custom] ORDER BY id DESC;

-- Tabla: BG_IHS_item
SELECT TOP 5 * FROM [Portal_2_0_budget].[dbo].[BG_IHS_item] ORDER BY id DESC;
SELECT TOP 5 * FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_IHS_item] ORDER BY id DESC;

-- Tabla: BG_IHS_combinacion
SELECT TOP 5 * FROM [Portal_2_0_budget].[dbo].[BG_IHS_combinacion] ORDER BY id DESC;
SELECT TOP 5 * FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_IHS_combinacion] ORDER BY id DESC;

-- Tabla: BG_IHS_division
SELECT TOP 5 * FROM [Portal_2_0_budget].[dbo].[BG_IHS_division] ORDER BY id DESC;
SELECT TOP 5 * FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_IHS_division] ORDER BY id DESC;

-- Tabla: BG_IHS_rel_division
SELECT TOP 5 * FROM [Portal_2_0_budget].[dbo].[BG_IHS_rel_division] ORDER BY id DESC;
SELECT TOP 5 * FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_IHS_rel_division] ORDER BY id DESC;

-- Tabla: BG_Forecast_item
SELECT TOP 5 * FROM [Portal_2_0_budget].[dbo].[BG_Forecast_item] ORDER BY id DESC;
SELECT TOP 5 * FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_Forecast_item] ORDER BY id DESC;

-- Tabla: BG_IHS_custom_rel_demanda
SELECT TOP 5 * FROM [Portal_2_0_budget].[dbo].[BG_IHS_custom_rel_demanda] ORDER BY id DESC;
SELECT TOP 5 * FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_IHS_custom_rel_demanda] ORDER BY id DESC;

-- Tabla: BG_IHS_plantas
SELECT TOP 5 * FROM [Portal_2_0_budget].[dbo].[BG_IHS_plantas] ORDER BY id DESC;
SELECT TOP 5 * FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_IHS_plantas] ORDER BY id DESC;

-- Tabla: BG_IHS_regiones
SELECT TOP 5 * FROM [Portal_2_0_budget].[dbo].[BG_IHS_regiones] ORDER BY id DESC;
SELECT TOP 5 * FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_IHS_regiones] ORDER BY id DESC;

-- Tabla: BG_IHS_rel_combinacion
SELECT TOP 5 * FROM [Portal_2_0_budget].[dbo].[BG_IHS_rel_combinacion] ORDER BY id DESC;
SELECT TOP 5 * FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_IHS_rel_combinacion] ORDER BY id DESC;

-- Tabla: BG_IHS_rel_cuartos
SELECT TOP 5 * FROM [Portal_2_0_budget].[dbo].[BG_IHS_rel_cuartos] ORDER BY id DESC;
SELECT TOP 5 * FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_IHS_rel_cuartos] ORDER BY id DESC;

-- Tabla: BG_IHS_rel_demanda
SELECT TOP 5 * FROM [Portal_2_0_budget].[dbo].[BG_IHS_rel_demanda] ORDER BY id DESC;
SELECT TOP 5 * FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_IHS_rel_demanda] ORDER BY id DESC;

-- Tabla: BG_IHS_rel_regiones
SELECT TOP 5 * FROM [Portal_2_0_budget].[dbo].[BG_IHS_rel_regiones] ORDER BY id DESC;
SELECT TOP 5 * FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_IHS_rel_regiones] ORDER BY id DESC;

-- Tabla: BG_IHS_segmentos
SELECT TOP 5 * FROM [Portal_2_0_budget].[dbo].[BG_IHS_segmentos] ORDER BY id DESC;
SELECT TOP 5 * FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_IHS_segmentos] ORDER BY id DESC;

-- Tabla: BG_forecast_cat_historico_ventas
SELECT TOP 5 * FROM [Portal_2_0_budget].[dbo].[BG_forecast_cat_historico_ventas] ORDER BY id DESC;
SELECT TOP 5 * FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_forecast_cat_historico_ventas] ORDER BY id DESC;

-- Tabla: BG_Forecast_cat_inventory_own
SELECT TOP 5 * FROM [Portal_2_0_budget].[dbo].[BG_Forecast_cat_inventory_own] ORDER BY id DESC;
SELECT TOP 5 * FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_Forecast_cat_inventory_own] ORDER BY id DESC;