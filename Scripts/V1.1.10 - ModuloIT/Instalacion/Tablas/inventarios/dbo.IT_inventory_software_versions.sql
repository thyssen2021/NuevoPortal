USE Portal_2_0
GO
IF object_id(N'IT_inventory_software_versions',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IT_inventory_software_versions]
      PRINT '<<< IT_inventory_software_versions en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los IT_inventory_software_versions
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/06/06
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [IT_inventory_software_versions](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_inventory_software] [int] NOT NULL,
	[version][varchar](50) NOT NULL,
	[activo] [bit] NOT NULL DEFAULT 1, 
 CONSTRAINT [PK_IT_inventory_software_versions] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

  -- restriccion de clave foranea
alter table [IT_inventory_software_versions]
 add constraint FK_IT_inventory_id_inventory_software
  foreign key (id_inventory_software)
  references IT_inventory_software(id);
  

-- restriccion unique
 alter table [IT_inventory_software_versions]
  add constraint UQ_inventory_version
  unique (id_inventory_software,[version]);
GO


 	  
IF object_id(N'IT_inventory_software_versions',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IT_inventory_software_versions en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IT_inventory_software_versions  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
