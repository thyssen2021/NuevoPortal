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

--BEGIN TRANSACTION
		IF object_id(N'budget_cuenta_sap','U') IS NOT NULL
		BEGIN			
			
			ALTER TABLE budget_cuenta_sap ADD aplica_formula bit NULL
			PRINT 'Se ha creado la columna aplica_formula en la tabla budget_cuenta_sap'		

			ALTER TABLE budget_cuenta_sap ADD formula [varchar](30) NULL
			PRINT 'Se ha creado la columna formula en la tabla budget_cuenta_sap'					
			
			ALTER TABLE budget_cuenta_sap ADD aplica_mxn bit NOT NULL default 0
			PRINT 'Se ha creado la columna aplica_mxn en la tabla budget_cuenta_sap'							

			ALTER TABLE budget_cuenta_sap ADD aplica_usd bit NOT NULL default 0
			PRINT 'Se ha creado la columna aplica_mxn en la tabla budget_cuenta_sap'

			ALTER TABLE budget_cuenta_sap ADD aplica_eur bit NOT NULL default 0
			PRINT 'Se ha creado la columna aplica_mxn en la tabla budget_cuenta_sap'

			ALTER TABLE budget_cuenta_sap ADD aplica_gastos_mantenimiento bit NOT NULL default 0
			PRINT 'Se ha creado la columna aplica_mxn en la tabla budget_cuenta_sap'


		END
		ELSE
		BEGIN
			PRINT 'La tabla budget_cuenta_sap NO EXISTE, no se puede crear las columnas'
		END

--COMMIT TRANSACTION


--update cuentas sap
update budget_cuenta_sap set aplica_formula = 1, formula = N'(a*b*c) + (d*e*f)' where sap_account='610030'
update budget_cuenta_sap set aplica_formula = 1, formula = N'(a*b*c) + (d*e*f)' where sap_account='610040'
update budget_cuenta_sap set aplica_formula = 1, formula = N'(a*b)+(c*d)' where sap_account='610070'
update budget_cuenta_sap set aplica_formula = 1, formula = N'a+b+c+d+e+f' where sap_account='650030'
update budget_cuenta_sap set aplica_formula = 1, formula = N'a+b+c' where sap_account='650031'
update budget_cuenta_sap set aplica_formula = 1, formula = N'a+b+c' where sap_account='650032'
update budget_cuenta_sap set aplica_formula = 1, formula = N'(a*b)+c' where sap_account='630060'
update budget_cuenta_sap set aplica_formula = 1, formula = N'(a*b)+c' where sap_account='630065'
update budget_cuenta_sap set aplica_formula = 1, formula = N'a+b+c' where sap_account='650011'
update budget_cuenta_sap set aplica_formula = 1, formula = N'a+b+c+d+e+f+g' where sap_account='650012'
update budget_cuenta_sap set aplica_formula = 1, formula = N'a+b+c+d+e+f' where sap_account='652100'
update budget_cuenta_sap set aplica_formula = 1, formula = N'a+b+c+d+e' where sap_account='630040'
update budget_cuenta_sap set aplica_formula = 1, formula = N'a+b+c+d+e+f+g+h+i+j+k+l' where sap_account='612010'
update budget_cuenta_sap set aplica_formula = 1, formula = N'a*b' where sap_account='630010'
update budget_cuenta_sap set aplica_formula = 1, formula = N'(a*b)+(c*d)' where sap_account='630020'
update budget_cuenta_sap set aplica_formula = 1, formula = N'a+b' where sap_account='630025'
update budget_cuenta_sap set aplica_formula = 1, formula = N'a+b+c' where sap_account='708040'
update budget_cuenta_sap set aplica_formula = 1, formula = N'a*b' where sap_account='708070'

