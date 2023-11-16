--use[Portal_2_0]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*****************************************************************************
*  Tipo de objeto:     Alter table
*  Autor :  Alfredo Xochitemol
*  Fecha de Creación: 
*  Log de Mantenimiento: 
*  Date          Modified By             Description            
*  ----------    --------------------    -------------------------------------
*  09/11/2023	Alfredo Xochitemol		 Se agrega indicador de si es suma
******************************************************************************/

BEGIN TRANSACTION
		IF object_id(N'budget_cantidad','U') IS NOT NULL
		BEGIN
			
			ALTER TABLE budget_cantidad ADD moneda_local_usd bit NOT NULL DEFAULT 1
			PRINT 'Se ha creado la moneda_local_usd class_1 en la tabla budget_cantidad'			
			
			--modifica restriccion unique
			ALTER TABLE [budget_cantidad]
			DROP CONSTRAINT UQ_budget_cantidad_anio_sap_mes;

			alter table [budget_cantidad]
			add constraint UQ_budget_cantidad_anio_sap_mes
			 unique (id_budget_rel_fy_centro,id_cuenta_sap,mes, currency_iso, moneda_local_usd);
			 
		END
		ELSE
		BEGIN
			PRINT 'La tabla budget_cantidad NO EXISTE, no se puede crear las columnas'
		END

COMMIT TRANSACTION

GO
