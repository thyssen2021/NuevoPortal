use [Portal_2_0]
GO
IF object_id(N'class_v3',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[class_v3]
      PRINT '<<< class_v3 en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los class_v3
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2021/10/06
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[class_v3](
	[Object] [nvarchar](30) NOT NULL,
	[Grade] [nvarchar](30) NULL,
	[Customer] [nvarchar](30) NULL,
	[Shape] [nvarchar](30) NULL,
	[Customer part number] [nvarchar](255) NULL,
	[Surface] [nvarchar](30) NULL,
	[Gauge - Metric] [nvarchar](255) NULL,
	[Mill] [nvarchar](255) NULL,
	[Width - Metr] [nvarchar](255) NULL,
	[Length(mm)] [nvarchar](255) NULL,
	[activo][bit] NOT NULL,
	primary key([Object])
) ON [PRIMARY]

GO

-- restricción default
ALTER TABLE [class_v3] ADD  CONSTRAINT [DF_class_v3_activo]  DEFAULT (1) FOR [activo]
GO


IF object_id(N'class_v3',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< class_v3 en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla class_v3  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
