--USE Portal_2_0
GO
IF object_id(N'SCDM_solicitud',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[SCDM_solicitud]
      PRINT '<<< SCDM_solicitud en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los SCDM_solicitud
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[SCDM_solicitud](
	[id] [int] IDENTITY(1,1) NOT NULL,	
	[id_tipo_solicitud] [int] NOT NULL,
	[id_tipo_cambio] [int]  NULL,
	[id_prioridad] [int] NOT NULL,
	[id_solicitante] [int] NOT NULL,
	[descripcion][varchar](180) NOT NULL,
	[justificacion][varchar](250) NOT NULL,
	[fecha_creacion][datetime] NOT NULL,
	[on_hold][bit] NOT NULL DEFAULT 0,
	[activo] [bit] NOT NULL DEFAULT 1,
 CONSTRAINT [PK_SCDM_solicitud_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

  -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud]
 add constraint FK_SCDM_solicitud_id_tipo_solicitud
  foreign key (id_tipo_solicitud)
  references SCDM_cat_tipo_solicitud(id);

  -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud]
 add constraint FK_SCDM_solicitud_id_tipo_cambio
  foreign key (id_tipo_cambio)
  references SCDM_cat_tipo_cambio(id);

  -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud]
 add constraint FK_SCDM_solicitud_id_prioridad
  foreign key (id_prioridad)
  references SCDM_cat_prioridad(id);

  -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud]
 add constraint FK_SCDM_solicitud_id_solicitante
  foreign key (id_solicitante)
  references empleados(id);

	  
IF object_id(N'SCDM_solicitud',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< SCDM_solicitud en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla SCDM_solicitud  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