--update settings de calculo moneda
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 0, aplica_eur = 0 where sap_account='610010';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 0, aplica_eur = 0 where sap_account='610030';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 0, aplica_eur = 0 where sap_account='610040';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 0, aplica_eur = 0 where sap_account='610070';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 0, aplica_eur = 0 where sap_account='610071';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 0, aplica_eur = 0 where sap_account='610072';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 0, aplica_eur = 0 where sap_account='610073';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 0, aplica_eur = 0 where sap_account='610074';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 0, aplica_eur = 0 where sap_account='610080';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 1, aplica_eur = 0 where sap_account='650030';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 1, aplica_eur = 0 where sap_account='650031';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 1, aplica_eur = 0 where sap_account='650032';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 1, aplica_eur = 0 where sap_account='610090';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 0, aplica_eur = 0 where sap_account='610091';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 0, aplica_eur = 0 where sap_account='630060';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 0, aplica_eur = 0 where sap_account='630065';
update budget_cuenta_sap set aplica_mxn = 0, aplica_usd = 0, aplica_eur = 0 where sap_account='630066';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 1, aplica_eur = 0 where sap_account='630067';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 1, aplica_eur = 0 where sap_account='650011';
update budget_cuenta_sap set aplica_mxn = 0, aplica_usd = 1, aplica_eur = 1 where sap_account='650012';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 0, aplica_eur = 0 where sap_account='650019';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 1, aplica_eur = 0 where sap_account='652000';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 1, aplica_eur = 0 where sap_account='652100';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 1, aplica_eur = 0 where sap_account='652200';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 0, aplica_eur = 0 where sap_account='653000';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 1, aplica_eur = 0 where sap_account='630040';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 0, aplica_eur = 0 where sap_account='706020';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 0, aplica_eur = 0 where sap_account='630050';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 0, aplica_eur = 0 where sap_account='650001';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 0, aplica_eur = 0 where sap_account='651000';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 0, aplica_eur = 0 where sap_account='651010';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 0, aplica_eur = 0 where sap_account='651020';
update budget_cuenta_sap set aplica_mxn = 0, aplica_usd = 0, aplica_eur = 0 where sap_account='651100';
update budget_cuenta_sap set aplica_mxn = 0, aplica_usd = 0, aplica_eur = 0 where sap_account='705040';
update budget_cuenta_sap set aplica_mxn = 0, aplica_usd = 0, aplica_eur = 0 where sap_account='705050';
update budget_cuenta_sap set aplica_mxn = 0, aplica_usd = 0, aplica_eur = 0 where sap_account='650018';
update budget_cuenta_sap set aplica_mxn = 0, aplica_usd = 0, aplica_eur = 0 where sap_account='650002';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 0, aplica_eur = 0 where sap_account='612010';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 0, aplica_eur = 0 where sap_account='612029';
update budget_cuenta_sap set aplica_mxn = 0, aplica_usd = 0, aplica_eur = 0 where sap_account='650025';
update budget_cuenta_sap set aplica_mxn = 0, aplica_usd = 0, aplica_eur = 0 where sap_account='650040';
update budget_cuenta_sap set aplica_mxn = 0, aplica_usd = 0, aplica_eur = 0 where sap_account='650041';
update budget_cuenta_sap set aplica_mxn = 0, aplica_usd = 0, aplica_eur = 0 where sap_account='651101';
update budget_cuenta_sap set aplica_mxn = 0, aplica_usd = 0, aplica_eur = 0 where sap_account='660000';
update budget_cuenta_sap set aplica_mxn = 0, aplica_usd = 0, aplica_eur = 0 where sap_account='660010';
update budget_cuenta_sap set aplica_mxn = 0, aplica_usd = 0, aplica_eur = 0 where sap_account='660011';
update budget_cuenta_sap set aplica_mxn = 0, aplica_usd = 0, aplica_eur = 0 where sap_account='660012';
update budget_cuenta_sap set aplica_mxn = 0, aplica_usd = 0, aplica_eur = 0 where sap_account='660020';
update budget_cuenta_sap set aplica_mxn = 0, aplica_usd = 0, aplica_eur = 0 where sap_account='690100';
update budget_cuenta_sap set aplica_mxn = 0, aplica_usd = 0, aplica_eur = 0 where sap_account='700000';
update budget_cuenta_sap set aplica_mxn = 0, aplica_usd = 0, aplica_eur = 0 where sap_account='700300';
update budget_cuenta_sap set aplica_mxn = 0, aplica_usd = 0, aplica_eur = 0 where sap_account='700600';
update budget_cuenta_sap set aplica_mxn = 0, aplica_usd = 0, aplica_eur = 0 where sap_account='700840';
update budget_cuenta_sap set aplica_mxn = 0, aplica_usd = 0, aplica_eur = 0 where sap_account='704090';
update budget_cuenta_sap set aplica_mxn = 0, aplica_usd = 0, aplica_eur = 0 where sap_account='705020';
update budget_cuenta_sap set aplica_mxn = 0, aplica_usd = 0, aplica_eur = 0 where sap_account='706000';
update budget_cuenta_sap set aplica_mxn = 0, aplica_usd = 0, aplica_eur = 0 where sap_account='706010';
update budget_cuenta_sap set aplica_mxn = 0, aplica_usd = 0, aplica_eur = 0 where sap_account='704080';
update budget_cuenta_sap set aplica_mxn = 0, aplica_usd = 0, aplica_eur = 0 where sap_account='704100';
update budget_cuenta_sap set aplica_mxn = 0, aplica_usd = 0, aplica_eur = 0 where sap_account='700310';
update budget_cuenta_sap set aplica_mxn = 0, aplica_usd = 0, aplica_eur = 0 where sap_account='704020';
update budget_cuenta_sap set aplica_mxn = 0, aplica_usd = 0, aplica_eur = 0 where sap_account='700910';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 0, aplica_eur = 0 where sap_account='630010';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 0, aplica_eur = 0 where sap_account='630020';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 0, aplica_eur = 0 where sap_account='630025';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 0, aplica_eur = 0 where sap_account='704110';
update budget_cuenta_sap set aplica_mxn = 0, aplica_usd = 1, aplica_eur = 0 where sap_account='704200';
update budget_cuenta_sap set aplica_mxn = 0, aplica_usd = 0, aplica_eur = 0 where sap_account='704220';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 0, aplica_eur = 0 where sap_account='707000';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 1, aplica_eur = 0 where sap_account='707010';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 1, aplica_eur = 0 where sap_account='707020';
update budget_cuenta_sap set aplica_mxn = 0, aplica_usd = 1, aplica_eur = 0 where sap_account='707021';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 1, aplica_eur = 1 where sap_account='707030';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 1, aplica_eur = 0 where sap_account='707040';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 1, aplica_eur = 1 where sap_account='707050';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 1, aplica_eur = 1 where sap_account='707062';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 1, aplica_eur = 0 where sap_account='707063';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 1, aplica_eur = 1 where sap_account='707064';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 1, aplica_eur = 0 where sap_account='707065';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 0, aplica_eur = 0 where sap_account='707066';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 0, aplica_eur = 0 where sap_account='707067';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 1, aplica_eur = 0 where sap_account='707068';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 1, aplica_eur = 0 where sap_account='707100';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 1, aplica_eur = 0 where sap_account='707110';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 1, aplica_eur = 0 where sap_account='707120';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 1, aplica_eur = 0 where sap_account='707130';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 1, aplica_eur = 0 where sap_account='707140';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 1, aplica_eur = 0 where sap_account='708000';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 1, aplica_eur = 0 where sap_account='708010';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 1, aplica_eur = 0 where sap_account='708011';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 1, aplica_eur = 0 where sap_account='708020';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 1, aplica_eur = 1 where sap_account='708021';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 0, aplica_eur = 0 where sap_account='708022';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 1, aplica_eur = 0 where sap_account='708040';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 1, aplica_eur = 0 where sap_account='708050';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 1, aplica_eur = 0 where sap_account='708060';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 0, aplica_eur = 0 where sap_account='708070';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 1, aplica_eur = 0 where sap_account='708080';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 1, aplica_eur = 0 where sap_account='708210';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 1, aplica_eur = 0 where sap_account='708251';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 1, aplica_eur = 0 where sap_account='708252';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 0, aplica_eur = 0 where sap_account='708253';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 1, aplica_eur = 0 where sap_account='709010';
update budget_cuenta_sap set aplica_mxn = 0, aplica_usd = 0, aplica_eur = 0 where sap_account='630030';
update budget_cuenta_sap set aplica_mxn = 0, aplica_usd = 0, aplica_eur = 0 where sap_account='708200';
update budget_cuenta_sap set aplica_mxn = 0, aplica_usd = 0, aplica_eur = 0 where sap_account='707061';
update budget_cuenta_sap set aplica_mxn = 0, aplica_usd = 0, aplica_eur = 0 where sap_account='707060';
update budget_cuenta_sap set aplica_mxn = 0, aplica_usd = 0, aplica_eur = 0 where sap_account='611020';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 0, aplica_eur = 0 where sap_account='708120';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 0, aplica_eur = 0 where sap_account='708160';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 1, aplica_eur = 0 where sap_account='708161';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 1, aplica_eur = 0 where sap_account='708162';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 0, aplica_eur = 0 where sap_account='708170';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 0, aplica_eur = 0 where sap_account='708180';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 1, aplica_eur = 0 where sap_account='708190';
update budget_cuenta_sap set aplica_mxn = 1, aplica_usd = 0, aplica_eur = 0 where sap_account='707001';



