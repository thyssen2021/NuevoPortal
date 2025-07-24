--USE Portal_2_0_budget
GO
IF object_id(N'BG_Forecast_cat_historico_scrap',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[BG_Forecast_cat_historico_scrap]
      PRINT '<<< BG_Forecast_cat_historico_scrap en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los datos BG_Forecast_cat_historico_scrap
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2024/06/05
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [BG_Forecast_cat_historico_scrap](
	[id] [int] IDENTITY(1,1) NOT NULL,	
	[id_planta][int] NOT NULL,
	[tipo_metal] [varchar](30) NOT NULL, 
	--[anio][int] NOT NULL,
	--[mes][int] NOT NULL,
	[fecha][date] NOT NULL,
	[scrap][FLOAT] NULL,
	--[scrap_default][FLOAT] NOT NULL,
	[scrap_ganancia][FLOAT] NOT NULL,

 CONSTRAINT [PK_BG_Forecast_cat_historico_scrap] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

  -- restriccion de clave foranea
alter table [BG_Forecast_cat_historico_scrap]
add constraint FK_BG_Forecast_cat_historico_scrap_id_planta
foreign key (id_planta)
references plantas(clave); 	  

--Agrega resticción check
ALTER TABLE [dbo].[BG_Forecast_cat_historico_scrap]  WITH CHECK ADD  CONSTRAINT [CK_Forecast_cat_historico_scrap_tipo_metal] CHECK  (([tipo_metal]='STEEL' OR [tipo_metal]='ALU' ))


 	  
IF object_id(N'BG_Forecast_cat_historico_scrap',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< BG_Forecast_cat_historico_scrap en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla BG_Forecast_cat_historico_scrap  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO


