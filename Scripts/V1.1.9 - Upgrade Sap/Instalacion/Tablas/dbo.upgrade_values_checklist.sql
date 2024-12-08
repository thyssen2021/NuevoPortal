USE Portal_2_0
GO
IF object_id(N'upgrade_values_checklist',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[upgrade_values_checklist]
      PRINT '<<< upgrade_values_checklist en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los upgrade_values_checklist
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/03/25
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [upgrade_values_checklist](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_revision] [int] NOT NULL,
	[id_checklist_item][int]NOT NULL,
	[estatus][varchar](20) NOT NULL,
	[nota][varchar](250) NULL,
	[activo] [bit] NOT NULL	
 CONSTRAINT [PK_upgrade_values_checklist] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restriccion de clave foranea
  alter table [upgrade_values_checklist]
 add constraint FK_upgrade_values_checklist_revision
  foreign key (id_revision)
  references upgrade_revision(id);

  -- restriccion de clave foranea
  alter table [upgrade_values_checklist]
 add constraint FK_upgrade_values_checklist_check_item
  foreign key (id_checklist_item)
  references upgrade_check_item(id);

  -- restricion check
ALTER TABLE [upgrade_values_checklist] ADD CONSTRAINT CK_upgrade_values_checklist_Estatus CHECK ([estatus] IN 
('PENDIENTE','OK','NO OK','N/A')
)
 	  
IF object_id(N'upgrade_values_checklist',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< upgrade_values_checklist en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla upgrade_values_checklist  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
