use[Portal_2_0]
GO
IF object_id(N'produccion_operadores',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[produccion_operadores]
      PRINT '<<< produccion_operadores en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los operadores de producción
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2021/10/06
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [dbo].[produccion_operadores](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_empleado][int] NOT NULL,
	[id_linea] [int] NOT NULL,	
	[activo] [bit] NULL
 CONSTRAINT [PK_produccion_operadores] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restriccion de clave foranea
alter table [produccion_operadores]
 add constraint FK_produccion_operadores_id_empleado
  foreign key (id_empleado)
  references empleados(id);

  -- restriccion de clave foranea
alter table [produccion_operadores]
 add constraint FK_produccion_operadores_id_linea
  foreign key (id_linea)
  references produccion_lineas(id);

  -- restricción default
ALTER TABLE [produccion_operadores] ADD  CONSTRAINT [DF_produccion_operadores_activo]  DEFAULT (1) FOR [activo]
GO

--inserta los registros de los turnos
--puebla
INSERT INTO [dbo].[produccion_operadores]([id_empleado],[id_linea],[activo])VALUES((SELECT id FROM empleados where numeroEmpleado='10004'),(SELECT id FROM produccion_lineas where linea='Blanking 1' AND clave_planta=1),1);
INSERT INTO [dbo].[produccion_operadores]([id_empleado],[id_linea],[activo])VALUES((SELECT id FROM empleados where numeroEmpleado='10160'),(SELECT id FROM produccion_lineas where linea='Blanking 1' AND clave_planta=1),1);

INSERT INTO [dbo].[produccion_operadores]([id_empleado],[id_linea],[activo])VALUES((SELECT id FROM empleados where numeroEmpleado='10225'),(SELECT id FROM produccion_lineas where linea='Blanking 2' AND clave_planta=1),1);
INSERT INTO [dbo].[produccion_operadores]([id_empleado],[id_linea],[activo])VALUES((SELECT id FROM empleados where numeroEmpleado='10096'),(SELECT id FROM produccion_lineas where linea='Blanking 2' AND clave_planta=1),1);
INSERT INTO [dbo].[produccion_operadores]([id_empleado],[id_linea],[activo])VALUES((SELECT id FROM empleados where numeroEmpleado='10054'),(SELECT id FROM produccion_lineas where linea='Blanking 2' AND clave_planta=1),1);

INSERT INTO [dbo].[produccion_operadores]([id_empleado],[id_linea],[activo])VALUES((SELECT id FROM empleados where numeroEmpleado='10001'),(SELECT id FROM produccion_lineas where linea='Blanking 3' AND clave_planta=1),1);
INSERT INTO [dbo].[produccion_operadores]([id_empleado],[id_linea],[activo])VALUES((SELECT id FROM empleados where numeroEmpleado='10041'),(SELECT id FROM produccion_lineas where linea='Blanking 3' AND clave_planta=1),1);
INSERT INTO [dbo].[produccion_operadores]([id_empleado],[id_linea],[activo])VALUES((SELECT id FROM empleados where numeroEmpleado='10040'),(SELECT id FROM produccion_lineas where linea='Blanking 3' AND clave_planta=1),1);
INSERT INTO [dbo].[produccion_operadores]([id_empleado],[id_linea],[activo])VALUES((SELECT id FROM empleados where numeroEmpleado='10207'),(SELECT id FROM produccion_lineas where linea='Blanking 3' AND clave_planta=1),1);

INSERT INTO [dbo].[produccion_operadores]([id_empleado],[id_linea],[activo])VALUES((SELECT id FROM empleados where numeroEmpleado='10035'),(SELECT id FROM produccion_lineas where linea='WG-161' AND clave_planta=1),1);
INSERT INTO [dbo].[produccion_operadores]([id_empleado],[id_linea],[activo])VALUES((SELECT id FROM empleados where numeroEmpleado='10022'),(SELECT id FROM produccion_lineas where linea='WG-161' AND clave_planta=1),1);
INSERT INTO [dbo].[produccion_operadores]([id_empleado],[id_linea],[activo])VALUES((SELECT id FROM empleados where numeroEmpleado='10035'),(SELECT id FROM produccion_lineas where linea='WG-163' AND clave_planta=1),1);
INSERT INTO [dbo].[produccion_operadores]([id_empleado],[id_linea],[activo])VALUES((SELECT id FROM empleados where numeroEmpleado='10022'),(SELECT id FROM produccion_lineas where linea='WG-163' AND clave_planta=1),1);

INSERT INTO [dbo].[produccion_operadores]([id_empleado],[id_linea],[activo])VALUES((SELECT id FROM empleados where numeroEmpleado='10038'),(SELECT id FROM produccion_lineas where linea='Slitter' AND clave_planta=1),1);

--silao
INSERT INTO [dbo].[produccion_operadores]([id_empleado],[id_linea],[activo])VALUES((SELECT id FROM empleados where numeroEmpleado='70406'),(SELECT id FROM produccion_lineas where linea='Blanking 1' AND clave_planta=2),1);
INSERT INTO [dbo].[produccion_operadores]([id_empleado],[id_linea],[activo])VALUES((SELECT id FROM empleados where numeroEmpleado='70465'),(SELECT id FROM produccion_lineas where linea='Blanking 1' AND clave_planta=2),1);
INSERT INTO [dbo].[produccion_operadores]([id_empleado],[id_linea],[activo])VALUES((SELECT id FROM empleados where numeroEmpleado='70066'),(SELECT id FROM produccion_lineas where linea='Blanking 1' AND clave_planta=2),1);
INSERT INTO [dbo].[produccion_operadores]([id_empleado],[id_linea],[activo])VALUES((SELECT id FROM empleados where numeroEmpleado='70249'),(SELECT id FROM produccion_lineas where linea='Blanking 1' AND clave_planta=2),1);

INSERT INTO [dbo].[produccion_operadores]([id_empleado],[id_linea],[activo])VALUES((SELECT id FROM empleados where numeroEmpleado='70154'),(SELECT id FROM produccion_lineas where linea='Blanking 2' AND clave_planta=2),1);
INSERT INTO [dbo].[produccion_operadores]([id_empleado],[id_linea],[activo])VALUES((SELECT id FROM empleados where numeroEmpleado='70470'),(SELECT id FROM produccion_lineas where linea='Blanking 2' AND clave_planta=2),1);
INSERT INTO [dbo].[produccion_operadores]([id_empleado],[id_linea],[activo])VALUES((SELECT id FROM empleados where numeroEmpleado='70430'),(SELECT id FROM produccion_lineas where linea='Blanking 2' AND clave_planta=2),1);
INSERT INTO [dbo].[produccion_operadores]([id_empleado],[id_linea],[activo])VALUES((SELECT id FROM empleados where numeroEmpleado='70376'),(SELECT id FROM produccion_lineas where linea='Blanking 2' AND clave_planta=2),1);

INSERT INTO [dbo].[produccion_operadores]([id_empleado],[id_linea],[activo])VALUES((SELECT id FROM empleados where numeroEmpleado='70200'),(SELECT id FROM produccion_lineas where linea='Blanking 3' AND clave_planta=2),1);
INSERT INTO [dbo].[produccion_operadores]([id_empleado],[id_linea],[activo])VALUES((SELECT id FROM empleados where numeroEmpleado='70469'),(SELECT id FROM produccion_lineas where linea='Blanking 3' AND clave_planta=2),1);
INSERT INTO [dbo].[produccion_operadores]([id_empleado],[id_linea],[activo])VALUES((SELECT id FROM empleados where numeroEmpleado='70407'),(SELECT id FROM produccion_lineas where linea='Blanking 3' AND clave_planta=2),1);
INSERT INTO [dbo].[produccion_operadores]([id_empleado],[id_linea],[activo])VALUES((SELECT id FROM empleados where numeroEmpleado='70355'),(SELECT id FROM produccion_lineas where linea='Blanking 3' AND clave_planta=2),1);
INSERT INTO [dbo].[produccion_operadores]([id_empleado],[id_linea],[activo])VALUES((SELECT id FROM empleados where numeroEmpleado='70425'),(SELECT id FROM produccion_lineas where linea='Blanking 3' AND clave_planta=2),1);
INSERT INTO [dbo].[produccion_operadores]([id_empleado],[id_linea],[activo])VALUES((SELECT id FROM empleados where numeroEmpleado='70305'),(SELECT id FROM produccion_lineas where linea='Blanking 3' AND clave_planta=2),1);
INSERT INTO [dbo].[produccion_operadores]([id_empleado],[id_linea],[activo])VALUES((SELECT id FROM empleados where numeroEmpleado='70313'),(SELECT id FROM produccion_lineas where linea='Blanking 3' AND clave_planta=2),1);
INSERT INTO [dbo].[produccion_operadores]([id_empleado],[id_linea],[activo])VALUES((SELECT id FROM empleados where numeroEmpleado='70095'),(SELECT id FROM produccion_lineas where linea='Blanking 3' AND clave_planta=2),1);

IF object_id(N'produccion_operadores',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< produccion_operadores en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla produccion_operadores  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
