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
*  Fecha de Creaci�n: 2022/03/25
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

-- restricci�n default
ALTER TABLE [upgrade_check_item] ADD  CONSTRAINT [DF_upgrade_check_item_activo]  DEFAULT (1) FOR [activo]
GO




INSERT [upgrade_check_item] ([descripcion], [activo]) VALUES ( N'Acceso a SAP P60',1)
INSERT [upgrade_check_item] ([descripcion], [activo]) VALUES ( N'Acceso a men� de favoritos',1)
INSERT [upgrade_check_item] ([descripcion], [activo]) VALUES ( N'Permisos correctos a transacciones',1)
INSERT [upgrade_check_item] ([descripcion], [activo]) VALUES ( N'Impresi�n de reportes (Remisiones, facturas, Orden compra, etc)',1)
INSERT [upgrade_check_item] ([descripcion], [activo]) VALUES ( N'Impresi�n de etiquetas de c�digo de barras',1)
INSERT [upgrade_check_item] ([descripcion], [activo]) VALUES ( N'Descarga de informaci�n a Excel o Txt',1)
INSERT [upgrade_check_item] ([descripcion], [activo]) VALUES ( N'Acceso a SAP por lectora de c�digo de barras (PDA)',1)
INSERT [upgrade_check_item] ([descripcion], [activo]) VALUES ( N'Movimientos desde PDA',1)
INSERT [upgrade_check_item] ([descripcion], [activo]) VALUES ( N'Recepci�n de correo (Crear material, Liberar solicitud, EDI)',1)
INSERT [upgrade_check_item] ([descripcion], [activo]) VALUES ( N'Actualizaci�n de bit�cora de producci�n',1)
--INSERT [upgrade_check_item] ([descripcion], [activo]) VALUES ( N'Captura de transacci�n probada',1)
--INSERT [upgrade_check_item] ([descripcion], [activo]) VALUES ( N'Transacci�n ',1)


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
