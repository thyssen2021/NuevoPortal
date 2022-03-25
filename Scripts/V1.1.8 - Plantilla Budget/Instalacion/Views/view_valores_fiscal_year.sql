use[Portal_2_0]
DROP VIEW IF EXISTS dbo.view_valores_fiscal_year;  
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
CREATE VIEW [dbo].view_valores_fiscal_year AS

SELECT ISNULL(ROW_NUMBER() OVER (ORDER BY t1.id_cuenta_sap), 0) as id,
t1.* FROM (SELECT  
		[id_budget_rel_fy_centro],
		fyc.id_anio_fiscal,
		fyc.id_centro_costo,
        v.[id_cuenta_sap],
		cs.sap_account,
		cs.name,
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
FROM budget_cantidad as v
		join budget_rel_fy_centro fyc on fyc.id = v.id_budget_rel_fy_centro
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
		bm.descripcion,
		bmb.descripcion,
		currency_iso)as t1

		
