USE Portal_2_0
GO
IF object_id(N'OT_rel_archivos',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[OT_rel_archivos]
      PRINT '<<< OT_rel_archivos en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los OT_rel_archivos
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/02/14
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [OT_rel_archivos](
	[id][int] IDENTITY(1,1) NOT NULL,
	[id_orden_trabajo] [int] NOT NULL,
	[id_documento] [int] NOT NULL,
	[tipo][varchar](15) NOT NULL
 CONSTRAINT [PK_OT_rel_archivos] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


--agregar FK
-- restriccion de clave foranea
alter table [OT_rel_archivos]
 add constraint FK_OT_rel_archivos_id_orden_trabajo
  foreign key (id_orden_trabajo)
  references orden_trabajo(id);

  -- restriccion de clave foranea
alter table [OT_rel_archivos]
 add constraint FK_OT_rel_archivos_id_archivo
  foreign key (id_documento)
  references biblioteca_digital(id);

  -- restricion check
ALTER TABLE [OT_rel_archivos] ADD CONSTRAINT CK_OT_rel_archivos_tipo CHECK ([tipo] IN 
('SOLICITUD','CIERRE')
)
GO

	  
IF object_id(N'OT_rel_archivos',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< OT_rel_archivos en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla OT_rel_archivos  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
