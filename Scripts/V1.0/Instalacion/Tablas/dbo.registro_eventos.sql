GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF object_id(N'registro_eventos',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[registro_eventos]
      PRINT '<<< registro_eventos en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar las excepciones no controladas
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 01/04/2021
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[registro_eventos](
	[IdEvento] [int] IDENTITY(1,1) NOT NULL,
	[IdUsuario] [varchar](15) NOT NULL,
	[FechaEvento] [datetime] NOT NULL,
	[TipoEvento] [char](1) NOT NULL,
	[Origen] [varchar](200) NOT NULL,
	[Descripcion] [nvarchar](4000) NOT NULL,
	[Gravedad] [tinyint] NOT NULL,
	[NumeroError] [int] NOT NULL,
 CONSTRAINT [PK_EventLog] PRIMARY KEY CLUSTERED 
(
	[IdEvento] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].registro_eventos ADD  CONSTRAINT [DF_EventLog_EventType]  DEFAULT ('I') FOR [TipoEvento]
GO

ALTER TABLE [dbo].registro_eventos ADD  CONSTRAINT [DF_EventLog_Severity]  DEFAULT ((0)) FOR [Gravedad]
GO


 	  
IF object_id(N'registro_eventos',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< registro_eventos en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla registro_eventos  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
