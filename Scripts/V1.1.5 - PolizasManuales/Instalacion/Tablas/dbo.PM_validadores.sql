USE Portal_2_0
GO
IF object_id(N'PM_validadores',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[PM_validadores]
      PRINT '<<< PM_validadores en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los PM_validadores
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2021/11/01
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [PM_validadores](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_empleado][int] NULL,
	[activo] [bit] NOT NULL	
 CONSTRAINT [PK_PM_validadores] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

--agregar FK
-- restriccion de clave foranea
alter table [PM_validadores]
 add constraint FK_PM_validadores_empleado
  foreign key (id_empleado)
  references empleados(id);

-- restricción default
ALTER TABLE [PM_validadores] ADD  CONSTRAINT [DF_PM_validadores_activo]  DEFAULT (1) FOR [activo]
GO


SET IDENTITY_INSERT [PM_validadores] ON 
--Desarrollo
INSERT [PM_validadores] ([id],[id_empleado],  [activo]) VALUES (1,438, 1)	-- Alfredo Xochitemol

SET IDENTITY_INSERT [PM_validadores] OFF
GO
 	  
IF object_id(N'PM_validadores',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< PM_validadores en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla PM_validadores  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
