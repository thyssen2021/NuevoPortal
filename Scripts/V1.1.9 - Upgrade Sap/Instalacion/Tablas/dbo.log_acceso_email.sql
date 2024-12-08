USE Portal_2_0
GO
IF object_id(N'log_acceso_email',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[log_acceso_email]
      PRINT '<<< log_acceso_email en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los log_acceso_email
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/05/09
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [log_acceso_email](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[fecha] [datetime] NOT NULL,
	[nombre_equipo] [varchar](50) NULL,
	[usuario] [varchar](100)  NULL
 CONSTRAINT [PK_log_acceso_email] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


 	  
IF object_id(N'log_acceso_email',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< log_acceso_email en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla log_acceso_email  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
