USE Portal_2_0
GO
IF object_id(N'CI_conteo_inventario',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[CI_conteo_inventario]
      PRINT '<<< CI_conteo_inventario en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los CI_conteo_inventario
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/04/18
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [CI_conteo_inventario](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[centro] [varchar](4) NOT NULL,
	[almacen] [varchar](4) NOT NULL,
	[ubicacion] [varchar](20) NULL,
	[articulo] [varchar](5) NULL,
	[material] [varchar](15) NULL,
	[lote] [varchar](10) NULL,
	[no_bobina] [varchar](20) NULL,
	[sap_cantidad] [float] NULL,
	[libre_utilizacion] [float] NULL,
	[bloqueado] [float] NULL,
	[control_calidad] [float] NULL,
	[unidad_base_medida] [varchar](10) NULL,
	[altura] [float] NULL,
	[espesor] [float] NULL,
	[peso] [float] NULL,

	
 CONSTRAINT [PK_CI_conteo_inventario] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restricción default
--ALTER TABLE [CI_conteo_inventario] ADD  CONSTRAINT [DF_CI_conteo_inventario_activo]  DEFAULT (1) FOR [activo]
GO


SET IDENTITY_INSERT [CI_conteo_inventario] ON 

INSERT INTO [dbo].[CI_conteo_inventario]([id],[centro],[almacen],[ubicacion],[articulo],[material],[lote],[no_bobina],[sap_cantidad],[libre_utilizacion],[bloqueado],[control_calidad],[unidad_base_medida],[altura],[espesor],[peso])VALUES(1,N'5390','SI01','PT11-02','AL','AL01846','1740','1006800000',50,50,null, null,'PC',null, null, null)
INSERT INTO [dbo].[CI_conteo_inventario]([id],[centro],[almacen],[ubicacion],[articulo],[material],[lote],[no_bobina],[sap_cantidad],[libre_utilizacion],[bloqueado],[control_calidad],[unidad_base_medida],[altura],[espesor],[peso])VALUES(2,N'5390','SI01','PT11-02','AL','AL01846','1739','1006800000',50,50,null, null,'PC',null, null, null)
INSERT INTO [dbo].[CI_conteo_inventario]([id],[centro],[almacen],[ubicacion],[articulo],[material],[lote],[no_bobina],[sap_cantidad],[libre_utilizacion],[bloqueado],[control_calidad],[unidad_base_medida],[altura],[espesor],[peso])VALUES(3,N'5390','SI01','PT-030','AL','AL01846','1734','9818400000',259,null,259, null,'PC',null, null, null)
INSERT INTO [dbo].[CI_conteo_inventario]([id],[centro],[almacen],[ubicacion],[articulo],[material],[lote],[no_bobina],[sap_cantidad],[libre_utilizacion],[bloqueado],[control_calidad],[unidad_base_medida],[altura],[espesor],[peso])VALUES(4,N'5390','SI01','PT-030','AL','AL01846','1733','9818400000',259,null,259, null,'PC',null, null, null)
INSERT INTO [dbo].[CI_conteo_inventario]([id],[centro],[almacen],[ubicacion],[articulo],[material],[lote],[no_bobina],[sap_cantidad],[libre_utilizacion],[bloqueado],[control_calidad],[unidad_base_medida],[altura],[espesor],[peso])VALUES(5,N'5390','SI01','PT-030','AL','AL01846','1718','9861000000',15,null,15, null,'PC',null, null, null)
INSERT INTO [dbo].[CI_conteo_inventario]([id],[centro],[almacen],[ubicacion],[articulo],[material],[lote],[no_bobina],[sap_cantidad],[libre_utilizacion],[bloqueado],[control_calidad],[unidad_base_medida],[altura],[espesor],[peso])VALUES(6,N'5390','SI01','PT17-02','AL','AL01846','1717','9861000000',15,null,15, null,'PC',null, null, null)
INSERT INTO [dbo].[CI_conteo_inventario]([id],[centro],[almacen],[ubicacion],[articulo],[material],[lote],[no_bobina],[sap_cantidad],[libre_utilizacion],[bloqueado],[control_calidad],[unidad_base_medida],[altura],[espesor],[peso])VALUES(7,N'5390','SI01','PT-030','AL','AL01884','1478','90174 02 01',302,null,302, null,'PC',null, null, null)
INSERT INTO [dbo].[CI_conteo_inventario]([id],[centro],[almacen],[ubicacion],[articulo],[material],[lote],[no_bobina],[sap_cantidad],[libre_utilizacion],[bloqueado],[control_calidad],[unidad_base_medida],[altura],[espesor],[peso])VALUES(8,N'5390','SI01','PT-030','AL','AL01884','1405','88154 01 01',85,null,85, null,'PC',null, null, null)
INSERT INTO [dbo].[CI_conteo_inventario]([id],[centro],[almacen],[ubicacion],[articulo],[material],[lote],[no_bobina],[sap_cantidad],[libre_utilizacion],[bloqueado],[control_calidad],[unidad_base_medida],[altura],[espesor],[peso])VALUES(9,N'5390','SI01','PT-030','AL','AL01884','1480','90174 02 01',30,null,30, null,'PC',null, null, null)
INSERT INTO [dbo].[CI_conteo_inventario]([id],[centro],[almacen],[ubicacion],[articulo],[material],[lote],[no_bobina],[sap_cantidad],[libre_utilizacion],[bloqueado],[control_calidad],[unidad_base_medida],[altura],[espesor],[peso])VALUES(10,N'5390','SI01','PT-030','AL','AL01884','1546','95370 01 01',107,null,107, null,'PC',null, null, null)
INSERT INTO [dbo].[CI_conteo_inventario]([id],[centro],[almacen],[ubicacion],[articulo],[material],[lote],[no_bobina],[sap_cantidad],[libre_utilizacion],[bloqueado],[control_calidad],[unidad_base_medida],[altura],[espesor],[peso])VALUES(11,N'5390','SI01','PT10-F2-02','AL','AL01884','1578','94192 03 01',13,13,null, null,'PC',null, null, null)
INSERT INTO [dbo].[CI_conteo_inventario]([id],[centro],[almacen],[ubicacion],[articulo],[material],[lote],[no_bobina],[sap_cantidad],[libre_utilizacion],[bloqueado],[control_calidad],[unidad_base_medida],[altura],[espesor],[peso])VALUES(12,N'5390','SI01','PT10-F2-02','AL','AL01884','1577','94192 03 01',302,302,null, null,'PC',null, null, null)
INSERT INTO [dbo].[CI_conteo_inventario]([id],[centro],[almacen],[ubicacion],[articulo],[material],[lote],[no_bobina],[sap_cantidad],[libre_utilizacion],[bloqueado],[control_calidad],[unidad_base_medida],[altura],[espesor],[peso])VALUES(13,N'5390','SI01','PT11-02','AL','AL01884','1574','94192 03 01',302,302,null, null,'PC',null, null, null)
INSERT INTO [dbo].[CI_conteo_inventario]([id],[centro],[almacen],[ubicacion],[articulo],[material],[lote],[no_bobina],[sap_cantidad],[libre_utilizacion],[bloqueado],[control_calidad],[unidad_base_medida],[altura],[espesor],[peso])VALUES(14,N'5390','SI01','TR-ZONE','AL','AL01884','1573','94192 01 01',131,131,null, null,'PC',null, null, null)
INSERT INTO [dbo].[CI_conteo_inventario]([id],[centro],[almacen],[ubicacion],[articulo],[material],[lote],[no_bobina],[sap_cantidad],[libre_utilizacion],[bloqueado],[control_calidad],[unidad_base_medida],[altura],[espesor],[peso])VALUES(15,N'5390','SI01','PT11-02','AL','AL01884','1568','94192 02 01',57,57,null, null,'PC',null, null, null)



SET IDENTITY_INSERT [CI_conteo_inventario] OFF
 	  
IF object_id(N'CI_conteo_inventario',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< CI_conteo_inventario en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla CI_conteo_inventario  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
