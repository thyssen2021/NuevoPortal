USE Portal_2_0
GO
IF object_id(N'PFA_Autorizador',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[PFA_Autorizador]
      PRINT '<<< PFA_Autorizador en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los PFA_Autorizador
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2021/11/01
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [PFA_Autorizador](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_empleado][int] NULL,
	[activo] [bit] NOT NULL	
 CONSTRAINT [PK_PFA_Autorizador] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

--agregar FK
-- restriccion de clave foranea
alter table [PFA_Autorizador]
 add constraint FK_PFA_Autorizador_empleado
  foreign key (id_empleado)
  references empleados(id);

-- restricción default
ALTER TABLE [PFA_Autorizador] ADD  CONSTRAINT [DF_PFA_Autorizador_activo]  DEFAULT (1) FOR [activo]
GO


SET IDENTITY_INSERT [PFA_Autorizador] ON 
--Sólo para desarrollo
--INSERT [PFA_Autorizador] ([id],[id_empleado],  [activo]) VALUES (1,438, 1)

SET IDENTITY_INSERT [PFA_Autorizador] OFF
GO
 	  
IF object_id(N'PFA_Autorizador',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< PFA_Autorizador en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla PFA_Autorizador  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
