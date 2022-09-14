USE Portal_2_0
GO
IF object_id(N'GV_solicitud',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[GV_solicitud]
      PRINT '<<< GV_solicitud en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los datos GV_solicitud
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/08/17
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [GV_solicitud](
	[id] [int] IDENTITY(1,1) NOT NULL,	
	[id_solicitante][int] NOT NULL, --de la tabla empleados
	[id_empleado][int] NOT NULL, --de la tabla empleados
	[id_jefe_directo][int] NOT NULL, --de la tabla empleados
	[id_controlling][int] NULL, --de la tabla empleados
	[id_contabilidad][int] NULL, --de la tabla empleados
	[id_nomina][int] NULL, --de la tabla empleados
	[fecha_solicitud][datetime] NOT NULL,
	[tipo_viaje][varchar](15) NOT NULL,
	[origen][varchar](100) NOT NULL,
	[destino][varchar](100) NOT NULL,
	[fecha_salida][datetime] NOT NULL,
	[fecha_regreso][datetime] NOT NULL,
	[id_medio_transporte][int] NULL,    --FK
	[medio_transporte_otro][varchar](80) NULL,
	[motivo_viaje][VARCHAR](350) NOT NULL,
	[moneda_extranjera][bit] NOT NULL,
	[moneda_extranjera_comentarios][VARCHAR](120) NULL,
	[moneda_extranjera_monto][DECIMAL](12,2) NULL,
	[iso_moneda_extranjera][varchar](3) NUll, --FK
	[boletos_avion][bit] NOT NULL,
	--[boletos_avion_comentarios][VARCHAR](120) NULL,
	[hospedaje][bit] NOT NULL,
	[reservaciones_comentarios][VARCHAR](120) NULL,
	[fecha_aceptacion_jefe_area][datetime]  NULL,
	[fecha_aceptacion_controlling][datetime]  NULL,
	[fecha_aceptacion_contabilidad][datetime]  NULL,
	[fecha_aceptacion_nomina][datetime]  NULL,
	[comentario_rechazo][varchar](355) NULL,
	[comentario_adicional][varchar](355) NULL,
	[estatus][varchar](30) NOT NULL,
 CONSTRAINT [PK_GV_solicitud] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

--agregar FK

  -- restriccion de clave foranea
alter table [GV_solicitud]
 add constraint FK_GV_solicitud_iso_moneda_extranjera
  foreign key (iso_moneda_extranjera)
  references currency(CurrencyISO);
  
    -- restriccion de clave foranea
  alter table [GV_solicitud]
 add constraint FK_GV_solicitud_id_solicitante
  foreign key (id_solicitante)
  references empleados(id);

  -- restriccion de clave foranea
  alter table [GV_solicitud]
 add constraint FK_GV_solicitud_id_empleado
  foreign key (id_empleado)
  references empleados(id);

  -- restriccion de clave foranea
  alter table [GV_solicitud]
 add constraint FK_GV_solicitud_id_jefe_directo
  foreign key (id_jefe_directo)
  references empleados(id);

   -- restriccion de clave foranea
  alter table [GV_solicitud]
 add constraint FK_GV_solicitud_contabilidad
  foreign key (id_contabilidad)
  references empleados(id);

     -- restriccion de clave foranea
  alter table [GV_solicitud]
 add constraint FK_GV_solicitud_id_controlling
  foreign key (id_controlling)
  references empleados(id);

      -- restriccion de clave foranea
  alter table [GV_solicitud]
 add constraint FK_GV_solicitud_id_nomina
  foreign key (id_nomina)
  references empleados(id);

        -- restriccion de clave foranea
  alter table [GV_solicitud]
 add constraint FK_GV_solicitud_id_medio_transporte
  foreign key (id_medio_transporte)
  references GV_medios_transporte(id);


-- restricción default
ALTER TABLE [GV_solicitud] ADD  CONSTRAINT [DF_GV_solicitud_fecha_creacion]  DEFAULT (GETDATE()) FOR [fecha_solicitud]

-- restricion check
ALTER TABLE [GV_solicitud] ADD CONSTRAINT CK_GV_solicitud_tipo_viaje CHECK ([tipo_viaje] IN 
('NACIONAL', 'EXTRANJERO')
)

-- restricion check
ALTER TABLE [GV_solicitud] ADD CONSTRAINT CK_GV_solicitud_Estatus CHECK ([estatus] IN 
('CREADO', 'ENVIADO_A_JEFE', 'RECHAZADO_JEFE', 'ENVIADO_CONTROLLING', 'RECHAZADO_CONTROLLING',
'ENVIADO_CONTABILIDAD', 'RECHAZADO_CONTABILIDAD', 'ENVIADO_NOMINA',
'RECHAZADO_NOMINA','FINALIZADO')
)
GO

 	  
IF object_id(N'GV_solicitud',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< GV_solicitud en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla GV_solicitud  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
