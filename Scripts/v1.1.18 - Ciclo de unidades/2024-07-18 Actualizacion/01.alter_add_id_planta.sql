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
*  21/03/2023	Alfredo Xochitemol		 Se agrega campo para placa  de plataforma 
*	 									 1 y 2.
******************************************************************************/

--Alter RU_registros
ALTER TABLE RU_registros
ADD id_planta int NOT NULL DEFAULT 1;

--   restriccion de clave foranea
alter table [dbo].RU_registros
 add constraint FK_RU_registros_id_planta
  foreign key (id_planta)
  references plantas(clave);

--Alter RU_usuarios_embarques
ALTER TABLE RU_usuarios_embarques
ADD id_planta int NOT NULL DEFAULT 1;

--   restriccion de clave foranea
alter table [dbo].RU_usuarios_embarques
 add constraint FK_RU_usuarios_embarques_id_planta
  foreign key (id_planta)
  references plantas(clave);

  --Alter RU_usuarios_vigilancia
ALTER TABLE RU_usuarios_vigilancia
ADD id_planta int NOT NULL DEFAULT 1;

--   restriccion de clave foranea
alter table [dbo].RU_usuarios_vigilancia
 add constraint FK_RU_usuarios_vigilancias_id_planta
  foreign key (id_planta)
  references plantas(clave);
   
  --Alter folio
ALTER TABLE RU_registros
ADD folio varchar(10) NOT NULL default '-' ;

--select id, folio from RU_registros

select count (*) as pendientes from RU_registros where folio ='-'

