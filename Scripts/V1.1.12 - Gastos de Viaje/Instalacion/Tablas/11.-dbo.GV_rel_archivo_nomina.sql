--use[Portal_2_0]
GO
IF object_id(N'GV_rel_archivo_nomina',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[GV_rel_archivo_nomina]
      PRINT '<<< GV_rel_archivo_nomina en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los datos de entrada de producción
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2021/10/06
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [dbo].[GV_rel_archivo_nomina](	
	[id] [int] IDENTITY(1,1) NOT NULL,	
	[id_gv_solicitud][int] NOT NULL,	--FK
	[id_biblioteca_digital][int] NOT NULL, --FK
	[cantidad][float] NOT NULL,
	[id_soporte_sap][int] NULL,   --FK
 CONSTRAINT [PK_GV_rel_archivo_nomina] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restriccion de clave foranea
alter table [GV_rel_archivo_nomina]
 add constraint FK_GV_rel_archivo_nomina_id_gv_solicitud
  foreign key (id_gv_solicitud)
  references GV_solicitud(id);

  -- restriccion de clave foranea
alter table [GV_rel_archivo_nomina]
 add constraint FK_GV_rel_archivo_nomina_id_biblioteca_digital
  foreign key (id_biblioteca_digital)
  references biblioteca_digital(id);

  
     -- restriccion de clave foranea
  alter table [GV_rel_archivo_nomina]
 add constraint FK_GV_rel_archivo_nomina_id_soporte_sap
  foreign key (id_soporte_sap)
  references biblioteca_digital(id);


GO

 	  
IF object_id(N'GV_rel_archivo_nomina',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< GV_rel_archivo_nomina en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla GV_rel_archivo_nomina  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
