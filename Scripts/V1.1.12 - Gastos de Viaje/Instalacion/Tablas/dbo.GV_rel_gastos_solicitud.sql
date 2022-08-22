USE Portal_2_0
GO
IF object_id(N'rel_gastos_solicitud',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[rel_gastos_solicitud]
      PRINT '<<< rel_gastos_solicitud en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los datos rel_gastos_solicitud
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/08/17
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [rel_gastos_solicitud](
	[id] [int] IDENTITY(1,1) NOT NULL,	
	[id_gv_solicitud][int] NOT NULL, --FK
	[id_tipo_gastos_viaje][int] NOT NULL, --FK
	[importe][decimal](14,2) NOT NULL, 
	[currency_iso][varchar](3) NOT NUll, --FK
	[cantidad][decimal](8,2) NOT NUll, 
	[comentarios][varchar](120) NULL,
	
 CONSTRAINT [PK_rel_gastos_solicitud] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

--agregar FK

  -- restriccion de clave foranea
alter table [rel_gastos_solicitud]
 add constraint FK_rel_gastos_solicitud_currency_iso
  foreign key (currency_iso)
  references currency(CurrencyISO);
  
    -- restriccion de clave foranea
  alter table [rel_gastos_solicitud]
 add constraint FK_rel_gastos_solicitud_id_tipo_gastos_viaje
  foreign key (id_tipo_gastos_viaje)
  references GV_tipo_gastos_viaje(id);

  -- restriccion de clave foranea
  alter table [rel_gastos_solicitud]
 add constraint FK_rel_gastos_solicitud_id_gv_solicitud
  foreign key (id_gv_solicitud)
  references GV_solicitud(id);

GO

 	  
IF object_id(N'rel_gastos_solicitud',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< rel_gastos_solicitud en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla rel_gastos_solicitud  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
