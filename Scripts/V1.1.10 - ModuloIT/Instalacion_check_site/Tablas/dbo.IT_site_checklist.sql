USE Portal_2_0
GO
IF object_id(N'IT_site_checklist',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IT_site_checklist]
      PRINT '<<< IT_site_checklist en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los IT_site_checklist
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/10/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [IT_site_checklist](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_site] [int] NOT NULL, 
	[id_sistemas] [int] NOT NULL, 
	[observacion_particular] [varchar](250) NULL,
	[fecha][datetime] NULL,
	[estatus][varchar](20) NOT NULL
 CONSTRAINT [PK_IT_site_checklist] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


-- restriccion de clave foranea
alter table [IT_site_checklist]
 add constraint FK_site_checklist_id_site
  foreign key (id_site)
  references IT_site(id);

  -- restriccion de clave foranea
alter table [IT_site_checklist]
 add constraint FK_site_checklist_id_sistemas
  foreign key (id_sistemas)
  references empleados(id);


  -- restricion check
ALTER TABLE [IT_site_checklist] ADD CONSTRAINT CK_IT_site_checklist_estatus CHECK ([estatus] IN 
('INICIADO','EN_PROCESO','FINALIZADO')
)

   	  
IF object_id(N'IT_site_checklist',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IT_site_checklist en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IT_site_checklist  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
