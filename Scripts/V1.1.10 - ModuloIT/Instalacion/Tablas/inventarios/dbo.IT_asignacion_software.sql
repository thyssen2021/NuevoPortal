USE Portal_2_0
GO
IF object_id(N'IT_asignacion_software',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IT_asignacion_software]
      PRINT '<<< IT_asignacion_software en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los IT_asignacion_software
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/06/29
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [IT_asignacion_software](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_inventory_software] [int] NOT NULL,
	[id_inventory_software_version] [int] NULL,
	[id_sistemas] [int] NOT NULL,
	[id_empleado] [int] NOT NULL,
 CONSTRAINT [PK_IT_asignacion_software] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


-- restriccion de clave foranea
alter table [IT_asignacion_software]
 add constraint FK_IT_asignacion_software_id_inventory_software
  foreign key (id_inventory_software)
  references IT_inventory_software(id);

  -- restriccion de clave foranea
alter table [IT_asignacion_software]
 add constraint FK_IT_asignacion_software_id_inventory_software_version
  foreign key (id_inventory_software_version)
  references IT_inventory_software_versions(id);

    -- restriccion de clave foranea
alter table [IT_asignacion_software]
 add constraint FK_IT_asignacion_software_id_sistemas
  foreign key (id_sistemas)
  references empleados(id);

      -- restriccion de clave foranea
alter table [IT_asignacion_software]
 add constraint FK_IT_asignacion_software_id_empleado
  foreign key (id_empleado)
  references empleados(id);

  -- restriccion unique
 alter table [IT_asignacion_software]
  add constraint UQ_IT_asignacion_software
  unique (id_empleado,id_inventory_software,id_inventory_software_version);


IF object_id(N'IT_asignacion_software',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IT_asignacion_software en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IT_asignacion_software  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
