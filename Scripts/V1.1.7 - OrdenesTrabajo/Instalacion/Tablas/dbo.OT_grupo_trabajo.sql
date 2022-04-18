USE Portal_2_0
GO
IF object_id(N'OT_grupo_trabajo',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[OT_grupo_trabajo]
      PRINT '<<< OT_grupo_trabajo en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los OT_grupo_trabajo
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/04/18
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [OT_grupo_trabajo](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[descripcion] [varchar](40) NOT NULL,
	[activo] [bit] NOT NULL	
 CONSTRAINT [PK_OT_grupo_trabajo] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restricción default
ALTER TABLE [OT_grupo_trabajo] ADD  CONSTRAINT [DF_OT_grupo_trabajo_activo]  DEFAULT (1) FOR [activo]
GO


SET IDENTITY_INSERT [OT_grupo_trabajo] ON 

INSERT [OT_grupo_trabajo] ([id], [descripcion],[activo]) VALUES (1, N'GRUPO 1',1)
INSERT [OT_grupo_trabajo] ([id], [descripcion],[activo]) VALUES (2, N'GRUPO 2',1)
INSERT [OT_grupo_trabajo] ([id], [descripcion],[activo]) VALUES (3, N'GRUPO 3',1)
INSERT [OT_grupo_trabajo] ([id], [descripcion],[activo]) VALUES (4, N'GRUPO 4',1)

SET IDENTITY_INSERT [OT_grupo_trabajo] OFF
GO
 	  
IF object_id(N'OT_grupo_trabajo',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< OT_grupo_trabajo en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla OT_grupo_trabajo  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
