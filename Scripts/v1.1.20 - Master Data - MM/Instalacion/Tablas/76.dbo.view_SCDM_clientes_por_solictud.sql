CREATE OR ALTER VIEW [dbo].[view_SCDM_clientes_por_solictud] AS

/* PENDIENTE POR AGREGAR EXTENSIONES */

SELECT ISNULL(ROW_NUMBER() OVER (ORDER BY f.id_solicitud), 0) as id,
f.* FROM (SELECT DISTINCT r.id_solicitud, c.Customer as sap_cliente, cl.descripcion as nombre_cliente FROM 
		(select distinct id_solicitud, numero_material from SCDM_solicitud_rel_item_material cm	-- creación de materiales
			UNION
		select distinct id_solicitud, nuevo_material as numero_material from SCDM_solicitud_rel_creacion_referencia mr   -- creación de materiales con referencia
			UNION
		select distinct id_solicitud, material_existente as numero_material from SCDM_solicitud_rel_cambio_ingenieria ci  -- cambio de ingenieria
			UNION
		select distinct id_solicitud, num_material as numero_material from SCDM_solicitud_rel_orden_compra oc  -- orden compra	
			UNION
		select distinct id_solicitud, material as numero_material from SCDM_solicitud_rel_activaciones ac  -- activaciones	
		) as r
join class_v3 as c on c.Object = r.numero_material
JOIN clientes as cl on c.Customer = cl.claveSAP) as f

		