USE Portal_2_0
GO
IF object_id(N'IT_notificaciones_email',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IT_notificaciones_email]
      PRINT '<<< IT_notificaciones_email en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los IT_notificaciones_email
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/04/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [IT_notificaciones_email](
	[id][int] IDENTITY(1,1) NOT NULL,
	[id_notificaciones_recordatorio] [int] NOT NULL,		--FK
	[fecha_recordatorio] [datetime] NOT NULL, --con hora aproximada
	[enviado] [bit] NOT NULL,	
 CONSTRAINT [PK_IT_notificaciones_email] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

--agregar FK

  -- restriccion de clave foranea
alter table [IT_notificaciones_email]
 add constraint FK_IT_notificaciones_email_id_notificaciones_recodatorio
  foreign key (id_notificaciones_recordatorio)
  references IT_notificaciones_recordatorio(id);
 
	  
IF object_id(N'IT_notificaciones_email',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IT_notificaciones_email en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IT_notificaciones_email  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
