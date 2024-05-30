--USE Portal_2_0
GO
IF object_id(N'SCDM_solicitud_rel_item_material',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[SCDM_solicitud_rel_item_material]
      PRINT '<<< SCDM_solicitud_rel_item_material en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los SCDM_solicitud_rel_item_material
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[SCDM_solicitud_rel_item_material](
	[id] [int] IDENTITY(1,1) NOT NULL,	
	[id_solicitud] [int] NOT NULL,
	[id_tipo_material] [int] NOT NULL,
	[id_tipo_venta] [int]  NULL,
	[id_cliente] [int]  NULL,
	[id_proveedor] [int]  NULL,
	[id_molino] [int]  NULL,
	[id_metal] [int]  NULL,
	[id_unidad_medida_inventario] [int] NULL,
	[id_tipo_recubrimiento] [int]  NULL,
	[id_clase_aprovisionamiento] [int]  NULL,
	[id_diametro_interior] [int]  NULL,
	[id_peso_recubrimiento] [int]  NULL,
	[id_parte_interior_exterior] [int]  NULL,
	[id_posicion_rollo] [int]  NULL,
	[id_ihs_1] [int]  NULL,
	[id_ihs_2] [int]  NULL,
	[id_ihs_3] [int]  NULL,
	[id_ihs_4] [int]  NULL,
	[id_ihs_5] [int]  NULL,
	[id_modelo_negocio] [int]  NULL,
	[id_tipo_transito] [int]  NULL,
	[id_procesador_externo] [int]  NULL,
	[id_disponente] [int]  NULL,
	[material_del_cliente][bit] NULL,
	[numero_odc_cliente][varchar](15) NULL,
	[requiere_ppaps][bit] NULL,
	[requiere_imds][bit] NULL,
	[numero_material][varchar](20) NULL,
	[descripcion_material_es][varchar](120) NULL,
	[descripcion_material_en][varchar](120) NULL,
	[grado_calidad][varchar](50) NULL,
	[numero_parte][varchar](60) NULL,
	[descripcion_numero_parte][varchar](80) NULL,
	[norma_referencia][varchar](60) NULL,
	[espesor_mm][float] NULL,
	[espesor_tolerancia_negativa_mm][float] NULL,
	[espesor_tolerancia_positiva_mm][float] NULL,
	[ancho_mm][float] NULL,
	[ancho_tolerancia_negativa_mm][float] NULL,
	[ancho_tolerancia_positiva_mm][float] NULL,
	[diametro_exterior_maximo_mm][float] NULL,
	[peso_min_kg][float] NULL,
	[peso_max_kg][float] NULL,
	[procesador_externo][bit] NULL,
	[numero_antiguo_material][varchar](50) NULL,
	[planicidad_mm][float] NULL,
	[msa_hoda][varchar](50) NULL,
	[requiere_consiliacion_puntas_colar][bit] NULL,
	[scrap_permitido_puntas_colas][float] NULL,
	[fecha_validez][datetime] NULL,
 CONSTRAINT [PK_SCDM_solicitud_rel_item_material_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

  -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_rel_item_material]
 add constraint FK_SCDM_solicitud_rel_item_material_id_solicitud
  foreign key (id_solicitud)
  references SCDM_solicitud(id);

   -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_rel_item_material]
 add constraint FK_SCDM_solicitud_rel_item_material_id_tipo_material
  foreign key (id_tipo_material)
  references SCDM_cat_tipo_materiales_solicitud(id);
    
   -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_rel_item_material]
 add constraint FK_SCDM_solicitud_rel_item_material_id_tipo_venta
  foreign key (id_tipo_venta)
  references SCDM_cat_tipo_venta(id);

   -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_rel_item_material]
 add constraint FK_SCDM_solicitud_rel_item_material_id_cliente
  foreign key (id_cliente)
  references clientes(clave);

   -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_rel_item_material]
 add constraint FK_SCDM_solicitud_rel_item_material_id_proveedor
  foreign key (id_proveedor)
  references proveedores(clave);

     -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_rel_item_material]
 add constraint FK_SCDM_solicitud_rel_item_material_id_molino
  foreign key (id_molino)
  references SCDM_cat_molino(id);
	  
     -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_rel_item_material]
 add constraint FK_SCDM_solicitud_rel_item_material_id_metal
  foreign key (id_metal)
  references SCDM_cat_tipo_metal(id);

       -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_rel_item_material]
 add constraint FK_SCDM_solicitud_rel_item_material_id_unidad_medida_inventario
  foreign key (id_unidad_medida_inventario)
  references SCDM_cat_unidades_medida(id);

         -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_rel_item_material]
 add constraint FK_SCDM_solicitud_rel_item_material_id_tipo_recubrimiento
  foreign key (id_tipo_recubrimiento)
  references SCDM_cat_tipo_recubrimiento(id);

  -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_rel_item_material]
 add constraint FK_SCDM_solicitud_rel_item_material_id_clase_aprovisionamiento
  foreign key (id_clase_aprovisionamiento)
  references SCDM_cat_clase_aprovisionamiento(id);

   -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_rel_item_material]
 add constraint FK_SCDM_solicitud_rel_item_material_id_diametro_interior
  foreign key (id_diametro_interior)
  references SCDM_cat_diametro_interior(id);

     -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_rel_item_material]
 add constraint FK_SCDM_solicitud_rel_item_material_id_peso_recubrimiento
  foreign key (id_peso_recubrimiento)
  references SCDM_cat_peso_recubrimiento(id);

     -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_rel_item_material]
 add constraint FK_SCDM_solicitud_rel_item_material_id_parte_interior_exterior
  foreign key (id_parte_interior_exterior)
  references SCDM_cat_parte_interior_exterior(id);

       -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_rel_item_material]
 add constraint FK_SCDM_solicitud_rel_item_material_id_posicion_rollo
  foreign key (id_posicion_rollo)
  references SCDM_cat_posicion_rollo_embarques(id);


         -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_rel_item_material]
 add constraint FK_SCDM_solicitud_rel_item_material_id_ihs_1
  foreign key (id_ihs_1)
  references SCDM_cat_ihs(id);

           -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_rel_item_material]
 add constraint FK_SCDM_solicitud_rel_item_material_id_ihs_2
  foreign key (id_ihs_2)
  references SCDM_cat_ihs(id);

           -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_rel_item_material]
 add constraint FK_SCDM_solicitud_rel_item_material_id_ihs_3
  foreign key (id_ihs_3)
  references SCDM_cat_ihs(id);

           -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_rel_item_material]
 add constraint FK_SCDM_solicitud_rel_item_material_id_ihs_4
  foreign key (id_ihs_4)
  references SCDM_cat_ihs(id);

           -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_rel_item_material]
 add constraint FK_SCDM_solicitud_rel_item_material_id_ihs_5
  foreign key (id_ihs_5)
  references SCDM_cat_ihs(id);

             -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_rel_item_material]
 add constraint FK_SCDM_solicitud_rel_item_material_id_modelo_negocio
  foreign key (id_modelo_negocio)
  references SCDM_cat_modelo_negocio(id);

  alter table [dbo].[SCDM_solicitud_rel_item_material]
 add constraint FK_SCDM_solicitud_rel_item_material_id_tipo_transito
  foreign key (id_tipo_transito)
  references SCDM_cat_tipo_transito(id);
    
  alter table [dbo].[SCDM_solicitud_rel_item_material]
 add constraint FK_SCDM_solicitud_rel_item_material_id_procesador_externo
  foreign key (id_procesador_externo)
  references proveedores(clave);

  alter table [dbo].[SCDM_solicitud_rel_item_material]
 add constraint FK_SCDM_solicitud_rel_item_material_id_disponente
  foreign key (id_disponente)
  references SCDM_cat_disponentes(id);

  --- ALTER TABLE CINTAS---

  	IF object_id(N'SCDM_solicitud_rel_item_material','U') IS NOT NULL
		BEGIN			
			ALTER TABLE SCDM_solicitud_rel_item_material ADD numero_cintas_resultantes int null;
			PRINT 'Se ha agregado la columna numero_cintas_resultantes'				
		END
		ELSE
		BEGIN
			PRINT 'La tabla SCDM_solicitud_rel_item_material NO EXISTE, no se puede crear las columnas'
		END

	IF object_id(N'SCDM_solicitud_rel_item_material','U') IS NOT NULL
		BEGIN			
			ALTER TABLE SCDM_solicitud_rel_item_material ADD id_diametro_interior_salida int null;
			PRINT 'Se ha agregado la columna id_diametro_interior_salida'				
		END
		ELSE
		BEGIN
			PRINT 'La tabla SCDM_solicitud_rel_item_material NO EXISTE, no se puede crear las columnas'
		END

	
		IF object_id(N'SCDM_solicitud_rel_item_material','U') IS NOT NULL
		BEGIN			
			ALTER TABLE SCDM_solicitud_rel_item_material ADD ancho_entrega_cinta_mm float null;
			PRINT 'Se ha agregado la columna ancho_entrega_cinta_mm'				
		END
		ELSE
		BEGIN
			PRINT 'La tabla SCDM_solicitud_rel_item_material NO EXISTE, no se puede crear las columnas'
		END


		   -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_rel_item_material]
 add constraint FK_SCDM_solicitud_rel_item_material_id_diametro_interior_salida
  foreign key (id_diametro_interior_salida)
  references SCDM_cat_diametro_interior(id);


   --- FIN  ALTER CINTAS ---


IF object_id(N'SCDM_solicitud_rel_item_material',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< SCDM_solicitud_rel_item_material en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla SCDM_solicitud_rel_item_material  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
