--USE Portal_2_0
GO
IF object_id(N'SCDM_solicitud_rel_lista_tecnica',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[SCDM_solicitud_rel_lista_tecnica]
      PRINT '<<< SCDM_solicitud_rel_lista_tecnica en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los SCDM_solicitud_rel_lista_tecnica
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[SCDM_solicitud_rel_lista_tecnica](
	[id_solicitud_rel_item_material] [int] NOT NULL,	
	[id_componente] [int] NULL,
	[resultado][varchar](20) NULL,
	[sobrante_mm][float] NULL,
	[cantidad_platinas][int] NULL,
	[cantidad_cintas][int] NULL,
	[fecha_validez][datetime] NULL,
	
 CONSTRAINT [PK_SCDM_solicitud_rel_lista_tecnica_1] PRIMARY KEY CLUSTERED 
(
	[id_solicitud_rel_item_material] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

  -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_rel_lista_tecnica]
 add constraint FK_SCDM_solicitud_rel_lista_tecnica_id_solicitud_rel_item_material
  foreign key (id_solicitud_rel_item_material)
  references SCDM_solicitud_rel_item_material(id);

   -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_rel_lista_tecnica]
 add constraint FK_SCDM_solicitud_rel_lista_tecnica_id_componente
  foreign key (id_componente)
  references SCDM_solicitud_rel_item_material(id);

    
IF object_id(N'SCDM_solicitud_rel_lista_tecnica',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< SCDM_solicitud_rel_lista_tecnica en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla SCDM_solicitud_rel_lista_tecnica  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
