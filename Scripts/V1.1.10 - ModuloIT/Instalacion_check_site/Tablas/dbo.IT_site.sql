USE Portal_2_0
GO
IF object_id(N'IT_site',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IT_site]
      PRINT '<<< IT_site en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los IT_site
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/10/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [IT_site](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_planta] [int] NOT NULL, 
	[descripcion] [varchar](250) NOT NULL,	
	[activo][bit] NOT NULL DEFAULT 1,
 CONSTRAINT [PK_IT_site] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


-- restriccion de clave foranea
alter table [IT_site]
 add constraint FK_id_planta
  foreign key (id_planta)
  references plantas(clave);

   	  
--inserta documento para pruebas
INSERT INTO IT_site (id_planta,descripcion,activo) VALUES (1,N'Site - Puebla',1)
INSERT INTO IT_site (id_planta,descripcion,activo) VALUES (2,N'Site - Silao',1)
INSERT INTO IT_site (id_planta,descripcion,activo) VALUES (3,N'Site - Saltillo',1)


IF object_id(N'IT_site',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IT_site en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IT_site  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
