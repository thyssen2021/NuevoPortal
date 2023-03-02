USE [Portal_2_0]
GO

--borra la vista si existe
IF OBJECT_ID ('dbo.view_valores_fiscal_year_budget_historico', 'V') IS NOT NULL  
DROP VIEW [dbo].[view_valores_fiscal_year_budget_historico];  
GO  

/****** Object:  View [dbo].[view_valores_fiscal_year_budget_historico]    Script Date: 23/02/2023 01:53:23 p.m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[view_valores_fiscal_year_budget_historico] AS

SELECT ISNULL(ROW_NUMBER() OVER (ORDER BY t1.id_cuenta_sap), 0) as id,
t1.* FROM (
SELECT  
		[id_budget_rel_fy_centro],
		fyc.id_anio_fiscal,
		fyc.id_centro_costo,
        v.[id_cuenta_sap],
		cs.sap_account,
		cs.name,
		cc.num_centro_costo as cost_center,
		cc.descripcion as department,
		cc.class_1,
		cc.class_2,
		(Select SUBSTRING( 
		( 
			 SELECT '/' + ISNULL(e.nombre,'')+' '+ ISNULL(e.apellido1,'') +' '+ ISNULL(e.apellido2,'')  'data()'
				 FROM budget_responsables as r 
				 join empleados e on e.id = r.id_responsable
					where r.id_budget_centro_costo = fyc.id_centro_costo
				 FOR XML PATH('') 
		), 2 , 9999)) AS responsable,
		bp.codigo_sap,
		bp.id as id_budget_plant,
		bm.descripcion as mapping,
		bmb.descripcion as [mapping_bridge],
		currency_iso,			
        SUM(CASE WHEN [Mes] = 1 THEN [Cantidad] ELSE NULL END) [Enero],
		SUM(CASE WHEN [Mes] = 2 THEN [Cantidad] ELSE NULL END) [Febrero],
		SUM(CASE WHEN [Mes] = 3 THEN [Cantidad] ELSE NULL END) [Marzo],
		SUM(CASE WHEN [Mes] = 4 THEN [Cantidad] ELSE NULL END) [Abril],
		SUM(CASE WHEN [Mes] = 5 THEN [Cantidad] ELSE NULL END) [Mayo],
		SUM(CASE WHEN [Mes] = 6 THEN [Cantidad] ELSE NULL END) [Junio],
		SUM(CASE WHEN [Mes] = 7 THEN [Cantidad] ELSE NULL END) [Julio],
		SUM(CASE WHEN [Mes] = 8 THEN [Cantidad] ELSE NULL END) [Agosto],
		SUM(CASE WHEN [Mes] = 9 THEN [Cantidad] ELSE NULL END) [Septiembre],
		SUM(CASE WHEN [Mes] = 10 THEN [Cantidad] ELSE NULL END) [Octubre],
		SUM(CASE WHEN [Mes] = 11 THEN [Cantidad] ELSE NULL END) [Noviembre],
		SUM(CASE WHEN [Mes] = 12 THEN [Cantidad] ELSE NULL END) [Diciembre],
		(SELECT top(1) comentarios FROM budget_rel_comentarios as c WHERE c.id_cuenta_sap=v.id_cuenta_sap AND c.id_budget_rel_fy_centro=v.id_budget_rel_fy_centro AND c.id_cuenta_sap = v.id_cuenta_sap) AS [Comentario]
FROM budget_cantidad_budget_historico as v
		join budget_rel_fy_centro fyc on fyc.id = v.id_budget_rel_fy_centro
		join budget_centro_costo cc on cc.id = fyc.id_centro_costo
		join budget_departamentos bd on bd.id = cc.id_budget_departamento
		join budget_plantas bp on bp.id = bd.id_budget_planta
		join budget_cuenta_sap cs on cs.id = v.id_cuenta_sap
		join budget_mapping bm on bm.id = cs.id_mapping
		join budget_mapping_bridge bmb on bmb.id = bm.id_mapping_bridge
GROUP BY 
		[id_budget_rel_fy_centro],
		fyc.id_anio_fiscal,
		fyc.id_centro_costo,
        v.[id_cuenta_sap],
		cs.sap_account,
		cs.name,
		cc.num_centro_costo,
		cc.descripcion,
		cc.class_1,
		cc.class_2,
		bp.codigo_sap,
		bp.id,
		bm.descripcion,
		bmb.descripcion,
		currency_iso

		UNION ALL

		select 
		cm.[id_budget_rel_fy_centro],
		fyc.id_anio_fiscal,
		fyc.id_centro_costo,
		cm.[id_cuenta_sap],
		cs.sap_account,
		cs.name,
		cc.num_centro_costo as cost_center,
		cc.descripcion as department,
		cc.class_1,
		cc.class_2,
		(Select SUBSTRING( 
		( 
			 SELECT '/' + ISNULL(e.nombre,'')+' '+ ISNULL(e.apellido1,'') +' '+ ISNULL(e.apellido2,'')  'data()'
				 FROM budget_responsables as r 
				 join empleados e on e.id = r.id_responsable
					where r.id_budget_centro_costo = fyc.id_centro_costo
				 FOR XML PATH('') 
		), 2 , 9999)) AS responsable,
		bp.codigo_sap,
		bp.id as id_budget_plant,
		bm.descripcion as mapping,
		bmb.descripcion as [mapping_bridge],
		(Select 'USD')[currency_iso],			
       null [Enero],
		null [Febrero],
		null [Marzo],
		null [Abril],
		null [Mayo],
		null [Junio],
		null [Julio],
		null [Agosto],
		null [Septiembre],
		null [Octubre],
		null [Noviembre],
		null [Diciembre],
		cm.comentarios
 from budget_rel_comentarios as cm
		join budget_rel_fy_centro fyc on fyc.id = cm.id_budget_rel_fy_centro
		join budget_centro_costo cc on cc.id = fyc.id_centro_costo
		join budget_departamentos bd on bd.id = cc.id_budget_departamento
		join budget_plantas bp on bp.id = bd.id_budget_planta
		join budget_cuenta_sap cs on cs.id = cm.id_cuenta_sap
		join budget_mapping bm on bm.id = cs.id_mapping
		join budget_mapping_bridge bmb on bmb.id = bm.id_mapping_bridge
where (SELECT COUNT(*) FROM budget_cantidad_budget_historico as bc WHERE bc.id_budget_rel_fy_centro=cm.id_budget_rel_fy_centro AND bc.id_cuenta_sap=cm.id_cuenta_sap )=0
)t1

GO



