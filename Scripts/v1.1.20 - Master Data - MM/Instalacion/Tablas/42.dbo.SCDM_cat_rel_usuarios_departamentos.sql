--USE Portal_2_0
GO
IF object_id(N'SCDM_cat_rel_usuarios_departamentos',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[SCDM_cat_rel_usuarios_departamentos]
      PRINT '<<< SCDM_cat_rel_usuarios_departamentos en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los SCDM_cat_rel_usuarios_departamentos
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[SCDM_cat_rel_usuarios_departamentos](
	[id] [int] IDENTITY(1,1) NOT NULL,	
	[id_departamento] [int] NOT NULL,
	[id_empleado] [int] NOT NULL,
	[activo] [bit] NOT NULL,
 CONSTRAINT [PK_SCDM_cat_rel_usuarios_departamentos_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

  -- restriccion de clave foranea
alter table [dbo].[SCDM_cat_rel_usuarios_departamentos]
 add constraint FK_SCDM_cat_rel_usuarios_departamentos_id_empleado
  foreign key (id_empleado)
  references empleados(id);

    -- restriccion de clave foranea
alter table [dbo].[SCDM_cat_rel_usuarios_departamentos]
 add constraint FK_SCDM_cat_rel_usuarios_departamentos_id_departamento
  foreign key (id_departamento)
  references SCDM_cat_departamentos_asignacion(id);

--Inserta valores

SET IDENTITY_INSERT [dbo].[SCDM_cat_rel_usuarios_departamentos] ON 

GO

INSERT [dbo].[SCDM_cat_rel_usuarios_departamentos] ([id],[id_departamento], [id_empleado], [activo]) VALUES (1, 9, 438, 1)
INSERT [dbo].[SCDM_cat_rel_usuarios_departamentos] ([id],[id_departamento], [id_empleado], [activo]) VALUES (2, 4, 316, 1)
INSERT [dbo].[SCDM_cat_rel_usuarios_departamentos] ([id],[id_departamento], [id_empleado], [activo]) VALUES (3, 4, 86, 1)
INSERT [dbo].[SCDM_cat_rel_usuarios_departamentos] ([id],[id_departamento], [id_empleado], [activo]) VALUES (4, 8, 528, 1)
INSERT [dbo].[SCDM_cat_rel_usuarios_departamentos] ([id],[id_departamento], [id_empleado], [activo]) VALUES (5, 8, 72, 1)
INSERT [dbo].[SCDM_cat_rel_usuarios_departamentos] ([id],[id_departamento], [id_empleado], [activo]) VALUES (6, 3, 450, 1)
INSERT [dbo].[SCDM_cat_rel_usuarios_departamentos] ([id],[id_departamento], [id_empleado], [activo]) VALUES (7, 3, 444, 1)
INSERT [dbo].[SCDM_cat_rel_usuarios_departamentos] ([id],[id_departamento], [id_empleado], [activo]) VALUES (8, 2, 131, 1)
INSERT [dbo].[SCDM_cat_rel_usuarios_departamentos] ([id],[id_departamento], [id_empleado], [activo]) VALUES (9, 6, 133, 1)
INSERT [dbo].[SCDM_cat_rel_usuarios_departamentos] ([id],[id_departamento], [id_empleado], [activo]) VALUES (10, 5, 345, 1)


SET IDENTITY_INSERT [dbo].[SCDM_cat_rel_usuarios_departamentos] OFF
GO


	  
IF object_id(N'SCDM_cat_rel_usuarios_departamentos',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< SCDM_cat_rel_usuarios_departamentos en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla SCDM_cat_rel_usuarios_departamentos  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
