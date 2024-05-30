--USE Portal_2_0
GO
IF object_id(N'SCDM_solicitud_rel_creacion_referencia',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[SCDM_solicitud_rel_creacion_referencia]
      PRINT '<<< SCDM_solicitud_rel_creacion_referencia en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los SCDM_solicitud_rel_creacion_referencia
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[SCDM_solicitud_rel_creacion_referencia](
	[id] [int] IDENTITY(1,1) NOT NULL,	
	[id_solicitud] [int] NOT NULL,	
	[id_tipo_venta] [int]  NULL,
	[id_tipo_material] [int]  NULL,
	[id_planta] [int]  NULL,
	[motivo_creacion] [varchar](100)  NULL,
	[material_existente][varchar](12) NULL,
	[nuevo_dato][varchar](300) NULL,
	[comentarios][varchar](120) NULL,
	[resultado][varchar](120) NULL,
	[ejecucion_correcta][bit]NULL,
	[nuevo_material][varchar](12) NULL,
 CONSTRAINT [PK_SCDM_solicitud_rel_creacion_referencia_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

  -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_rel_creacion_referencia]
 add constraint FK_SCDM_solicitud_rel_creacion_referencia_id_solicitud
  foreign key (id_solicitud)
  references SCDM_solicitud(id);

   -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_rel_creacion_referencia]
 add constraint FK_SCDM_solicitud_rel_creacion_referencia_id_tipo_material
  foreign key (id_tipo_material)
  references SCDM_cat_tipo_materiales_solicitud(id);
    
   -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_rel_creacion_referencia]
 add constraint FK_SCDM_solicitud_rel_creacion_referencia_id_tipo_venta
  foreign key (id_tipo_venta)
  references SCDM_cat_tipo_venta(id);

   -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_rel_creacion_referencia]
 add constraint FK_SCDM_solicitud_rel_creacion_referencia_id_planta
  foreign key (id_planta)
  references plantas(clave);

--   -- restriccion de clave foranea
--alter table [dbo].[SCDM_solicitud_rel_creacion_referencia]
-- add constraint FK_SCDM_solicitud_rel_creacion_referencia_id_motivo_creacion
--  foreign key (id_motivo_creacion)
--  references SCDM_cat_motivo_creacion(id);

IF object_id(N'SCDM_solicitud_rel_creacion_referencia',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< SCDM_solicitud_rel_creacion_referencia en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla SCDM_solicitud_rel_creacion_referencia  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
