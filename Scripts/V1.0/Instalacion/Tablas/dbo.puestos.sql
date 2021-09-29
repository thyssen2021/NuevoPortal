GO
IF object_id(N'puesto',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[puesto]
      PRINT '<<< puesto en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los puestos más relaciones
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2021/09/27
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


GO
CREATE TABLE [puesto](
	[clave] [int] IDENTITY(1,1) NOT NULL,
	[activo] [bit] NOT NULL,
	[descripcion] [varchar](80) NOT NULL,	
	[areaClave] [int] NULL,
 CONSTRAINT [PK_puesto] PRIMARY KEY CLUSTERED 
(
	[clave] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

--agregar FK
-- restriccion de clave foranea
alter table [puesto]
 add constraint FK_puestos_area_clave
  foreign key (areaClave)
  references area(clave);

  -- restricción default
ALTER TABLE [puesto] ADD  CONSTRAINT [DF_puesto_activo]  DEFAULT (1) FOR [activo]
GO

--Inserts
GO

 	  
IF object_id(N'puesto',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< puesto en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla puesto  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
