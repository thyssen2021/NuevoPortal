--USE Portal_2_0
GO
IF object_id(N'SCDM_solicitud_rel_cambio_ingenieria',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[SCDM_solicitud_rel_cambio_ingenieria]
      PRINT '<<< SCDM_solicitud_rel_cambio_ingenieria en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los SCDM_solicitud_rel_cambio_ingenieria
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[SCDM_solicitud_rel_cambio_ingenieria](
	[id] [int] IDENTITY(1,1) NOT NULL,	
	[id_solicitud] [int] NOT NULL,	
	[id_planta] [int]  NULL,
	[id_tipo_material] [int] NULL,
	[material_existente] [varchar](12) NULL,
	[numero_antiguo_material] [varchar](50) NULL,
	[peso_bruto] [float] NULL,	
	[peso_neto] [float] NULL,
	[unidad_medida_inventario] [varchar](5) NULL,
	[commodity] [varchar](35) NULL,
	[grado_calidad] [varchar](35) NULL,
	[espesor_mm] [float] NULL,
	[espesor_tolerancia_negativa_mm] [float] NULL,
	[espesor_tolerancia_positiva_mm] [float] NULL,
	[ancho_mm] [float] NULL,
	[ancho_tolerancia_negativa_mm] [float] NULL,
	[ancho_tolerancia_positiva_mm] [float] NULL,
	[avance_mm] [float] NULL,
	[avance_tolerancia_negativa_mm] [float] NULL,
	[avance_tolerancia_positiva_mm] [float] NULL,
	[planicidad_mm] [float] NULL,
	[superficie] [varchar](35) NULL,
	[tratamiento_superficial] [varchar](35) NULL,
	[peso_recubrimiento] [varchar](35) NULL,
	[molino] [varchar](35) NULL,
	[forma] [varchar](20) NULL,
	[cliente] [varchar](120) NULL,
	[numero_parte] [varchar](60) NULL,
	[msa_honda] [varchar](35) NULL,
	[diametro_interior] [float] NULL,
	[diametro_exterior] [float] NULL,
	[nuevo_dato] [varchar](300) NULL,
	[comentarios] [varchar](120) NULL,
	[ejecucion_correcta] [varchar](120) NULL,
	[resultado] [varchar](120) NULL, --Mensaje SAP
 CONSTRAINT [PK_SCDM_solicitud_rel_cambio_ingenieria_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

  -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_rel_cambio_ingenieria]
 add constraint FK_SCDM_solicitud_rel_cambio_ingenieria_id_solicitud
  foreign key (id_solicitud)
  references SCDM_solicitud(id);

   -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_rel_cambio_ingenieria]
 add constraint FK_SCDM_solicitud_rel_cambio_ingenieria_id_planta
  foreign key (id_planta)
  references plantas(clave);

     -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_rel_cambio_ingenieria]
 add constraint FK_SCDM_solicitud_rel_cambio_ingenieria_id_tipo_material
  foreign key (id_tipo_material)
  references [SCDM_cat_tipo_materiales_solicitud](id);

IF object_id(N'SCDM_solicitud_rel_cambio_ingenieria',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< SCDM_solicitud_rel_cambio_ingenieria en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla SCDM_solicitud_rel_cambio_ingenieria  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
