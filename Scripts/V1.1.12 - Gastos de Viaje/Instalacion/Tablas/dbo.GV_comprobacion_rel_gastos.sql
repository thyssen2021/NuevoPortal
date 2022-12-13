USE Portal_2_0
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
	[id_comprobacion_tipo_gastos_viaje][int] NOT NULL, --FK
	[factura][varchar](12) NUll,
	[fecha_factura][datetime]NUll,
	[uuid][varchar](32) NULL,
	[currency_iso][varchar](3) NOT NUll, --FK
	[importe_factura][decimal](14,2) NOT NULL, 
	[tipo_cambio][float] NULL,
	[validacion][varchar](51) NUll,  -- definir campo 
	[iva][float] NULL,
	
 CONSTRAINT [PK_GV_comprobacion_rel_gastos] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

--agregar FK

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
 add constraint FK_GV_comprobacion_rel_gastos_id_gv_solicitud
  foreign key (id_gv_solicitud)
  references GV_solicitud(id);

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
