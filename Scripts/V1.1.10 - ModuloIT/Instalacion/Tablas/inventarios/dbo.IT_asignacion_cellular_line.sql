USE Portal_2_0
GO
IF object_id(N'IT_asignacion_cellular_line',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IT_asignacion_cellular_line]
      PRINT '<<< IT_asignacion_cellular_line en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los IT_asignacion_cellular_line
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/07/05
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [IT_asignacion_cellular_line](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_inventory_cellular_line] [int] NOT NULL,	
	[id_empleado] [int] NOT NULL,
	[id_sistemas] [int] NOT NULL,
	[fecha_asignacion] [datetime] NULL,
	[fecha_desasignacion] [datetime] NULL,
	[es_asignacion_actual] [bit] NOT NULL,
 CONSTRAINT [PK_IT_asignacion_cellular_line] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


  -- restriccion de clave foranea
alter table [IT_asignacion_cellular_line]
 add constraint FK_IT_asignacion_cellular_line_id_inventory_line
  foreign key (id_inventory_cellular_line)
  references IT_inventory_cellular_line(id);

    -- restriccion de clave foranea
alter table [IT_asignacion_cellular_line]
 add constraint FK_IT_asignacion_cellular_line_id_sistemas
  foreign key (id_sistemas)
  references empleados(id);

      -- restriccion de clave foranea
alter table [IT_asignacion_cellular_line]
 add constraint FK_IT_asignacion_cellular_line_id_empleado
  foreign key (id_empleado)
  references empleados(id);



IF object_id(N'IT_asignacion_cellular_line',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IT_asignacion_cellular_line en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IT_asignacion_cellular_line  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
