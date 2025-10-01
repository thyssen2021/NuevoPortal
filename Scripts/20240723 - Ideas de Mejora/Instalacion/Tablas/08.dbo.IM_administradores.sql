GO
IF object_id(N'IM_administradores',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IM_administradores]
      PRINT '<<< IM_administradores en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los IM_cat_impacto
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2021/12/09
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE IM_administradores(
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_planta][int] NOT NULL,	--FK
	[id_empleado][int] NOT NULL, ---FK
	[recibe_correo] [bit] NOT NULL DEFAULT 1,
 CONSTRAINT [PK_IM_administradores] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

  -- restriccion de clave foranea
alter table [dbo].IM_administradores
 add constraint FK_IM_administradores_id_planta
  foreign key (id_planta)
  references plantas(clave);

 -- restriccion de clave foranea
alter table [dbo].IM_administradores
 add constraint FK_IM_administradores_id_empleado
  foreign key (id_empleado)
  references empleados(id);


SET IDENTITY_INSERT IM_administradores ON 
GO

--AXC
INSERT IM_administradores ([id], [id_planta], id_empleado, recibe_correo ) VALUES (1, 1, 438 , 1)
INSERT IM_administradores ([id], [id_planta], id_empleado, recibe_correo ) VALUES (2, 2, 438 , 1)
INSERT IM_administradores ([id], [id_planta], id_empleado, recibe_correo ) VALUES (3, 3, 438 , 1)
INSERT IM_administradores ([id], [id_planta], id_empleado, recibe_correo ) VALUES (4, 4, 438 , 1)
INSERT IM_administradores ([id], [id_planta], id_empleado, recibe_correo ) VALUES (5, 5, 438 , 1)
-- Salvador
INSERT IM_administradores ([id], [id_planta], id_empleado, recibe_correo ) VALUES (6, 1, 171 , 1)
INSERT IM_administradores ([id], [id_planta], id_empleado, recibe_correo ) VALUES (7, 2, 171 , 1)
INSERT IM_administradores ([id], [id_planta], id_empleado, recibe_correo ) VALUES (8, 3, 171 , 1)
INSERT IM_administradores ([id], [id_planta], id_empleado, recibe_correo ) VALUES (9, 4, 171 , 1)
INSERT IM_administradores ([id], [id_planta], id_empleado, recibe_correo ) VALUES (10, 5, 171 , 1)

SET IDENTITY_INSERT IM_administradores OFF
GO

IF object_id(N'IM_administradores',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IM_administradores en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IM_administradores  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
