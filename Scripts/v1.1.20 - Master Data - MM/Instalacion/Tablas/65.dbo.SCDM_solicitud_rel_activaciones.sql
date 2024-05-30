--USE Portal_2_0
GO
IF object_id(N'SCDM_solicitud_rel_activaciones',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[SCDM_solicitud_rel_activaciones]
      PRINT '<<< SCDM_solicitud_rel_activaciones en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los SCDM_solicitud_rel_activaciones
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[SCDM_solicitud_rel_activaciones](
	[id] [int] IDENTITY(1,1) NOT NULL,	
	[id_solicitud] [int] NOT NULL,		
	[material][varchar](30) NOT NULL,
	[planta][varchar](4) NOT NULL,
	[sales_org][varchar](4) NOT NULL DEFAULT '5000',
	[estatus_planta][varchar](2) NOT NULL,
	[estatus_dchain][varchar](2) NOT NULL,
	[fecha][datetime] NOT NULL,
	[ejecucion_correcta][varchar](120) NULL,
	[mensaje_sap][varchar](120) NULL,
	

 CONSTRAINT [PK_SCDM_solicitud_rel_activaciones_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

  -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_rel_activaciones]
 add constraint FK_SCDM_solicitud_rel_activaciones_id_solicitud
  foreign key (id_solicitud)
  references SCDM_solicitud(id);


--   -- restriccion de clave foranea
--alter table [dbo].[SCDM_solicitud_rel_activaciones]
-- add constraint FK_SCDM_solicitud_rel_activaciones_id_motivo_creacion
--  foreign key (id_motivo_creacion)
--  references SCDM_cat_motivo_creacion(id);

IF object_id(N'SCDM_solicitud_rel_activaciones',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< SCDM_solicitud_rel_activaciones en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla SCDM_solicitud_rel_activaciones  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
