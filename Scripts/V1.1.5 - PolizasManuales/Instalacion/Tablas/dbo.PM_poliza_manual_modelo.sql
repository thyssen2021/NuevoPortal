USE Portal_2_0
GO
IF object_id(N'PM_poliza_manual_modelo',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[PM_poliza_manual_modelo]
      PRINT '<<< PM_poliza_manual_modelo en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los PM_poliza_manual_modelo
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/01/20
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [PM_poliza_manual_modelo](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[descripcion] [varchar](80) NOT NULL,
	[fecha_creacion][datetime] NOT NULL DEFAULT GETDATE(),
	[comentario][varchar](250)NULL,
	[activo] [bit] NOT NULL	
 CONSTRAINT [PK_PM_poliza_manual_modelo] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restricción default
ALTER TABLE [PM_poliza_manual_modelo] ADD  CONSTRAINT [DF_PM_poliza_manual_modelo_activo]  DEFAULT (1) FOR [activo]
GO

IF object_id(N'PM_poliza_manual_modelo',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< PM_poliza_manual_modelo en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla PM_poliza_manual_modelo  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
