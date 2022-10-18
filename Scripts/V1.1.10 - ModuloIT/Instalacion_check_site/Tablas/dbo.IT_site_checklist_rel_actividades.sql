USE Portal_2_0
GO
IF object_id(N'IT_site_checklist_rel_actividades',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IT_site_checklist_rel_actividades]
      PRINT '<<< IT_site_checklist_rel_actividades en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los IT_site_checklist_rel_actividades
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/10/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [IT_site_checklist_rel_actividades](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_site_checklist] [int] NOT NULL, 
	[id_it_site_actividad] [int] NOT NULL, 
	[observacion] [varchar](150) NULL,
	[estatus][varchar](10) NOT NULL
 CONSTRAINT [PK_IT_site_checklist_rel_actividades] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


-- restriccion de clave foranea
alter table [IT_site_checklist_rel_actividades]
 add constraint FK_site_checklist_id_site_checklist
  foreign key (id_site_checklist)
  references IT_site_checklist(id);

  -- restriccion de clave foranea
alter table [IT_site_checklist_rel_actividades]
 add constraint FK_site_checklist_id_it_site_actividad
  foreign key (id_it_site_actividad)
  references IT_site_Actividades(id);


  -- restricion check
ALTER TABLE [IT_site_checklist_rel_actividades] ADD CONSTRAINT CK_IT_site_checklist_rel_actividades_estatus CHECK ([estatus] IN 
('OK','NO OK','N/A')
)

   	  
IF object_id(N'IT_site_checklist_rel_actividades',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IT_site_checklist_rel_actividades en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IT_site_checklist_rel_actividades  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
