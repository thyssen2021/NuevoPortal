USE Portal_2_0
GO
IF object_id(N'inspeccion_categoria_fallas',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[inspeccion_categoria_fallas]
      PRINT '<<< inspeccion_categoria_fallas en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar las categorias de las piezas de descarte
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/01/06
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [inspeccion_categoria_fallas](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[descripcion] [varchar](50) NOT NULL,
	[activo] [bit] NOT NULL	
 CONSTRAINT [PK_inspeccion_categoria_fallas] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restricción default
ALTER TABLE [inspeccion_categoria_fallas] ADD  CONSTRAINT [DF_inspeccion_categoria_fallas_activo]  DEFAULT (1) FOR [activo]
GO


SET IDENTITY_INSERT [inspeccion_categoria_fallas] ON 

INSERT [inspeccion_categoria_fallas] ([id], [descripcion], [activo]) VALUES (1, N'Fallas de Almacen de recibo', 1)
INSERT [inspeccion_categoria_fallas] ([id], [descripcion], [activo]) VALUES (2, N'Fallas de Troquel', 1)
INSERT [inspeccion_categoria_fallas] ([id], [descripcion], [activo]) VALUES (3, N'Fallas de Proceso', 1)
INSERT [inspeccion_categoria_fallas] ([id], [descripcion], [activo]) VALUES (4, N'Daños por Manejo Logistico', 1)
INSERT [inspeccion_categoria_fallas] ([id], [descripcion], [activo]) VALUES (5, N'Fallas de Molino', 1)

SET IDENTITY_INSERT [inspeccion_categoria_fallas] OFF
GO




 	  
IF object_id(N'inspeccion_categoria_fallas',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< inspeccion_categoria_fallas en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla inspeccion_categoria_fallas  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
