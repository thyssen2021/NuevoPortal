USE Portal_2_0
GO
IF object_id(N'OT_responsables',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[OT_responsables]
      PRINT '<<< OT_responsables en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los OT_responsables
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/02/14
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [OT_responsables](
	[id][int] IDENTITY(1,1) NOT NULL,
	[id_empleado] [int] NOT NULL,
	[supervisor] [bit] NOT NULL default 0,
	[id_supervisor] [int] NULL,
	[activo] [bit] NOT NULL default 1
 CONSTRAINT [PK_OT_responsables] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


--agregar FK
-- restriccion de clave foranea
alter table [OT_responsables]
 add constraint FK_OT_responsables_id_empleado
  foreign key (id_empleado)
  references empleados(id);

  -- restriccion de clave foranea
alter table [OT_responsables]
 add constraint FK_OT_responsables_id_supervisor
  foreign key (id_supervisor)
  references empleados(id);

	  
IF object_id(N'OT_responsables',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< OT_responsables en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla OT_responsables  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
