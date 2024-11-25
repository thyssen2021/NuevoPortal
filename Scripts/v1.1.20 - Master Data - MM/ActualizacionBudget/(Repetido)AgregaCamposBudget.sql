--Inserta los valores faltantes a SCDM_solicitud_rel_item_material
ALTER TABLE SCDM_solicitud_rel_item_material ADD peso_bruto_real_bascula float null;
ALTER TABLE SCDM_solicitud_rel_item_material ADD peso_neto_real_bascula float null;

ALTER TABLE SCDM_solicitud_rel_item_material ALTER COLUMN angulo_a float null; -- Cambio de tipo de dato
ALTER TABLE SCDM_solicitud_rel_item_material ALTER COLUMN angulo_b float null; -- Cambio de tipo de dato
ALTER TABLE SCDM_solicitud_rel_item_material ALTER COLUMN piezas_por_paquete float null; -- Cambio de tipo de dato
ALTER TABLE SCDM_solicitud_rel_item_material ALTER COLUMN piezas_por_auto float null; -- Cambio de tipo de dato
ALTER TABLE SCDM_solicitud_rel_item_material ALTER COLUMN piezas_por_golpe float null; -- Cambio de tipo de dato

ALTER TABLE SCDM_solicitud_rel_item_material ADD pieza_doble varchar(2) null;
ALTER TABLE SCDM_solicitud_rel_item_material ADD reaplicacion bit null;
ALTER TABLE SCDM_solicitud_rel_item_material ADD peso_maximo_tolerancia_negativa float null;
ALTER TABLE SCDM_solicitud_rel_item_material ADD peso_maximo_tolerancia_positiva float null;
ALTER TABLE SCDM_solicitud_rel_item_material ADD peso_minimo_tolerancia_negativa float null;
ALTER TABLE SCDM_solicitud_rel_item_material ADD peso_minimo_tolerancia_positiva float null;

--Inserta los valores faltantes a SCDM_solicitud_rel_creacion_referencia



ALTER TABLE SCDM_solicitud_rel_creacion_referencia ADD peso_bruto_real_bascula float null
ALTER TABLE SCDM_solicitud_rel_creacion_referencia ADD peso_neto_real_bascula float null
ALTER TABLE SCDM_solicitud_rel_creacion_referencia ADD angulo_a float null
ALTER TABLE SCDM_solicitud_rel_creacion_referencia ADD angulo_b float null
ALTER TABLE SCDM_solicitud_rel_creacion_referencia ADD scrap_permitido_puntas_colas float null
ALTER TABLE SCDM_solicitud_rel_creacion_referencia ADD pieza_doble varchar(2) null
ALTER TABLE SCDM_solicitud_rel_creacion_referencia ADD reaplicacion bit null
ALTER TABLE SCDM_solicitud_rel_creacion_referencia ADD conciliacion_puntas_colas bit null
ALTER TABLE SCDM_solicitud_rel_creacion_referencia ADD conciliacion_scrap_ingenieria bit null
ALTER TABLE SCDM_solicitud_rel_creacion_referencia ADD modelo_negocio varchar(80) null
ALTER TABLE SCDM_solicitud_rel_creacion_referencia ADD posicion_rollo varchar(80) null
ALTER TABLE SCDM_solicitud_rel_creacion_referencia ADD IHS_num_1 varchar(120) null
ALTER TABLE SCDM_solicitud_rel_creacion_referencia ADD IHS_num_2 varchar(120) null
ALTER TABLE SCDM_solicitud_rel_creacion_referencia ADD IHS_num_3 varchar(120) null
ALTER TABLE SCDM_solicitud_rel_creacion_referencia ADD IHS_num_4 varchar(120) null
ALTER TABLE SCDM_solicitud_rel_creacion_referencia ADD IHS_num_5 varchar(120) null
ALTER TABLE SCDM_solicitud_rel_creacion_referencia ADD piezas_por_auto float null
ALTER TABLE SCDM_solicitud_rel_creacion_referencia ADD piezas_por_golpe float null
ALTER TABLE SCDM_solicitud_rel_creacion_referencia ADD piezas_por_paquete float null
ALTER TABLE SCDM_solicitud_rel_creacion_referencia ADD peso_inicial float null
ALTER TABLE SCDM_solicitud_rel_creacion_referencia ADD peso_maximo float null
ALTER TABLE SCDM_solicitud_rel_creacion_referencia ADD peso_maximo_tolerancia_positiva float null
ALTER TABLE SCDM_solicitud_rel_creacion_referencia ADD peso_maximo_tolerancia_negativa float null
ALTER TABLE SCDM_solicitud_rel_creacion_referencia ADD peso_minimo float null
ALTER TABLE SCDM_solicitud_rel_creacion_referencia ADD peso_minimo_tolerancia_positiva float null
ALTER TABLE SCDM_solicitud_rel_creacion_referencia ADD peso_minimo_tolerancia_negativa float null

select * from SCDM_solicitud_rel_creacion_referencia