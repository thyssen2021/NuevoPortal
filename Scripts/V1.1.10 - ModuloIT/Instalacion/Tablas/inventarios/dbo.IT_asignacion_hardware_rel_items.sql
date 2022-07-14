USE Portal_2_0
GO
IF object_id(N'IT_asignacion_hardware_rel_items',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IT_asignacion_hardware_rel_items]
      PRINT '<<< IT_asignacion_hardware_rel_items en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los IT_asignacion_hardware_rel_items
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/07/11
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [IT_asignacion_hardware_rel_items](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_asignacion_hardware] [int] NOT NULL, --responsiva
	[id_it_inventory_item][int] NULL,
	[id_it_inventory_generico][int] NULL,
	[comments][varchar](250) NULL, 
 CONSTRAINT [PK_IT_asignacion_hardware_rel_items] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

  -- restriccion de clave foranea
alter table [IT_asignacion_hardware_rel_items]
 add constraint FK_IT_inventory_item_id_asignacion_hardware
  foreign key (id_asignacion_hardware)
  references IT_asignacion_hardware(id);

    -- restriccion de clave foranea
alter table [IT_asignacion_hardware_rel_items]
 add constraint FK_IT_inventory_item_id_it_inventory_item
  foreign key (id_it_inventory_item)
  references IT_inventory_items(id);

      -- restriccion de clave foranea
alter table [IT_asignacion_hardware_rel_items]
 add constraint FK_IT_inventory_item_id_it_inventory_generico
  foreign key (id_it_inventory_generico)
  references IT_inventory_items_genericos(id);

 	  
IF object_id(N'IT_asignacion_hardware_rel_items',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IT_asignacion_hardware_rel_items en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IT_asignacion_hardware_rel_items  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
