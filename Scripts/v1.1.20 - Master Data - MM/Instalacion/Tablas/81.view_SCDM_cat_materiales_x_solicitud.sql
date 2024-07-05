USE [Portal_2_0_scdm]
GO

/****** Object:  View [dbo].[view_SCDM_materiales_x_solicitud]    Script Date: 04/07/2024 04:17:11 p.m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER VIEW [dbo].[view_SCDM_materiales_x_solicitud] AS

SELECT ISNULL(ROW_NUMBER() OVER (ORDER BY t1.numero_solicitud), 0) as id,
t1.* FROM (
--Solicitudes de creacion de materiales y C&B
select x.id_solicitud as [numero_solicitud], ts.descripcion as [tipo_solicitud], NULL as tipo_cambio, tm.descripcion as [tipo_material], x.numero_material, p.descripcion as [planta],
emp.nombre+' '+ISNULL(emp.apellido1,'')+' '+ISNULL(emp.apellido2,'') as solicitante, pr.descripcion as prioridad, s.fecha_creacion as fecha_solicitud
from SCDM_solicitud_rel_item_material as x
join SCDM_solicitud AS S ON s.id = x.id_solicitud
join SCDM_cat_tipo_solicitud as ts on ts.id = s.id_tipo_solicitud
join plantas as p on p.clave = s.planta_solicitud
join SCDM_cat_prioridad as pr on pr.id = s.id_prioridad
join empleados as emp on emp.id = s.id_solicitante
join SCDM_cat_tipo_materiales_solicitud as tm on tm.id = x.id_tipo_material

union

--solicitudes de creación con referencia
select x.id_solicitud, ts.descripcion as [tipo_solicitud], NULL as tipo_cambio, x.tipo_material_text as [tipo_material], x.nuevo_material, p.descripcion as [planta],
emp.nombre+' '+ISNULL(emp.apellido1,'')+' '+ISNULL(emp.apellido2,'') as solicitante, pr.descripcion as prioridad, s.fecha_creacion as fecha_solicitud
from SCDM_solicitud_rel_creacion_referencia as x
join SCDM_solicitud AS S ON s.id = x.id_solicitud
join SCDM_cat_tipo_solicitud as ts on ts.id = s.id_tipo_solicitud
join plantas as p on p.clave = s.planta_solicitud
join SCDM_cat_prioridad as pr on pr.id = s.id_prioridad
join empleados as emp on emp.id = s.id_solicitante

union

--Cambio de ingenieria
select x.id_solicitud, ts.descripcion as [tipo_solicitud], tc.descripcion as tipo_cambio, x.tipo_material_text as [tipo_material], x.material_existente, p.descripcion as [planta],
emp.nombre+' '+ISNULL(emp.apellido1,'')+' '+ISNULL(emp.apellido2,'') as solicitante, pr.descripcion as prioridad, s.fecha_creacion as fecha_solicitud
from SCDM_solicitud_rel_cambio_ingenieria as x
join SCDM_solicitud AS S ON s.id = x.id_solicitud
join SCDM_cat_tipo_solicitud as ts on ts.id = s.id_tipo_solicitud
join plantas as p on p.clave = s.planta_solicitud
join SCDM_cat_prioridad as pr on pr.id = s.id_prioridad
join empleados as emp on emp.id = s.id_solicitante
join SCDM_cat_tipo_cambio as tc on tc.id = s.id_tipo_cambio

union

--Cambio en activaciones
select x.id_solicitud, ts.descripcion as [tipo_solicitud], tc.descripcion as tipo_cambio, 'N/D' as [tipo_material], x.material, p.descripcion as [planta],
emp.nombre+' '+ISNULL(emp.apellido1,'')+' '+ISNULL(emp.apellido2,'') as solicitante, pr.descripcion as prioridad, s.fecha_creacion as fecha_solicitud
from SCDM_solicitud_rel_activaciones as x
join SCDM_solicitud AS S ON s.id = x.id_solicitud
join SCDM_cat_tipo_solicitud as ts on ts.id = s.id_tipo_solicitud
join plantas as p on p.clave = s.planta_solicitud
join SCDM_cat_prioridad as pr on pr.id = s.id_prioridad
join empleados as emp on emp.id = s.id_solicitante
join SCDM_cat_tipo_cambio as tc on tc.id = s.id_tipo_cambio
--where tc.id = 1 or tc.id = 11

union

--extensiones
select x.id_solicitud, ts.descripcion as [tipo_solicitud], NULL as tipo_cambio, 'N/D' as [tipo_material], x.material, p.descripcion as [planta],
emp.nombre+' '+ISNULL(emp.apellido1,'')+' '+ISNULL(emp.apellido2,'') as solicitante, pr.descripcion as prioridad, s.fecha_creacion as fecha_solicitud
from SCDM_solicitud_rel_extension_usuario as x
join SCDM_solicitud AS S ON s.id = x.id_solicitud
join SCDM_cat_tipo_solicitud as ts on ts.id = s.id_tipo_solicitud
join plantas as p on p.clave = s.planta_solicitud
join SCDM_cat_prioridad as pr on pr.id = s.id_prioridad
join empleados as emp on emp.id = s.id_solicitante

union
--ordenes de compra
--Cambio en activaciones
select x.id_solicitud, ts.descripcion as [tipo_solicitud], tc.descripcion as tipo_cambio, 'N/D' as [tipo_material], x.num_material, p.descripcion as [planta],
emp.nombre+' '+ISNULL(emp.apellido1,'')+' '+ISNULL(emp.apellido2,'') as solicitante, pr.descripcion as prioridad, s.fecha_creacion as fecha_solicitud
from SCDM_solicitud_rel_orden_compra as x
join SCDM_solicitud AS S ON s.id = x.id_solicitud
join SCDM_cat_tipo_solicitud as ts on ts.id = s.id_tipo_solicitud
join plantas as p on p.clave = s.planta_solicitud
join SCDM_cat_prioridad as pr on pr.id = s.id_prioridad
join empleados as emp on emp.id = s.id_solicitante
/*left*/ join SCDM_cat_tipo_cambio as tc on tc.id = s.id_tipo_cambio

)t1
GO
