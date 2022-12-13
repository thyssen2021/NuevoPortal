use[Portal_2_0]
GO
IF object_id(N'GV_comprobacion',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[GV_comprobacion]
      PRINT '<<< GV_comprobacion en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los datos de entrada de producci�n
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creaci�n: 2021/10/06
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [dbo].[GV_comprobacion](	
	[id_gv_solicitud][int] NOT NULL,	
	[id_centro_costo][int] NOT NULL,
	[business_card][varchar](16) NULL,
	[fecha_aceptacion_jefe_area][datetime]  NULL,
	[fecha_aceptacion_controlling][datetime]  NULL,
	[fecha_aceptacion_contabilidad][datetime]  NULL,
	[fecha_aceptacion_nomina][datetime]  NULL,
	[comentario_rechazo][varchar](355) NULL,
	[comentario_adicional][varchar](355) NULL,
	[estatus][varchar](30) NOT NULL,
	
 CONSTRAINT [PK_GV_comprobacion] PRIMARY KEY CLUSTERED 
(
	[id_gv_solicitud] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restriccion de clave foranea
alter table [GV_comprobacion]
 add constraint FK_GV_comprobacion_id_gv_solicitud
  foreign key (id_gv_solicitud)
  references GV_solicitud(id);

  -- restriccion de clave foranea
alter table [GV_comprobacion]
 add constraint FK_GV_comprobacion_id_centro_costo
  foreign key (id_centro_costo)
  references GV_centros_costo(id);

GO

GO

-- restricion check
ALTER TABLE [GV_comprobacion] ADD CONSTRAINT CK_GV_comprobacion_Estatus CHECK ([estatus] IN 
('CREADO', 'ENVIADO_A_JEFE', 'RECHAZADO_JEFE', 'ENVIADO_CONTROLLING', 'RECHAZADO_CONTROLLING',
'ENVIADO_NOMINA','RECHAZADO_NOMINA', 'CONFIRMADO_NOMINA', 'RECHAZADO_CONTABILIDAD', 'CONFIRMADO_CONTABILIDAD', 
'FINALIZADO')
)
 	  
IF object_id(N'GV_comprobacion',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< GV_comprobacion en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla GV_comprobacion  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
