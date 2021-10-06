use[Portal_2_0]
GO
IF object_id(N'produccion_lineas',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[produccion_lineas]
      PRINT '<<< produccion_lineas en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los produccion_lineas más relaciones
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2021/10/06
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [dbo].[produccion_lineas](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[linea] [nvarchar](25) NOT NULL,
	[clave_planta] [int] NOT NULL,	
	[activo] [bit] NULL
 CONSTRAINT [PK_produccion_lineas] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restriccion de clave foranea
alter table [produccion_lineas]
 add constraint FK_produccion_lineas_planta_clave
  foreign key (clave_planta)
  references plantas(clave);

  -- restricción default
ALTER TABLE [produccion_lineas] ADD  CONSTRAINT [DF_produccion_lineas_activo]  DEFAULT (1) FOR [activo]
GO

--inserta los registros de los turnos
--puebla
INSERT INTO [dbo].[produccion_lineas]([linea],[clave_planta],[activo])VALUES('Blanking 1',1,1);
INSERT INTO [dbo].[produccion_lineas]([linea],[clave_planta],[activo])VALUES('Blanking 2',1,1);
INSERT INTO [dbo].[produccion_lineas]([linea],[clave_planta],[activo])VALUES('Blanking 3',1,1);
INSERT INTO [dbo].[produccion_lineas]([linea],[clave_planta],[activo])VALUES('WG-161',1,1);
INSERT INTO [dbo].[produccion_lineas]([linea],[clave_planta],[activo])VALUES('WG-163',1,1);
INSERT INTO [dbo].[produccion_lineas]([linea],[clave_planta],[activo])VALUES('Slitter',1,1);
--silao
INSERT INTO [dbo].[produccion_lineas]([linea],[clave_planta],[activo])VALUES('Blanking 1',2,1);
INSERT INTO [dbo].[produccion_lineas]([linea],[clave_planta],[activo])VALUES('Blanking 2',2,1);
INSERT INTO [dbo].[produccion_lineas]([linea],[clave_planta],[activo])VALUES('Blanking 3',2,1);
INSERT INTO [dbo].[produccion_lineas]([linea],[clave_planta],[activo])VALUES('WG-161',2,1);
INSERT INTO [dbo].[produccion_lineas]([linea],[clave_planta],[activo])VALUES('WG-163',2,1);
INSERT INTO [dbo].[produccion_lineas]([linea],[clave_planta],[activo])VALUES('Slitter',2,1);


IF object_id(N'produccion_lineas',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< produccion_lineas en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla produccion_lineas  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
