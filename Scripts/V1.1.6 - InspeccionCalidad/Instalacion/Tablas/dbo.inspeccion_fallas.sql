use[Portal_2_0]
GO
IF object_id(N'inspeccion_fallas',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[inspeccion_fallas]
      PRINT '<<< inspeccion_fallas en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los tipos de fallas
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2021/10/06
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [dbo].[inspeccion_fallas](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_categoria_falla][int] NOT NULL, --FK
	[descripcion] [varchar](80) NOT NULL,
	[aplica_en_calculo][bit] NOT NULL,
	[dano_interno][bit] NOT NULL,
	[dano_externo][bit] NOT NULL,
	[activo] [bit] NOT NULL
 CONSTRAINT [PK_inspeccion_fallas] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restriccion de clave foranea
alter table [inspeccion_fallas]
 add constraint FK_inspeccion_fallas_id_categoria
  foreign key (id_categoria_falla)
  references inspeccion_categoria_fallas(id);

--restriccion check
--verifica que solo un tipo de daño este activo
ALTER TABLE [inspeccion_fallas] ADD  CONSTRAINT [CK_dano_interno_externo]  CHECK (CAST(dano_interno AS INT)+CAST(dano_externo AS INT)=1);

  -- restricción default
ALTER TABLE [inspeccion_fallas] ADD  CONSTRAINT [DF_inspeccion_fallas_activo]  DEFAULT (1) FOR [activo]
GO

--inserta las fallas
SET IDENTITY_INSERT [inspeccion_fallas] ON 

--SELECT * FROM dbo.inspeccion_categoria_fallas
--fallas almacen recibido
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(1,1,N'Golpe o Daño en cantos',1,1,0,1)
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(2,1,N'Golpes en diametro externo',1,1,0,1)
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(3,1,N'Marcas en diametro interno',1,1,0,1)
--fallas de troquel
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(4,2,N'Golpes de troquel de Troquel',1,1,0,1)
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(5,2,N'Rayones de Troquel',1,1,0,1)
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(6,2,N'Incrustación de chatarra',1,1,0,1)
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(7,2,N'Rebaba',1,1,0,1)
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(8,2,N'Linealidad',1,1,0,1)
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(9,2,N'Se atora al avance / Acordion',1,1,0,1)
--fallas de proceso
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(10,3,N'Inspección / Liberación de proceso',0,1,0,1)
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(11,3,N'Bases y Tapas',0,1,0,1)
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(12,3,N'Descuadre',1,1,0,1)
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(13,3,N'Simetria / Desplazamiento',1,1,0,1)
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(14,3,N'Avance No OK',1,1,0,1)
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(15,3,N'Ángulo No Ok',1,1,0,1)
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(16,3,N'Planicidad',1,1,0,1)
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(17,3,N'Rayones',1,1,0,1)
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(18,3,N'Rodillos Aplanadora',1,1,0,1)
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(19,3,N'Rodillo Ciclico',1,1,0,1)
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(20,3,N'Golpes de Apilador',1,1,0,1)
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(21,3,N'Rodillo alimentador',1,1,0,1)
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(22,3,N'Roll mark (706)',1,1,0,1)
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(23,3,N'Hilos de goma (Wipers) Solo Puebla',1,1,0,1)
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(24,3,N'Fin de banda',0,1,0,1)
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(25,3,N'Material con suciedad',1,1,0,1)
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(26,3,N'Pruebas',0,1,0,1)
--daños por manejo logistico
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(27,4,N'Golpes o Daño en cantos',1,0,1,1)
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(28,4,N'Golpe / Marca de INICIO de rollo',1,0,1,1)
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(29,4,N'Golpe / Marca FIN de rollo',1,0,1,1)
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(30,4,N'Friccion / Abrasión',1,0,1,1)
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(31,4,N'Oxido / Emulsión',1,0,1,1)
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(32,4,N'Soldadura Solo Puebla',1,0,1,1)
--fallas de molino
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(33,5,N'Peso fuera de especificación',1,0,1,1)
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(34,5,N'Diametro reducido',1,0,1,1)
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(35,5,N'Ancho Fuera de Especificación',1,0,1,1)
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(36,5,N'Marca de fleje',1,0,1,1)
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(37,5,N'Humedad',1,0,1,1)
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(38,5,N'Suciedad / Contaminación',1,0,1,1)
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(39,5,N'Roll marks',1,0,1,1)
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(40,5,N'Material Ondulado ',1,0,1,1)
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(41,5,N'Incrustaciones de Zinc',1,0,1,1)
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(42,5,N'Insectos',1,0,1,1)
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(43,5,N'Laminación',1,0,1,1)
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(44,5,N'Latigazos / Pinchers',1,0,1,1)
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(45,5,N'Manchas',1,0,1,1)
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(46,5,N'Poros',1,0,1,1)
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(47,5,N'Puntos brillantes',1,0,1,1)
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(48,5,N'Rayas / Scratches',1,0,1,1)
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(49,5,N'Stencil',1,0,1,1)
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(50,5,N'Dross Solo Puebla',1,0,1,1)
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(51,5,N'Holds Solo Puebla',1,0,1,1)
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(52,5,N'Ash / Ceniza Solo Puebla',1,0,1,1)
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(53,5,N'Escurrimiento de Zinc Solo Puebla',1,0,1,1)
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(54,5,N'Camber Solo Puebla',1,0,1,1)
INSERT INTO [dbo].[inspeccion_fallas] ([id],[id_categoria_falla],[descripcion],[aplica_en_calculo],[dano_interno],[dano_externo],[activo])VALUES(55,5,N'Otros',1,0,1,1)


SET IDENTITY_INSERT [inspeccion_fallas] OFF
GO


--INFO
IF object_id(N'inspeccion_fallas',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< inspeccion_fallas en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla inspeccion_fallas  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
