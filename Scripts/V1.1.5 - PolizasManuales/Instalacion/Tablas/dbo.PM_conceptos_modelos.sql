USE Portal_2_0
GO
IF object_id(N'PM_conceptos_modelo',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[PM_conceptos_modelo]
      PRINT '<<< PM_conceptos_modelo en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los PM_conceptos_modelo
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/01/20
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [PM_conceptos_modelo](
	[id][int] IDENTITY(1,1) NOT NULL,
	[id_poliza_modelo] [int] NOT NULL,
	[cuenta] [varchar](15) NOT NULL,
	[cc] [varchar](15) NULL,
	[concepto][varchar](120) NULL,
	[poliza][varchar](10) NULL
 CONSTRAINT [PK_PM_conceptos_modelo] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


--agregar FK
-- restriccion de clave foranea
alter table [PM_conceptos_modelo]
 add constraint FK_PM_conceptos_modelo_id_poliza
  foreign key (id_poliza_modelo)
  references PM_poliza_manual_modelo(id);

	  
IF object_id(N'PM_conceptos_modelo',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< PM_conceptos_modelo en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla PM_conceptos_modelo  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
