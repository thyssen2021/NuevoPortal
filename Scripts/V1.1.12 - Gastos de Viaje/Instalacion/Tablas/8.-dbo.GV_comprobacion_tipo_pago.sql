--USE Portal_2_0
GO
IF object_id(N'GV_comprobacion_tipo_pago',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[GV_comprobacion_tipo_pago]
      PRINT '<<< GV_comprobacion_tipo_pago en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los GV_comprobacion_tipo_pago
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/12/22
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [GV_comprobacion_tipo_pago](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[descripcion] [varchar](50) NOT NULL,
	[activo] [bit] NOT NULL	
 CONSTRAINT [PK_GV_comprobacion_tipo_pago] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restricción default
ALTER TABLE [GV_comprobacion_tipo_pago] ADD  CONSTRAINT [DF_GV_comprobacion_tipo_pago_activo]  DEFAULT (1) FOR [activo]
GO


SET IDENTITY_INSERT [GV_comprobacion_tipo_pago] ON 

INSERT [GV_comprobacion_tipo_pago] ([id], [descripcion], [activo]) VALUES (1, N'Efectivo', 1)
INSERT [GV_comprobacion_tipo_pago] ([id], [descripcion], [activo]) VALUES (2, N'Tarjeta AMEX', 1)
INSERT [GV_comprobacion_tipo_pago] ([id], [descripcion], [activo]) VALUES (3, N'Tarjeta Si Vale', 1)
INSERT [GV_comprobacion_tipo_pago] ([id], [descripcion], [activo]) VALUES (4, N'Tarjeta Personal', 1)

SET IDENTITY_INSERT [GV_comprobacion_tipo_pago] OFF
GO

 	  
IF object_id(N'GV_comprobacion_tipo_pago',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< GV_comprobacion_tipo_pago en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla GV_comprobacion_tipo_pago  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
