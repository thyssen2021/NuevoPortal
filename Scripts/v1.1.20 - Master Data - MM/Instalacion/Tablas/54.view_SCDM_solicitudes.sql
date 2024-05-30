--use[Portal_2_0]
DROP VIEW IF EXISTS dbo.view_SCDM_solicitudes;  
/*****************************************************************************
*  Tipo de objeto: View
*  Funcion: Vista para mostrar el año fiscal 
*  Autor :  Alfredo Xochitemol Cruz
*  Fecha de Creación: 25/02/2022
*  Log de Mantenimiento: 
*  Date          Modified By             Description            
*  ----------    --------------------    -------------------------------------
*  
******************************************************************************/
		
GO
CREATE VIEW [dbo].view_SCDM_solicitudes AS

select top (500) s.id,t.descripcion as [tipo_solicitud], tc.descripcion as [tipo_cambio], pr.descripcion as [prioridad], plt.codigoSap as [planta_sap], plt.descripcion as [planta],
(e.nombre+' '+e.apellido1+' '+isnull(e.apellido2,'')) as [solicitante], s.descripcion, s.justificacion, s.fecha_creacion, s.on_hold, s.activo,
CASE
	WHEN (select count(*) from SCDM_solicitud_asignaciones where id_solicitud = s.id AND fecha_cierre is null AND fecha_rechazo is null) = 0 
		AND (select top 1 fecha_cierre from SCDM_solicitud_asignaciones where id_solicitud = s.id order by id desc) is not null 
		THEN 'Finalizada'
	ELSE 'En proceso'
END  as estatus
from SCDM_solicitud as s 
left join SCDM_cat_tipo_solicitud as t on s.id_tipo_solicitud = t.id
left join SCDM_cat_tipo_cambio as tc on tc.id = s.id_tipo_cambio
left join SCDM_cat_prioridad as pr on pr.id = s.id_prioridad
left join SCDM_rel_solicitud_plantas as relp on relp.id_solicitud = s.id
left join plantas as plt on plt.clave = relp.id_planta
left join empleados as e on e.id = s.id_solicitante
order by s.id desc
