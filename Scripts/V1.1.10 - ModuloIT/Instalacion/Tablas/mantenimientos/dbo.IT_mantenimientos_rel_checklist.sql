USE Portal_2_0
GO
IF object_id(N'IT_mantenimientos_rel_checklist',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IT_mantenimientos_rel_checklist]
      PRINT '<<< IT_mantenimientos_rel_checklist en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los IT_mantenimientos_rel_checklist
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/06/06
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [IT_mantenimientos_rel_checklist](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_mantenimiento] [int] NOT NULL,
	[id_item_checklist_mantenimiento] [int] NOT NULL,
	[terminado] bit NULL,
	[comentarios] [varchar](300) NULL	
 CONSTRAINT [PK_IT_mantenimientos_rel_checklist] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

  -- restriccion de clave foranea
alter table [IT_mantenimientos_rel_checklist]
 add constraint FK_IT_mantenimientos_rel_checklist_id_mantenimiento
  foreign key (id_mantenimiento)
  references IT_mantenimientos(id);

    -- restriccion de clave foranea
alter table [IT_mantenimientos_rel_checklist]
 add constraint FK_IT_mantenimientos_rel_checklist_id_item_checklist_mantenimiento
  foreign key (id_item_checklist_mantenimiento)
  references IT_mantenimientos_checklist_item(id);

GO


 	  
IF object_id(N'IT_mantenimientos_rel_checklist',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IT_mantenimientos_rel_checklist en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IT_mantenimientos_rel_checklist  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
