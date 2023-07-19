--USE Portal_2_0
GO
IF object_id(N'SCDM_cat_usuarios_revision_departamento',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[SCDM_cat_usuarios_revision_departamento]
      PRINT '<<< SCDM_cat_usuarios_revision_departamento en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los SCDM_cat_usuarios_revision_departamento
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[SCDM_cat_usuarios_revision_departamento](
	[id] [int] IDENTITY(1,1) NOT NULL,	
	[id_scdm_departamento][int] NOT NULL,
	[id_empleado][int] NOT NULL,
	[tipo][varchar](20) NOT NULL,
	[activo] [bit]  NOT NULL DEFAULT 1,	
 CONSTRAINT [PK_SCDM_cat_usuarios_revision_departamento_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

  -- restriccion de clave foranea
alter table [dbo].[SCDM_cat_usuarios_revision_departamento]
 add constraint FK_SCDM_cat_usuarios_revision_departamento_id_scdm_departamento
  foreign key (id_scdm_departamento)
  references SCDM_cat_departamentos_asignacion(id);

    -- restriccion de clave foranea
alter table [dbo].[SCDM_cat_usuarios_revision_departamento]
 add constraint FK_SCDM_cat_usuarios_revision_departamento_id_empleado
  foreign key (id_empleado)
  references empleados(id);

  --restricion de valores
  -- restricion check
ALTER TABLE [SCDM_cat_usuarios_revision_departamento] ADD CONSTRAINT CK_SCDM_cat_usuarios_revision_departamento_tipo CHECK ([tipo] IN 
('PRIMARIO', 'SECUNDARIO')
)
GO



SET IDENTITY_INSERT [dbo].[SCDM_cat_usuarios_revision_departamento] ON 
SET IDENTITY_INSERT [dbo].[SCDM_cat_usuarios_revision_departamento] OFF

	  
IF object_id(N'SCDM_cat_usuarios_revision_departamento',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< SCDM_cat_usuarios_revision_departamento en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla SCDM_cat_usuarios_revision_departamento  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
