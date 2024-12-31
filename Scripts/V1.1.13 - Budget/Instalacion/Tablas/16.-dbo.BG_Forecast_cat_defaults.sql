--USE Portal_2_0_budget
GO
IF object_id(N'BG_Forecast_cat_defaults',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[BG_Forecast_cat_defaults]
      PRINT '<<< BG_Forecast_cat_defaults en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los datos BG_Forecast_cat_defaults
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2024/06/05
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [BG_Forecast_cat_defaults](
	[id] [int] IDENTITY(1,1) NOT NULL,	
	[scrap_acero_valor_puebla] [float] NOT NULL default 0, 
	[scrap_acero_ganancia_puebla] [float] NOT NULL default 0, 
	[scrap_aluminio_valor_puebla] [float] NOT NULL default 0, 
	[scrap_aluminio_ganancia_puebla] [float] NOT NULL default 0, 
	[scrap_acero_valor_silao] [float] NOT NULL default 0, 
	[scrap_acero_ganancia_silao] [float] NOT NULL default 0, 
	[scrap_aluminio_valor_silao] [float] NOT NULL default 0, 
	[scrap_aluminio_ganancia_silao] [float] NOT NULL default 0, 
	 
 CONSTRAINT [PK_BG_Forecast_cat_defaults] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

--insert into [BG_Forecast_cat_defaults](scrap_acero_valor_puebla) values (350)

  -- restriccion de clave foranea
--alter table [BG_Forecast_cat_defaults]
-- add constraint FK_BG_Forecast_cat_defaults_id_bg_forecast_reporte
--  foreign key (id_bg_forecast_reporte)
--  references BG_Forecast_reporte(id); 	  

--Agrega resticción check
--ALTER TABLE [dbo].[BG_Forecast_cat_defaults]  WITH CHECK ADD  CONSTRAINT [CK_Forecast_cat_historico_scrap_tipo_metal] CHECK  (([tipo_metal]='STEEL' OR [tipo_metal]='ALU' ))


 	  
IF object_id(N'BG_Forecast_cat_defaults',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< BG_Forecast_cat_defaults en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla BG_Forecast_cat_defaults  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
