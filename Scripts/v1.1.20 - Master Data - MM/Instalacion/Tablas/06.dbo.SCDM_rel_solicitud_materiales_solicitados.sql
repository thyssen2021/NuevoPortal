--USE Portal_2_0
GO
IF object_id(N'SCDM_rel_solicitud_materiales_solicitados',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[SCDM_rel_solicitud_materiales_solicitados]
      PRINT '<<< SCDM_rel_solicitud_materiales_solicitados en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los SCDM_rel_solicitud_materiales_solicitados
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[SCDM_rel_solicitud_materiales_solicitados](
	[id] [int] IDENTITY(1,1) NOT NULL,	
	[id_solicitud] [int] NOT NULL,
	[id_tipo_material] [int] NOT NULL,	
 CONSTRAINT [PK_SCDM_rel_solicitud_materiales_solicitados_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

  -- restriccion de clave foranea
alter table [dbo].[SCDM_rel_solicitud_materiales_solicitados]
 add constraint FK_SCDM_rel_solicitud_materiales_solicitados_id_solicitud
  foreign key (id_solicitud)
  references SCDM_solicitud(id);

  -- restriccion de clave foranea
alter table [dbo].[SCDM_rel_solicitud_materiales_solicitados]
 add constraint FK_SCDM_rel_solicitud_materiales_solicitados_id_tipo_material
  foreign key (id_tipo_material)
  references SCDM_cat_tipo_materiales_solicitud(id);

	  
IF object_id(N'SCDM_rel_solicitud_materiales_solicitados',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< SCDM_rel_solicitud_materiales_solicitados en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla SCDM_rel_solicitud_materiales_solicitados  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
