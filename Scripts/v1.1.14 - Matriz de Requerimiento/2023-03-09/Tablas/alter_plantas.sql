--use Portal_2_0
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
*  21/03/2023	Alfredo Xochitemol		 Se agrega campo para comentarios tkmmnet
******************************************************************************/

BEGIN TRANSACTION
		IF object_id(N'plantas','U') IS NOT NULL
		BEGIN
			
			ALTER TABLE plantas ADD tkorgstreet varchar(80) NULL
			PRINT 'Se ha creado el campo tkorgstreet en la tabla plantas'
			
			ALTER TABLE plantas ADD tkorgpostalcode varchar(5) NULL
			PRINT 'Se ha creado el campo tkorgpostalcode en la tabla plantas'
		
			ALTER TABLE plantas ADD tkorgpostaladdress varchar(80) NULL
			PRINT 'Se ha creado el campo tkorgpostaladdress en la tabla plantas'
		
			ALTER TABLE plantas ADD tkorgaddonaddr varchar(80) NULL
			PRINT 'Se ha creado el campo tkorgaddonaddr en la tabla plantas'
		
			ALTER TABLE plantas ADD tkorgfedst varchar(30) NULL
			PRINT 'Se ha creado el campo tkorgfedst en la tabla plantas'
		
			ALTER TABLE plantas ADD tkorgcountry varchar(30) NULL
			PRINT 'Se ha creado el campo tkorgcountry en la tabla plantas'

			ALTER TABLE plantas ADD tkorgcountrykey varchar(3) NULL
			PRINT 'Se ha creado el campo tkorgcountrykey en la tabla plantas'
			
			ALTER TABLE plantas ADD tkapsite varchar(80) NULL
			PRINT 'Se ha creado el campo tkapsite en la tabla plantas'
		

		END
		ELSE
		BEGIN
			PRINT 'La tabla plantas NO EXISTE, no se puede crear las columnas'
		END

COMMIT TRANSACTION

GO

