USE [Portal_2_0]
GO

/*****************************************************************************
*  Tipo de objeto:     Alter table
*  Autor :  Alfredo Xochitemol
*  Fecha de Creación: 
*  Log de Mantenimiento: 
*  Date          Modified By             Description            
*  ----------    --------------------    -------------------------------------
*  06/09/2022	Alfredo Xochitemol		 Se agrega estatus "RECHAZADO_CONTABILIDAD"
******************************************************************************/
--elimina restricción check
ALTER TABLE poliza_manual
DROP CONSTRAINT CK_poliza_manual_Estatus;


-- restricion check
ALTER TABLE [poliza_manual] ADD CONSTRAINT CK_poliza_manual_Estatus CHECK ([estatus] IN 
('CREADO', 'ENVIADO_A_AREA', 'RECHAZADO_VALIDADOR', 'RECHAZADO_AUTORIZADOR', 'VALIDADO_POR_AREA',
'ENVIADO_SEGUNDA_VALIDACION', 'AUTORIZADO_SEGUNDA_VALIDACION', 'ENVIADO_A_CONTABILIDAD',
'ENVIADO_A_DIRECCION','RECHAZADO_DIRECCION', 'FINALIZADO',
'RECHAZADO_CONTABILIDAD'
)
)
GO