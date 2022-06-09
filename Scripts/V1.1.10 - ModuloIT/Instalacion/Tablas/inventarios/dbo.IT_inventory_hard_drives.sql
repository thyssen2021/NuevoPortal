USE Portal_2_0
GO
IF object_id(N'IT_inventory_hard_drives',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IT_inventory_hard_drives]
      PRINT '<<< IT_inventory_hard_drives en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los IT_inventory_hard_drives
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/06/06
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [IT_inventory_hard_drives](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_inventory_item] [int] NOT NULL,
	[disk_name][char] NOT NULL,
	[total_drive_space_mb][int] NULL,
	[free_drive_space_mb][int] NULL,
	[type_drive][varchar](20) NULL, --definir lista en codigo
 CONSTRAINT [PK_IT_inventory_hard_drives] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

  -- restriccion de clave foranea
alter table [IT_inventory_hard_drives]
 add constraint FK_IT_inventory_item
  foreign key (id_inventory_item)
  references IT_inventory_items(id);


-- restricion check
ALTER TABLE [IT_inventory_hard_drives] ADD CONSTRAINT CK_IT_inventory_hard_drive_type CHECK ([type_drive] IN 
('HDD', 'SSD','M.2')
)

-- restriccion unique
 alter table [IT_inventory_hard_drives]
  add constraint UQ_inventory_hard_drives
  unique (id_inventory_item,disk_name);
GO


 	  
IF object_id(N'IT_inventory_hard_drives',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IT_inventory_hard_drives en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IT_inventory_hard_drives  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
