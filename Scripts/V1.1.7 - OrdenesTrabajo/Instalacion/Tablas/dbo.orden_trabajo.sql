USE Portal_2_0
GO
IF object_id(N'orden_trabajo',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[orden_trabajo]
      PRINT '<<< orden_trabajo en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los datos orden_trabajo
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/02/14
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [orden_trabajo](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_solicitante][int] NOT NULL,
	[id_asignacion][int] NULL,
	[id_responsable][int] NULL,
	[id_area][int] NOT NULL,
	[id_linea][int] NULL,
	[id_documento_solicitud][int] NULL,
	[id_documento_cierre][int] NULL,
	[fecha_solicitud][datetime] NOT NULL,
	[nivel_urgencia][varchar](15) NOT NULL,
	[titulo][varchar](80) NOT NULL,
	[descripcion][varchar](300) NOT NULL,
	[fecha_asignacion][datetime] NULL,
	[fecha_en_proceso][datetime] NULL,
	[fecha_cierre][datetime] NULL,
	[estatus][varchar](20) NOT NULL,
	[comentario][varchar](300) NOT NULL,
 CONSTRAINT [PK_orden_trabajo] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

--agregar FK

-- restriccion de clave foranea
  alter table [orden_trabajo]
 add constraint FK_orden_trabajo_id_solicitante
  foreign key (id_solicitante)
  references empleados(id);

  -- restriccion de clave foranea
  alter table [orden_trabajo]
 add constraint FK_orden_trabajo_id_asignacion
  foreign key (id_asignacion)
  references empleados(id);

  -- restriccion de clave foranea
  alter table [orden_trabajo]
 add constraint FK_orden_trabajo_id_responsable
  foreign key (id_responsable)
  references empleados(id);

  -- restriccion de clave foranea
  alter table [orden_trabajo]
 add constraint FK_orden_trabajo_id_area
  foreign key (id_area)
  references Area(clave);

  -- restriccion de clave foranea
  alter table [orden_trabajo]
 add constraint FK_orden_trabajo_id_linea
  foreign key (id_linea)
  references produccion_lineas(id);

  -- restriccion de clave foranea
alter table [orden_trabajo]
 add constraint FK_orden_trabajo_id_documento_solicitud
  foreign key (id_documento_solicitud)
  references biblioteca_digital(id);

   -- restriccion de clave foranea
alter table [orden_trabajo]
 add constraint FK_orden_trabajo_id_documento_cierre
  foreign key (id_documento_cierre)
  references biblioteca_digital(id);

-- restricción default
ALTER TABLE [orden_trabajo] ADD  CONSTRAINT [DF_orden_trabajo_fecha_solicitud]  DEFAULT (GETDATE()) FOR [fecha_solicitud]

-- restricion check
ALTER TABLE [orden_trabajo] ADD CONSTRAINT CK_orden_trabajo_Estatus CHECK ([estatus] IN 
('ABIERTO','ASIGNADO','EN_PROCESO', 'CERRADO')
)
GO

 	  
IF object_id(N'orden_trabajo',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< orden_trabajo en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla orden_trabajo  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
