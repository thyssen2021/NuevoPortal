--USE Portal_2_0
GO
IF object_id(N'IT_wsus',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IT_wsus]
      PRINT '<<< IT_wsus en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar las IT_wsus
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/06/22
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[IT_wsus](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name][varchar](200) NULL, --hostname
	[ip] [varchar](100) NULL,
	[operating_system] [varchar](120) NULL,	
 CONSTRAINT [PK_IT_wsus] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


IF object_id(N'IT_wsus',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IT_wsus en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IT_wsus  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END