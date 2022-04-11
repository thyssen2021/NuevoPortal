USE Portal_2_0
GO
IF object_id(N'OT_personal_mantenimiento',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[OT_personal_mantenimiento]
      PRINT '<<< OT_personal_mantenimiento en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los OT_personal_mantenimiento
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/04/08
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [OT_personal_mantenimiento](
	[id][int] IDENTITY(1,1) NOT NULL,
	[id_empleado] [int] NOT NULL,
	--Agregar id Equipo de trabajo
	[activo][bit] NOT NULL
 CONSTRAINT [PK_OT_personal_mantenimiento] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


--agregar FK
  -- restriccion de clave foranea
  alter table [OT_personal_mantenimiento]
 add constraint FK_OT_personal_mantenimiento_id_empleado
  foreign key (id_empleado)
  references empleados(id);
GO

--inserta datos

INSERT INTO [dbo].[OT_personal_mantenimiento]([id_empleado],[activo]) VALUES(438,1)
INSERT INTO [dbo].[OT_personal_mantenimiento]([id_empleado],[activo]) VALUES(334,1)
INSERT INTO [dbo].[OT_personal_mantenimiento]([id_empleado],[activo]) VALUES(352,1)
INSERT INTO [dbo].[OT_personal_mantenimiento]([id_empleado],[activo]) VALUES(360,0)
INSERT INTO [dbo].[OT_personal_mantenimiento]([id_empleado],[activo]) VALUES(149,1)


GO



	  
IF object_id(N'OT_personal_mantenimiento',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< OT_personal_mantenimiento en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla OT_personal_mantenimiento  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
