USE Portal_2_0
GO
IF object_id(N'upgrade_check_item',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[upgrade_check_item]
      PRINT '<<< upgrade_check_item en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los upgrade_check_item
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/03/25
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [upgrade_check_item](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[descripcion] [varchar](80) NOT NULL,
	[activo] [bit] NOT NULL	
 CONSTRAINT [PK_upgrade_check_item] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restricción default
ALTER TABLE [upgrade_check_item] ADD  CONSTRAINT [DF_upgrade_check_item_activo]  DEFAULT (1) FOR [activo]
GO




INSERT [upgrade_check_item] ([descripcion], [activo]) VALUES ( N'Acceso a SAP P60',1)
INSERT [upgrade_check_item] ([descripcion], [activo]) VALUES ( N'Acceso a menú de favoritos',1)
INSERT [upgrade_check_item] ([descripcion], [activo]) VALUES ( N'Permisos correctos a transacciones',1)
INSERT [upgrade_check_item] ([descripcion], [activo]) VALUES ( N'Impresión de reportes (Remisiones, facturas, Orden compra, etc)',1)
INSERT [upgrade_check_item] ([descripcion], [activo]) VALUES ( N'Impresión de etiquetas de código de barras',1)
INSERT [upgrade_check_item] ([descripcion], [activo]) VALUES ( N'Descarga de información a Excel o Txt',1)
INSERT [upgrade_check_item] ([descripcion], [activo]) VALUES ( N'Acceso a SAP por lectora de código de barras (PDA)',1)
INSERT [upgrade_check_item] ([descripcion], [activo]) VALUES ( N'Movimientos desde PDA',1)
INSERT [upgrade_check_item] ([descripcion], [activo]) VALUES ( N'Recepción de correo (Crear material, Liberar solicitud, EDI)',1)
INSERT [upgrade_check_item] ([descripcion], [activo]) VALUES ( N'Actualización de bitácora de producción',1)
--INSERT [upgrade_check_item] ([descripcion], [activo]) VALUES ( N'Captura de transacción probada',1)
--INSERT [upgrade_check_item] ([descripcion], [activo]) VALUES ( N'Transacción ',1)


GO




 	  
IF object_id(N'upgrade_check_item',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< upgrade_check_item en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla upgrade_check_item  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
