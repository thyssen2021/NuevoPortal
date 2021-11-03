USE Portal_2_0
GO
IF object_id(N'PFA_Border_port',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[PFA_Border_port]
      PRINT '<<< PFA_Border_port en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los PFA_Border_port
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2021/11/01
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [PFA_Border_port](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[descripcion] [varchar](30) NOT NULL,
	[activo] [bit] NOT NULL	
 CONSTRAINT [PK_PFA_Border_port] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restricción default
ALTER TABLE [PFA_Border_port] ADD  CONSTRAINT [DF_PFA_Border_port_activo]  DEFAULT (1) FOR [activo]
GO


SET IDENTITY_INSERT [PFA_Border_port] ON 

INSERT [PFA_Border_port] ([id], [descripcion], [activo]) VALUES (1, N'Veracruz', 1)
INSERT [PFA_Border_port] ([id], [descripcion], [activo]) VALUES (2, N'Altamira', 1)
INSERT [PFA_Border_port] ([id], [descripcion], [activo]) VALUES (3, N'Lazaro Cardenas', 1)
INSERT [PFA_Border_port] ([id], [descripcion], [activo]) VALUES (4, N'Manzanillo', 1)
INSERT [PFA_Border_port] ([id], [descripcion], [activo]) VALUES (5, N'Tampico', 1)
INSERT [PFA_Border_port] ([id], [descripcion], [activo]) VALUES (6, N'Laredo', 1)
INSERT [PFA_Border_port] ([id], [descripcion], [activo]) VALUES (7, N'Other', 1)

SET IDENTITY_INSERT [PFA_Border_port] OFF
GO




 	  
IF object_id(N'PFA_Border_port',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< PFA_Border_port en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla PFA_Border_port  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
