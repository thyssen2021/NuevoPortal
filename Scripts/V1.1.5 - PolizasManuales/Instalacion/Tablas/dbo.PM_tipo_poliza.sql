USE Portal_2_0
GO
IF object_id(N'PM_tipo_poliza',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[PM_tipo_poliza]
      PRINT '<<< PM_tipo_poliza en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los PM_tipo_poliza
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2021/12/09
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [PM_tipo_poliza](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[descripcion] [varchar](30) NOT NULL,
	[activo] [bit] NOT NULL	
 CONSTRAINT [PK_PM_tipo_poliza] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restricción default
ALTER TABLE [PM_tipo_poliza] ADD  CONSTRAINT [DF_PM_tipo_poliza_activo]  DEFAULT (1) FOR [activo]
GO


SET IDENTITY_INSERT [PM_tipo_poliza] ON 

INSERT [PM_tipo_poliza] ([id], [descripcion], [activo]) VALUES (1, N'FIJA', 1)

SET IDENTITY_INSERT [PM_tipo_poliza] OFF
GO

 	  
IF object_id(N'PM_tipo_poliza',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< PM_tipo_poliza en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla PM_tipo_poliza  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
