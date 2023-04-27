USE Portal_2_0
GO
IF object_id(N'IT_notificaciones_checklist',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IT_notificaciones_checklist]
      PRINT '<<< IT_notificaciones_checklist en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los IT_notificaciones_checklist
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/04/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [IT_notificaciones_checklist](
	[id][int] IDENTITY(1,1) NOT NULL,
	[id_notificaciones_recordatorio] [int] NOT NULL,	--FK
	[descripcion][varchar](80) NOT NULL,
	[estatus][varchar](20)
 CONSTRAINT [PK_IT_notificaciones_checklist] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

--agregar FK

  -- restriccion de clave foranea
alter table [IT_notificaciones_checklist]
 add constraint FK_IT_notificaciones_checklist_id_notificaciones_recodatorio
  foreign key (id_notificaciones_recordatorio)
  references IT_notificaciones_recordatorio(id);

  
-- restricion check
ALTER TABLE [IT_notificaciones_checklist] ADD CONSTRAINT CK_IT_notificaciones_checklist_estatus CHECK ([estatus] IN 
('PENDIENTE', 'EN PROCESO', 'TERMINADO', 'N/A')
)
 
	  
IF object_id(N'IT_notificaciones_checklist',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IT_notificaciones_checklist en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IT_notificaciones_checklist  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
