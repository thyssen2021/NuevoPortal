--USE Portal_2_0
GO
IF object_id(N'SCDM_solicitud_asignaciones',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[SCDM_solicitud_asignaciones]
      PRINT '<<< SCDM_solicitud_asignaciones en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los SCDM_solicitud_asignaciones
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[SCDM_solicitud_asignaciones](
	[id] [int] IDENTITY(1,1) NOT NULL,	
	[id_solicitud][int] NOT NULL,
	[id_scdm_departamento][int] NOT NULL,
	[fecha_asignacion][datetime] NOT NULL,
	[fecha_cierre][datetime] NULL,
	[fecha_rechazo][datetime] NULL,
	[comentario_rechazo][varchar](250) NULL,
 CONSTRAINT [PK_SCDM_solicitud_asignaciones_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

  -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_asignaciones]
 add constraint FK_SCDM_solicitud_asignaciones_id_solicitud
  foreign key (id_solicitud)
  references SCDM_solicitud(id);

  -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_asignaciones]
 add constraint FK_SCDM_solicitud_asignaciones_id_scdm_departamento
  foreign key (id_scdm_departamento)
  references SCDM_cat_departamentos_asignacion(id);

SET IDENTITY_INSERT [dbo].[SCDM_solicitud_asignaciones] ON 
SET IDENTITY_INSERT [dbo].[SCDM_solicitud_asignaciones] OFF

	  
IF object_id(N'SCDM_solicitud_asignaciones',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< SCDM_solicitud_asignaciones en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla SCDM_solicitud_asignaciones  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
