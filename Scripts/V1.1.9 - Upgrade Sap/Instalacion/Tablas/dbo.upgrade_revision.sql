USE Portal_2_0
GO
IF object_id(N'upgrade_revision',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[upgrade_revision]
      PRINT '<<< upgrade_revision en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los upgrade_revision
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/03/25
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [upgrade_revision](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_upgrade_usuario] [int] NOT NULL,
	[id_upgrade_departamento] [int] NOT NULL,
	[activo] [bit] NOT NULL	
 CONSTRAINT [PK_upgrade_revision] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restriccion de clave foranea
  alter table [upgrade_revision]
 add constraint FK_upgrade_revision_usuario
  foreign key (id_upgrade_usuario)
  references upgrade_usuarios(id);

  -- restriccion de clave foranea
  alter table [upgrade_revision]
 add constraint FK_upgrade_revision_departamento
  foreign key (id_upgrade_departamento)
  references upgrade_departamentos(id);


-- restricción default
ALTER TABLE [upgrade_revision] ADD  CONSTRAINT [DF_upgrade_revision_activo]  DEFAULT (1) FOR [activo]
GO

GO

INSERT INTO [dbo].[upgrade_revision]([id_upgrade_usuario],[id_upgrade_departamento],[activo])VALUES(1 ,1,1)
INSERT INTO [dbo].[upgrade_revision]([id_upgrade_usuario],[id_upgrade_departamento],[activo])VALUES(2 ,2,1)
INSERT INTO [dbo].[upgrade_revision]([id_upgrade_usuario],[id_upgrade_departamento],[activo])VALUES(3 ,3,1)
INSERT INTO [dbo].[upgrade_revision]([id_upgrade_usuario],[id_upgrade_departamento],[activo])VALUES(4 ,3,1)
INSERT INTO [dbo].[upgrade_revision]([id_upgrade_usuario],[id_upgrade_departamento],[activo])VALUES(5 ,4,1)
INSERT INTO [dbo].[upgrade_revision]([id_upgrade_usuario],[id_upgrade_departamento],[activo])VALUES(6 ,4,1)
INSERT INTO [dbo].[upgrade_revision]([id_upgrade_usuario],[id_upgrade_departamento],[activo])VALUES(7 ,5,1)
INSERT INTO [dbo].[upgrade_revision]([id_upgrade_usuario],[id_upgrade_departamento],[activo])VALUES(8 ,5,1)
INSERT INTO [dbo].[upgrade_revision]([id_upgrade_usuario],[id_upgrade_departamento],[activo])VALUES(9 ,6,1)
INSERT INTO [dbo].[upgrade_revision]([id_upgrade_usuario],[id_upgrade_departamento],[activo])VALUES(10 ,7,1)
INSERT INTO [dbo].[upgrade_revision]([id_upgrade_usuario],[id_upgrade_departamento],[activo])VALUES(11 ,7,1)
INSERT INTO [dbo].[upgrade_revision]([id_upgrade_usuario],[id_upgrade_departamento],[activo])VALUES(12 ,8,1)
INSERT INTO [dbo].[upgrade_revision]([id_upgrade_usuario],[id_upgrade_departamento],[activo])VALUES(13 ,8,1)
INSERT INTO [dbo].[upgrade_revision]([id_upgrade_usuario],[id_upgrade_departamento],[activo])VALUES(14 ,8,1)
INSERT INTO [dbo].[upgrade_revision]([id_upgrade_usuario],[id_upgrade_departamento],[activo])VALUES(15 ,9,1)
INSERT INTO [dbo].[upgrade_revision]([id_upgrade_usuario],[id_upgrade_departamento],[activo])VALUES(16 ,9,1)
INSERT INTO [dbo].[upgrade_revision]([id_upgrade_usuario],[id_upgrade_departamento],[activo])VALUES(17 ,10,1)
INSERT INTO [dbo].[upgrade_revision]([id_upgrade_usuario],[id_upgrade_departamento],[activo])VALUES(18 ,11,1)
INSERT INTO [dbo].[upgrade_revision]([id_upgrade_usuario],[id_upgrade_departamento],[activo])VALUES(8 ,12,1)
INSERT INTO [dbo].[upgrade_revision]([id_upgrade_usuario],[id_upgrade_departamento],[activo])VALUES(19 ,13,1)
INSERT INTO [dbo].[upgrade_revision]([id_upgrade_usuario],[id_upgrade_departamento],[activo])VALUES(20 ,14,1)
INSERT INTO [dbo].[upgrade_revision]([id_upgrade_usuario],[id_upgrade_departamento],[activo])VALUES(21 ,15,1)
INSERT INTO [dbo].[upgrade_revision]([id_upgrade_usuario],[id_upgrade_departamento],[activo])VALUES(22 ,15,1)
INSERT INTO [dbo].[upgrade_revision]([id_upgrade_usuario],[id_upgrade_departamento],[activo])VALUES(23 ,16,1)
INSERT INTO [dbo].[upgrade_revision]([id_upgrade_usuario],[id_upgrade_departamento],[activo])VALUES(24 ,16,1)
INSERT INTO [dbo].[upgrade_revision]([id_upgrade_usuario],[id_upgrade_departamento],[activo])VALUES(25 ,17,1)
INSERT INTO [dbo].[upgrade_revision]([id_upgrade_usuario],[id_upgrade_departamento],[activo])VALUES(26 ,18,1)
INSERT INTO [dbo].[upgrade_revision]([id_upgrade_usuario],[id_upgrade_departamento],[activo])VALUES(27 ,19,1)
INSERT INTO [dbo].[upgrade_revision]([id_upgrade_usuario],[id_upgrade_departamento],[activo])VALUES(28 ,19,1)
INSERT INTO [dbo].[upgrade_revision]([id_upgrade_usuario],[id_upgrade_departamento],[activo])VALUES(29 ,20,1)
INSERT INTO [dbo].[upgrade_revision]([id_upgrade_usuario],[id_upgrade_departamento],[activo])VALUES(30 ,21,1)
INSERT INTO [dbo].[upgrade_revision]([id_upgrade_usuario],[id_upgrade_departamento],[activo])VALUES(31 ,22,1)
INSERT INTO [dbo].[upgrade_revision]([id_upgrade_usuario],[id_upgrade_departamento],[activo])VALUES(32 ,22,1)

delete from [dbo].[upgrade_revision] where id_upgrade_departamento=12

GO


 	  
IF object_id(N'upgrade_revision',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< upgrade_revision en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla upgrade_revision  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
