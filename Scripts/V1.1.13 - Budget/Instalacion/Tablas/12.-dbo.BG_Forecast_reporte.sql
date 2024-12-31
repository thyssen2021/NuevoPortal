--USE Portal_2_0
GO
IF object_id(N'BG_Forecast_reporte',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[BG_Forecast_reporte]
      PRINT '<<< BG_Forecast_reporte en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los datos BG_Forecast_reporte
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/11/09
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [BG_Forecast_reporte](
	[id] [int] IDENTITY(1,1) NOT NULL,	
	[id_ihs_version] [int] NOT NULL,
	[fecha][datetime] NOT NULL,
	[descripcion][VARCHAR](80) NULL,	
	[activo][bit] NOT NULL DEFAULT 1,
 CONSTRAINT [PK_BG_Forecast_reporte] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

 -- restriccion de clave foranea
alter table [BG_Forecast_reporte]
 add constraint FK_BG_Forecast_reporte_id_ihs_version
  foreign key (id_ihs_version)
  references BG_IHS_versiones(id);
 	  
IF object_id(N'BG_Forecast_reporte',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< BG_Forecast_reporte en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla BG_Forecast_reporte  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO

