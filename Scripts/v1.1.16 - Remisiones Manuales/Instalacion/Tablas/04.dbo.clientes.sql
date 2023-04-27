--USE Portal_2_0
GO
IF object_id(N'clientes',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[clientes]
      PRINT '<<< clientes en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los clientes
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[clientes](
	[clave] [int] IDENTITY(1,1) NOT NULL,
	[activo] [bit] NULL,
	[claveSAP] [varchar](50) NULL,
	[descripcion] [varchar](100) NULL,
	[pais] [varchar](20) NULL,
	[direccion] [varchar](100) NULL,
 CONSTRAINT [PK_Cliente] PRIMARY KEY CLUSTERED 
(
	[clave] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

--inserta datos
SET IDENTITY_INSERT [dbo].[clientes] ON 
GO
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (1, 1, N'945', N'VOLKSWAGEN DE MEXICO S.A. DE C.V.', N'MX ', N'AUTOPISTA MEXICO-PUEBLA, KM 116 CUAUTLANCINGO PUE')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (2, 1, N'946', N'BENTELER DE MEXICO, S.A. DE C.V', N'MX ', N'PROLONGACION DIAGONAL DEFENSORES DE PUEBLA PUE')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (3, 1, N'950', N'CLIENTE OBSOLETO CHRYSLER', N'MX ', N'Prol. Paseo de la Reforma 1240 Desarrollo Santa Fe DF')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (4, 1, N'953', N'GRUPO KERVO S.A DE C.V.', N'MX ', N'KM 12 CARRETERA GUADALAJARA S/N EL SALTO JAL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (5, 1, N'954', N'AUTOTEK MÉXICO, S. A. DE C.V.', N'MX ', N'PROLONGACION DE LA CALLE F 50 1 PUEBLA PUE')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (6, 1, N'955', N'USG MEXICO S.A. DE C.V.', N'MX ', N'LOS ARCOS CUAUTLANCINGO S/N CUAUTLANCINGO PUEBLA PUE')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (7, 1, N'956', N'ACEROS Y METALES HERZA S.A DE C.V.', N'MX ', N'CALLE CENTEOTL 290 ATZCAPOTZALCO DF')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (8, 1, N'957', N'LUNKOMEX, S.A. DE C.V.', N'MX ', N'AV. RESURRECCION SUR 6 PUEBLA PUE')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (9, 1, N'958', N'FEG DE QUERETARO S.A. DE C.V.', N'MX ', N'CERRADA DE LA NORIA 106 SANTIAGO DE QUERETARO QRO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (10, 1, N'959', N'HYLSA SA. DE C.V', N'MX ', N'ANT.CAMINO AL RANCHO STA MARIA 7468 PUEBLA PUE')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (11, 1, N'960', N'METALSA  S.A. DE C.V.', N'MX ', N'CARRETERA MIGUEL ALEMAN KM 16 100 APODACA NUEVO LEON NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (12, 1, N'961', N'Industrias Crinsa, S.A. de C.V.', N'MX ', N'Av. Ceilán 959-42 Azcapozalco Col. Industrial Vallejo DF')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (13, 1, N'962', N'ACERO PRIME S. DE R.L. DE C.V.', N'MX ', N'EJE 128 No. 209 SAN LUIS POTOSI SLP')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (14, 1, N'963', N'ACEROCINTA S.A. DE C.V.', N'MX ', N'AV CENTRAL COL IND VALLEJO 228 DELEGACION GUSTAVO A MADERO DF')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (15, 1, N'964', N'ARTICULOS GALVANIZADOS S.A. DE C.V.', N'MX ', N'AV JARDIN 289 MEXICO DF')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (16, 1, N'965', N'CARTEC S.A. DE C.V.', N'MX ', N'CARRIL NTE A SN CRISTOBAL SN KM 14.5 SIN COLINIA CHACHAPA, AMOZO PUE')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (17, 1, N'966', N'FISCHER MEXICANA', N'MX ', N'EJE 124 ZONA INDUSTRIAL 115 SAN LUIS POTOSI SLP')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (18, 1, N'967', N'LINDE PULLMAN DE QUERETARO', N'MX ', N'CARR. QRO SL.P. KM 28.5 MANZAN 25-2 JAUREGUI QRO SLP')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (19, 1, N'968', N'BOSAL MEXICO S.A. DE C.V.', N'MX ', N'ACCESO 1 FRACC IND LA MONTAÑA 126 QUERETARO QRO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (20, 1, N'969', N'TRANSFORMACION Y SERVICIO DE ACERO  S. A. DE C.V.', N'MX ', N'WAKE 215-A COL. LIBERTAD DELEGACION AZCAPOTZALCO DF')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (21, 1, N'970', N'THYSSENKRUPP TAILORED BLANKS SA CV', N'MX ', N'KM 117 AUT MEX PUE S/N CUAUTLANCINGO PUE')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (22, 1, N'975', N'Industrias NORM, S.A. de C.V.', N'MX ', N'Camino a San Lorenzo 1202 col. sanctorum PUE')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (23, 1, N'977', N'NICOMETAL MEXICANA S.A. DE C.V.', N'MX ', N'CIRC. AGUASCALIENTES NTE 139 P.IND DEL VALLE DE AGUASCALIENTES AGS')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (24, 1, N'980', N'HONDA TRADING DE MEXICO S.A. DE C.V', N'MX ', N'AV. TORRES LANDA 204 5 PISO CELAYA GTO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (25, 1, N'989', N'GESTAMP PUEBLA S.A. DE C.V.', N'MX ', N'AUTOMOCIÓN 8 CUAUTLANCINGO PUE')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (26, 1, N'994', N'MAPFRE TEPEYAC S.A. DE C.V.', N'MX ', N'BLVD MAGNOCENTRO 5 HUIXQUILUCAN DE DEGOLLADO MEX')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (27, 1, N'997', N'NISSAN MEXICANA S.A. DE C.V', N'MX ', N'AV INSURGENTES SUR 1958 COL FLORIDA , MEXICO DF')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (28, 1, N'999', N'THYSSENKRUPP STEEL NORTH AMERICA INC', N'US ', N'22355 WEST 11 MILE ROAD SOUTHFIELD, MI MI')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (29, 1, N'1001', N'THYSSENKRUPP STEEL EUROPE AG', N'DE ', N'KAISER-WILHELM-STRASSE 100 DUISBURG GERMANY ')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (30, 1, N'1011', N'DISTRIBUIDORA DE METALES Y CARTONES', N'MX ', N'CARRETERA VILLA DE GARCIA KM. 2.7 4 SANTA CATARINA NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (31, 1, N'1012', N'MARUBENI-ITOCHU STEEL MEXICO, S.A. DE C.V.', N'MX ', N'BLVD. MANUEL AVILA CAMACHO 24 1703 MIGUEL HIDALGO CMX')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (32, 1, N'1036', N'CIE CELAYA, S.A. DE C.V.', N'MX ', N'AVE. NORTE CUATRO 100 CELAYA, GUANAJUATO GTO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (33, 1, N'1050', N'Estampados Magna de México, S.A. de C.V.', N'MX ', N'AV. Santa María 1501 Ramos Arizpe COA')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (34, 1, N'1087', N'Shiloh de Mexico, S.A. de C.V.', N'MX ', N'Av. Shiloh s/n Ramos Arizpe COA')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (35, 1, N'1101', N'PINTURA ESTAMPADO Y MONTAJE S.A.P.I DE C.V', N'MX ', N'AV. CONCEPCION BEISTEGUI 2007 CELAYA GTO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (36, 1, N'1107', N'Serviacero Planos, S. de R.L. de ', N'MX ', N'BLVD. HERMANOS ALDAMA 4002 CD. INDUSTRIAL, LEON GTO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (37, 1, N'1123', N'FORMEX MEXICO, S.A. DE C.V.', N'MX ', N'BOULEVARD MAGNA 2000 RAMOS ARIZPE COA')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (38, 1, N'1189', N'PROVEEDORA DE ACEROS Y METALES CARM S.A. DE C.V.  ', N'MX ', N'RÍO NAZAS 280 A SAN LUIS POTOSÍ SLP')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (39, 1, N'1200', N'FLEX N GATE MEXICO, S. DE R.L. DE C', N'MX ', N'AV. INGENIERO ANTONIO GUTIERREZ COR SAN JOSE ITURBIDE GTO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (40, 1, N'1237', N'JOSE SAFONT REYNOSO', N'MX ', N'SOR JUANA INES DE LA CRUZ 153 SAN LUIS POTOSI SLP')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (41, 1, N'1260', N'NICOMETAL MEXICANA, S.A. DE C.V.', N'MX ', N'Circuito Aguascalientes Norte 139 SAN FRANCISCO DE LOS ROMO AGS')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (42, 1, N'1261', N'COMERCIALIZADORA DE ACEROS RECICLABLES S', N'MX ', N'MELGA SAN LORENZO ALMECATLA No. 21 CUAUTLANCINGO PUE')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (43, 1, N'1280', N'VOIT AUTOMOTIVE DE MÉXICO S.A. DE C               ', N'MX ', N'CARR. GUADALAJARA-EL SALTO K.M. 12  EL SALTO JAL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (44, 1, N'1283', N'STEEL TECHNOLOGIES DE MEXICO,       S.A. DE C.V.  ', N'MX ', N'FEDERALISMO 204 FRACC.IND.LA SILLA GUADALUPE NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (45, 1, N'1285', N'METAL ONE AMERICA, INC                            ', N'US ', N'SUITE 1170 FRANKLIN ROAD 27777 SOUTHFIELD MI')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (46, 1, N'1293', N'GENERAL MOTORS DE MEXICO            S. DE R.L. DE ', N'MX ', N'Av. Ejercito Nacional 843 COL GRANADA DEL. MIGUEL HIDALGO DF')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (47, 1, N'1295', N'PULLMAN DE PUEBLA S.A. DE C.V.                    ', N'MX ', N'AUTOPISTA MEXICO-PUEBLA KM 115 CONJUNTO OCOTLAN, BODEGA C SN F. OC PUE')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (48, 1, N'1296', N'SERVILAMINA SUMMIT MEXICANA S.A. DE               ', N'MX ', N'ACCESO III 15 A QUERETARO QRO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (49, 1, N'1339', N'VRK AUTOMOTIVE SYSTEMS, S.A. DE C.V               ', N'MX ', N'AV. LA NORIA 111 PARQUE INDUSTRIAL QUERETARO QRO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (50, 1, N'1358', N'CHRYSLER DE MEXICO, S.A. DE C.V.                  ', N'MX ', N'Prolongacion Paseo de la Refor 1240 Delegacion Cuajimalpa DF')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (51, 1, N'1360', N'NUGAR, S.A.P.I. de C.V.                           ', N'MX ', N'PLANO REGULADOR No. 8 XOCOYAHUALCO, TLALNEPANTLA MEX')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (52, 1, N'1386', N'SSA MEXICO, S.A. DE C.V.                          ', N'MX ', N'INSURGENTES SUR 1898 PISO 11 COL.FLORIDA, DELEG. ALVARO OBREGON DF')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (53, 1, N'1397', N'Zenaida Sandoval Reyes                            ', N'MX ', N'CALLE SUR 2 "A", No.39 INT. S/N MUNICIPIO TULTITLAN MEX')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (54, 1, N'1408', N'GONVAUTO PUEBLA S.A. DE C.V.                      ', N'MX ', N'AUTOMOCIÓN 10 CUAUTLANCINGO PUE')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (55, 1, N'1413', N'HIROTEC MEXICO S.A. DE C.V.                       ', N'MX ', N'CARRETERA PANAMERICANA 5.5 2 A LC SILAO DE LA VICTORIA GTO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (56, 1, N'1421', N'TERNIUM MEXICO, S.A. DE C.V.                      ', N'MX ', N'AV UNIVERSIDAD 992 CUAUHTEMOC NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (57, 1, N'1430', N'POSCO MPPC, S.A. DE C.V.                          ', N'MX ', N'CARRETERA 190 KM 0.550 HUEJOTZINGO,PUEBLA PUE')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (58, 1, N'1439', N'NATIONAL MATERIAL OF MEXICO         S. DE R.L. DE ', N'MX ', N'AV. SEXTA ORIENTE 150 APODACA NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (59, 1, N'1472', N'PWO DE MEXICO, S.A. DE C.V.                       ', N'MX ', N'CARRIL NORTE A SAN CRISTOBAL, AUT. AMOZOC PUE')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (60, 1, N'1483', N'ACEROTEK S.A. DE C.V.                             ', N'MX ', N'REGINO VARGAS 408 SAN NICOLAS DE#LOS GARZA NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (61, 1, N'1494', N'FORD MOTOR COMPANY S.A. DE C.V.                   ', N'MX ', N'CIRCUITO GUILLERMO GONZÁLEZ CAMAREN DELEGACIÓN ÁLVARO OBREGÓN CMX')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (62, 1, N'1500', N'FORD MOTOR COMPANY                                ', N'US ', N'CENTRAL ACCOUNT SERV. PO. BOX1718, DEARBORN, MICHIGAN MI')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (63, 1, N'1515', N'THYSSENKRUPP INDUSTRIAL SERVICES                  ', N'US ', N'8001 THYSSENKRUPP PARKWAY NORTHWOOD OH')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (64, 1, N'1522', N'TWB DE MEXICO S.A. DE C.V.                        ', N'MX ', N'KM 117 AUTOPISTA MEXICO PUEBLA CUAUTLANCINGO PUE')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (65, 1, N'1524', N'MANUFACTURAS ESTAMPADAS S.A. DE C.V               ', N'MX ', N'JUAN RUIZ DE ALARCON 305 CHIHUAHUA CHI')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (66, 1, N'1538', N'UNITED STATES STEEL CORP.                         ', N'US ', N'P.O. BOX 267 PITTSBURGH PA')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (67, 1, N'1546', N'MARTINREA DEVELOPMENTS DE MEXICO    S.A. DE C.V.  ', N'MX ', N'CARRETERA ANTIGUA ARTEAGA 2653 SALTILLO COA')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (68, 1, N'1561', N'ARCELORMITTAL BURNS HARBOR LLC                    ', N'US ', N'19TH FLOOR ONE SOUTH DEARBORN CHICAGO IN')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (69, 1, N'1565', N'I/N KOTE                                          ', N'US ', N'EDISON ROAD 30755 NEW CARLISLE IN')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (70, 1, N'1579', N'DAEWOO INTERNATIONAL MEXICO,        S.A. DE C.V.  ', N'MX ', N'PASEO DE LOS TAMARINDOS 400 A PISO 4 , COL. BOSQUES DE LAS LOMAS DF')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (71, 1, N'1594', N'POSCO AMERICA CORP.                               ', N'US ', N'6465 EAST JOHNS CROSSING SUITE 200 JOHNS CREEK GA')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (72, 1, N'1595', N'STEEL TECHNOLOGIES LLC                            ', N'US ', N'BELLEVILLE ROAD 5501 CANTON MI')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (73, 1, N'1597', N'NARMX QUERETARO S.A. DE C.V.                      ', N'MX ', N'CARR. QUERETARO-SAN LUIS MANZANA 1 QUERETARO QRO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (74, 1, N'1605', N'SALZGITTER MANNESMANN PRECISION     S.A. DE C.V.  ', N'MX ', N'CALLE A 239 PARQUE INDUSTRIAL EL SALTO JAL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (75, 1, N'1618', N'ROCKWELL AUTOMATION MONTERREY       MANUFACTURING,', N'MX ', N'ARISTOTELES 125-4 PARQUE INDUSTRIAL KALOS NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (76, 1, N'1619', N'MISA-NATIONAL METAL PROCESSING      S.A. DE C.V.  ', N'MX ', N'AV. MONTES URALES 11 PARQUE INDUSTRIAL OPCION GTO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (77, 1, N'1642', N'COPPER AND BRASS SALES                            ', N'US ', N'5200 SOUTH ASHLAND WAY FRANKLIN WI')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (78, 1, N'1660', N'TKMNA, INC.                         COPPER AND BRA', N'US ', N'22355 WEST ELEVEN MILE ROAD SOUTHFIELD MI')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (79, 1, N'1670', N'MC INDUSTRIACERO, S.A. DE C.V.                    ', N'MX ', N'PRESA HUAPANGO No.186 RECURSOS HIDRAULICOS, MEXICO MEX')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (80, 1, N'1684', N'KEN-MAC METALS DIVISION                           ', N'US ', N'17901 ENGLEWOOD DR. MIDDLEBURG HEIGHTS OH')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (81, 1, N'1690', N'LAMINAS Y ACEROS DE QUERETARO,      S.A. DE C.V.  ', N'MX ', N'AVENIDA CANDILES 198-4 LOS CANDILES QUERETARO MEX')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (82, 1, N'1695', N'SIDCOMEX, S.A. DE C.V.                            ', N'MX ', N'KM 117 AUTOPISTA MEXICO PUEBLA S/N CUAUTLANCINGO PUE')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (83, 1, N'1712', N'STRIP STEEL S. DE R.L.                            ', N'MX ', N'LATERAL AUTOPISTA MEXICO PUEBLA OCOTLAN, CORONANGO PUEBLA PUE')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (84, 1, N'1743', N'PYRAMID INDUSTRIES DE MEXICO        S. DE R.L. DE ', N'MX ', N'CALLE ACCESO V 107 D QUERETARO QRO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (85, 1, N'1744', N'SCHNEIDER ELECTRIC MEXICO,          S.A. DE C.V.  ', N'MX ', N'AV. JAVIER ROJO GOMEZ 1121-A IZTAPALAPA, CIUDAD DE MEXICO CMX')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (86, 1, N'1747', N'THYSSENKRUPP STEEL USA, LLC                       ', N'US ', N'1 THYSSENKRUPP DRIVE CALVERT, AL AL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (87, 1, N'1769', N'NAVA HERMANOS, S.A. DE C.V.                       ', N'MX ', N'AV. LOPEZ MATEOS 4109 SAN NICOLAS DE LOS GARZA NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (88, 1, N'1776', N'LUK PUEBLA, S.A. DE C.V.                          ', N'MX ', N'AVENIDA RESURRECCION NORTE No.12 PARQUE INDUSTRIAL RESURRECCION PUE')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (89, 1, N'1801', N'SONORA FORMING, S.A. DE C.V.                      ', N'MX ', N'BLVD. HENRY FORD 43 SONORA SON')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (90, 1, N'1805', N'IPAR MOLD, S.A. DE C.V.                           ', N'MX ', N'CALLE PUERTO VERACRUZ No.227 "C" COL. LA FE, NUEVO LEON NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (91, 1, N'1825', N'ISRINGHAUSEN MEXICO,                S.A. DE C.V.  ', N'MX ', N'SERVIDUMBRE DEL PASO LOTE V1 No.851 SALTILLO COA')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (92, 1, N'1840', N'CENTERLINE MEXICO, S. DE RL DE C.V.               ', N'MX ', N'AVENIDA LA NORIA No:110 PARQUE INDUSTRIAL QUERETARO QRO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (93, 1, N'1875', N'NICOMETAL HIDALGO, S.A. DE C.V.                   ', N'MX ', N'PONIENTE 1 LOTE 10 A ATITALAQUIA HGO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (94, 1, N'1895', N'TRANSFORMACION Y SERVICIO DE ACERO  S.A. DE C.V.  ', N'MX ', N'WAKE 215 A AZCAPOZALCO CMX')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (95, 1, N'1898', N'ESPECIALIDADES TROQUELADAS S.A DE C               ', N'MX ', N'AV. LIBERTAD 248 GUADALUPE NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (96, 1, N'1899', N'ABINSA S.A. DE C.V.                               ', N'MX ', N'AVE. ADOLFO LOPEZ MATEOS KM. 6. S/N JARDINES DE CASA BLANCA NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (97, 1, N'1902', N'THYSSENKRUPP MATERIALS CA LTD.                    ', N'CA ', N'2821 LANGSTAFF ROAD CONCORD, ONTARIO, CANADA ON')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (98, 1, N'1925', N'PIEDRA SOLIDA ESTRUCTURAL,          S.A. DE C.V.  ', N'MX ', N'CALLE RINCON DEL LLANO No.7703 RINCON DE GUADALUPE, N.L. NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (99, 1, N'1951', N'LUNKOMEX, S. DE R.L. DE C.V.                      ', N'MX ', N'AV. RESURRECCION SUR No.6 PUEBLA PUE')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (100, 1, N'1952', N'NISSAN TRADING CORPORATION          AMERICAS      ', N'MX ', N'AVENIDA UNIVERSIDAD 1001 PISO No.8 BOSQUES DEL PRADO, AGUASCALIENTES AGS')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (101, 1, N'1953', N'HONDA TRADING DE MEXICO S.A. DE C.V               ', N'MX ', N'AV. TORRES LANDA 204 5 PISO CELAYA GTO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (102, 1, N'1956', N'ALLGAIER DE PUEBLA S.A.P.I. DE C.V.               ', N'MX ', N'AUTOPISTA PUEBLA ORIZABA KM 14.5 AMOZOC PUE')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (103, 1, N'1957', N'MARCO ANTONIO MONTES DE OCA ALLENDE               ', N'MX ', N'WAKE 215 A AZCAPOTZALCO DF')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (104, 1, N'1959', N'SAN LUIS METAL FORMING S.A. DE C.V.               ', N'MX ', N'FRANKFURT 201 VILLA DE REYES SLP')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (105, 1, N'1960', N'GENI DE MEXICO S.A. DE C.V.                       ', N'MX ', N'CARRETERA AL AEROPUERTO KM 2 700 HUEJOTZINGO PUE')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (106, 1, N'1970', N'SCHNEIDER ELECTRIC USA, INC.                      ', N'US ', N'STE 200 N MARTINGALE Rd, TAX ID: 36-2440683 IL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (107, 1, N'1989', N'FELICIANO CUAZITL SASTRE                          ', N'MX ', N'AV. 20 DE NOVIEMBRE No.89 SAN JUAN CUAUTLANCINGO PUE')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (108, 1, N'1997', N'AM/NS CALVERT LLC                                 ', N'US ', N'1 AMNS WAY CALVERT AL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (109, 1, N'2002', N'SAPA INDUSTRIAL EXTRUSIONS                        ', N'US ', N'53 POTTSVILLE STREET CRESSONA, PA PA')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (110, 1, N'2007', N'ACEROS PIEDRA SOLIDA                S.A. DE C.V.  ', N'MX ', N'CUAUTEMOC CENTRO No. 122 GUADALUPE, NUEVO LEON NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (111, 1, N'2010', N'GESTAMP PUEBLA II S.A. DE C.V.                    ', N'MX ', N'CARR. AVENIDA 18 DE NOCIEMBRE 1062 PUEBLA, PUEBLA PUE')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (112, 1, N'2012', N'GE ELECTRICAL DISTRIBUTION          EQUIPMENT SA D', N'MX ', N'BLVD. DIAZ ORDAZ 333 PISO 2 SAN PEDRO GARZA GARCIA, NL NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (113, 1, N'2014', N'REYME STEEL                         S. DE R.L. DE ', N'MX ', N'EL PUERTO G, MANZANA 7, LOTE 12 S/N TLALNEPANTLA DE BAZ, EDO. MEXICO MEX')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (114, 1, N'2021', N'GESTAMP AGUASCALIENTES S.A. DE C.V.               ', N'MX ', N'AVENIDA JAPON 124 PARQUE INDUSTRIAL SAN FRANCISCO DE LOS ROMO AGS')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (115, 1, N'2023', N'MULTISISTEMAS DE SEGURIDAD          DEL BAJIO S.A ', N'MX ', N'LUIS VEGA Y MONROY No. 330 SANTIAGO DE QUERETARO QRO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (116, 1, N'2025', N'METAL TRIPLE M MEXICO SA DE CV                    ', N'MX ', N'JOSE VASCONCELOS No.638 A VALLE DEL CAMPESTRE, NUEVO LEON NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (117, 1, N'2026', N'TENIGAL S. DE R.L. DE C.V.                        ', N'MX ', N'AV. VICENTE GUERRERO 151 SAN NICOLAS DE LOS GARZA NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (118, 1, N'2027', N'ROCKWELL AUTOMATION MONTERREY       MANUFACTURING ', N'MX ', N'CAMINO VECINAL 3051 NUEVO LEON NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (119, 1, N'2028', N'ROCKWELL AUTOMATION,INC.                          ', N'US ', N'S SECOND STREET 1201 MILWAUKEE WI')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (120, 1, N'2053', N'USG MEXICO SA DE CV                               ', N'MX ', N'PASEO TAMARINDOS No. 400 Int. B Ciudad de Mexico CMX')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (121, 1, N'2055', N'FCA MEXICO, S.A. DE C.V.                          ', N'MX ', N'PROLONGACION PASEO DE LA REFORMA 12 DELEGACION CUAJIMALPA CMX')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (122, 1, N'2063', N'HAMMOND POWER SOLUTIONS, INC.                     ', N'US ', N'LAKE ST 1100 BARABOO WI')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (123, 1, N'2068', N'AUDI MEXICO SA DE CV                              ', N'MX ', N'CHINANTLA No. 10 COL LA PAZ, PUEBLA PUE')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (124, 1, N'2069', N'OPERADORA DE SITES MEXICANOS        S.A. DE C.V   ', N'MX ', N'LAGO ZURICH No. 245 DELEGACION MIGUEL HIDALGO DF')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (125, 1, N'2071', N'PREMET S.A. DE C.V.                               ', N'MX ', N'COSMOS 5104 FRANCISCO I MADERO,CHIHUAHUA CHI')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (126, 1, N'2086', N'METALES LOZANO SA DE CV                           ', N'MX ', N'HELEODORO PEREZ OTE 2824 ARGENTINA, NUEVO LEON NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (127, 1, N'2087', N'SEGANA S.A. DE C.V.                               ', N'MX ', N'CALLE CANATLAN No. 240 GOMEZ PALACIO, DURANGO DGO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (128, 1, N'2103', N'ANTONIO MORO HERRERA                              ', N'MX ', N'RETORNO 9-E MODULO 57 DPTO.11 COL. BOSQUES DE SAN SEBASTIAN PUE')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (129, 1, N'2114', N'MENAV ACEROS SA DE CV                             ', N'MX ', N'TEMOAYA 12 Int. 3 CENTRO URBANO, CUAUTITLAN IZACLLI MEX')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (130, 1, N'2121', N'GGM PUEBLA S.A. DE C.V.                           ', N'MX ', N'LATERAL ARCO NORTE KM 25 No. 4 CORONANGO, PUE. PUE')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (131, 1, N'2125', N'ARTICULOS DE LAMINA GARLI S.A. DE C               ', N'MX ', N'PROLONGACION FRAY PEDRO DE GANTE TEXCOCO MEX')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (132, 1, N'2129', N'RACK PROCESSING DE MEXICO           S. DE R.L. DE ', N'MX ', N'KIOTO LOTE 1 MANZANA 1 NAVE 7-8 IRAPUATO GTO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (133, 1, N'2133', N'NOVA TUBE AND COIL DE MEXICO        S DE RL DE CV ', N'MX ', N'BLVD. SANTA MARIA No:1600 RAMOS ARIZPE COAHUILA COA')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (134, 1, N'2149', N'LAMINA Y TRANSFORMACION             S.A. DE C.V.  ', N'MX ', N'PRESA HUAPANGO 176 S/N TULTITLAN MEXICO MEX')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (135, 1, N'2160', N'POSCO AMERICA COMERCIALIZADORA      S. DE R.L. DE ', N'MX ', N'AV PASEO DE LA REFORMA 250, PISO 9 CUAUHTEMOC CMX')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (136, 1, N'2171', N'GESTAMP CHATTANOOGA, LLC.                         ', N'US ', N'3063 HICKORY VALLEY RD CHATTANOOGA, TN TN')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (137, 1, N'2177', N'CELAY S.A. DE C.V.                                ', N'MX ', N'AV. NORTE 4 No.110 CELAYA, GUANAJUATO GTO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (138, 1, N'2182', N'GE TRANSPORTES FERROVIARIOS S/A                   ', N'BR ', N'AV. GENERAL DAVID SARNOFF, 4600 CONTAGEM MG')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (139, 1, N'2189', N'POSCO INTERNATIONAL MEXICO S.A. DE                ', N'MX ', N'CALLE PASEO DE LOS TAMARINDO PISO 4 CUAJIMALPA DE MORELO, CIUDAD DE MEX CMX')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (140, 1, N'2193', N'GE MEXICO S.A. DE C.V.                            ', N'MX ', N'ANTONIO DOVALI JAIME. 70 TORRE B ALVARO OBREGON CMX')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (141, 1, N'2195', N'FERROMEX DEL NORTE S.A. DE C.V.                   ', N'MX ', N'BLVD VITO ALESSIO ROBLES 6599 SALTILLO COA')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (142, 1, N'2200', N'ARCELORMITTAL TAILORED BLANKS SILAO S. DE R.L. DE ', N'MX ', N'PASEO DE LOS INDUSTRIALE LOTE 15 19 SILAO DE LA VICTORIA GTO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (143, 1, N'2201', N'RAFAEL VICENTE VAZQUEZ                            ', N'MX ', N'C MARIA AMPARO VIDERIQUE DE SHEI 13 SAN BUENAVENTURA, PAPALOTLA XICOTEN TLX')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (144, 1, N'2208', N'PRODUCTOS GALVANIZADOS GAMA,        S.A. DE C.V.  ', N'MX ', N'CAMINO A SANTA MARTHA TEXCOCO MEX')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (145, 1, N'2209', N'GESTAMP TOLUCA SA DE CV                           ', N'MX ', N'AV INDEPENDENCIA SN TOLUCA MEX')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (146, 1, N'2211', N'GE TRANSPORTATION                                 ', N'US ', N'EAST LAKE ROAD 2901 ERIE PA')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (147, 1, N'2220', N'SWEDISH STEEL AB MEXICO S.A. DE C.V               ', N'MX ', N'BATALLON SAN PATRICIO 109 403 SAN PEDRO GARZA GARCIA NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (148, 1, N'2221', N'DAIMLER COMPRA Y MANUFACTURA MEXICO S DE RL DE CV ', N'MX ', N'AV PASEO DE LOS TAMARINDOS 9 PISO16 COL. BOSQUES DE LAS LOMAS CMX')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (149, 1, N'2229', N'ACEROS AUTOMOTRICES DEL BAJÍO       S.A. DE C.V.  ', N'MX ', N'CERRADA DE CALICANTO 5 TULTITLÁN MEX')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (150, 1, N'2231', N'DAIMLER AG                                        ', N'DE ', N'MERCEDESSTRASSE 137 STUTTGART 8')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (151, 1, N'2233', N'INMOBILIARIA ANCLEU S.A. DE C.V.                  ', N'MX ', N'5TA AVENIDA 345 COL. COLOMBRES, MONTERREY NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (152, 1, N'2250', N'GESTAMP SAN LUIS POTOSI,            S.A.P.I DE C.V', N'MX ', N'CIRCUITO SAN CRISTOBAL PONIENTE 109 SAN LUIS POTOSI SLP')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (153, 1, N'2254', N'GE INDIA INDUSTRIAL PVT LTD                       ', N'IN ', N'PLOT A78/1, MIDC CHAKAN INDUSTRIAL PUNE 13')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (154, 1, N'2261', N'SMS FOREMEX S DE RL DE CV                         ', N'MX ', N'PRADO NORTE 1SECC 577 LOMAS DE CHAPULTEPEC , MIGUEL HIDAL CMX')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (155, 1, N'2262', N'ACEROS ANCLEU SA DE CV                            ', N'MX ', N'ARAUCARIA 4856 LOS CEDROS, MONTERREY NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (156, 1, N'2265', N'THYSSENKRUPP STEEL SERVICES                       ', N'US ', N'ONE THYSSEN PARK DETROIT MI')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (157, 1, N'2266', N'THYSSENKRUPP MATERIALS TRADING NA                 ', N'US ', N'11 WEST MILE ROAD, SOUTFIELD MI')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (158, 1, N'2267', N'MAPFRE MEXICO SA DE CV                            ', N'MX ', N'AV REVOLUCION 507 SAN PEDRO DE LOS PINOS, BENITO JUAR CMX')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (159, 1, N'2268', N'COMERCIALIZADORA UNIVERSAL JACO     S.A. DE C.V.  ', N'MX ', N'LAGO ZIRAHUEN 102 SAN NICOLAS DE LOS GARZA, MONTERREY NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (160, 1, N'2269', N'BRANCH OF GE VIETNAM LTD                          ', N'VN ', N'LAND PLOTS H1 H6 F13A F13B ZULIA RRD')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (161, 1, N'2270', N'HAMMOND POWER SOLUTIONS SA DE CV                  ', N'MX ', N'AVANTE 810 PARQUE INDUSTRIAL GUADALUPE, GUADAL NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (162, 1, N'2271', N'CLAUDIA RODRIGUEZ GARCIA                          ', N'MX ', N'CAÑADA 2A NUEVO LEON, CUAUTLANCINGO PUE')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (163, 1, N'2272', N'SIDERAL REPRESENTACIONES Y          COMERCIALIZADO', N'MX ', N'A LINCOLN 277 MITRAS NORTE, MONTERREY NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (164, 1, N'2273', N'JUANA ROSALINDA OLVERA BARRIENTOS                 ', N'MX ', N'SANGRE DE CRISTO 119 FRACCEL COLONIAL, IRAPUATO GTO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (165, 1, N'2274', N'NICOMETAL BAJIO S.A. DE C.V.                      ', N'MX ', N'AVENIDA CELAYA 106 AMISTAD DEL BAJIO, APASEO EL GRANDE GTO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (166, 1, N'2275', N'ALFREDO NOGALES LOPEZ                             ', N'MX ', N'CALLE SANTA ANA 28 FRACC. SAN JOSE, SAN ANDRES CHOLULA PUE')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (167, 1, N'2276', N'ANTONIO MAZATAN DE LA PARRA                       ', N'MX ', N'AV. SENDA ETERNA 177 QUERETARO QRO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (168, 1, N'2277', N'THYSSENKRUPP MATERIALS SERVICES GMB               ', N'DE ', N'THYSSENKRUPP ALLEE 1 ESSEN 5')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (169, 1, N'2278', N'CARRIER MEXICO S.A. DE C.V.                       ', N'MX ', N'GALEANA OTE 469 SANTA CATARINA NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (170, 1, N'2283', N'POSCO INTERNATIONAL CORPORATION                   ', N'KR ', N'CONVENCIA DAERO YEOUNSU-GU 165 INCHEON, KOREA ')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (171, 1, N'2284', N'LOCOMOTIVE MANUFACTURING AND SERVIC S.A. DE C.V.  ', N'MX ', N'AV. ANTONIO DOVALI JAIME PISO 4 Y 5 ALVARO OBREGON, CIUDAD DE MEXICO CMX')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (172, 1, N'2285', N'VOESTALPINE AUTOMOTIVE COMPONENTS   AGUASCALIENTES', N'MX ', N'CARRETERA PANAMERICANA SUR DSP 107 AGUASCALIENTES, AGUASCALIENTES AGS')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (173, 1, N'2286', N'PLATING FINISHING AND COATING       S. DE R.L. DE ', N'MX ', N'CALLE ANTONIO DE RODA 195 A JOSE GALVEZ, SAN LUIS POTOSI SLP')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (174, 1, N'2287', N'SALZGITTER MANNESMAN INTERNATIONAL                ', N'DE ', N'SCHWANNSTRASSE 12 DUESSELDORF, GERMANY 4')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (175, 1, N'2288', N'ABB MEXICO S.A. DE C.V.                           ', N'MX ', N'AVENIDA CENTRAL 310 SAN LUIS POTOSI, SAN LUIS POTOSI SLP')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (176, 1, N'2289', N'SRG GLOBAL MEXICO S. DE R.L. DE C.V               ', N'MX ', N'AVENIDA RIO SAN LORENZO 1921 IRAPUATO, GUANAJUATO GTO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (177, 1, N'2291', N'NUTRES COMEDORES INDUSTRIALES       S. DE R.L. DE ', N'MX ', N'AV. ADOLFO LOPEZ MATEOS 38 A PUEBLA, PUEBLA PUE')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (178, 1, N'2292', N'EISEN NAVARRETE S.A. DE C.V.                      ', N'MX ', N'EXCELSIOR 790 SEGUNDO PISO TLALNEPANTLA DE BAZ, MEXICO MEX')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (179, 1, N'2296', N'RICARDO IGNACIO BOLLO                             ', N'MX ', N'KM 117 AUTOPISTA MEXICO SIN NUMERO CUAUTLANCINGO, PUEBLA PUE')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (180, 1, N'2297', N'LEONARDO PILOTZI MONTIEL                          ', N'MX ', N'HIDALGO 1 CENTRO CHIAUTEMPAN, TLAXCALA TLX')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (181, 1, N'2298', N'TOYOTA TSUSHO MEXICO S.A. DE C.V.                 ', N'MX ', N'CALLE SEPTIMA 300 SUITE 1020 APODACA, NUEVO LEON NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (182, 1, N'2299', N'BRAVO ENERGY MEXICO S. DE R.L. DE C               ', N'MX ', N'MESA DE LEON 107 QUERETARO, QUERETARO QRO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (183, 1, N'2300', N'T A 2000 S.A. DE C.V.                             ', N'MX ', N'CARR FEDERAL MEXICO VER KM 321 SN 2 VERACRUZ DE IGNACIO DE LLAVE VER')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (184, 1, N'2301', N'ACEROS YICAL S.A. DE C.V.                         ', N'MX ', N'CALLE POTASIO 302 SAN NICOLAS DE LOS GARZA, NUEVO LEO NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (185, 1, N'200036', N'FCA MEXICO, S.A. DE C.V.            (SALTILLO)    ', N'MX ', N'CARRETERA SALTILLO - ZACATECAS SALTILLO, COAHUILA DE ZARAGOZA COA')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (186, 1, N'200115', N'FCA MEXICO, S.A. DE C.V.            (TOLUCA)      ', N'MX ', N'CARRETERA MEXICO - TOLUCA KM. 60.5 TOLUCA, MEXICO MEX')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (187, 1, N'200789', N'GM DE MEXICO SILAO PLANT                          ', N'MX ', N'Carretera Silao - Guanajuato Km. 3. Silao GTO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (188, 1, N'200826', N'AVENTEC C/O LAGERMEX SILAO                        ', N'MX ', N'AUTOPISTA SILAO A IRAPUATO KM 5.5 GUANAJUATO SIN')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (189, 1, N'200827', N'ESTAMPADOS MAGNA S.A. DE C.V.                     ', N'MX ', N'AV. SANTA MARIA 1501 RAMOS ARIZPE COA')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (190, 1, N'200977', N'Industria Automotriz, S.A. de C.V.                ', N'MX ', N'Carretera Tlaxcala a Texcol Km. 1.1 Tlaxcala TLX')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (191, 1, N'200990', N'SHILOH DE MEXICO, S.A. DE C.V.                    ', N'MX ', N'Av. Shiloh s/n Ramos Arizpe COA')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (192, 1, N'200991', N'Nugar, S.A. de C.V.                               ', N'MX ', N'Av. 4 No. 12 Tultitlan, Planta Tultitlan I MEX')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (193, 1, N'200995', N'GM c/o VRK Automotive Systems, SA d               ', N'MX ', N'Av. La Noria No. 111 Queretaro QRO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (194, 1, N'200996', N'GM c/o Noble de Mexico                            ', N'MX ', N'Paseo de los Industriales Lote A 19 Silao GTO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (195, 1, N'201015', N'Benteler de Mexico S.A. de C.V.                   ', N'MX ', N'Prol.Av. Defensores de la Re No.999 Puebla PUE')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (196, 1, N'201020', N'AUTOTEK MEXICO, S.A. de C.V.        Cosma Internat', N'MX ', N'Prol. Calle No. 50 s/n Puebla PUE')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (197, 1, N'201058', N'Serviacero Plano de Queretaro                     ', N'MX ', N'Av. La Noria 129 Queretaro QRO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (198, 1, N'201090', N'Aceroprime, S. de R.L. de C.V.      Planta Ramos A', N'MX ', N'Av. Gamma # 527 Ramos Arizpe COA')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (199, 1, N'201127', N'CONSTRUCCION,PAILERIA Y DISEÑO      INDUSTRIAL S.A', N'MX ', N'CERRADA DE LA GUARDA 4-D PASEOS DEL ANGEL PUE')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (200, 1, N'201175', N'PANEL REY, S.A .                                  ', N'MX ', N'CARRETERA MONTERREY-MONCLOV KM. 115 MONTERREY NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (201, 1, N'201196', N'DOFASCO DE MEXICO S.A. DE C.V.                    ', N'MX ', N'CARRETERA MONTERREY-SALTILLO AP. 46 COL ARCO VIAL LIBRAMIENTO NORESTE NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (202, 1, N'201240', N'DE ACERO                                          ', N'MX ', N'AV MEXICALI 1189 ZONA IND. NORTE AN LATERAL AUTOPISTA PUEBLA ORIZABA PUE')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (203, 1, N'201242', N'GENERAL MOTORS MEXICO C/O LMS I.    DESTINATARIO  ', N'US ', N'6101 C/O LMS INTERNATIONAL GILBERT LAREDO, TX. TX')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (204, 1, N'201256', N'OGIHARA PROEZA MEXICO                             ', N'MX ', N'PROL. MADERO ORIENTE 4215 COL. FIERRO NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (205, 1, N'201268', N'GLEASON S.A. DE C.V.                              ', N'MX ', N'CALLE RIVA PALACIO 62 SAN LORENZO TLALNEPANTLA DF')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (206, 1, N'201292', N'Magna Formex Automotive Industries,               ', N'MX ', N'Blvd. Magna 2000 Parq. Ind. Santa M Ramos Arizpe COA')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (207, 1, N'201293', N'VRK AUTOMOTIVE SYSTEMS, S.A. DE C.V               ', N'MX ', N'AV. LA NORIA 111 PARQUE INDUSTRIAL QUERETARO QRO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (208, 1, N'201296', N'ESTAMPADOS MARTINREA, S. A. DE C.V.               ', N'MX ', N'AVENIDA INDUSTRIAL DE LA TRANS 3131 RAMOS ARIZPE COA')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (209, 1, N'201330', N'SKD DE MEXICO S. DE R.L. DE C.V.                  ', N'MX ', N'FILIBERTO GOMEZ CENTRO IND. TLA 104 TLANEPANTLA ESTADO DE MEXICO MEX')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (210, 1, N'201333', N'Internacional de Metales Comerciale  S.A. de C.V. ', N'MX ', N'OCCIDENTE 303 Col. Independencia; Monterrey NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (211, 1, N'201334', N'Francisco Chavez Treviño, S.A. de C               ', N'MX ', N'Bonifacio Salinas No. 206 Fracc Industrial las Ameritas,Guada NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (212, 1, N'201360', N'ACEROS PLANOS DE LEON, S.A. DE C.V.               ', N'MX ', N'BLVD. HERMANOS ALDAMA 4002 LEON GTO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (213, 1, N'201368', N'General Motors-San Luis Potosi                    ', N'MX ', N'Supercarretera 80 SLP-Villa deArria Villa de Reyes Laguna de San Vicent SLP')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (214, 1, N'201386', N'LAGERMEX, S.A . DE C.V.                           ', N'MX ', N'AVENIDA DE LOS INDUSTRIALES S/N SILAO GTO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (215, 1, N'201387', N'METALSA, S.A. DE C.V.                             ', N'MX ', N'AV INDUSTRIAS MANZANA 3 ZONA I 4410 SAN LUIS POTOSI SLP')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (216, 1, N'201396', N'WHIRLPOOL INTERNATIONAL S. DE R.L.  V.            ', N'MX ', N'CARRETERA MIGUEL ALEMAN KM. 16.13 APODACA, COL EL MILAGRO NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (217, 1, N'201398', N'GESTAMP MEXICO S.A. DE C.V                        ', N'MX ', N'AV JAPON PARQUE INDUSTRIAL 124 SAN FRANCISCO AGUASCALIENTES AGS')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (218, 1, N'201399', N'GESTAMP PUEBLA S.A. DE C.V.                       ', N'MX ', N'CALLE AUTOMOCION 8 PUEBLA PUE')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (219, 1, N'201411', N'Pintura Estampado y Montaje, SA de                ', N'MX ', N'Carretera Celaya a Salmanca Km. 5 Celaya GTO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (220, 1, N'201413', N'Metalsa, S.A. de C.V.                             ', N'MX ', N'Carretera Miguel Alemán KM16 100 Apodaca, Nuevo Leon NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (221, 1, N'201416', N'ArcelorMittal Indiana Harbor Inc.                 ', N'US ', N'12 250 W.U.S.Highway Burns Harbor IN 46304 IN')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (222, 1, N'201422', N'Ford Motor Company, S.A. de C.V.                  ', N'MX ', N'Km.36.5 Autopista México-Querétaro Zona Industrial Salitre MEX')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (223, 1, N'201427', N'ATLAS TOOL INC.                                   ', N'US ', N'29880 GROESBECK HWY ROSEVILLE, MICHIGAN MI')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (224, 1, N'201436', N'MARTINREA DEVELOPMENTS DE MEXICO                  ', N'MX ', N'FILIBERTO GOMEZ 104 CENTRO INDUSTRIAL TLALNEPANTLA MEX')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (225, 1, N'201438', N'INDUSTRIAS ACROS WHIRLPOOL,         S.A. DE C.V.  ', N'MX ', N'CARRETERA PANAMERICANA KM.280 CELAYA, GTO. GTO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (226, 1, N'201441', N'BAYMEX, S.A. DE C.V.                              ', N'MX ', N'HELIOS 310 PAR.IND.FINSA AEROPUERTO, APODACA NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (227, 1, N'201442', N'GRUPO COLLADO, S.A. DE C.V.                       ', N'MX ', N'AV. LOPEZ MATEOS 502 NORTE LAGRANGE SAN NICOLAS DE LOS GARZA NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (228, 1, N'201443', N'AUTOTEK MEXICO, S.A. DE C.V.                      ', N'MX ', N'CIRCUITO INTERIOR 135 COL. ZONA INDUSTRIAL DEL POTOSI SLP')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (229, 1, N'201445', N'Nacional Materials of Mexico, S. de de C.V.       ', N'MX ', N'Sexta Oriente 150 Apodaca NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (230, 1, N'201455', N'THYSSENKRUPP STEEL NORTH AMERICA IN               ', N'US ', N'48210 ONE THYSSEN PARK DETROIT MICHIGAN MI')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (231, 1, N'201463', N'BASILISK CINCO, S.A. DE C.V.                      ', N'MX ', N'AV. UNIVERSIDAD 34 COL. XICOHTENCATL TLX')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (232, 1, N'201469', N'ACERO PRIME, S. DE R. L. DE C.V.                  ', N'MX ', N'EJE 128 ZONA INDUSTRIAL DEL POT 209 SAN LUIS POTOSI SLP')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (233, 1, N'201470', N'POSCO MPPC S.A. DE C.V.                           ', N'MX ', N'Lote 4 , Calle Carrusel 1 Ponie 106 P.I. Logistik,Km.5.0,Villa de Reyes SLP')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (234, 1, N'201472', N'ACEROS SIDERURGICOS BRAVO           S.A. DE C.V.  ', N'MX ', N'CERRADA 3 DE PALOMARES 35 P. A. COL.MAGISTERIAL DF')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (235, 1, N'201478', N'COLLADO AUTOMOTRIZ                                ', N'MX ', N'ADOLPH B. HORN JR. No.2001 PARQUE INDUSTRIAL SAN JORGE JAL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (236, 1, N'201480', N'BAY MEX, S.A. DE C.V.                             ', N'MX ', N'AV. LUIS DONALDO COLOSIO 202 APODACA, NL, NUEVO LEON NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (237, 1, N'201485', N'AUTOTEK MEXICO, S.A. de C.V.        Cosma Internat', N'MX ', N'Prol. Calle No. 50 s/n Puebla PUE')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (238, 1, N'201486', N'FORMEX MEXICO S.A. DE C.V.                        ', N'MX ', N'BLVD. MAGNA 2000 RAMOS ARIZPE COA')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (239, 1, N'201487', N'Pintura Estampado y Montaje SAPI    de C.V.       ', N'MX ', N'Carretera Celaya Salmanca Km. 5 Celaya GTO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (240, 1, N'201488', N'Benteler de Mexico S.A. de C.V.                   ', N'MX ', N'Calle Acacias Nave 10 Parque Industrial Finsa PUE')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (241, 1, N'201501', N'THYSSENKRUPP TAILORED BLANKS SA CV                ', N'MX ', N'KM 117 AUT MEX PUE S/N CUAUTLANCINGO PUE')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (242, 1, N'201545', N'ROCKWELL AUTOMATION MONTERREY       MANUFACTURING,', N'MX ', N'CAMINO VECINAL PARQ IND FINSA 3051 GUADALUPE NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (243, 1, N'201552', N'Flex N'' Gate Mexico, S. de R.L.     de C.V.       ', N'MX ', N'Ave. Principal 1 San José Iturbide GTO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (244, 1, N'201553', N'MARTINREA DEVELOPMENTS DE MEXICO                  ', N'MX ', N'Industrias de la Transformacion 131 Ramos Arizpe COA')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (245, 1, N'201554', N'LUNKOMEX S.A. DE C.V.                             ', N'MX ', N'AV. RESURRECCION SUR 6 PUEBLA PUE')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (246, 1, N'201555', N'GESTAMP MEXICO S.A. DE C.V                        ', N'MX ', N'AV JAPON PARQUE INDUSTRIAL 124 SAN FRANCISCO AGUASCALIENTES AGS')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (247, 1, N'201565', N'AMTB SUMMIT, S. DE R.L. DE C.V.                   ', N'MX ', N'PASEO DE LOS INDUSTRIALES PONIE S/N PARQUE INDUSTRIAL FIPASI, SILAO GTO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (248, 1, N'201567', N'GM de México - Ramos Arizpe         Vehicle Assy  ', N'MX ', N'Carretera a Saltillo Monter Km. 7.5 Ramos Arizpe COA')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (249, 1, N'201572', N'GESTAMP TOLUCA S.A. DE C.V.                       ', N'MX ', N'PRIVADA INDEPENDENCIA S/N COL. ZONA INDUSTRIAL TOLUCA MEX')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (250, 1, N'201596', N'GESTAMP TOLUCA S.A. DE C.V.                       ', N'MX ', N'PRIVADA INDEPENDENCIA S/N COL. ZONA INDUSTRIAL TOLUCA MEX')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (251, 1, N'201601', N'NARMX QUERETARO S.A. DE C.V.                      ', N'MX ', N'CARRETERA QUERETARO SAN LUI KM 28.5 PARQUE INDUTRIAL QUERETARO QRO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (252, 1, N'201605', N'GRUPO COLLADO S.A. DE C.V.                        ', N'MX ', N'FILIBERTO GOMEZ 38 FRACC. INDUSTRIAL SAN NICOLAS MEX')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (253, 1, N'201649', N'MAQUILA ACERO AG S.A. DE C.V.                     ', N'MX ', N'Recursos Hidraúlicos 9 Letra A Los Reyes, Tultitlán MEX')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (254, 1, N'201650', N'Ford Motor Company, S.A. de C.V.    Hermosillo Stp', N'MX ', N'Carretera a la Colorada km 4.5 Hermosillo SON')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (255, 1, N'201653', N'COIL PLUS                                         ', N'MX ', N'AV. LAMBDA 1450 12-B RAMOS ARIZPE COA')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (256, 1, N'201656', N'ACEROPRIME, S. DE R.L. DE C.V.      PLANTA SAN LUI', N'MX ', N'Eje 128 209 ZONA INDUSTRIAL DEL POTOSI SLP')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (257, 1, N'201658', N'CMS TURBO MECANICA, S.A. DE C.V.    (LASSER & MANU', N'MX ', N'RIO QUERETARO 54-B COL. LA SIERRITA, QUERETARO QRO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (258, 1, N'201664', N'GE ELECTRICAL DISTRIBUTION EQUIPMEN               ', N'MX ', N'CAMINO OJO DE AGUA 203 APODACA NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (259, 1, N'201690', N'ARIN INC                                          ', N'US ', N'29139 CALAHAN ROSEVILLE MI')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (260, 1, N'201696', N'VRK AUTOMOTIVE SYSTEMS              PLANTA PUEBLA ', N'MX ', N'KM 117 Autopista Mex-Pue 200 Parque Ind FINSA II, Nave 26 Modulo PUE')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (261, 1, N'201712', N'MCI OROZCO                                        ', N'MX ', N'SIN NOMBRE LOTE 3 SAN MIGUELITO S/N JUAREZ NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (262, 1, N'201714', N'LAGERMEX SALTILLO                                 ', N'MX ', N'Km 17.5 Aut. Saltillo - Zacatecas Parque Ind Sta. Fe COA')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (263, 1, N'201722', N'SEGANA S.A . DE C.V.                              ', N'MX ', N'CANATLAN 240 GOMEZ PALACIO DGO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (264, 1, N'201723', N'INTEMEC                                           ', N'MX ', N'CARRETERA TLAXCALA-TEXOLOC KM 1.1 COL. LA LOMA XICOTENCATL TLX')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (265, 1, N'201733', N'ESTAMPADOS DEL BAJIO                              ', N'MX ', N'AV. LAS AGUILAS 533-A COL. SAN MIGUELITO GTO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (266, 1, N'201734', N'CELOMEX S.A. DE C.V.                              ', N'MX ', N'AV. SALAMANCA ESQ. CALLE DEL CA S/N COL. INDUSTRIAL GTO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (267, 1, N'201737', N'NISSAN MEXICANA, SA DE CV                         ', N'MX ', N'CAR. FEDERAL LAGOS DE MORENO KM.75 AGUASCALIENTES AGS')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (268, 1, N'201740', N'OPERAT MONTENEGRO                                 ', N'MX ', N'CIRCUITO DE LA PRODUCTIVIDAD No.201 MUNICIPIO DEL SALTO JALISCO JAL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (269, 1, N'201745', N'FORD Dearborn Tool & Die (MS07A)                  ', N'US ', N'3001 Miller Road Dearborn MI')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (270, 1, N'201752', N'FORMACERO                                         ', N'MX ', N'AV. DE LOS INSURGENTES No.2854 COL. GANADERA GTO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (271, 1, N'201754', N'NKP MEXICO, S.A. DE C.V.                          ', N'MX ', N'CALLE "A" No.239 INT.1 COL. PARQUE INDUSTRIAL EL SALTO JAL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (272, 1, N'201760', N'SAN LUIS METAL FORMING                            ', N'MX ', N'BLVD. FRANKFURT No.201 MANZANA 11 LAGUNA DE SAN VICENTE SLP')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (273, 1, N'201762', N'Volkswagen de México                S.A. de C.V.  ', N'MX ', N'Autopista México Puebla Km 116 S/N Av. Sn Lorenzo Almecatla PUE')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (274, 1, N'201765', N'GRUPO DE INGENIERIA CAPRICORNIO,    S.A. DE C.V.  ', N'MX ', N'CARR. LATERAL GUANAJUATO-SI No.2000 AGUAS BUENAS, SILAO GTO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (275, 1, N'201774', N'HONDA MEXICO SA DE CV                             ', N'MX ', N'CARRETERA A EL CASTILLO 7250 EL SALTO JAL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (276, 1, N'201778', N'GRUPO KASOKU INDUSTRIAL,            S.A. DE C.V.  ', N'MX ', N'LAUREL LOTE 1 MANZANA 4 2da. FRACC. DE CRESPO, CELAYA GTO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (277, 1, N'201851', N'ELIZARRARAZ AYALA FELIPE DE JESUS                 ', N'MX ', N'AV JACARANDAS 2134 IRAPUATO GTO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (278, 1, N'201855', N'IPAR MOLD, S.A . DE C.V.                          ', N'MX ', N'BLVD. TLC 57-B APODACA NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (279, 1, N'201863', N'SCHNEIDER ELECTRIC USA INC.                       ', N'MX ', N'BLVD. TLC S/N APODACA NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (280, 1, N'201865', N'SCHNEIDER ELECTRIC USA INC.                       ', N'MX ', N'BOULEVARD TLC 1000 APODACA NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (281, 1, N'201868', N'MAZDA MOTOR MANUFACTURING DE MEXICO S.A. DE C.V.  ', N'MX ', N'Av Hiroshima 1000 Complejo Industrial Salamanca GTO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (282, 1, N'201883', N'SCHNEIDER ELECTRIC                  PLANTA 4      ', N'MX ', N'BOULEVARD APODACA 100 APODACA NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (283, 1, N'201888', N'Y-TEC KEYLEX MEXICO SA DE CV                      ', N'MX ', N'AV. HIROSHIMA #1000 INT.5 SALAMANCA, GTO. GTO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (284, 1, N'201896', N'HONDA DE MEXICO, S.A. DE C.V.                     ', N'MX ', N'CARRETERA LIBRAMIENTO SUR KM.6 COL. LA LUZ, CELAYA, GTO. GTO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (285, 1, N'201899', N'TWB TAILORED BLANKS, S.A. DE C.V.                 ', N'MX ', N'AV. DE LOS INDUSTRIALES S/N PARQUE INDUSTRIAL FIPASI, SILAO GTO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (286, 1, N'201905', N'G-ONE AUTO PARTS DE MEXICO,         S.A. DE C.V.  ', N'MX ', N'AVENIDA AMISTAD No.104 PASEO EL GRANDE, GUANAJUATO GTO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (287, 1, N'201908', N'NISSAN MEXICANA, S.A. DE C.V.       (PLANTA AGUASC', N'MX ', N'CARRETERA FEDERAL KM.75 LAGOS DE MORENO, AGUASCALIENTES AGS')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (288, 1, N'201913', N'TWM DE MEXICO, S.A. DE C.V.                       ', N'MX ', N'AV. DE LOS INDUSTRIALES S/N PARQUE INDUSTRIAL FIPASI, SILAO GTO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (289, 1, N'201918', N'INDUSTRIAS ELECTRONICAS PACIFICO    S.A. DE C.V.  ', N'MX ', N'NAFTA No. 850 APODACA NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (290, 1, N'201919', N'INDUSTRIAS ELECTRONICAS PACIFICO,   S.A. DE C.V.  ', N'MX ', N'BLV. APODACA No. 900 APODACA, N.L. NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (291, 1, N'201926', N'SCHNEIDER ELECTRIC USA, INC.        HUNTINGTON    ', N'US ', N'6 COMERCIAL ROAD HUNTINGTON, INDIANA IN')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (292, 1, N'201932', N'MARTINREA SILAO                                   ', N'MX ', N'CALLE SAN ROQUE No.158 SILAO, GTO. GTO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (293, 1, N'201935', N'NKPM(NKP MEXICO, S.A. DE C.V.)                    ', N'MX ', N'AV. CELAYA No.104 APASEO EL GRANDE, GTO. GTO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (294, 1, N'201938', N'MANUFACTURAS ZAPALINAME,            S.A. DE C.V.  ', N'MX ', N'CARRETERA SALTILLO-ZACATECAS KM.4.5 LA ANGOSTURA COA')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (295, 1, N'201940', N'PRECISION STRIP                                   ', N'US ', N'518 WEST, 73rd STREET ANDERSON, IN IN')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (296, 1, N'201944', N'TWB DE MEXICO S.A. DE C.V.                        ', N'MX ', N'AV. DE LOS INDUSTRIALES S/N PARQUE INDUSTRIAL FIPASI, SILAO GTO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (297, 1, N'201948', N'INDUSTRIAS ELECTRONICAS PACIFICO,   S.A. DE C.V.  ', N'MX ', N'BLV.APODACA 100 PARQUE TECNOLOGICO, APODACA NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (298, 1, N'201970', N'ROCKWELL TECATE S.A. DE C.V.                      ', N'MX ', N'BLVD ENCINO No. 101 PARQUE INDUSTRIAL TECATE, TECATE BC BC')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (299, 1, N'201971', N'ROCKWELL AUTOMATION MONTERREY       MANUFACTURING ', N'MX ', N'CAMINO VECINAL No. 3051 PARQUE INDUSTRIAL FINSA AEROPUERTO NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (300, 1, N'201972', N'ROCKWELL AUTOMATION MONTERREY       MANUFACTURING ', N'MX ', N'CAMINO VECINAL No. 3052 PARQUE INDUSTRIAL FINSA AEROPUERTO NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (301, 1, N'201975', N'NO UTILIZAR                                       ', N'MX ', N'CARR. QUERETARO SAN LUIS PO KM 28.5 PARQUE INDUSTRIAL QUERETARO QRO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (302, 1, N'201976', N'Y-TEC KEYLEX MEXICO SA DE CV                      ', N'MX ', N'AV. HIROSHIMA #1000 INT.5 SALAMANCA, GTO. GTO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (303, 1, N'201977', N'PROVEPUERTAS SA DE CV                             ', N'MX ', N'JOSE LOPEZ PORTILLO No. 333 VALLE DEL CANADA GENERAL ESCOBEDO NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (304, 1, N'201979', N'USG MEXICO S.A. DE C.V.                           ', N'MX ', N'LOS ARCOS CUAUTLANCINGO S/N CUAUTLANCINGO PUEBLA PUE')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (305, 1, N'201980', N'USG  Mèxico, S.A. de C.V.                         ', N'MX ', N'Cart. Monclova Km.15, El Carmen Camino a la Laguna KM2.5 NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (306, 1, N'201984', N'NAVA PLANTA B                                     ', N'MX ', N'AV. SAN FRANCISCO No. 109 PARQUE INDUSTRIAL EXI APODACA NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (307, 1, N'201996', N'HAMMOND POWER SOLUTIONS, SA DE CV                 ', N'MX ', N'AVENIDA LAS TORRES 900 GUADALUPE, NL NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (308, 1, N'202004', N'AUDI MEXICO SA DE CV                              ', N'MX ', N'SITE OFFIC EG N110 KM 10 DE LA AUTOPISTA AUDI PUE')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (309, 1, N'202008', N'CREACERO SA DE CV                                 ', N'MX ', N'AVE. AGUILA AZTECA No 121 CIENAGA DE FLORES NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (310, 1, N'202015', N'BODEGA DE LAMINAS                                 ', N'MX ', N'CARR A COLOMBIA KM 10.5 S/N CENTRO, NUEVO LEON NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (311, 1, N'202020', N'METALES LOZANO SA DE CV                           ', N'MX ', N'SAN IGNACIO No. 226 SAN NICOLAS DE LOS GARZA, NL NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (312, 1, N'202027', N'FERRETERIA Y ACEROS 2000            S.A. DE C.V.  ', N'MX ', N'MEZQUITAL No. 3 COL. SAN PABLO QRO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (313, 1, N'202029', N'THYSSENKRUPP MATERIALS              DE MEXICO SA D', N'MX ', N'KM 117 AUTOPISTA MEXICO S/N CUAUTLANCINGO, PUEBLA PUE')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (314, 1, N'202035', N'GGM PUEBLA S.A. DE C.V.                           ', N'MX ', N'LATERAL ARCO NORTE KM 25 No. 4 FRANCISCO OCOTLAN, CORONANGO, PUE PUE')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (315, 1, N'202036', N'UNEMAQ S DE RL DE CV                              ', N'MX ', N'MARIO COLIN SANCHEZ LOTE 5B S/N MANZANA 24 A, ATLACOMULCO, MEXICO MEX')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (316, 1, N'202045', N'RACK PROCESSING DE MEXICO           S DE RL DE CV ', N'MX ', N'CALLE KIOTO LOTE 1 DE JANAMO, IRAPUATO, GTO. GTO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (317, 1, N'202080', N'GESTAMP PUEBLA II S.A. DE C.V.                    ', N'MX ', N'CARRETERA MEXICO-VERACRUZ No. 1062 Zona Industrial Camino a Manzanilla PUE')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (318, 1, N'202085', N'GE TRANSPORTES FERROVIARIOS                       ', N'BR ', N'AV. GENERAL DAVID SARNOFF 4600 CID. INDUSTRIAL CONTAGEM MG, BRASIL MG')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (319, 1, N'202097', N'POSCO MPPC, S.A. DE C.V.                          ', N'MX ', N'CARRETERA 190 KM 0.550 HUEJOTZINGO,PUEBLA ')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (320, 1, N'202113', N'ERIE DISTRIBUTION                                 ', N'US ', N'3601 MAIN SOUTH AVE. BUILDING 63 ERIE PA')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (321, 1, N'202115', N'GESTAMP SAN LUIS S.A.P.I DE C.V.                  ', N'MX ', N'CIRCUITO SAN CRISTOBAL PONIEN MZ. 2 COL. CD SATELITE, SAN LUIS POTOSI SLP')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (322, 1, N'202126', N'INDUSTRIAS ELECTRONICAS PACIFICO    SA DE CV      ', N'MX ', N'BOULEVARD ESCOBEDO 317 APODACA NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (323, 1, N'202135', N'VOLSKWAGEN AG                                     ', N'DE ', N'BERLINER RING 2 WOLFSBURG 3')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (324, 1, N'202136', N'SALZGITTER MANNESMANN STAHLSERVICE                ', N'DE ', N'EMIL-ROHRMANN-STR. 22 D NIEDERLASSUNG SCHWERTE 5')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (325, 1, N'202137', N'EL JAGUAR SA DE CV                                ', N'MX ', N'CALLE SAN MIGUEL DE ALLENDE 1502 CD. INDUSTRIAL GTO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (326, 1, N'202139', N'COOPERATION MANUFACTURING PLANT     AGUASCALIENTES', N'MX ', N'CARRETERA ESTATAL 2 AEROPUER KM 1.1 PEÑUELAS AGS')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (327, 1, N'202141', N'SCHNEIDER ELECTRIC USA, INC                       ', N'US ', N'1124 FRANKLIN STREET HUNINGTON, IN WV')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (328, 1, N'202150', N'TWB de Mexico, S.A. de C.V.                       ', N'MX ', N'KM 117 AUTOPISTA MEXICO PUEBLA SN PARQUE INDUSTRIAL BRALEMEX, CUAUTLA PUE')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (329, 1, N'202151', N'HIROTEC CORPORATION                               ', N'JP ', N'ISHIUCHIMINAMI5 2 1 SAEKIKU 34')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (330, 1, N'202152', N'CLAUDIA RODRIGUEZ GARCIA                          ', N'MX ', N'CAÑADA 2A NUEVO LEON, CUAUTLANCINGO PUE')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (331, 1, N'202153', N'HIROTEC MEXICO S.A. DE C.V.                       ', N'MX ', N'CARRETERA PANAMERICANA 5.5 2 A LC SILAO DE LA VICTORIA GTO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (332, 1, N'202154', N'MISA-NATIONAL METAL PROCESSING      S.A. DE C.V.  ', N'MX ', N'AV. MONTES URALES 11 PARQUE INDUSTRIAL OPCION GTO')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (333, 1, N'202156', N'NAVISTAR MEXICO S. DE R.L. DE C.V.                ', N'MX ', N'EJERCITO NACIONAL 904 PISO 8 MIGUEL HIDALGO, CIUDAD DE MEXICO CMX')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (334, 1, N'202157', N'SCHNEIDER ELECTRIC MEXICO,          S.A. DE C.V.  ', N'MX ', N'AV. MICH N20 IZTAPALAPA, CIUDAD DE MEXICO CMX')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (335, 1, N'202158', N'NAVISTAR MEXICO S. DE R.L. DE C.V.                ', N'MX ', N'CARRETERA MONTERREY MONCLOVA 5000 ESCOBEDO, NUEVO LEON NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (336, 1, N'202159', N'GE TRANSPORTATION                                 ', N'MX ', N'BVLD. TLC 2000 APODACA, NUEVO LEON NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (337, 1, N'202160', N'HAMMOND POWER SOLUTIONS, S.A. DE C.               ', N'MX ', N'AVENIDA AVANTE 810 GUADALUPE, NUEVO LEON NL')
GO
INSERT [dbo].[clientes] ([clave], [activo], [claveSAP], [descripcion], [pais], [direccion]) VALUES (338, 1, N'202161', N'CMMG INDUSTRIAS DE PRECISION        S.A. DE C.V.  ', N'MX ', N'WHASHINGTON 3701 3 CHIHUAHUA, CHIHUAHUA CHI')
GO

SET IDENTITY_INSERT [dbo].[clientes] OFF
GO

	  
IF object_id(N'clientes',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< clientes en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla clientes  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
