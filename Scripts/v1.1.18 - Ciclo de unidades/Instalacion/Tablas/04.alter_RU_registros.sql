use Portal_2_0
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
*  21/03/2023	Alfredo Xochitemol		 Se agrega campo para placa  de plataforma 
*	 									 1 y 2.
******************************************************************************/

BEGIN TRANSACTION
		IF object_id(N'RU_registros','U') IS NOT NULL
		BEGIN
			
			ALTER TABLE RU_registros ADD placa_plataforma_uno varchar(20) NULL
			PRINT 'Se ha creado el campo placa_plataforma_uno en la tabla RU_registros'	
			
			ALTER TABLE RU_registros ADD placa_plataforma_dos varchar(20) NULL
			PRINT 'Se ha creado el campo placa_plataforma_dos en la tabla RU_registros'				
		END
		ELSE
		BEGIN
			PRINT 'La tabla RU_registros NO EXISTE, no se puede crear las columnas'
		END

COMMIT TRANSACTION

GO

