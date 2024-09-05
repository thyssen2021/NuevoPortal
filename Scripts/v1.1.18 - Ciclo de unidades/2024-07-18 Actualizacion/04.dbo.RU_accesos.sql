--use[Portal_2_0]
GO
IF object_id(N'RU_accesos',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[RU_accesos]
      PRINT '<<< RU_accesos en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los RU_accesos más relaciones
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2024/07/19
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[RU_accesos](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[descripcion] [nvarchar](50) NOT NULL,
	[id_planta] [int] NOT NULL,	
	[activo] [bit] NULL
 CONSTRAINT [PK_RU_accesos] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restriccion de clave foranea
alter table [RU_accesos]
 add constraint FK_RU_accesos_id_planta
  foreign key (id_planta)
  references plantas(clave);

  -- restricción default
ALTER TABLE [RU_accesos] ADD  CONSTRAINT [DF_RU_accesos_activo]  DEFAULT (1) FOR [activo]
GO

--inserta los registros de los turnos
--puebla
--INSERT INTO [dbo].[RU_accesos]([linea],[clave_planta],[activo])VALUES('Blanking 1',1,1);
--INSERT INTO [dbo].[RU_accesos]([linea],[clave_planta],[activo])VALUES('Blanking 2',1,1);
--INSERT INTO [dbo].[RU_accesos]([linea],[clave_planta],[activo])VALUES('Blanking 3',1,1);
--INSERT INTO [dbo].[RU_accesos]([linea],[clave_planta],[activo])VALUES('WG-161',1,1);
--INSERT INTO [dbo].[RU_accesos]([linea],[clave_planta],[activo])VALUES('WG-163',1,1);
--INSERT INTO [dbo].[RU_accesos]([linea],[clave_planta],[activo])VALUES('Slitter',1,1);
--silao
--INSERT INTO [dbo].[RU_accesos]([linea],[clave_planta],[activo])VALUES('Blanking 1',2,1);
--INSERT INTO [dbo].[RU_accesos]([linea],[clave_planta],[activo])VALUES('Blanking 2',2,1);
--INSERT INTO [dbo].[RU_accesos]([linea],[clave_planta],[activo])VALUES('Blanking 3',2,1);
--INSERT INTO [dbo].[RU_accesos]([linea],[clave_planta],[activo])VALUES('WG-161',2,1);
--INSERT INTO [dbo].[RU_accesos]([linea],[clave_planta],[activo])VALUES('WG-163',2,1);
--INSERT INTO [dbo].[RU_accesos]([linea],[clave_planta],[activo])VALUES('Slitter',2,1);


IF object_id(N'RU_accesos',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< RU_accesos en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla RU_accesos  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO

--Insert de Prueba
INSERT INTO [dbo].[RU_accesos]([descripcion],[id_planta],[activo])VALUES('Accesso Principal',1,1);


--- Modifica RU_registros para incluir la entrada ; No debe haber registros de Silao
--Alter RU_registros
ALTER TABLE RU_registros
ADD id_acceso int NOT NULL DEFAULT 1; --Debe estar registrada el acceso de puebla ya que será el default

-- restriccion de clave foranea
alter table [dbo].RU_registros
 add constraint FK_RU_registros_id_acceso
  foreign key (id_acceso)
  references RU_accesos(id);
  GO
-- Alter RU_registros
ALTER TABLE RU_registros
ADD id_salida int NULL DEFAULT 1; -- DEbe ser null y despues asignar el id 1 a todos los registros

-- restriccion de clave foranea
alter table [dbo].RU_registros
 add constraint FK_RU_registros_id_salida
  foreign key (id_salida)
  references RU_accesos(id);
  GO
  update RU_registros set id_salida = 1;

-- ALTER TABLE RU_registros
--DROP COLUMN id_salida;

--ALTER TABLE RU_registros
--DROP CONSTRAINT FK_RU_registros_id_salida;

--select * from RU_accesos
--select * from RU_registros