USE Portal_2_0
GO
IF object_id(N'IT_notificaciones_actividad',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IT_notificaciones_actividad]
      PRINT '<<< IT_notificaciones_actividad en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los IT_notificaciones_actividad
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/04/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [IT_notificaciones_actividad](
	[id][int] IDENTITY(1,1) NOT NULL,
	[descripcion] [varchar](100) NOT NULL,
	[periodo] [int] NULL,
	[tipo_periodo] [varchar](20) NULL,
	[es_recurrente] [bit] NOT NULL,
	[mensaje][varchar](1000) NOT NULL,
	[asunto][varchar](80) NOT NULL
 CONSTRAINT [PK_IT_notificaciones_actividad] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restricion check
ALTER TABLE [IT_notificaciones_actividad] ADD CONSTRAINT CK_it_notificaciones_actividad_tipo_periodo CHECK ([tipo_periodo] IN 
('DIAS', 'SEMANAS', 'MESES', 'AÑOS')
)

	  
IF object_id(N'IT_notificaciones_actividad',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IT_notificaciones_actividad en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IT_notificaciones_actividad  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
