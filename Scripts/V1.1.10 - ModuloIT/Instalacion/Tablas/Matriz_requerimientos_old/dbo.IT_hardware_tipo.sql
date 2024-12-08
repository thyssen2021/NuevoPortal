USE Portal_2_0
GO
IF object_id(N'IT_hardware_tipo',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IT_hardware_tipo]
      PRINT '<<< IT_hardware_tipo en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los IT_hardware_tipo
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/05/10
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [IT_hardware_tipo](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[descripcion] [varchar](40) NOT NULL,
	[aplica_descripcion] [bit] NOT NULL,
	[activo] [bit] NOT NULL	
 CONSTRAINT [PK_IT_hardware_tipo] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restricción default
ALTER TABLE [IT_hardware_tipo] ADD  CONSTRAINT [DF_IT_hardware_tipo_activo]  DEFAULT (1) FOR [activo]
GO


SET IDENTITY_INSERT [IT_hardware_tipo] ON 

INSERT [IT_hardware_tipo] ([id], [descripcion],[aplica_descripcion], [activo]) VALUES (1, N'Laptop',0, 1)
INSERT [IT_hardware_tipo] ([id], [descripcion],[aplica_descripcion], [activo]) VALUES (2, N'Desktop',0, 1)
INSERT [IT_hardware_tipo] ([id], [descripcion],[aplica_descripcion], [activo]) VALUES (3, N'Monitor',0, 1)
INSERT [IT_hardware_tipo] ([id], [descripcion],[aplica_descripcion], [activo]) VALUES (4, N'Mouse',0, 1)
INSERT [IT_hardware_tipo] ([id], [descripcion],[aplica_descripcion], [activo]) VALUES (5, N'Teclado',0, 1)
INSERT [IT_hardware_tipo] ([id], [descripcion],[aplica_descripcion], [activo]) VALUES (6, N'Otro (especificar)',1, 1)

SET IDENTITY_INSERT [IT_hardware_tipo] OFF
GO


 	  
IF object_id(N'IT_hardware_tipo',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IT_hardware_tipo en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IT_hardware_tipo  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
