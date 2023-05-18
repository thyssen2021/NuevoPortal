USE Portal_2_0
GO
IF object_id(N'RU_registros',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[RU_registros]
      PRINT '<<< RU_registros en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los RU_registros
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/05/15
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[RU_registros](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[fecha_ingreso_vigilancia][datetime] NOT NULL,
	[linea_transporte] [varchar](50) NOT NULL,
	[placas_tractor][varchar](20) NOT NULL,
	[nombre_operador][varchar](100) NOT NULL,
	[id_vigilancia_ingreso][int] NOT NULL,  --FK
	[comentarios_vigilancia_ingreso][varchar](150) NULL,
	[hora_embarques_recepcion][datetime] NULL,
	[id_embarques_recepcion][int] NULL,		--FK
	[comentarios_embarques_recepcion][varchar](150) NULL,
	[hora_embarques_liberacion][datetime] NULL,
	[id_embarques_liberacion][int] NULL,		--FK
	[comentarios_embarques_liberacion][varchar](150) NULL,
	[hora_vigilancia_liberacion][datetime] NULL,
	[comentarios_vigilancia_liberacion][varchar](150) NULL,
	[id_vigilancia_liberacion][int] NULL,		--FK
	[hora_vigilancia_salida][datetime] NULL,
	[comentarios_vigilancia_salida][varchar](150) NULL,
	[id_vigilancia_salida][int] NULL,		--FK
	[hora_cancelacion][datetime] NULL,
	[nombre_cancelacion][varchar](100) NULL,
	[comentario_cancelacion][varchar](150) NULL,
	[carga] [bit] NOT NULL DEFAULT 0,
	[descarga] [bit] NOT NULL DEFAULT 0,
	[activo] [bit] NOT NULL DEFAULT 1,	
 CONSTRAINT [PK_RU_registros] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

-- Foreign Keys
-- restriccion de clave foranea
alter table [dbo].[RU_registros]
 add constraint FK_RU_registros_id_vigilancia_ingreso
  foreign key (id_vigilancia_ingreso)
  references RU_usuarios_vigilancia(id);

  -- restriccion de clave foranea
alter table [dbo].[RU_registros]
 add constraint FK_RU_registros_id_embarques_recepcion
  foreign key (id_embarques_recepcion)
  references empleados(id);

  -- restriccion de clave foranea
alter table [dbo].[RU_registros]
 add constraint FK_RU_registros_id_embarques_liberacion
  foreign key (id_embarques_liberacion)
  references empleados(id);

  -- restriccion de clave foranea
alter table [dbo].[RU_registros]
 add constraint FK_RU_registros_id_vigilancia_liberacion
  foreign key (id_vigilancia_liberacion)
  references RU_usuarios_vigilancia(id);

    -- restriccion de clave foranea
alter table [dbo].[RU_registros]
 add constraint FK_RU_registros_id_vigilancia_salida
  foreign key (id_vigilancia_salida)
  references RU_usuarios_vigilancia(id);



--inserta datos
GO
--SET IDENTITY_INSERT [dbo].[RU_registros] ON 
--SET IDENTITY_INSERT [dbo].[RU_registros] OFF
	  
IF object_id(N'RU_registros',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< RU_registros en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla RU_registros  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
