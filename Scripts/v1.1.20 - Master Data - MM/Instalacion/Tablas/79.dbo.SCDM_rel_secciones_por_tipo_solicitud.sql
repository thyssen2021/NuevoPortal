--USE Portal_2_0
GO
IF object_id(N'SCDM_rel_secciones_por_tipo_solicitud',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[SCDM_rel_secciones_por_tipo_solicitud]
      PRINT '<<< SCDM_rel_secciones_por_tipo_solicitud en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Almacena la relacion de secciones permitidas según el tipo de soliditud
*  Autor :  Alfredo XochitemolCruz
*  Fecha de Creación: 2024/06/26
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[SCDM_rel_secciones_por_tipo_solicitud](
	[id] [int] IDENTITY(1,1) NOT NULL,	
	[id_tipo_solicitud] [int] NOT NULL,
	[id_tipo_cambio] [int] NULL,
	[id_seccion] [int] NOT NULL,	
 CONSTRAINT [PK_SCDM_rel_secciones_por_tipo_solicitud_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

  -- restriccion de clave foranea
alter table [dbo].[SCDM_rel_secciones_por_tipo_solicitud]
 add constraint FK_SCDM_rel_secciones_por_tipo_solicitud_id_tipo_solicitud
  foreign key (id_tipo_solicitud)
  references SCDM_cat_tipo_solicitud(id);

    -- restriccion de clave foranea
alter table [dbo].[SCDM_rel_secciones_por_tipo_solicitud]
 add constraint FK_SCDM_rel_secciones_por_tipo_solicitud_id_tipo_cambio
  foreign key (id_tipo_cambio)
  references SCDM_cat_tipo_cambio(id);

  -- restriccion de clave foranea
alter table [dbo].[SCDM_rel_secciones_por_tipo_solicitud]
 add constraint FK_SCDM_rel_secciones_por_tipo_solicitud_id_seccion
  foreign key (id_seccion)
  references SCDM_cat_secciones(id);

	  
IF object_id(N'SCDM_rel_secciones_por_tipo_solicitud',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< SCDM_rel_secciones_por_tipo_solicitud en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla SCDM_rel_secciones_por_tipo_solicitud  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
