--USE Portal_2_0
GO
IF object_id(N'GV_medios_transporte',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[GV_medios_transporte]
      PRINT '<<< GV_medios_transporte en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los GV_medios_transporte
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/08/17
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [GV_medios_transporte](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[descripcion] [varchar](50) NOT NULL,
	[activo] [bit] NOT NULL	
 CONSTRAINT [PK_GV_medios_transporte] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restricción default
ALTER TABLE [GV_medios_transporte] ADD  CONSTRAINT [DF_GV_medios_transporte_activo]  DEFAULT (1) FOR [activo]
GO


SET IDENTITY_INSERT [GV_medios_transporte] ON 

INSERT [GV_medios_transporte] ([id], [descripcion], [activo]) VALUES (1, N'Avión', 1)
INSERT [GV_medios_transporte] ([id], [descripcion], [activo]) VALUES (2, N'Auto de la empresa', 1)
INSERT [GV_medios_transporte] ([id], [descripcion], [activo]) VALUES (3, N'Auto propio', 1)
INSERT [GV_medios_transporte] ([id], [descripcion], [activo]) VALUES (4, N'Autobus', 1)
INSERT [GV_medios_transporte] ([id], [descripcion], [activo]) VALUES (5, N'Renta de auto', 1)


SET IDENTITY_INSERT [GV_medios_transporte] OFF
GO

 	  
IF object_id(N'GV_medios_transporte',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< GV_medios_transporte en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla GV_medios_transporte  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
