USE Portal_2_0
GO
IF object_id(N'IT_inventory_hardware_type',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IT_inventory_hardware_type]
      PRINT '<<< IT_inventory_hardware_type en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los IT_inventory_hardware_type
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creaci�n: 2022/06/06
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [IT_inventory_hardware_type](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[descripcion] [varchar](40) NOT NULL,
	[activo] [bit] NOT NULL	
 CONSTRAINT [PK_IT_inventory_hardware_type] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restricci�n default
ALTER TABLE [IT_inventory_hardware_type] ADD  CONSTRAINT [DF_IT_inventory_hardware_type_activo]  DEFAULT (1) FOR [activo]
GO


SET IDENTITY_INSERT [IT_inventory_hardware_type] ON 

INSERT [IT_inventory_hardware_type] ([id], [descripcion], [activo]) VALUES (1, N'Laptop', 1)
INSERT [IT_inventory_hardware_type] ([id], [descripcion], [activo]) VALUES (2, N'Desktop',1)
INSERT [IT_inventory_hardware_type] ([id], [descripcion], [activo]) VALUES (3, N'Monitor', 1)
INSERT [IT_inventory_hardware_type] ([id], [descripcion], [activo]) VALUES (4, N'Printer', 1)
INSERT [IT_inventory_hardware_type] ([id], [descripcion], [activo]) VALUES (5, N'Label Printer', 1)
INSERT [IT_inventory_hardware_type] ([id], [descripcion], [activo]) VALUES (6, N'PDA', 1)
INSERT [IT_inventory_hardware_type] ([id], [descripcion], [activo]) VALUES (7, N'Server', 1)
INSERT [IT_inventory_hardware_type] ([id], [descripcion], [activo]) VALUES (8, N'Tablet', 1)
INSERT [IT_inventory_hardware_type] ([id], [descripcion], [activo]) VALUES (9, N'Radio', 1)
INSERT [IT_inventory_hardware_type] ([id], [descripcion], [activo]) VALUES (10, N'AP', 1)
INSERT [IT_inventory_hardware_type] ([id], [descripcion], [activo]) VALUES (11, N'Smartphone', 1)

SET IDENTITY_INSERT [IT_inventory_hardware_type] OFF
GO


 	  
IF object_id(N'IT_inventory_hardware_type',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IT_inventory_hardware_type en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IT_inventory_hardware_type  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO