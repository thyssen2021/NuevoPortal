USE Portal_2_0
GO
IF object_id(N'GV_comprobacion_tipo_gastos_viaje',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[GV_comprobacion_tipo_gastos_viaje]
      PRINT '<<< GV_comprobacion_tipo_gastos_viaje en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los GV_comprobacion_tipo_gastos_viaje
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creaci�n: 2022/08/17
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [GV_comprobacion_tipo_gastos_viaje](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[descripcion] [varchar](50) NOT NULL,
	[cuenta] [varchar](6) NOT NULL,
	[activo] [bit] NOT NULL	
 CONSTRAINT [PK_GV_comprobacion_tipo_gastos_viaje] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restricci�n default
ALTER TABLE [GV_comprobacion_tipo_gastos_viaje] ADD  CONSTRAINT [DF_GV_comprobacion_tipo_gastos_viaje_activo]  DEFAULT (1) FOR [activo]
GO


SET IDENTITY_INSERT [GV_comprobacion_tipo_gastos_viaje] ON 

INSERT [GV_comprobacion_tipo_gastos_viaje] ([id], [descripcion], [cuenta], [activo]) VALUES (1, N'No Deducibles',N'610010', 1)
INSERT [GV_comprobacion_tipo_gastos_viaje] ([id], [descripcion], [cuenta], [activo]) VALUES (2, N'Hospedaje',N'610030', 1)
INSERT [GV_comprobacion_tipo_gastos_viaje] ([id], [descripcion], [cuenta], [activo]) VALUES (3, N'Comidas',N'610040', 1)
INSERT [GV_comprobacion_tipo_gastos_viaje] ([id], [descripcion], [cuenta], [activo]) VALUES (4, N'Vuelos',N'610070', 1)
INSERT [GV_comprobacion_tipo_gastos_viaje] ([id], [descripcion], [cuenta], [activo]) VALUES (5, N'Transporte terrestre',N'610071', 1)
INSERT [GV_comprobacion_tipo_gastos_viaje] ([id], [descripcion], [cuenta], [activo]) VALUES (6, N'Renta de automovil',N'610072', 1)
INSERT [GV_comprobacion_tipo_gastos_viaje] ([id], [descripcion], [cuenta], [activo]) VALUES (7, N'Casetas',N'610073', 1)
INSERT [GV_comprobacion_tipo_gastos_viaje] ([id], [descripcion], [cuenta], [activo]) VALUES (8, N'Combustible',N'610074', 1)
INSERT [GV_comprobacion_tipo_gastos_viaje] ([id], [descripcion], [cuenta], [activo]) VALUES (9, N'Varios',N'610080', 1)



SET IDENTITY_INSERT [GV_comprobacion_tipo_gastos_viaje] OFF
GO

 	  
IF object_id(N'GV_comprobacion_tipo_gastos_viaje',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< GV_comprobacion_tipo_gastos_viaje en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla GV_comprobacion_tipo_gastos_viaje  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
