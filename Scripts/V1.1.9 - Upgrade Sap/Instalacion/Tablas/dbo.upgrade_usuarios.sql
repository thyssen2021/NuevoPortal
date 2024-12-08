USE Portal_2_0
GO
IF object_id(N'upgrade_usuarios',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[upgrade_usuarios]
      PRINT '<<< upgrade_usuarios en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los upgrade_usuarios
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/03/25
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [upgrade_usuarios](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_empleado] [int] NOT NULL,
	[key_user][varchar](30) NOT NULL,
	[activo] [bit] NOT NULL	
 CONSTRAINT [PK_upgrade_usuarios] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restriccion de clave foranea
  alter table [upgrade_usuarios]
 add constraint FK_upgrade_usuarios_empleado
  foreign key (id_empleado)
  references empleados(id);


-- restricción default
ALTER TABLE [upgrade_usuarios] ADD  CONSTRAINT [DF_upgrade_usuarios_activo]  DEFAULT (1) FOR [activo]
GO

INSERT INTO [dbo].[upgrade_usuarios]([id_empleado],[key_user],[activo]) VALUES(121, N'MONROYG',1)
INSERT INTO [dbo].[upgrade_usuarios]([id_empleado],[key_user],[activo]) VALUES(74, N'HERNANDEZJ',1)
INSERT INTO [dbo].[upgrade_usuarios]([id_empleado],[key_user],[activo]) VALUES(103, N'DIAZER',1)
INSERT INTO [dbo].[upgrade_usuarios]([id_empleado],[key_user],[activo]) VALUES(67, N'VEGA',1)
INSERT INTO [dbo].[upgrade_usuarios]([id_empleado],[key_user],[activo]) VALUES(75, N'BALEON',1)
INSERT INTO [dbo].[upgrade_usuarios]([id_empleado],[key_user],[activo]) VALUES(88, N'ROMEROIV',1)
INSERT INTO [dbo].[upgrade_usuarios]([id_empleado],[key_user],[activo]) VALUES(133, N'CAMPOSJC',1)
INSERT INTO [dbo].[upgrade_usuarios]([id_empleado],[key_user],[activo]) VALUES(143, N'VALLEJOJM',1)
INSERT INTO [dbo].[upgrade_usuarios]([id_empleado],[key_user],[activo]) VALUES(165, N'TELLOGE',1)
INSERT INTO [dbo].[upgrade_usuarios]([id_empleado],[key_user],[activo]) VALUES(131, N'RODRIGUEZAL',1)
INSERT INTO [dbo].[upgrade_usuarios]([id_empleado],[key_user],[activo]) VALUES(153, N'AGUILARK',1)
INSERT INTO [dbo].[upgrade_usuarios]([id_empleado],[key_user],[activo]) VALUES(178, N'RAMIREZC',1)
INSERT INTO [dbo].[upgrade_usuarios]([id_empleado],[key_user],[activo]) VALUES(135, N'PEREZIV',1)
INSERT INTO [dbo].[upgrade_usuarios]([id_empleado],[key_user],[activo]) VALUES(132, N'HERNANDEZVM',1)
INSERT INTO [dbo].[upgrade_usuarios]([id_empleado],[key_user],[activo]) VALUES(114, N'ZOMPANTZINJ',1)
INSERT INTO [dbo].[upgrade_usuarios]([id_empleado],[key_user],[activo]) VALUES(86, N'GARCIARO',1)
INSERT INTO [dbo].[upgrade_usuarios]([id_empleado],[key_user],[activo]) VALUES(164, N'BRAVOA',1)
INSERT INTO [dbo].[upgrade_usuarios]([id_empleado],[key_user],[activo]) VALUES(83, N'PEREZI',1)
INSERT INTO [dbo].[upgrade_usuarios]([id_empleado],[key_user],[activo]) VALUES(118, N'MOROA',1)
INSERT INTO [dbo].[upgrade_usuarios]([id_empleado],[key_user],[activo]) VALUES(98, N'DIAZAR',1)
INSERT INTO [dbo].[upgrade_usuarios]([id_empleado],[key_user],[activo]) VALUES(407, N'COLEOTEIA',1)
INSERT INTO [dbo].[upgrade_usuarios]([id_empleado],[key_user],[activo]) VALUES(406, N'MORENOE',1)
INSERT INTO [dbo].[upgrade_usuarios]([id_empleado],[key_user],[activo]) VALUES(425, N'SALINASJ',1)
INSERT INTO [dbo].[upgrade_usuarios]([id_empleado],[key_user],[activo]) VALUES(426, N'FERNANDEZLA',1)
INSERT INTO [dbo].[upgrade_usuarios]([id_empleado],[key_user],[activo]) VALUES(343, N'VANZZINIF',1)
INSERT INTO [dbo].[upgrade_usuarios]([id_empleado],[key_user],[activo]) VALUES(315, N'CALDERONJF',1)
INSERT INTO [dbo].[upgrade_usuarios]([id_empleado],[key_user],[activo]) VALUES(142, N'LINOB',1)
INSERT INTO [dbo].[upgrade_usuarios]([id_empleado],[key_user],[activo]) VALUES(345, N'TERRONESC',1)
INSERT INTO [dbo].[upgrade_usuarios]([id_empleado],[key_user],[activo]) VALUES(316, N'ARELLANOC',1)
INSERT INTO [dbo].[upgrade_usuarios]([id_empleado],[key_user],[activo]) VALUES(393, N'TOLEDOS',1)
INSERT INTO [dbo].[upgrade_usuarios]([id_empleado],[key_user],[activo]) VALUES(310, N'SANCHEZL',1)
INSERT INTO [dbo].[upgrade_usuarios]([id_empleado],[key_user],[activo]) VALUES(308, N'MANRIQUEZSA',1)
--Se agregan usuarios de IT
INSERT INTO [dbo].[upgrade_usuarios]([id_empleado],[key_user],[activo]) VALUES(183, N'EDGAR',1)
INSERT INTO [dbo].[upgrade_usuarios]([id_empleado],[key_user],[activo]) VALUES(448, N'RAUL',1)
INSERT INTO [dbo].[upgrade_usuarios]([id_empleado],[key_user],[activo]) VALUES(363, N'JANETH',1)
--
INSERT INTO [dbo].[upgrade_usuarios]([id_empleado],[key_user],[activo]) VALUES(82, N'IGNACIO',1)
INSERT INTO [dbo].[upgrade_usuarios]([id_empleado],[key_user],[activo]) VALUES(239, N'ARENAS',1)
INSERT INTO [dbo].[upgrade_usuarios]([id_empleado],[key_user],[activo]) VALUES(216, N'MARTINEZF',1)
INSERT INTO [dbo].[upgrade_usuarios]([id_empleado],[key_user],[activo]) VALUES(387, N'MONTOYA',1)
INSERT INTO [dbo].[upgrade_usuarios]([id_empleado],[key_user],[activo]) VALUES(309, N'CHAGOYA',1)
INSERT INTO [dbo].[upgrade_usuarios]([id_empleado],[key_user],[activo]) VALUES(377, N'ALCANTAR',1)
INSERT INTO [dbo].[upgrade_usuarios]([id_empleado],[key_user],[activo]) VALUES(306, N'MUNIZ',1)
INSERT INTO [dbo].[upgrade_usuarios]([id_empleado],[key_user],[activo]) VALUES(113, N'GOMEZ',1)



 	  
IF object_id(N'upgrade_usuarios',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< upgrade_usuarios en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla upgrade_usuarios  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
