use[Portal_2_0]
GO
IF object_id(N'log_envio_correo',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[log_envio_correo]
      PRINT '<<< log_envio_correo en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los log_envio_correo
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2021/10/14
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [dbo].[log_envio_correo](
	--nuevos campos
	[id] [int] IDENTITY(1,1) NOT NULL,
	[email] [nvarchar](255) NOT NULL,
	[categoria] [nvarchar](50) NOT NULL,
	[fecha] [datetime] NOT NULL,
	[intentos_envio][int] NOT NULL,
	[mensaje][nvarchar](100) NULL,
	[enviado] [bit] NOT NULL
 CONSTRAINT [PK_log_envio_correo] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

  -- restricción default
ALTER TABLE [log_envio_correo] ADD  CONSTRAINT [DF_log_envio_correo_activo]  DEFAULT (0) FOR [enviado]

  -- restricción default
ALTER TABLE [log_envio_correo] ADD  CONSTRAINT [DF_log_envio_correo_intentos]  DEFAULT (0) FOR [intentos_envio]

  -- restricción default
ALTER TABLE [log_envio_correo] ADD  CONSTRAINT [DF_log_envio_correo_fecha]  DEFAULT (GETDATE()) FOR [fecha]

GO


IF object_id(N'log_envio_correo',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< log_envio_correo en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla log_envio_correo  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
