USE Portal_2_0
GO
IF object_id(N'budget_responsables',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[budget_responsables]
      PRINT '<<< budget_responsables en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los datos budget_responsables
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/03/23
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [budget_responsables](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_budget_centro_costo][int] NOT NULL,
	[id_responsable][int] NOT NULL,
	[estatus][bit] NOT NULL DEFAULT 1,
 CONSTRAINT [PK_budget_responsables] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

--agregar FK

-- restriccion de clave foranea
  alter table [budget_responsables]
 add constraint FK_budget_responsables_id_budget_centro_costo
  foreign key (id_budget_centro_costo)
  references budget_centro_costo(id);

  -- restriccion de clave foranea
  alter table [budget_responsables]
 add constraint FK_budget_responsables_id_centro_costo
  foreign key (id_responsable)
  references empleados(id);

  --INSERTA RESPONSABLES
 INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (1,(SELECT TOP (1) id from empleados WHERE correo ='olaf.voss@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (2,(SELECT TOP (1) id from empleados WHERE correo ='cesar.balcazar@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (3,(SELECT TOP (1) id from empleados WHERE correo ='karla.garciag@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (4,(SELECT TOP (1) id from empleados WHERE correo ='karla.garciag@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (5,(SELECT TOP (1) id from empleados WHERE correo ='manuela.breustedt@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (6,(SELECT TOP (1) id from empleados WHERE correo ='edgar.corona@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (7,(SELECT TOP (1) id from empleados WHERE correo ='jesus.olvera@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (8,(SELECT TOP (1) id from empleados WHERE correo ='josemaria.ochoa@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (9,(SELECT TOP (1) id from empleados WHERE correo ='leonardo.pilotzi@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (10,(SELECT TOP (1) id from empleados WHERE correo ='josem.rosales@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (11,(SELECT TOP (1) id from empleados WHERE correo ='juancarlos.campos@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (12,(SELECT TOP (1) id from empleados WHERE correo ='imelda.lopez@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (13,(SELECT TOP (1) id from empleados WHERE correo ='imelda.lopez@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (14,(SELECT TOP (1) id from empleados WHERE correo ='cesar.garibay@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (15,(SELECT TOP (1) id from empleados WHERE correo ='fernando.gamboa@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (16,(SELECT TOP (1) id from empleados WHERE correo ='mauricio.garcia@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (17,(SELECT TOP (1) id from empleados WHERE correo ='leonardo.pilotzi@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (18,(SELECT TOP (1) id from empleados WHERE correo ='fernando.gamboa@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (19,(SELECT TOP (1) id from empleados WHERE correo ='karla.garciag@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (20,(SELECT TOP (1) id from empleados WHERE correo ='karla.garciag@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (21,(SELECT TOP (1) id from empleados WHERE correo ='edgar.corona@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (22,(SELECT TOP (1) id from empleados WHERE correo ='leonardo.pilotzi@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (23,(SELECT TOP (1) id from empleados WHERE correo ='juancarlos.campos@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (24,(SELECT TOP (1) id from empleados WHERE correo ='saul.hernandez@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (25,(SELECT TOP (1) id from empleados WHERE correo ='salvador.sierra@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (26,(SELECT TOP (1) id from empleados WHERE correo ='francisco.calyeca@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (27,(SELECT TOP (1) id from empleados WHERE correo ='miguel.monroy@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (28,(SELECT TOP (1) id from empleados WHERE correo ='miguel.monroy@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (29,(SELECT TOP (1) id from empleados WHERE correo ='miguel.monroy@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (30,(SELECT TOP (1) id from empleados WHERE correo ='miguel.monroy@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (31,(SELECT TOP (1) id from empleados WHERE correo ='miguel.monroy@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (32,(SELECT TOP (1) id from empleados WHERE correo ='miguel.monroy@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (33,(SELECT TOP (1) id from empleados WHERE correo ='rodrigo.gonzalez@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (34,(SELECT TOP (1) id from empleados WHERE correo ='sandro.hernandez@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (35,(SELECT TOP (1) id from empleados WHERE correo ='miguel.monroy@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (36,(SELECT TOP (1) id from empleados WHERE correo ='gerardo.cendejas@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (37,(SELECT TOP (1) id from empleados WHERE correo ='juan.hernandez@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (38,(SELECT TOP (1) id from empleados WHERE correo ='mareyli.ramos@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (39,(SELECT TOP (1) id from empleados WHERE correo ='hugo.gomez@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (40,(SELECT TOP (1) id from empleados WHERE correo ='leonardo.pilotzi@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (41,(SELECT TOP (1) id from empleados WHERE correo ='fernando.gamboa@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (42,(SELECT TOP (1) id from empleados WHERE correo ='fernando.escamilla@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (43,(SELECT TOP (1) id from empleados WHERE correo ='karla.garciag@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (44,(SELECT TOP (1) id from empleados WHERE correo ='edgar.corona@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (45,(SELECT TOP (1) id from empleados WHERE correo ='leonardo.pilotzi@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (46,(SELECT TOP (1) id from empleados WHERE correo ='juancarlos.campos@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (47,(SELECT TOP (1) id from empleados WHERE correo ='candy.terrones@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (48,(SELECT TOP (1) id from empleados WHERE correo ='miccio.hernandez@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (49,(SELECT TOP (1) id from empleados WHERE correo ='saulo.toledo@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (50,(SELECT TOP (1) id from empleados WHERE correo ='jorge.bernal@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (51,(SELECT TOP (1) id from empleados WHERE correo ='jorge.bernal@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (52,(SELECT TOP (1) id from empleados WHERE correo ='jorge.bernal@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (53,(SELECT TOP (1) id from empleados WHERE correo ='jorge.bernal@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (54,(SELECT TOP (1) id from empleados WHERE correo ='alejandro.guerra@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (55,(SELECT TOP (1) id from empleados WHERE correo ='eustolio.melendez@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (56,(SELECT TOP (1) id from empleados WHERE correo ='jorge.bernal@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (57,(SELECT TOP (1) id from empleados WHERE correo ='fabian.calderon@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (58,(SELECT TOP (1) id from empleados WHERE correo ='fermin.vanzzini@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (59,(SELECT TOP (1) id from empleados WHERE correo ='luis.sanchez@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (60,(SELECT TOP (1) id from empleados WHERE correo ='alvaro.coleote@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (61,(SELECT TOP (1) id from empleados WHERE correo ='leonardo.pilotzi@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (62,(SELECT TOP (1) id from empleados WHERE correo ='fernando.gamboa@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (63,(SELECT TOP (1) id from empleados WHERE correo ='karla.garciag@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (64,(SELECT TOP (1) id from empleados WHERE correo ='karla.garciag@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (65,(SELECT TOP (1) id from empleados WHERE correo ='edgar.corona@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (66,(SELECT TOP (1) id from empleados WHERE correo ='leonardo.pilotzi@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (67,(SELECT TOP (1) id from empleados WHERE correo ='juancarlos.campos@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (68,(SELECT TOP (1) id from empleados WHERE correo ='alvaro.coleote@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (69,(SELECT TOP (1) id from empleados WHERE correo ='cesar.garibay@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (70,(SELECT TOP (1) id from empleados WHERE correo ='alvaro.coleote@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (71,(SELECT TOP (1) id from empleados WHERE correo ='alvaro.coleote@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (72,(SELECT TOP (1) id from empleados WHERE correo ='alvaro.coleote@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (73,(SELECT TOP (1) id from empleados WHERE correo ='alvaro.coleote@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (74,(SELECT TOP (1) id from empleados WHERE correo ='alvaro.coleote@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (75,(SELECT TOP (1) id from empleados WHERE correo ='arturo.gutierrez@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (76,(SELECT TOP (1) id from empleados WHERE correo ='leonardo.pilotzi@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (77,(SELECT TOP (1) id from empleados WHERE correo ='arturo.gutierrez@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (78,(SELECT TOP (1) id from empleados WHERE correo ='arturo.gutierrez@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (79,(SELECT TOP (1) id from empleados WHERE correo ='arturo.gutierrez@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (80,(SELECT TOP (1) id from empleados WHERE correo ='arturo.gutierrez@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (81,(SELECT TOP (1) id from empleados WHERE correo ='arturo.gutierrez@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (82,(SELECT TOP (1) id from empleados WHERE correo ='arturo.gutierrez@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (83,(SELECT TOP (1) id from empleados WHERE correo ='arturo.gutierrez@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (77,(SELECT TOP (1) id from empleados WHERE correo ='enrique.perez@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (78,(SELECT TOP (1) id from empleados WHERE correo ='enrique.perez@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (79,(SELECT TOP (1) id from empleados WHERE correo ='enrique.perez@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (80,(SELECT TOP (1) id from empleados WHERE correo ='enrique.perez@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (81,(SELECT TOP (1) id from empleados WHERE correo ='enrique.perez@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (82,(SELECT TOP (1) id from empleados WHERE correo ='enrique.perez@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (83,(SELECT TOP (1) id from empleados WHERE correo ='enrique.perez@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (84,(SELECT TOP (1) id from empleados WHERE correo ='enrique.perez@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (85,(SELECT TOP (1) id from empleados WHERE correo ='enrique.perez@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (86,(SELECT TOP (1) id from empleados WHERE correo ='arturo.gutierrez@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (87,(SELECT TOP (1) id from empleados WHERE correo ='arturo.gutierrez@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (88,(SELECT TOP (1) id from empleados WHERE correo ='arturo.gutierrez@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (89,(SELECT TOP (1) id from empleados WHERE correo ='arturo.gutierrez@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (86,(SELECT TOP (1) id from empleados WHERE correo ='enrique.perez@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (87,(SELECT TOP (1) id from empleados WHERE correo ='enrique.perez@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (88,(SELECT TOP (1) id from empleados WHERE correo ='enrique.perez@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (89,(SELECT TOP (1) id from empleados WHERE correo ='enrique.perez@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (90,(SELECT TOP (1) id from empleados WHERE correo ='jorge.salinas@lagermex.com.mx'),1)
INSERT INTO [dbo].[budget_responsables] ([id_budget_centro_costo],[id_responsable],[estatus]) VALUES (91,(SELECT TOP (1) id from empleados WHERE correo ='leonardo.pilotzi@lagermex.com.mx'),1)

  -- FIN RESPONSABLES
 	  
IF object_id(N'budget_responsables',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< budget_responsables en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla budget_responsables  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO

