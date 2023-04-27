--USE Portal_2_0
GO
IF object_id(N'RM_remision_motivo',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[RM_remision_motivo]
      PRINT '<<< RM_remision_motivo en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los RM_remision_motivo
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[RM_remision_motivo](
	[clave] [int] IDENTITY(1,1) NOT NULL,
	[activo] [bit] NOT NULL,
	[descripcion] [varchar](200) NOT NULL,
	[correoElectronicoResponsable] [varchar](200) NULL,
	[seCierraEnSAP] [bit] NOT NULL,
 CONSTRAINT [PK_CatalogoRemisionMotivo] PRIMARY KEY CLUSTERED 
(
	[clave] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[RM_remision_motivo] ON 
GO
INSERT [dbo].[RM_remision_motivo] ([clave], [activo], [descripcion], [correoElectronicoResponsable], [seCierraEnSAP]) VALUES (1, 1, N'PROBLEMA CON EL ASN', NULL, 1)
GO
INSERT [dbo].[RM_remision_motivo] ([clave], [activo], [descripcion], [correoElectronicoResponsable], [seCierraEnSAP]) VALUES (3, 1, N'FALTA PRECIO /CANTIDAD /PERIODO Y BLOQUEADO
', NULL, 1)
GO
INSERT [dbo].[RM_remision_motivo] ([clave], [activo], [descripcion], [correoElectronicoResponsable], [seCierraEnSAP]) VALUES (5, 1, N'INVENTARIOS EN PROCESO
', NULL, 1)
GO
INSERT [dbo].[RM_remision_motivo] ([clave], [activo], [descripcion], [correoElectronicoResponsable], [seCierraEnSAP]) VALUES (7, 1, N'MATERIAL SIN PEDIDO', NULL, 1)
GO
INSERT [dbo].[RM_remision_motivo] ([clave], [activo], [descripcion], [correoElectronicoResponsable], [seCierraEnSAP]) VALUES (8, 1, N'SALIDAS DE MATERIAL POR DEFECTOS O RETORNOS CMS
', NULL, 1)
GO
INSERT [dbo].[RM_remision_motivo] ([clave], [activo], [descripcion], [correoElectronicoResponsable], [seCierraEnSAP]) VALUES (9, 1, N'SALIDAS DE MATERIAL POR DEFECTOS O RETORNOS POR FALLAS DE MATERIAL PROPIO
', NULL, 1)
GO
INSERT [dbo].[RM_remision_motivo] ([clave], [activo], [descripcion], [correoElectronicoResponsable], [seCierraEnSAP]) VALUES (10, 1, N'INCONVENIENTE PARA LIBERACION DE MATERIAL POR CALIDAD
', NULL, 1)
GO
INSERT [dbo].[RM_remision_motivo] ([clave], [activo], [descripcion], [correoElectronicoResponsable], [seCierraEnSAP]) VALUES (11, 1, N'NUMERO DE PARTE SAP NO HA SIDO CREADO
', NULL, 1)
GO
INSERT [dbo].[RM_remision_motivo] ([clave], [activo], [descripcion], [correoElectronicoResponsable], [seCierraEnSAP]) VALUES (12, 1, N'NO EXISTE ORDEN DE COMPRA PARA PROCESAMIENTO EXTERNO
', NULL, 1)
GO
INSERT [dbo].[RM_remision_motivo] ([clave], [activo], [descripcion], [correoElectronicoResponsable], [seCierraEnSAP]) VALUES (13, 1, N'OTROS / INGRESAR EXPLICACION', NULL, 1)
GO
INSERT [dbo].[RM_remision_motivo] ([clave], [activo], [descripcion], [correoElectronicoResponsable], [seCierraEnSAP]) VALUES (14, 1, N'NO SE REGULARIZA EN SAP / SALIDA MATERIAL NO PRODUCTIVO', NULL, 0)
GO
INSERT [dbo].[RM_remision_motivo] ([clave], [activo], [descripcion], [correoElectronicoResponsable], [seCierraEnSAP]) VALUES (15, 1, N'POR DEFINIR / NO ES SUFICIENTE EL MENSAJE DE SAP', NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[RM_remision_motivo] OFF
GO
	  
IF object_id(N'RM_remision_motivo',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< RM_remision_motivo en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla RM_remision_motivo  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
