USE Portal_2_0
GO
IF object_id(N'RH_menu_comedor_platillos',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[RH_menu_comedor_platillos]
      PRINT '<<< RH_menu_comedor_platillos en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los RH_menu_comedor_platillos
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [RH_menu_comedor_platillos](
	[id][int] IDENTITY(1,1) NOT NULL,
	[orden_display] [int] NOT NULL,
	[tipo_platillo] [varchar](30) NOT NULL,
	[nombre_platillo] [varchar](60) NOT NULL,
	[fecha] [datetime] NOT NULL,
	[kcal][int] NULL
 CONSTRAINT [PK_RH_menu_comedor_platillos] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

	  
IF object_id(N'RH_menu_comedor_platillos',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< RH_menu_comedor_platillos en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla RH_menu_comedor_platillos  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
