--USE Portal_2_0
GO
IF object_id(N'RU_usuarios_vigilancia',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[RU_usuarios_vigilancia]
      PRINT '<<< RU_usuarios_vigilancia en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los RU_usuarios_vigilancia
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[RU_usuarios_vigilancia](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[nombre][varchar](120) NOT NULL,
	[activo] [bit] NOT NULL DEFAULT 1,	
 CONSTRAINT [PK_RU_usuarios_vigilancia] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
GO

--inserta datos
GO
SET IDENTITY_INSERT [dbo].[RU_usuarios_vigilancia] ON 

INSERT INTO [RU_usuarios_vigilancia] (id, nombre, activo) VALUES (1, N'GUILLERMO CHOLULA', 1)
INSERT INTO [RU_usuarios_vigilancia] (id, nombre, activo) VALUES (2, N'ANTONIO CRISANTO', 1)
INSERT INTO [RU_usuarios_vigilancia] (id, nombre, activo) VALUES (3, N'SAUL NAVA', 1)

SET IDENTITY_INSERT [dbo].[RU_usuarios_vigilancia] OFF
	  
IF object_id(N'RU_usuarios_vigilancia',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< RU_usuarios_vigilancia en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla RU_usuarios_vigilancia  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
