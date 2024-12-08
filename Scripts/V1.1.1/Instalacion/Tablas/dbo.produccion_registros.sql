use[Portal_2_0]
GO
IF object_id(N'produccion_registros',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[produccion_registros]
      PRINT '<<< produccion_registros en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los produccion_registros más relaciones
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2021/09/27
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [dbo].[produccion_registros](	
	[id] [int] IDENTITY(1,1) NOT NULL,
	[clave_planta] [int] NULL,	
	[id_linea] [int] NULL,	
	[id_operador] [int] NULL,	
	[id_supervisor] [int] NULL,	
	[id_turno] [int] NULL,	
	[sap_platina][varchar](30) NULL,
	[sap_rollo][varchar](30) NULL,
	[fecha][datetime] NULL,
	[activo] [bit] NULL	
 CONSTRAINT [PK_produccion_registros] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restriccion de clave foranea
alter table [produccion_registros]
 add constraint FK_produccion_registros_planta_clave
  foreign key (clave_planta)
  references plantas(clave);

  -- restriccion de clave foranea
alter table [produccion_registros]
 add constraint FK_produccion_registros_linea
  foreign key (id_linea)
  references produccion_lineas(id);

    -- restriccion de clave foranea
alter table [produccion_registros]
 add constraint FK_produccion_registros_operador
  foreign key (id_operador)
  references produccion_operadores(id);

  -- restriccion de clave foranea
alter table [produccion_registros]
 add constraint FK_produccion_registros_supervisor
  foreign key (id_supervisor)
  references produccion_supervisores(id);
 
   -- restriccion de clave foranea
alter table [produccion_registros]
 add constraint FK_produccion_registros_turnos
  foreign key (id_turno)
  references produccion_turnos(id);

  -- restricción default
ALTER TABLE [produccion_registros] ADD  CONSTRAINT [DF_produccion_registros_activo]  DEFAULT (1) FOR [activo]
-- restricción default
ALTER TABLE [produccion_registros] ADD  CONSTRAINT [DF_produccion_registros_fecha]  DEFAULT (GETDATE()) FOR [fecha]
GO

 	  
IF object_id(N'produccion_registros',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< produccion_registros en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla produccion_registros  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
