USE Portal_2_0
GO
IF object_id(N'budget_anio_fiscal',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[budget_anio_fiscal]
      PRINT '<<< budget_anio_fiscal en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los datos budget_anio_fiscal
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/02/23
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [budget_anio_fiscal](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[descripcion][varchar](20) NOT NULL,
	[anio_inicio][int] NOT NULL,
	[mes_inicio][int] NOT NULL,
	[anio_fin][int] NOT NULL,
	[mes_fin][int] NOT NULL,
 CONSTRAINT [PK_budget_anio_fiscal] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


 	  
IF object_id(N'budget_anio_fiscal',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< budget_anio_fiscal en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla budget_anio_fiscal  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
