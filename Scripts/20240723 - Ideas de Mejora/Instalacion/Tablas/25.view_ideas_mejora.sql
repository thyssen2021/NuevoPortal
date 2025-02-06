--USE [Portal_2_0_calidad]
GO

/****** Object:  View [dbo].[view_ideas_mejora]    Script Date: 12/08/2024 11:07:02 a.m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


--select * from plantas
--select * from IM_cat_estatus
--select * from IM_rel_estatus

CREATE OR ALTER       VIEW [dbo].[view_ideas_mejora] AS

SELECT 
--ISNULL(ROW_NUMBER() OVER (ORDER BY im.id), 0) as consecutivo,
		im.id
		, CASE
			 WHEN clave_planta = 1 THEN 'PUE-'+ CONVERT(varchar(10), im.id)
			 WHEN clave_planta = 2 THEN 'SIL-'+ CONVERT(varchar(10), im.id)
			 WHEN clave_planta = 3 THEN 'SAL-'+ CONVERT(varchar(10), im.id)
			 WHEN clave_planta = 4 THEN 'SLP-'+ CONVERT(varchar(10), im.id)
			ELSE CONVERT(varchar(10), im.id)
		END AS folio
		, im.clave_planta
		, im.nombre_planta
		, im.captura as fecha_captura
		, im.titulo
		, im.situacionActual
		, im.objetivo            
		, im.descripcion
		,CASE
			WHEN im.comiteAceptada = 1 THEN Convert(bit, 1)
			ELSE Convert(bit, 0)
		END As comiteAceptada
		,CASE
			WHEN (select count(*) from IM_rel_proponente where ideaMejoraClave = im.id) > 1 THEN
			 Convert(bit, 1)
			ELSE Convert(bit, 0)
		END As ideaEnEquipo
		--clasificacion
		,im.clasificacionClave
		, (SELECT top 1 cl.descripcion from IM_cat_clasificacion as cl
			where cl.id = im.clasificacionClave
		) as Clasificacion_text		
		--nivel de impacto
		, im.nivelImpactoClave 
		, (SELECT top 1 imp.descripcion from IM_cat_nivel_impacto as imp
			where imp.id = im.nivelImpactoClave
		) as nivel_impacto_text		
		--proceso implementacion
		,CASE
			WHEN im.enProcesoImplementacion = 1 THEN Convert(bit, 1)
			ELSE Convert(bit, 0)
		END As enProcesoImplementacion
		--planta implementacion
		,(SELECT top 1 ar.id_planta from IM_cat_area as ar
			where ar.id = im.areaImplementacionClave
		) as plantaImplementacionClave
		,(SELECT top 1 pi.descripcion from IM_cat_area as ar
			join plantas as pi on pi.clave = ar.id_planta
			where ar.id = im.areaImplementacionClave
		) as planta_implementacion_text
		--area implementacion
		,im.areaImplementacionClave
		,(SELECT top 1 ar.descripcion from IM_cat_area as ar
			where ar.id = im.areaImplementacionClave
		) as area_implementacion_text	
		--IDEA IMPLEMENTADA
		,CASE
			WHEN im.ideaImplementada = 1 THEN Convert(bit, 1)
			ELSE Convert(bit, 0)
		END As ideaImplementada
		-- fecha de implementación
		,CASE
			WHEN year(im.implementacionFecha)<2000  THEN null
			ELSE im.implementacionFecha 
		END As implementacionFecha
		--reconocimiento
		,im.reconocimentoClave
		,(SELECT top 1 rc.descripcion from IM_cat_reconocimiento as rc
			where rc.id = im.reconocimentoClave
		) as reconocimiento_text
		--reconocimiento monto
		,im.reconocimientoMonto
		--estatus
		, (SELECT TOP 1 e.id from IM_rel_estatus as re 
			left join IM_cat_estatus as e on e.id = re.catalogoIdeaMejoraEstatusClave
			where ideaMejoraClave= im.id 
		order by re.id desc ) as estatus_id
	, (SELECT TOP 1 e.descripcion from IM_rel_estatus as re 
			left join IM_cat_estatus as e on e.id = re.catalogoIdeaMejoraEstatusClave
			where ideaMejoraClave= im.id 
		order by re.id desc ) as estatus_text
		,im.tipo_idea
		,
		--Concat Proponentes
		(SELECT stuff((
			SELECT '-' + CONVERT(varchar(10), '|' + CONVERT(varchar(10), e.id_empleado)+'|')
			FROM IM_rel_proponente e
			where im1.id = e.ideaMejoraClave
			FOR XML PATH('')
			), 1, 1, '') proponentes
			FROM IM_Idea_mejora as im1 where id = im.id) AS proponentes
		,	
		--Proponente 1 ID
		(
		select 
		 em1.id from IM_rel_proponente as p
		join empleados as em1 on em1.id  = p.id_empleado 
		where p.ideaMejoraClave = im.id 
		Order by p.id
		OFFSET 0 ROWS
		FETCH NEXT 1 ROWS ONLY
		)as [proponente_1_id],
		--Proponente 1 Nombre
		(
		select 
		'('+ISNULL(em.numeroEmpleado,'')+') '+em.nombre +' '+ ISNULL(em.apellido1,'') +' '+ ISNULL(em.apellido2,'') from IM_rel_proponente as p
		join empleados as em on em.id  = p.id_empleado
		where ideaMejoraClave = im.id 
		Order by p.id
		OFFSET 0 ROWS
		FETCH NEXT 1 ROWS ONLY
		)as [proponente_1_nombre],
		--Proponente 2 ID
		(
		select 
		 em1.id from IM_rel_proponente as p
		join empleados as em1 on em1.id  = p.id_empleado 
		where p.ideaMejoraClave = im.id 
		Order by p.id
		OFFSET 1 ROWS
		FETCH NEXT 1 ROWS ONLY
		)as [proponente_2_id],
		--Proponente 2 Nombre
		(
		select 
		'('+ISNULL(em.numeroEmpleado,'')+') '+em.nombre +' '+ ISNULL(em.apellido1,'') +' '+ ISNULL(em.apellido2,'') from IM_rel_proponente as p
		join empleados as em on em.id  = p.id_empleado
		where ideaMejoraClave = im.id 
		Order by p.id
		OFFSET 1 ROWS
		FETCH NEXT 1 ROWS ONLY
		)as [proponente_2_nombre]
		,--Proponente 3 ID
		(
		select 
		 em1.id from IM_rel_proponente as p
		join empleados as em1 on em1.id  = p.id_empleado 
		where p.ideaMejoraClave = im.id 
		Order by p.id
		OFFSET 2 ROWS
		FETCH NEXT 1 ROWS ONLY
		)as [proponente_3_id],
		--Proponente 3 Nombre
		(
		select 
		'('+ISNULL(em.numeroEmpleado,'')+') '+em.nombre +' '+ ISNULL(em.apellido1,'') +' '+ ISNULL(em.apellido2,'') from IM_rel_proponente as p
		join empleados as em on em.id  = p.id_empleado
		where ideaMejoraClave = im.id 
		Order by p.id
		OFFSET 2 ROWS
		FETCH NEXT 1 ROWS ONLY
		)as [proponente_3_nombre]
		,--Proponente 4 ID
		(
		select 
		 em1.id from IM_rel_proponente as p
		join empleados as em1 on em1.id  = p.id_empleado 
		where p.ideaMejoraClave = im.id 
		Order by p.id
		OFFSET 3 ROWS
		FETCH NEXT 1 ROWS ONLY
		)as [proponente_4_id],
		--Proponente 4 Nombre
		(
		select 
		'('+ISNULL(em.numeroEmpleado,'')+') '+em.nombre +' '+ ISNULL(em.apellido1,'') +' '+ ISNULL(em.apellido2,'') from IM_rel_proponente as p
		join empleados as em on em.id  = p.id_empleado
		where ideaMejoraClave = im.id 
		Order by p.id
		OFFSET 3 ROWS
		FETCH NEXT 1 ROWS ONLY
		)as [proponente_4_nombre]
		,--Proponente 5 ID
		(
		select 
		 em1.id from IM_rel_proponente as p
		join empleados as em1 on em1.id  = p.id_empleado 
		where p.ideaMejoraClave = im.id 
		Order by p.id
		OFFSET 4 ROWS
		FETCH NEXT 1 ROWS ONLY
		)as [proponente_5_id],
		--Proponente 5 Nombre
		(
		select 
		'('+ISNULL(em.numeroEmpleado,'')+') '+em.nombre +' '+ ISNULL(em.apellido1,'') +' '+ ISNULL(em.apellido2,'') from IM_rel_proponente as p
		join empleados as em on em.id  = p.id_empleado
		where ideaMejoraClave = im.id 
		Order by p.id
		OFFSET 4 ROWS
		FETCH NEXT 1 ROWS ONLY
		)as [proponente_5_nombre]
	FROM( select i.*, p.descripcion as nombre_planta from IM_Idea_mejora as i
		left join plantas as p on p.clave = i.clave_planta) im
GO

--select * from IM_cat_impacto
--select count(*) from IM_rel_proponente where ideaMejoraClave = 20
