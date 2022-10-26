USE Portal_2_0
GO
IF object_id(N'BG_IHS_combinacion',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[BG_IHS_combinacion]
      PRINT '<<< BG_IHS_combinacion en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los datos BG_IHS_combinacion
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/10/26
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [BG_IHS_combinacion](
	[id] [int] IDENTITY(1,1) NOT NULL,	
	[vehicle][VARCHAR](100) NOT NULL,
	[production_brand][VARCHAR](30) NOT NULL,
	[sop_start_of_production][datetime] NULL,
	[eop_end_of_production][datetime] NULL,
	[comentario][varchar](150) NULL,
	[activo][bit] NOT NULL DEFAULT 1,	
 CONSTRAINT [PK_BG_IHS_combinacion] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

  -- restriccion unique
 alter table [BG_IHS_combinacion]
  add constraint UQ_BG_IHS_combinacion_vehicle
  unique (vehicle); -- ver cual será el campo unico para elemento de IHS personalizado
 
 	  
IF object_id(N'BG_IHS_combinacion',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< BG_IHS_combinacion en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla BG_IHS_combinacion  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO

