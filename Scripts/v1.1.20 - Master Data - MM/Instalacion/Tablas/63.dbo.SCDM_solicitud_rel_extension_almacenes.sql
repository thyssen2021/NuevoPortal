--USE Portal_2_0
GO
IF object_id(N'SCDM_solicitud_rel_extension_almacenes',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[SCDM_solicitud_rel_extension_almacenes]
      PRINT '<<< SCDM_solicitud_rel_extension_almacenes en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los SCDM_solicitud_rel_extension_almacenes
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[SCDM_solicitud_rel_extension_almacenes](
    [id] [int] IDENTITY(1,1) NOT NULL,	
	[id_solicitud_rel_item_material] [int] NULL,
	[id_cat_almacenes][int] NOT NULL,
	[ubicacion][varchar](50) NULL,
	[almacen_ejecucion_correcta][varchar](120) NULL,
	[almacen_mensaje_sap][varchar](120) NULL,
	
 CONSTRAINT [PK_SCDM_solicitud_rel_extension_almacenes_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

--Insert into [SCDM_solicitud_rel_extension_almacenes] values(1,2,'Ubicacion','sap ejecuacion correcta', 'mensaje SAP')

  -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_rel_extension_almacenes]
 add constraint FK_SCDM_solicitud_rel_extension_almacenes_id_solicitud_rel_item_material
  foreign key (id_solicitud_rel_item_material)
  references SCDM_solicitud_rel_item_material(id);

 
-- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_rel_extension_almacenes]
 add constraint FK_SCDM_solicitud_rel_extension_almacenes_id_cat_almacenes
  foreign key (id_cat_almacenes)
  references SCDM_cat_almacenes(id);

    
IF object_id(N'SCDM_solicitud_rel_extension_almacenes',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< SCDM_solicitud_rel_extension_almacenes en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla SCDM_solicitud_rel_extension_almacenes  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
