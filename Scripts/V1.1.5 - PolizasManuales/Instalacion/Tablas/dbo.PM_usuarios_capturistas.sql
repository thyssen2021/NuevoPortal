USE Portal_2_0
GO
IF object_id(N'PM_usuarios_capturistas',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[PM_usuarios_capturistas]
      PRINT '<<< PM_usuarios_capturistas en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los PM_usuarios_capturistas
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/01/20
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [PM_usuarios_capturistas](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_empleado][int] NOT NULL,
	[id_departamento][int] NOT NULL,
	[activo] [bit] NOT NULL	
 CONSTRAINT [PK_PM_usuarios_capturistas] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

--agregar FK
-- restriccion de clave foranea
alter table [PM_usuarios_capturistas]
 add constraint FK_PM_usuarios_capturistas_empleado
  foreign key (id_empleado)
  references empleados(id);

  -- restriccion de clave foranea
alter table [PM_usuarios_capturistas]
 add constraint FK_PM_usuarios_capturistas_departamento
  foreign key (id_departamento)
  references PM_departamentos(id);

-- restricción default
ALTER TABLE [PM_usuarios_capturistas] ADD  CONSTRAINT [DF_PM_usuarios_capturistas_activo]  DEFAULT (1) FOR [activo]
GO


SET IDENTITY_INSERT [PM_usuarios_capturistas] ON 
--Desarrollo
--INSERT [PM_usuarios_capturistas] ([id],[id_empleado],[id_departamento],[activo]) VALUES (1,438,1,1)	-- Alfredo Xochitemol

SET IDENTITY_INSERT [PM_usuarios_capturistas] OFF
GO
 	  
IF object_id(N'PM_usuarios_capturistas',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< PM_usuarios_capturistas en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla PM_usuarios_capturistas  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
