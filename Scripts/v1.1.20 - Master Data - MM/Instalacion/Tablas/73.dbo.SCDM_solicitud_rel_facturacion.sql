--USE Portal_2_0
GO
IF object_id(N'SCDM_solicitud_rel_facturacion',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[SCDM_solicitud_rel_facturacion]
      PRINT '<<< SCDM_solicitud_rel_facturacion en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los SCDM_solicitud_rel_facturacion
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [dbo].[SCDM_solicitud_rel_facturacion](
    [id] [int] IDENTITY(1,1) NOT NULL,
	[id_solicitud_rel_item_material] [int] NULL,
	[id_solicitud_rel_item_creacion_referencia] [int] NULL,
	[unidad_medida] [varchar](5) NULL,
	[clave_producto_servicio] [varchar](15) NULL,
	[cliente] [varchar](5) NULL,
	[descripcion_en] [varchar](50) NULL,
	-- campos Ivette
	[uso_CFDI_01][bit] NULL,
	[uso_CFDI_02][bit] NULL,
	[uso_CFDI_03][bit] NULL,
	[uso_CFDI_04][bit] NULL,
	[uso_CFDI_05][bit] NULL,
	[uso_CFDI_06][bit] NULL,
	[uso_CFDI_07][bit] NULL,
	[uso_CFDI_08][bit] NULL,
	[uso_CFDI_09][bit] NULL,
	[uso_CFDI_10][bit] NULL,
	[mensaje_sap] [varchar](120) NULL,
	[ejecucion_correcta] [varchar](120) NULL,
	
 CONSTRAINT [PK_SCDM_solicitud_rel_facturacion_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

--Insert into [SCDM_solicitud_rel_facturacion] values(1,2,'Ubicacion','sap ejecuacion correcta', 'mensaje SAP')

  -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_rel_facturacion]
 add constraint FK_SCDM_solicitud_rel_facturacion_id_solicitud_rel_item_material
  foreign key (id_solicitud_rel_item_material)
  references SCDM_solicitud_rel_item_material(id);
 
-- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_rel_facturacion]
 add constraint FK_SCDM_solicitud_rel_facturacion_id_solicitud_rel_item_creacion_referencia
  foreign key (id_solicitud_rel_item_creacion_referencia)
  references SCDM_solicitud_rel_creacion_referencia(id);

    
IF object_id(N'SCDM_solicitud_rel_facturacion',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< SCDM_solicitud_rel_facturacion en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla SCDM_solicitud_rel_facturacion  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
