use [Portal_2_0]
GO
IF object_id(N'bom_en_sap',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[bom_en_sap]
      PRINT '<<< bom_en_sap en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los bom_en_sap
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2021/10/06
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[bom_en_sap](
	[Material] [nvarchar](30) NOT NULL,
	[Plnt] [nvarchar](20) NOT NULL,
	[BOM] [nvarchar](30) NOT NULL,
	[AltBOM] [nvarchar](10) NOT NULL,
	[Item] [nvarchar](10) NOT NULL,
	[Component] [nvarchar](30) NULL,
	[Created by] [nvarchar](50) NULL,
	[BOM1] [nvarchar](30) NULL,
	[Node] [nvarchar](30) NULL,
	[Quantity] [float] NULL,
	[Un] [nvarchar](20) NULL,
	[Created] [datetime] NULL,
	[activo][bit] NOT NULL,
	primary key(Material, Plnt, BOM, AltBOM,item)
) ON [PRIMARY]

GO

-- restricción default
ALTER TABLE [bom_en_sap] ADD  CONSTRAINT [DF_bom_en_sap_activo]  DEFAULT (1) FOR [activo]
GO


IF object_id(N'bom_en_sap',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< bom_en_sap en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla bom_en_sap  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
