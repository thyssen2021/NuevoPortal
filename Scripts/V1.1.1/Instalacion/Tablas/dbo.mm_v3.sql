use [Portal_2_0]
GO
IF object_id(N'mm_v3',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[mm_v3]
      PRINT '<<< mm_v3 en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los mm_v3
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2021/10/06
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [dbo].[mm_v3](
	[Material] [nvarchar](30) NOT NULL,
	[Plnt] [nvarchar](30) NOT NULL,
	[MS] [nvarchar](30) NULL,
	[Material Description] [nvarchar](255) NULL,
	[Type of Material] [nvarchar](255) NULL,
	[Type of Metal] [nvarchar](255) NULL,
	[Old material no#] [nvarchar](255) NULL,
	[Head and Tails Scrap Conciliation] [nvarchar](255) NULL,
	[Engineering Scrap conciliation] [nvarchar](255) NULL,
	[Business Model] [nvarchar](255) NULL,
	[Re-application] [nvarchar](255) NULL,
	[IHS number 1] [nvarchar](255) NULL,
	[IHS number 2] [nvarchar](255) NULL,
	[IHS number 3] [nvarchar](255) NULL,
	[IHS number 4] [nvarchar](255) NULL,
	[IHS number 5] [nvarchar](255) NULL,
	[Type of Selling] [nvarchar](255) NULL,
	[Package Pieces] [nvarchar](255) NULL,
	[Gross weight] [float] NULL,
	[Un#] [nvarchar](255) NULL,
	[Net weight] [float] NULL,
	[Un#1] [nvarchar](255) NULL,
	[Thickness] [float] NULL,
	[Width] [float] NULL,
	[Advance] [float] NULL,
	[Head and Tail allowed scrap] [float] NULL,
	[Pieces per car] [float] NULL,
	[Initial Weight] [float] NULL,
	[Min Weight] [float] NULL,
	[Maximum Weight] [float] NULL,
	[activo][bit] NOT NULL
	primary key(Material,Plnt)
) ON [PRIMARY]
GO

-- restricción default
ALTER TABLE [mm_v3] ADD  CONSTRAINT [DF_mm_v3_activo]  DEFAULT (1) FOR [activo]
GO


IF object_id(N'mm_v3',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< mm_v3 en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla mm_v3  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
