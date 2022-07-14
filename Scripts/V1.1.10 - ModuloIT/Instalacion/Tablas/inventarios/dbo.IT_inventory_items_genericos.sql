USE Portal_2_0
GO
IF object_id(N'IT_inventory_items_genericos',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IT_inventory_items_genericos]
      PRINT '<<< IT_inventory_items_genericos en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los IT_inventory_items_genericos
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/05/11
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [IT_inventory_items_genericos](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_inventory_type] [int] NOT NULL,
	[id_tipo_accesorio] [int] NULL,
	[brand][varchar](80) NULL,
	[model][varchar](80) NULL,
	[comments][varchar](250) NULL,
	[active][bit] NOT NULL,		

 CONSTRAINT [PK_IT_inventory_items_genericos] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


-- restriccion de clave foranea
alter table IT_inventory_items_genericos
add constraint FK_inventory_items_genericos_id_tipo_accesorio
foreign key (id_tipo_accesorio)
references IT_inventory_tipos_accesorios(id);

-- restriccion de clave foranea
alter table [IT_inventory_items_genericos]
add constraint FK_IT_inventory_items_genericos_id_inventory_type
foreign key (id_inventory_type)
references IT_inventory_hardware_type(id);

   	  
IF object_id(N'IT_inventory_items_genericos',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IT_inventory_items_genericos en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IT_inventory_items_genericos  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
