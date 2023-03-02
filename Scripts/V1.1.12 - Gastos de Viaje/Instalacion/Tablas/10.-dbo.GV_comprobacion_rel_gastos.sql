--USE Portal_2_0
GO
IF object_id(N'GV_comprobacion_rel_gastos',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[GV_comprobacion_rel_gastos]
      PRINT '<<< GV_comprobacion_rel_gastos en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los datos GV_comprobacion_rel_gastos
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/08/17
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [GV_comprobacion_rel_gastos](
	[id] [int] IDENTITY(1,1) NOT NULL,	
	[id_gv_solicitud][int] NOT NULL, --FK
	[num_cc][int] NULL, 
	[id_centro_costo][int] NULL,
	[concepto_tipo][varchar](30),
	[id_comprobacion_tipo_gastos_viaje][int] NULL, --FK	
	[id_comprobacion_tipo_pago][int] NULL, --FK	
	[num_concepto][int] NULL,   --para identificar que pertenecen a la misma factura (utilizar uuid)
	[fecha_factura][datetime]NUll,
	[uuid][varchar](36) NULL,	
	[tipo_cambio][float] NULL,
	[currency_iso][varchar](3) NUll, --FK
	[descripcion][varchar](350) NULL,
	[cantidad][float] NULL,
	[precio_unitario][decimal](14,2) NULL, 
	[importe][decimal](14,2) NULL,  --cantidad x precio unitario 
	[importe_cc][decimal](14,2) NULL,  --total del CC
	[descuento][decimal](14,2) NULL, 
	[iva_porcentaje][float] NULL, 
	[iva_total][float] NULL,
	[isr_porcentaje][float] NULL, 
	[isr_total][float] NULL,
	[ieps_porcentaje][float] NULL, 
	[ieps_total][float] NULL,
	[total_translados][decimal](14,2) NULL,
	[total_retenciones][decimal](14,2) NULL,
	[impuestos_locales][decimal](14,2) NULL,
	[total_mxn][decimal](14,2) NULL,
	[comentario][varchar](150) NULL,
	[factura][varchar](25) NUll, -- definir campo
	--para archivos
	[id_archivo_xml][INT] NULL,
	[id_archivo_pdf][INT] NULL,
	[id_archivo_comprobante_extranjero][INT] NULL,

 CONSTRAINT [PK_GV_comprobacion_rel_gastos] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

--agregar FK

 -- restriccion de clave foranea
alter table [GV_comprobacion_rel_gastos]
 add constraint FK_GV_comprobacion_rel_gastos_id_archivo_xml
  foreign key (id_archivo_xml)
  references biblioteca_digital(id);

  -- restriccion de clave foranea
alter table [GV_comprobacion_rel_gastos]
 add constraint FK_GV_comprobacion_rel_gastos_id_archivo_pdf
  foreign key (id_archivo_pdf)
  references biblioteca_digital(id);

  -- restriccion de clave foranea
alter table [GV_comprobacion_rel_gastos]
 add constraint FK_GV_comprobacion_rel_gastos_id_archivo_comprobante_extranjero
  foreign key (id_archivo_comprobante_extranjero)
  references biblioteca_digital(id);


  -- restriccion de clave foranea
alter table [GV_comprobacion_rel_gastos]
 add constraint FK_GV_comprobacion_rel_gastos_currency_iso
  foreign key (currency_iso)
  references currency(CurrencyISO);
  
    -- restriccion de clave foranea
  alter table [GV_comprobacion_rel_gastos]
 add constraint FK_GV_comprobacion_rel_gastos_id_comprobacion_tipo_gastos_viaje
  foreign key (id_comprobacion_tipo_gastos_viaje)
  references GV_comprobacion_tipo_gastos_viaje(id);

     -- restriccion de clave foranea
  alter table [GV_comprobacion_rel_gastos]
 add constraint FK_GV_comprobacion_rel_gastos_id_comprobacion_tipo_pago
  foreign key (id_comprobacion_tipo_pago)
  references GV_comprobacion_tipo_pago(id);

  -- restriccion de clave foranea
  alter table [GV_comprobacion_rel_gastos]
 add constraint FK_GV_comprobacion_rel_gastos_id_gv_solicitud
  foreign key (id_gv_solicitud)
  references GV_solicitud(id);

  --  -- restriccion de clave foranea
alter table [GV_comprobacion_rel_gastos]
 add constraint FK_GV_comprobacion_rel_gastos_id_centro_costo
  foreign key (id_centro_costo)
  references GV_centros_costo(id);

  -- restricion check
ALTER TABLE [GV_comprobacion_rel_gastos] ADD CONSTRAINT CK_comprobacion_rel_gastos_concepto_tipo CHECK ([concepto_tipo] IN 
('COFIDI_RESUMEN','COFIDI_CONCEPTO','COFIDI_TOTALES','XML_RESUMEN', 'XML_CONCEPTO', 'XML_TOTALES' , 'GASTO_EXTRANJERO', 'GASTO_SIN_COMPROBANTE'
,'XML_CONCEPTO_CC','COFIDI_CONCEPTO_CC'
)
)
 	  

GO

 	  
IF object_id(N'GV_comprobacion_rel_gastos',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< GV_comprobacion_rel_gastos en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla GV_comprobacion_rel_gastos  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
