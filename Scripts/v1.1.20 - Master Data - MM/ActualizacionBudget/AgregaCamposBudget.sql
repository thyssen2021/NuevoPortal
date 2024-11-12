--Inserta los valores faltantes a SCDM_solicitud_rel_item_material
ALTER TABLE SCDM_solicitud_rel_item_material ADD peso_bruto_real_bascula float null;
ALTER TABLE SCDM_solicitud_rel_item_material ADD peso_neto_real_bascula float null;
ALTER TABLE SCDM_solicitud_rel_item_material ALTER COLUMN angulo_a int null; -- Cambio de tipo de dato
ALTER TABLE SCDM_solicitud_rel_item_material ALTER COLUMN angulo_a int null; -- Cambio de tipo de dato
ALTER TABLE SCDM_solicitud_rel_item_material ADD pieza_doble varchar(2) null;
ALTER TABLE SCDM_solicitud_rel_item_material ADD reaplicacion bit null;
ALTER TABLE SCDM_solicitud_rel_item_material ADD peso_maximo_tolerancia_negativa float null;
ALTER TABLE SCDM_solicitud_rel_item_material ADD peso_maximo_tolerancia_positiva float null;
ALTER TABLE SCDM_solicitud_rel_item_material ADD peso_minimo_tolerancia_negativa float null;
ALTER TABLE SCDM_solicitud_rel_item_material ADD peso_minimo_tolerancia_positiva float null;

