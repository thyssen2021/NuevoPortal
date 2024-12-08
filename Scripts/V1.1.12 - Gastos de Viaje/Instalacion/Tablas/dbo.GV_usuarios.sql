USE Portal_2_0
GO
IF object_id(N'GV_usuarios',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[GV_usuarios]
      PRINT '<<< GV_usuarios en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los datos GV_usuarios
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/09/19
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [GV_usuarios](
	[id] [int] IDENTITY(1,1) NOT NULL,		
	[id_empleado][int] NOT NULL, --de la tabla empleados	
	[departamento][varchar](30) NOT NULL,
	-- No es necesario un campo de activo, simplemente se eliminan
 CONSTRAINT [PK_GV_usuarios] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

  -- restriccion de clave foranea
  alter table [GV_usuarios]
 add constraint FK_GV_usuarios_id_empleado
  foreign key (id_empleado)
  references empleados(id);

-- restricion check
ALTER TABLE [GV_usuarios] ADD CONSTRAINT CK_GV_usuarios_departamento CHECK ([departamento] IN 
('CONTROLLING', 'NOMINA', 'CUENTASPORPAGAR')
)

  -- restriccion unique
 alter table [GV_usuarios]
  add constraint UQ_GV_usuarios
  unique (id_empleado,departamento);
   	  

GO
 
-- datos de prueba

insert into [GV_usuarios] (id_empleado, departamento) VALUES ((SELECT TOP(1) id from empleados where numeroEmpleado='20344'),'CONTROLLING'); --Ismael Romero
insert into [GV_usuarios] (id_empleado, departamento) VALUES ((SELECT TOP(1) id from empleados where numeroEmpleado='20261'),'NOMINA'); --Victor Morales 
insert into [GV_usuarios] (id_empleado, departamento) VALUES ((SELECT TOP(1) id from empleados where numeroEmpleado='20371'),'NOMINA'); --Fernando Hernandez
insert into [GV_usuarios] (id_empleado, departamento) VALUES ((SELECT TOP(1) id from empleados where numeroEmpleado='20365'),'CUENTASPORPAGAR'); --Alejandra
insert into [GV_usuarios] (id_empleado, departamento) VALUES ((SELECT TOP(1) id from empleados where numeroEmpleado='20378'),'CUENTASPORPAGAR'); --Ariana


IF object_id(N'GV_usuarios',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< GV_usuarios en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla GV_usuarios  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
