USE Portal_2_0
GO
IF object_id(N'BG_IHS_regiones',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[BG_IHS_regiones]
      PRINT '<<< BG_IHS_regiones en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los BG_IHS_regiones
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/10/06
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [BG_IHS_regiones](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[descripcion] [varchar](50) NOT NULL,
	[activo] [bit] NOT NULL	
 CONSTRAINT [PK_BG_IHS_regiones] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restricción default
ALTER TABLE [BG_IHS_regiones] ADD  CONSTRAINT [DF_BG_IHS_regiones_activo]  DEFAULT (1) FOR [activo]
GO


SET IDENTITY_INSERT [BG_IHS_regiones] ON 

INSERT [BG_IHS_regiones] ([id], [descripcion], [activo]) VALUES (1, N'CENTER', 1)
INSERT [BG_IHS_regiones] ([id], [descripcion], [activo]) VALUES (2, N'SOUTH', 1)
INSERT [BG_IHS_regiones] ([id], [descripcion], [activo]) VALUES (3, N'NORTH-WEST', 1)
INSERT [BG_IHS_regiones] ([id], [descripcion], [activo]) VALUES (4, N'NORTH', 1)
INSERT [BG_IHS_regiones] ([id], [descripcion], [activo]) VALUES (5, N'EXPORT', 1)

SET IDENTITY_INSERT [BG_IHS_regiones] OFF
GO

 	  
IF object_id(N'BG_IHS_regiones',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< BG_IHS_regiones en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla BG_IHS_regiones  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END	
SET ANSI_PADDING OFF
GO
