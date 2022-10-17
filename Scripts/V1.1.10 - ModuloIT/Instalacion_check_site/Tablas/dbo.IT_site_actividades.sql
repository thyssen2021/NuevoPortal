USE Portal_2_0
GO
IF object_id(N'IT_site_actividades',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IT_site_actividades]
      PRINT '<<< IT_site_actividades en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los IT_site_actividades
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/10/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [IT_site_actividades](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[descripcion] [varchar](120) NOT NULL,	
	[referencia] [varchar](120) NOT NULL,	
	[activo][bit] NOT NULL DEFAULT 1,
 CONSTRAINT [PK_IT_site_actividades] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

   	  
--inserta documento para pruebas
INSERT INTO IT_site_actividades (descripcion,referencia ,activo) VALUES (N'Aire acondicionado',N'18 - 21 °C',1)
INSERT INTO IT_site_actividades (descripcion,referencia ,activo) VALUES (N'Led Servidores',N'Verde',1)
INSERT INTO IT_site_actividades (descripcion,referencia ,activo) VALUES (N'UPS display',N'Estatus en display',1)
INSERT INTO IT_site_actividades (descripcion,referencia ,activo) VALUES (N'Switches ',N'Led estatus verde',1)
INSERT INTO IT_site_actividades (descripcion,referencia ,activo) VALUES (N'Firewall',N'Led estatus verde',1)
INSERT INTO IT_site_actividades (descripcion,referencia ,activo) VALUES (N'Conmutador',N'Led estatus verde',1)
INSERT INTO IT_site_actividades (descripcion,referencia ,activo) VALUES (N'Equipo Internet Alestra',N'Led estatus verde',1)
INSERT INTO IT_site_actividades (descripcion,referencia ,activo) VALUES (N'Equipo Internet Metrocarrier',N'Led estatus verde',1)
INSERT INTO IT_site_actividades (descripcion,referencia ,activo) VALUES (N'DVR monitoreo',N'Revisar Monitor',1)
INSERT INTO IT_site_actividades (descripcion,referencia ,activo) VALUES (N'ION SDWAN',N'Led estatus verde',1)
INSERT INTO IT_site_actividades (descripcion,referencia ,activo) VALUES (N'Cableado estructurado',N'Orden y etiquetado',1)


IF object_id(N'IT_site_actividades',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IT_site_actividades en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IT_site_actividades  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
