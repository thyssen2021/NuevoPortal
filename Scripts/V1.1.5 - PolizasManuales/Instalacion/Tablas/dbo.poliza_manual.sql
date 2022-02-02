USE Portal_2_0
GO
IF object_id(N'poliza_manual',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[poliza_manual]
      PRINT '<<< poliza_manual en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los datos poliza_manual
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2021/12/09
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [poliza_manual](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_PM_tipo_poliza][int] NOT NULL,
	[currency_iso][varchar](3) NOT NULL,
	[id_planta][int] NOT NULL,
	[id_elaborador][int] NOT NULL,
	[id_validador][int] NULL, --de la tabla empleados
	[id_autorizador][int] NULL, --de la tabla empleados
	[id_contabilidad][int] NULL, --de la tabla empleados
	[id_direccion][int] NULL, --de la tabla empleados
	[id_documento_soporte][int] NULL,
	[id_documento_registro][int] NULL,
	[numero_documento_sap][varchar](15) NULL,
	[fecha_creacion][datetime] NOT NULL, --agregar default
	[fecha_documento][datetime] NOT NULL,
	[fecha_validacion][datetime] NULL,
	[fecha_autorizacion][datetime] NULL,
	[fecha_direccion][datetime] NULL,
	[fecha_registro][datetime] NULL,
	[comentario_rechazo][varchar](355) NULL,
	[descripcion_poliza][varchar](355) NULL,
	[estatus][varchar](30) NOT NULL,
 CONSTRAINT [PK_poliza_manual] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

--agregar FK
-- restriccion de clave foranea
alter table [poliza_manual]
 add constraint FK_poliza_manual_tipo_poliza
  foreign key (id_PM_tipo_poliza)
  references PM_tipo_poliza(id);

  -- restriccion de clave foranea
alter table [poliza_manual]
 add constraint FK_poliza_manual_currency
  foreign key (currency_iso)
  references currency(CurrencyISO);

     -- restriccion de clave foranea
  alter table [poliza_manual]
 add constraint FK_poliza_manual_planta
  foreign key (id_planta)
  references plantas(clave);

    -- restriccion de clave foranea
  alter table [poliza_manual]
 add constraint FK_poliza_manual_elaborador
  foreign key (id_elaborador)
  references empleados(id);

  -- restriccion de clave foranea
  alter table [poliza_manual]
 add constraint FK_poliza_manual_validador
  foreign key (id_validador)
  references empleados(id);

  -- restriccion de clave foranea
  alter table [poliza_manual]
 add constraint FK_poliza_manual_autorizador
  foreign key (id_autorizador)
  references empleados(id);

   -- restriccion de clave foranea
  alter table [poliza_manual]
 add constraint FK_poliza_manual_contabilidad
  foreign key (id_contabilidad)
  references empleados(id);

     -- restriccion de clave foranea
  alter table [poliza_manual]
 add constraint FK_poliza_manual_direccion
  foreign key (id_direccion)
  references empleados(id);

  -- restriccion de clave foranea
alter table [poliza_manual]
 add constraint FK_poliza_manual_documento_soporte
  foreign key (id_documento_soporte)
  references biblioteca_digital(id);

   -- restriccion de clave foranea
alter table [poliza_manual]
 add constraint FK_poliza_manual_documento_registro
  foreign key (id_documento_registro)
  references biblioteca_digital(id);

-- restricción default
ALTER TABLE [poliza_manual] ADD  CONSTRAINT [DF_poliza_manual_fecha_creacion]  DEFAULT (GETDATE()) FOR [fecha_creacion]

-- restricion check
ALTER TABLE [poliza_manual] ADD CONSTRAINT CK_poliza_manual_Estatus CHECK ([estatus] IN 
('CREADO', 'ENVIADO_A_AREA', 'RECHAZADO_VALIDADOR', 'RECHAZADO_AUTORIZADOR', 'VALIDADO_POR_AREA',
'ENVIADO_SEGUNDA_VALIDACION', 'AUTORIZADO_SEGUNDA_VALIDACION', 'ENVIADO_A_CONTABILIDAD',
'ENVIADO_A_DIRECCION','RECHAZADO_DIRECCION', 'FINALIZADO')
)
GO

 	  
IF object_id(N'poliza_manual',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< poliza_manual en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla poliza_manual  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
