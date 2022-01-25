USE Portal_2_0
GO
IF object_id(N'PM_departamentos',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[PM_departamentos]
      PRINT '<<< PM_departamentos en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los PM_departamentos
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/01/20
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [PM_departamentos](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_empleado_jefe][int] NULL,
	[descripcion][varchar](30) NOT NULL,
	[id_departamento_validacion][int] NULL,	
	[activo] [bit] NOT NULL	
 CONSTRAINT [PK_PM_departamentos] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

--agregar FK
-- restriccion de clave foranea
alter table [PM_departamentos]
 add constraint FK_PM_departamentos_empleado
  foreign key (id_empleado_jefe)
  references empleados(id);

  -- restriccion de clave foranea
alter table [PM_departamentos]
 add constraint FK_PM_departamentos_depto_validacion
  foreign key (id_departamento_validacion)
  references PM_departamentos(id);

-- restricción default
ALTER TABLE [PM_departamentos] ADD  CONSTRAINT [DF_PM_departamentos_activo]  DEFAULT (1) FOR [activo]
GO


SET IDENTITY_INSERT [PM_departamentos] ON 
--Desarrollo
INSERT [PM_departamentos] ([id],[descripcion],[activo]) VALUES (1,'Controlling', 1)
INSERT [PM_departamentos] ([id],[descripcion],[activo]) VALUES (2,'Contabilidad', 1)	
SET IDENTITY_INSERT [PM_departamentos] OFF
GO

--Inserta id_departamento_validacion
UPDATE [PM_departamentos] set id_departamento_validacion=2 where id=1;
UPDATE [PM_departamentos] set id_departamento_validacion=1 where id=2;

 	  
IF object_id(N'PM_departamentos',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< PM_departamentos en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla PM_departamentos  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
