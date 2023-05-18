--USE Portal_2_0
GO
IF object_id(N'RU_usuarios_embarques',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[RU_usuarios_embarques]
      PRINT '<<< RU_usuarios_embarques en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los RU_usuarios_embarques
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/05/15
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[RU_usuarios_embarques](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_empleado][int] NOT NULL,
	[recibe] [bit] NOT NULL DEFAULT 0,
	[libera] [bit] NOT NULL DEFAULT 0,
	[activo] [bit] NOT NULL DEFAULT 1,	
 CONSTRAINT [PK_RU_usuarios_embarques] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

-- Foreign Keys
-- restriccion de clave foranea
alter table [dbo].[RU_usuarios_embarques]
 add constraint FK_RU_usuarios_embarquess_id_empleado
  foreign key (id_empleado)
  references empleados(id);


GO

--inserta datos
GO
SET IDENTITY_INSERT [dbo].[RU_usuarios_embarques] ON 

INSERT INTO [RU_usuarios_embarques] (id, id_empleado, recibe, libera, activo) VALUES (1,436,1,1, 1)
INSERT INTO [RU_usuarios_embarques] (id, id_empleado, recibe, libera, activo) VALUES (2,149,1,1, 1)
INSERT INTO [RU_usuarios_embarques] (id, id_empleado, recibe, libera, activo) VALUES (3,176,1,1, 1)
INSERT INTO [RU_usuarios_embarques] (id, id_empleado, recibe, libera, activo) VALUES (4,161,1,1, 1)
INSERT INTO [RU_usuarios_embarques] (id, id_empleado, recibe, libera, activo) VALUES (5,152,1,1, 1)
INSERT INTO [RU_usuarios_embarques] (id, id_empleado, recibe, libera, activo) VALUES (6,159,1,1, 1)
INSERT INTO [RU_usuarios_embarques] (id, id_empleado, recibe, libera, activo) VALUES (7,181,1,1, 1)
INSERT INTO [RU_usuarios_embarques] (id, id_empleado, recibe, libera, activo) VALUES (8,103,1,1, 1)
INSERT INTO [RU_usuarios_embarques] (id, id_empleado, recibe, libera, activo) VALUES (9,163,1,1, 1)
INSERT INTO [RU_usuarios_embarques] (id, id_empleado, recibe, libera, activo) VALUES (10,5,0,1, 1)
INSERT INTO [RU_usuarios_embarques] (id, id_empleado, recibe, libera, activo) VALUES (11,4,0,1, 1)
INSERT INTO [RU_usuarios_embarques] (id, id_empleado, recibe, libera, activo) VALUES (12,6,0,1, 1)
INSERT INTO [RU_usuarios_embarques] (id, id_empleado, recibe, libera, activo) VALUES (13,18,0,1, 1)
INSERT INTO [RU_usuarios_embarques] (id, id_empleado, recibe, libera, activo) VALUES (14,19,0,1, 1)
INSERT INTO [RU_usuarios_embarques] (id, id_empleado, recibe, libera, activo) VALUES (15,47,0,1, 1)
INSERT INTO [RU_usuarios_embarques] (id, id_empleado, recibe, libera, activo) VALUES (16,188,0,1, 1)


SET IDENTITY_INSERT [dbo].[RU_usuarios_embarques] OFF
	  
IF object_id(N'RU_usuarios_embarques',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< RU_usuarios_embarques en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla RU_usuarios_embarques  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
