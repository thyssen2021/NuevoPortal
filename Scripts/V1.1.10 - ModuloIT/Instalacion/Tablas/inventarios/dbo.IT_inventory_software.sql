USE Portal_2_0
GO
IF object_id(N'IT_inventory_software',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IT_inventory_software]
      PRINT '<<< IT_inventory_software en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los IT_inventory_software
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/05/10
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [IT_inventory_software](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[descripcion] [varchar](40) NOT NULL,
	--[aplica_descripcion] [bit] NOT NULL,
	[activo] [bit] NOT NULL	
 CONSTRAINT [PK_IT_inventory_software] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restricción default
ALTER TABLE [IT_inventory_software] ADD  CONSTRAINT [DF_IT_inventory_software_activo]  DEFAULT (1) FOR [activo]
GO


SET IDENTITY_INSERT [IT_inventory_software] ON 

INSERT [IT_inventory_software] ([id], [descripcion], [activo]) VALUES (1, N'TRESS SYSTEMS',1)
INSERT [IT_inventory_software] ([id], [descripcion], [activo]) VALUES (2, N'OEE Analyzer',1)
INSERT [IT_inventory_software] ([id], [descripcion], [activo]) VALUES (3, N'SAP',1)
INSERT [IT_inventory_software] ([id], [descripcion], [activo]) VALUES (4, N'Webex Teams',1)
INSERT [IT_inventory_software] ([id], [descripcion], [activo]) VALUES (5, N'Webex Meetings',1)
INSERT [IT_inventory_software] ([id], [descripcion], [activo]) VALUES (6, N'VNC Server & Viewer',1)
INSERT [IT_inventory_software] ([id], [descripcion], [activo]) VALUES (7, N'Microsoft Office Professional Plus',1)
INSERT [IT_inventory_software] ([id], [descripcion], [activo]) VALUES (8, N'Microsoft Visio',1)
INSERT [IT_inventory_software] ([id], [descripcion], [activo]) VALUES (9, N'Microsoft Project',1)
INSERT [IT_inventory_software] ([id], [descripcion], [activo]) VALUES (10, N'Microsoft Teams',1)
INSERT [IT_inventory_software] ([id], [descripcion], [activo]) VALUES (11, N'McAfee Endpoint Security',1)
INSERT [IT_inventory_software] ([id], [descripcion], [activo]) VALUES (12, N'AutoCAD Full Edition',1)
INSERT [IT_inventory_software] ([id], [descripcion], [activo]) VALUES (13, N'Cisco vpn client',1)
INSERT [IT_inventory_software] ([id], [descripcion], [activo]) VALUES (14, N'Cisco AnyConnect Secure Mobility Client',1)
INSERT [IT_inventory_software] ([id], [descripcion], [activo]) VALUES (15, N'Cofidi',1)
INSERT [IT_inventory_software] ([id], [descripcion], [activo]) VALUES (16, N'MP9',1)
INSERT [IT_inventory_software] ([id], [descripcion], [activo]) VALUES (17, N'Adobe Acrobat Suscription',1)
INSERT [IT_inventory_software] ([id], [descripcion], [activo]) VALUES (18, N'Revuelta XXI',1)
INSERT [IT_inventory_software] ([id], [descripcion], [activo]) VALUES (19, N'Proxmox Mail Gateway',1)
INSERT [IT_inventory_software] ([id], [descripcion], [activo]) VALUES (20, N'Gensuite',1)
INSERT [IT_inventory_software] ([id], [descripcion], [activo]) VALUES (21, N'DATALYZER',1)
INSERT [IT_inventory_software] ([id], [descripcion], [activo]) VALUES (22, N'Minitab',1)
INSERT [IT_inventory_software] ([id], [descripcion], [activo]) VALUES (23, N'SoftExpert FMEA',1)
INSERT [IT_inventory_software] ([id], [descripcion], [activo]) VALUES (24, N'SQL Server estandar',1)
INSERT [IT_inventory_software] ([id], [descripcion], [activo]) VALUES (25, N'Windows 10',1)
INSERT [IT_inventory_software] ([id], [descripcion], [activo]) VALUES (26, N'Autocad LT',1)
INSERT [IT_inventory_software] ([id], [descripcion], [activo]) VALUES (27, N'Windows Server estándar',1)
INSERT [IT_inventory_software] ([id], [descripcion], [activo]) VALUES (28, N'Ivanti Velocity TE Client License',1)



SET IDENTITY_INSERT [IT_inventory_software] OFF
GO


 	  
IF object_id(N'IT_inventory_software',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IT_inventory_software en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IT_inventory_software  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
