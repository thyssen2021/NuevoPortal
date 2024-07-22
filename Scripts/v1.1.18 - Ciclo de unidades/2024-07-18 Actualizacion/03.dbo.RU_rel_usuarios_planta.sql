--USE Portal_2_0
GO
IF object_id(N'RU_rel_usuarios_planta',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[RU_rel_usuarios_planta]
      PRINT '<<< RU_rel_usuarios_planta en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los RU_rel_usuarios_planta
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/05/15
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[RU_rel_usuarios_planta](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_usuario] [nvarchar](128) NOT NULL,
	[id_planta][int] NOT NULL,	
 CONSTRAINT [PK_RU_rel_usuarios_planta] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restriccion de clave foranea
alter table [dbo].[RU_rel_usuarios_planta]
 add constraint FK_RU_rel_usuarios_plantas_id_usuario
  foreign key (id_usuario)
  references AspNetUsers(Id);

-- restriccion de clave foranea
alter table [dbo].[RU_rel_usuarios_planta]
 add constraint FK_RU_rel_usuarios_plantas_id_planta
  foreign key (id_planta)
  references plantas(clave);


GO

--inserta datos
GO

	  
IF object_id(N'RU_rel_usuarios_planta',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< RU_rel_usuarios_planta en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla RU_rel_usuarios_planta  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO

-- Inserta los rel usarios planta


GO
SET IDENTITY_INSERT [dbo].[RU_rel_usuarios_planta] ON 
GO
INSERT [dbo].[RU_rel_usuarios_planta] ([id], [id_usuario], [id_planta]) VALUES (1, N'026e9375-3007-45d2-82a2-32f0ccb1d32c', 1)
GO
INSERT [dbo].[RU_rel_usuarios_planta] ([id], [id_usuario], [id_planta]) VALUES (2, N'a30f266c-1a7e-4370-ac58-70d256c0bec5', 1)
GO
INSERT [dbo].[RU_rel_usuarios_planta] ([id], [id_usuario], [id_planta]) VALUES (3, N'99aa631d-411b-4b62-a773-b0be6f11b01b', 1)
GO
INSERT [dbo].[RU_rel_usuarios_planta] ([id], [id_usuario], [id_planta]) VALUES (4, N'661bd3c7-c9d3-4207-96b3-6670ae31a46c', 1)
GO
INSERT [dbo].[RU_rel_usuarios_planta] ([id], [id_usuario], [id_planta]) VALUES (5, N'fcd708e4-7d31-47a2-af28-d5d075f7966b', 1)
GO
SET IDENTITY_INSERT [dbo].[RU_rel_usuarios_planta] OFF
GO


