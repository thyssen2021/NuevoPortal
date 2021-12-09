USE Portal_2_0
GO
IF object_id(N'PM_conceptos',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[PM_conceptos]
      PRINT '<<< PM_conceptos en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los PM_conceptos
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2021/12/09
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [PM_conceptos](
	[id_poliza] [int] IDENTITY(1,1) NOT NULL,
	[cuenta] [int] NOT NULL,
	[cc] [int] NULL,
	[concepto][varchar](80) NULL,
	[poliza][varchar](10) NULL,
	[debe][decimal](11,2) NULL,
	[haber][decimal](11,2) NULL

 CONSTRAINT [PK_PM_conceptos] PRIMARY KEY CLUSTERED 
(
	[id_poliza] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
	  
IF object_id(N'PM_conceptos',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< PM_conceptos en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla PM_conceptos  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
