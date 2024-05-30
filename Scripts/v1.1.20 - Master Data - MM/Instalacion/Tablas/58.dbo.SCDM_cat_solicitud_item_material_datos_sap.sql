--use[Portal_2_0]
GO
IF object_id(N'SCDM_solicitud_item_material_datos_sap',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[SCDM_solicitud_item_material_datos_sap]
      PRINT '<<< SCDM_solicitud_item_material_datos_sap en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los datos de entrada de producción
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2021/10/06
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [dbo].[SCDM_solicitud_item_material_datos_sap](	
	[id_scdm_solicitud_rel_item_material][int] NOT NULL,	
	[materiales_x_solicitud_ejecucion_correcta][varchar](120) NULL,
	[materiales_x_solicitud_mensaje_sap][varchar](120) NULL,
	[materiales_x_solicitud_nuevo_numero_material][varchar](15) NULL,
	[class_001_ejecucion_correcta][varchar](120) NULL,
	[class_001_mensaje_sap][varchar](120) NULL,
	[class_023_ejecucion_correcta][varchar](120) NULL,
	[class_023_mensaje_sap][varchar](120) NULL,
	[aum_ejecucion_correcta][varchar](120) NULL,
	[aum_mensaje_sap][varchar](120) NULL,
	[dp_ejecucion_correcta][varchar](120) NULL,
	[dp_mensaje_sap][varchar](120) NULL,
	[budget_ejecucion_correcta][varchar](120) NULL,
	[budget_mensaje_sap][varchar](120) NULL,

 CONSTRAINT [PK_SCDM_solicitud_item_material_datos_sap] PRIMARY KEY CLUSTERED 
(
	[id_scdm_solicitud_rel_item_material] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restriccion de clave foranea
alter table [SCDM_solicitud_item_material_datos_sap]
 add constraint FK_SCDM_solicitud_item_material_datos_sap_id_scdm_solicitud_rel_item_material
  foreign key (id_scdm_solicitud_rel_item_material)
  references scdm_solicitud_rel_item_material(id);
GO

 	  
IF object_id(N'SCDM_solicitud_item_material_datos_sap',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< SCDM_solicitud_item_material_datos_sap en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla SCDM_solicitud_item_material_datos_sap  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
