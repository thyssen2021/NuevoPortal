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
*  07/09/2022	Alfredo Xochitemol		 Se agrega campo para fecha de renovación (fin)
*  18/10/2022   Alfredo Xochitemol		 Se agrega campo para fecha de renovación (inicio) 
******************************************************************************/

BEGIN TRANSACTION
		IF object_id(N'IT_inventory_cellular_line','U') IS NOT NULL
		BEGIN
			
			ALTER TABLE IT_inventory_cellular_line ADD fecha_renovacion_inicio DATETIME NULL
			PRINT 'Se ha creado la columna fecha_renovacion en la tabla IT_inventory_cellular_line'

		END
		ELSE
		BEGIN
			PRINT 'La tabla IT_inventory_cellular_line NO EXISTE, no se puede crear las columnas'
		END

COMMIT TRANSACTION

GO



--actualiza las fechas de renovacion 
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-14', fecha_renovacion='2023-10-14' where numero_celular='8443623463'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2020-10-16', fecha_renovacion='2022-11-03' where numero_celular='8443480016'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-25', fecha_renovacion='2023-10-25' where numero_celular='8444446709'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-21', fecha_renovacion='2023-10-21' where numero_celular='8444273112'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-14', fecha_renovacion='2023-10-14' where numero_celular='8443506700'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-15', fecha_renovacion='2023-10-15' where numero_celular='8443507897'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2018-09-28', fecha_renovacion='2020-09-28' where numero_celular='4727371020'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2018-09-28', fecha_renovacion='2020-09-28' where numero_celular='4727374450'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2020-10-16', fecha_renovacion='2022-11-03' where numero_celular='4727387892'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2020-10-16', fecha_renovacion='2022-11-03' where numero_celular='4727387893'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2020-10-16', fecha_renovacion='2022-11-03' where numero_celular='4727387891'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2018-12-27', fecha_renovacion='2020-12-27' where numero_celular='4721378930'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2020-10-16', fecha_renovacion='2022-11-03' where numero_celular='4721378931'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2020-10-16', fecha_renovacion='2022-11-03' where numero_celular='4721019414'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2020-10-16', fecha_renovacion='2022-11-03' where numero_celular='4721077488'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2020-10-16', fecha_renovacion='2022-11-03' where numero_celular='4727371086'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2020-10-16', fecha_renovacion='2022-11-03' where numero_celular='4721591878'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2020-10-16', fecha_renovacion='2022-11-03' where numero_celular='4721591883'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2020-10-16', fecha_renovacion='2022-11-03' where numero_celular='4721087392'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2020-10-16', fecha_renovacion='2022-11-03' where numero_celular='4721409251'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2020-10-16', fecha_renovacion='2022-11-03' where numero_celular='4721409828'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2020-10-16', fecha_renovacion='2022-11-03' where numero_celular='4727388691'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-14', fecha_renovacion='2023-10-14' where numero_celular='4727377661'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-14', fecha_renovacion='2023-10-14' where numero_celular='4727377662'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-14', fecha_renovacion='2023-10-14' where numero_celular='4721339236'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-14', fecha_renovacion='2023-10-14' where numero_celular='4727377867'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-25', fecha_renovacion='2023-10-25' where numero_celular='4721074356'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-26', fecha_renovacion='2023-10-26' where numero_celular='4721010504'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-14', fecha_renovacion='2023-10-14' where numero_celular='4725652502'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-14', fecha_renovacion='2023-10-14' where numero_celular='4721010203'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-14', fecha_renovacion='2023-10-14' where numero_celular='4727377714'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-11-03', fecha_renovacion='2023-11-03' where numero_celular='4727388602'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-14', fecha_renovacion='2023-10-14' where numero_celular='4721617156'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-25', fecha_renovacion='2023-10-25' where numero_celular='4727387896'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-14', fecha_renovacion='2023-10-14' where numero_celular='4727387898'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-14', fecha_renovacion='2023-10-14' where numero_celular='4721010493'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-25', fecha_renovacion='2023-10-25' where numero_celular='4727388515'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2020-10-16', fecha_renovacion='2022-11-03' where numero_celular='4721372806'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2020-11-03', fecha_renovacion='2022-11-03' where numero_celular='4721379008'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-14', fecha_renovacion='2023-10-14' where numero_celular='4721010502'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-14', fecha_renovacion='2023-10-14' where numero_celular='4731246823'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-14', fecha_renovacion='2023-10-14' where numero_celular='4721378828'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-14', fecha_renovacion='2023-10-14' where numero_celular='4721378829'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-14', fecha_renovacion='2023-10-14' where numero_celular='4721378831'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-14', fecha_renovacion='2023-10-14' where numero_celular='4721378832'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-14', fecha_renovacion='2023-10-14' where numero_celular='4721378833'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2022-05-10', fecha_renovacion='2024-05-10' where numero_celular='4721339148'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2022-05-10', fecha_renovacion='2024-05-10' where numero_celular='4721339169'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2020-10-16', fecha_renovacion='2022-11-03' where numero_celular='2222650356'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-25', fecha_renovacion='2023-10-25' where numero_celular='2223520801'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2018-12-27', fecha_renovacion='2020-12-27' where numero_celular='2221400830'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2018-12-28', fecha_renovacion='2020-12-28' where numero_celular='2224265217'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2018-12-28', fecha_renovacion='2020-12-28' where numero_celular='2224265461'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2020-10-16', fecha_renovacion='2022-11-03' where numero_celular='2225887662'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2018-12-28', fecha_renovacion='2020-12-28' where numero_celular='2223461195'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2020-10-16', fecha_renovacion='2022-11-03' where numero_celular='2221141884'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2020-10-16', fecha_renovacion='2022-11-03' where numero_celular='2225647589'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2020-10-16', fecha_renovacion='2022-11-03' where numero_celular='2226610023'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2020-10-16', fecha_renovacion='2022-11-03' where numero_celular='2227094899'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2020-10-16', fecha_renovacion='2022-11-03' where numero_celular='2225772627'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2020-10-16', fecha_renovacion='2022-11-03' where numero_celular='2221140070'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2020-10-16', fecha_renovacion='2022-11-03' where numero_celular='2221236848'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2020-10-16', fecha_renovacion='2022-11-03' where numero_celular='2221543536'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2020-10-16', fecha_renovacion='2022-11-03' where numero_celular='2221765266'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2020-10-16', fecha_renovacion='2022-11-03' where numero_celular='2221774672'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2020-10-16', fecha_renovacion='2022-11-03' where numero_celular='2221861726'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-14', fecha_renovacion='2023-10-14' where numero_celular='2223569808'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2020-10-16', fecha_renovacion='2022-11-03' where numero_celular='2227082789'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2020-10-16', fecha_renovacion='2022-11-03' where numero_celular='2227082791'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2020-10-16', fecha_renovacion='2022-11-03' where numero_celular='2221360474'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-26', fecha_renovacion='2023-10-26' where numero_celular='2221394161'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-25', fecha_renovacion='2023-10-25' where numero_celular='2227107211'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2018-12-28', fecha_renovacion='2020-12-28' where numero_celular='2227107824'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2018-12-28', fecha_renovacion='2020-12-28' where numero_celular='2221394828'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2020-10-16', fecha_renovacion='2022-11-03' where numero_celular='2224841242'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-25', fecha_renovacion='2023-10-25' where numero_celular='2224597973'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2017-09-29', fecha_renovacion='2019-03-29' where numero_celular='2228138468'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2017-11-16', fecha_renovacion='2019-05-16' where numero_celular='2221141210'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2020-10-16', fecha_renovacion='2022-11-03' where numero_celular='2225645116'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2018-07-06', fecha_renovacion='2020-01-06' where numero_celular='2223788353'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2018-07-06', fecha_renovacion='2020-01-06' where numero_celular='2225178611'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2020-10-16', fecha_renovacion='2022-11-03' where numero_celular='2224556739'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2020-10-16', fecha_renovacion='2022-11-03' where numero_celular='2224584253'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2020-10-16', fecha_renovacion='2022-11-03' where numero_celular='2223362015'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2019-08-23', fecha_renovacion='2021-08-23' where numero_celular='2222650357'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-26', fecha_renovacion='2023-10-26' where numero_celular='2224557507'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-15', fecha_renovacion='2023-10-15' where numero_celular='2223059238'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2019-08-23', fecha_renovacion='2021-08-23' where numero_celular='2225546380'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-25', fecha_renovacion='2023-10-25' where numero_celular='2223358898'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-25', fecha_renovacion='2023-10-25' where numero_celular='2224290720'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-25', fecha_renovacion='2023-10-25' where numero_celular='2221862266'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-25', fecha_renovacion='2023-10-25' where numero_celular='2229540787'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-14', fecha_renovacion='2023-10-14' where numero_celular='2225547415'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-21', fecha_renovacion='2023-10-21' where numero_celular='2229132101'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-14', fecha_renovacion='2023-10-14' where numero_celular='2225547450'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-25', fecha_renovacion='2023-10-25' where numero_celular='2212724705'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-25', fecha_renovacion='2023-10-25' where numero_celular='2225547310'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-14', fecha_renovacion='2023-10-14' where numero_celular='2229540473'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-14', fecha_renovacion='2023-10-14' where numero_celular='2227081359'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-25', fecha_renovacion='2023-10-25' where numero_celular='2225547380'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-14', fecha_renovacion='2023-10-14' where numero_celular='2229139860'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-14', fecha_renovacion='2023-10-14' where numero_celular='2222650358'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-14', fecha_renovacion='2023-10-14' where numero_celular='2227094352'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-14', fecha_renovacion='2023-10-14' where numero_celular='2227094487'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-25', fecha_renovacion='2023-10-25' where numero_celular='2223106010'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-14', fecha_renovacion='2023-10-14' where numero_celular='2221202742'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-14', fecha_renovacion='2023-10-14' where numero_celular='2227094495'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-25', fecha_renovacion='2023-10-25' where numero_celular='2229139854'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-14', fecha_renovacion='2023-10-14' where numero_celular='2227094898'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-14', fecha_renovacion='2023-10-14' where numero_celular='2223244216'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-14', fecha_renovacion='2023-10-14' where numero_celular='2225547438'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-14', fecha_renovacion='2023-10-14' where numero_celular='2229132099'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-14', fecha_renovacion='2023-10-14' where numero_celular='2229206308'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-14', fecha_renovacion='2023-10-14' where numero_celular='2225549855'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-14', fecha_renovacion='2023-10-14' where numero_celular='2225545844'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-14', fecha_renovacion='2023-10-14' where numero_celular='2225506561'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-14', fecha_renovacion='2023-10-14' where numero_celular='2225668353'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-14', fecha_renovacion='2023-10-14' where numero_celular='2227077713'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-14', fecha_renovacion='2023-10-14' where numero_celular='2221577335'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-14', fecha_renovacion='2023-10-14' where numero_celular='2223246525'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-10-19', fecha_renovacion='2023-10-19' where numero_celular='2216672259'
UPDATE IT_inventory_cellular_line set fecha_renovacion_inicio='2021-11-08', fecha_renovacion='2023-11-08' where numero_celular='2216676260'
