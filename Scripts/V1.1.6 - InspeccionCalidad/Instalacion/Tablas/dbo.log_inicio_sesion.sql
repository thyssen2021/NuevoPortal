use[Portal_2_0]
GO
IF object_id(N'log_inicio_sesion',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[log_inicio_sesion]
      PRINT '<<< log_inicio_sesion en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar el log de inicio de sesión
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/01/18
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [dbo].[log_inicio_sesion](
	[id][int] IDENTITY(1,1) NOT NULL,
	[id_usuario][nvarchar](128) NOT NULL,		
	[fecha] [datetime] NOT NULL DEFAULT GETDATE()
 CONSTRAINT [PK_log_inicio_sesion] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restriccion de clave foranea
alter table [log_inicio_sesion]
 add constraint FK_log_inicio_sesion_id_usuario
  foreign key (id_usuario)
  references AspNetUsers(id);
GO

IF object_id(N'log_inicio_sesion',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< log_inicio_sesion en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla log_inicio_sesion  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
