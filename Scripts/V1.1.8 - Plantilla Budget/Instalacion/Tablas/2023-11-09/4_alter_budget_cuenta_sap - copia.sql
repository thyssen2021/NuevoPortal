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
		IF object_id(N'budget_cuenta_sap','U') IS NOT NULL
		BEGIN			
			
			ALTER TABLE budget_cuenta_sap ADD aplica_formula bit NULL
			PRINT 'Se ha creado la columna aplica_formula en la tabla budget_cuenta_sap'		

			ALTER TABLE budget_cuenta_sap ADD formula [varchar](30) NULL
			PRINT 'Se ha creado la columna formula en la tabla budget_cuenta_sap'					
			

		END
		ELSE
		BEGIN
			PRINT 'La tabla budget_cuenta_sap NO EXISTE, no se puede crear las columnas'
		END

COMMIT TRANSACTION

--update cuentas sap
update budget_cuenta_sap set aplica_formula = 1, formula = N'(a*b*c) + (d*e*f)' where sap_account='610030'

GO


