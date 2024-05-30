--USE Portal_2_0
GO
IF object_id(N'SCDM_cat_usuarios_revision_departamento',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[SCDM_cat_usuarios_revision_departamento]
      PRINT '<<< SCDM_cat_usuarios_revision_departamento en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los SCDM_cat_usuarios_revision_departamento
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[SCDM_cat_usuarios_revision_departamento](
	[id] [int] IDENTITY(1,1) NOT NULL,	
	[id_scdm_departamento][int] NOT NULL,
	[id_empleado][int] NOT NULL,
	[id_planta_solicitud][int] NOT NULL,
	[tipo][varchar](20) NOT NULL,
	[activo] [bit]  NOT NULL DEFAULT 1,	
 CONSTRAINT [PK_SCDM_cat_usuarios_revision_departamento_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

  -- restriccion de clave foranea
alter table [dbo].[SCDM_cat_usuarios_revision_departamento]
 add constraint FK_SCDM_cat_usuarios_revision_departamento_id_scdm_departamento
  foreign key (id_scdm_departamento)
  references SCDM_cat_departamentos_asignacion(id);

    -- restriccion de clave foranea
alter table [dbo].[SCDM_cat_usuarios_revision_departamento]
 add constraint FK_SCDM_cat_usuarios_revision_departamento_id_empleado
  foreign key (id_empleado)
  references empleados(id);

    -- restriccion de clave foranea
alter table [dbo].[SCDM_cat_usuarios_revision_departamento]
 add constraint FK_SCDM_cat_usuarios_revision_departamento_id_planta
  foreign key (id_planta_solicitud)
  references plantas(clave);



  --restricion de valores
  -- restricion check
ALTER TABLE [SCDM_cat_usuarios_revision_departamento] ADD CONSTRAINT CK_SCDM_cat_usuarios_revision_departamento_tipo CHECK ([tipo] IN 
('PRIMARIO', 'SECUNDARIO')
)
GO


--SET IDENTITY_INSERT [dbo].[SCDM_cat_usuarios_revision_departamento] ON 

--delete from [SCDM_cat_usuarios_revision_departamento]; DBCC CHECKIDENT ('SCDM_cat_usuarios_revision_departamento', RESEED, 0);

INSERT INTO [dbo].[SCDM_cat_usuarios_revision_departamento]([id_scdm_departamento],[id_empleado],[id_planta_solicitud],[tipo],[activo]) VALUES(5,88,1,'PRIMARIO',1)
INSERT INTO [dbo].[SCDM_cat_usuarios_revision_departamento]([id_scdm_departamento],[id_empleado],[id_planta_solicitud],[tipo],[activo]) VALUES(5,75,1,'SECUNDARIO',1)
INSERT INTO [dbo].[SCDM_cat_usuarios_revision_departamento]([id_scdm_departamento],[id_empleado],[id_planta_solicitud],[tipo],[activo]) VALUES(2,131,1,'PRIMARIO',1)
INSERT INTO [dbo].[SCDM_cat_usuarios_revision_departamento]([id_scdm_departamento],[id_empleado],[id_planta_solicitud],[tipo],[activo]) VALUES(6,153,1,'PRIMARIO',1)
INSERT INTO [dbo].[SCDM_cat_usuarios_revision_departamento]([id_scdm_departamento],[id_empleado],[id_planta_solicitud],[tipo],[activo]) VALUES(3,450,1,'PRIMARIO',1)
INSERT INTO [dbo].[SCDM_cat_usuarios_revision_departamento]([id_scdm_departamento],[id_empleado],[id_planta_solicitud],[tipo],[activo]) VALUES(3,444,1,'SECUNDARIO',1)
INSERT INTO [dbo].[SCDM_cat_usuarios_revision_departamento]([id_scdm_departamento],[id_empleado],[id_planta_solicitud],[tipo],[activo]) VALUES(3,178,1,'SECUNDARIO',1)
INSERT INTO [dbo].[SCDM_cat_usuarios_revision_departamento]([id_scdm_departamento],[id_empleado],[id_planta_solicitud],[tipo],[activo]) VALUES(7,151,1,'PRIMARIO',1)
INSERT INTO [dbo].[SCDM_cat_usuarios_revision_departamento]([id_scdm_departamento],[id_empleado],[id_planta_solicitud],[tipo],[activo]) VALUES(7,153,1,'PRIMARIO',1)
INSERT INTO [dbo].[SCDM_cat_usuarios_revision_departamento]([id_scdm_departamento],[id_empleado],[id_planta_solicitud],[tipo],[activo]) VALUES(7,101,1,'PRIMARIO',1)
INSERT INTO [dbo].[SCDM_cat_usuarios_revision_departamento]([id_scdm_departamento],[id_empleado],[id_planta_solicitud],[tipo],[activo]) VALUES(4,114,1,'PRIMARIO',1)
INSERT INTO [dbo].[SCDM_cat_usuarios_revision_departamento]([id_scdm_departamento],[id_empleado],[id_planta_solicitud],[tipo],[activo]) VALUES(4,86,1,'SECUNDARIO',1)
INSERT INTO [dbo].[SCDM_cat_usuarios_revision_departamento]([id_scdm_departamento],[id_empleado],[id_planta_solicitud],[tipo],[activo]) VALUES(8,475,1,'SECUNDARIO',1)
INSERT INTO [dbo].[SCDM_cat_usuarios_revision_departamento]([id_scdm_departamento],[id_empleado],[id_planta_solicitud],[tipo],[activo]) VALUES(5,426,3,'PRIMARIO',1)
INSERT INTO [dbo].[SCDM_cat_usuarios_revision_departamento]([id_scdm_departamento],[id_empleado],[id_planta_solicitud],[tipo],[activo]) VALUES(5,407,3,'SECUNDARIO',1)
INSERT INTO [dbo].[SCDM_cat_usuarios_revision_departamento]([id_scdm_departamento],[id_empleado],[id_planta_solicitud],[tipo],[activo]) VALUES(2,131,3,'PRIMARIO',1)
INSERT INTO [dbo].[SCDM_cat_usuarios_revision_departamento]([id_scdm_departamento],[id_empleado],[id_planta_solicitud],[tipo],[activo]) VALUES(6,133,3,'PRIMARIO',1)
INSERT INTO [dbo].[SCDM_cat_usuarios_revision_departamento]([id_scdm_departamento],[id_empleado],[id_planta_solicitud],[tipo],[activo]) VALUES(3,450,3,'PRIMARIO',1)
INSERT INTO [dbo].[SCDM_cat_usuarios_revision_departamento]([id_scdm_departamento],[id_empleado],[id_planta_solicitud],[tipo],[activo]) VALUES(3,444,3,'SECUNDARIO',1)
INSERT INTO [dbo].[SCDM_cat_usuarios_revision_departamento]([id_scdm_departamento],[id_empleado],[id_planta_solicitud],[tipo],[activo]) VALUES(3,178,3,'SECUNDARIO',1)
INSERT INTO [dbo].[SCDM_cat_usuarios_revision_departamento]([id_scdm_departamento],[id_empleado],[id_planta_solicitud],[tipo],[activo]) VALUES(7,151,3,'PRIMARIO',1)
INSERT INTO [dbo].[SCDM_cat_usuarios_revision_departamento]([id_scdm_departamento],[id_empleado],[id_planta_solicitud],[tipo],[activo]) VALUES(7,153,3,'PRIMARIO',1)
INSERT INTO [dbo].[SCDM_cat_usuarios_revision_departamento]([id_scdm_departamento],[id_empleado],[id_planta_solicitud],[tipo],[activo]) VALUES(7,101,3,'PRIMARIO',1)
INSERT INTO [dbo].[SCDM_cat_usuarios_revision_departamento]([id_scdm_departamento],[id_empleado],[id_planta_solicitud],[tipo],[activo]) VALUES(4,407,3,'PRIMARIO',1)
INSERT INTO [dbo].[SCDM_cat_usuarios_revision_departamento]([id_scdm_departamento],[id_empleado],[id_planta_solicitud],[tipo],[activo]) VALUES(4,426,3,'SECUNDARIO',1)
INSERT INTO [dbo].[SCDM_cat_usuarios_revision_departamento]([id_scdm_departamento],[id_empleado],[id_planta_solicitud],[tipo],[activo]) VALUES(8,105,1,'PRIMARIO',1)
INSERT INTO [dbo].[SCDM_cat_usuarios_revision_departamento]([id_scdm_departamento],[id_empleado],[id_planta_solicitud],[tipo],[activo]) VALUES(8,72,1,'PRIMARIO',1)
INSERT INTO [dbo].[SCDM_cat_usuarios_revision_departamento]([id_scdm_departamento],[id_empleado],[id_planta_solicitud],[tipo],[activo]) VALUES(8,475,3,'SECUNDARIO',1)
INSERT INTO [dbo].[SCDM_cat_usuarios_revision_departamento]([id_scdm_departamento],[id_empleado],[id_planta_solicitud],[tipo],[activo]) VALUES(5,345,2,'PRIMARIO',1)
INSERT INTO [dbo].[SCDM_cat_usuarios_revision_departamento]([id_scdm_departamento],[id_empleado],[id_planta_solicitud],[tipo],[activo]) VALUES(5,142,2,'SECUNDARIO',1)
INSERT INTO [dbo].[SCDM_cat_usuarios_revision_departamento]([id_scdm_departamento],[id_empleado],[id_planta_solicitud],[tipo],[activo]) VALUES(2,131,2,'PRIMARIO',1)
INSERT INTO [dbo].[SCDM_cat_usuarios_revision_departamento]([id_scdm_departamento],[id_empleado],[id_planta_solicitud],[tipo],[activo]) VALUES(6,133,2,'PRIMARIO',1)
INSERT INTO [dbo].[SCDM_cat_usuarios_revision_departamento]([id_scdm_departamento],[id_empleado],[id_planta_solicitud],[tipo],[activo]) VALUES(3,450,2,'PRIMARIO',1)
INSERT INTO [dbo].[SCDM_cat_usuarios_revision_departamento]([id_scdm_departamento],[id_empleado],[id_planta_solicitud],[tipo],[activo]) VALUES(3,444,2,'SECUNDARIO',1)
INSERT INTO [dbo].[SCDM_cat_usuarios_revision_departamento]([id_scdm_departamento],[id_empleado],[id_planta_solicitud],[tipo],[activo]) VALUES(3,178,2,'SECUNDARIO',1)
INSERT INTO [dbo].[SCDM_cat_usuarios_revision_departamento]([id_scdm_departamento],[id_empleado],[id_planta_solicitud],[tipo],[activo]) VALUES(7,151,2,'PRIMARIO',1)
INSERT INTO [dbo].[SCDM_cat_usuarios_revision_departamento]([id_scdm_departamento],[id_empleado],[id_planta_solicitud],[tipo],[activo]) VALUES(7,153,2,'PRIMARIO',1)
INSERT INTO [dbo].[SCDM_cat_usuarios_revision_departamento]([id_scdm_departamento],[id_empleado],[id_planta_solicitud],[tipo],[activo]) VALUES(7,101,2,'PRIMARIO',1)
INSERT INTO [dbo].[SCDM_cat_usuarios_revision_departamento]([id_scdm_departamento],[id_empleado],[id_planta_solicitud],[tipo],[activo]) VALUES(4,316,2,'PRIMARIO',1)
INSERT INTO [dbo].[SCDM_cat_usuarios_revision_departamento]([id_scdm_departamento],[id_empleado],[id_planta_solicitud],[tipo],[activo]) VALUES(4,302,2,'SECUNDARIO',1)
INSERT INTO [dbo].[SCDM_cat_usuarios_revision_departamento]([id_scdm_departamento],[id_empleado],[id_planta_solicitud],[tipo],[activo]) VALUES(8,105,3,'PRIMARIO',1)
INSERT INTO [dbo].[SCDM_cat_usuarios_revision_departamento]([id_scdm_departamento],[id_empleado],[id_planta_solicitud],[tipo],[activo]) VALUES(8,72,3,'PRIMARIO',1)
INSERT INTO [dbo].[SCDM_cat_usuarios_revision_departamento]([id_scdm_departamento],[id_empleado],[id_planta_solicitud],[tipo],[activo]) VALUES(8,475,2,'PRIMARIO',1)
INSERT INTO [dbo].[SCDM_cat_usuarios_revision_departamento]([id_scdm_departamento],[id_empleado],[id_planta_solicitud],[tipo],[activo]) VALUES(8,105,2,'PRIMARIO',1)
INSERT INTO [dbo].[SCDM_cat_usuarios_revision_departamento]([id_scdm_departamento],[id_empleado],[id_planta_solicitud],[tipo],[activo]) VALUES(8,72,2,'PRIMARIO',1)
INSERT INTO [dbo].[SCDM_cat_usuarios_revision_departamento]([id_scdm_departamento],[id_empleado],[id_planta_solicitud],[tipo],[activo]) VALUES(2,131,4,'PRIMARIO',1)
INSERT INTO [dbo].[SCDM_cat_usuarios_revision_departamento]([id_scdm_departamento],[id_empleado],[id_planta_solicitud],[tipo],[activo]) VALUES(1,135,1,'PRIMARIO',1)
INSERT INTO [dbo].[SCDM_cat_usuarios_revision_departamento]([id_scdm_departamento],[id_empleado],[id_planta_solicitud],[tipo],[activo]) VALUES(1,135,2,'PRIMARIO',1)
INSERT INTO [dbo].[SCDM_cat_usuarios_revision_departamento]([id_scdm_departamento],[id_empleado],[id_planta_solicitud],[tipo],[activo]) VALUES(1,135,3,'PRIMARIO',1)
INSERT INTO [dbo].[SCDM_cat_usuarios_revision_departamento]([id_scdm_departamento],[id_empleado],[id_planta_solicitud],[tipo],[activo]) VALUES(1,135,4,'PRIMARIO',1)

--SET IDENTITY_INSERT [dbo].[SCDM_cat_usuarios_revision_departamento] OFF

	  
IF object_id(N'SCDM_cat_usuarios_revision_departamento',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< SCDM_cat_usuarios_revision_departamento en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla SCDM_cat_usuarios_revision_departamento  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
