--USE Portal_2_0
GO
IF object_id(N'SCDM_cat_ihs',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[SCDM_cat_ihs]
      PRINT '<<< SCDM_cat_ihs en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los SCDM_cat_ihs
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[SCDM_cat_ihs](
	[id] [int] IDENTITY(1,1) NOT NULL,	
	[descripcion] [varchar](250) NOT NULL,
	[activo] [bit] NOT NULL,
 CONSTRAINT [PK_SCDM_cat_ihs_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

--Inserta valores

SET IDENTITY_INSERT [dbo].[SCDM_cat_ihs] ON 

GO

 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (1, 1, N'155021084_BMW:BMW:2-Series:G42 [0-3.5 PC]{San Luis Potosi}2021-08') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (2, 1, N'255771084_BMW:BMW:2-Series:G42(2) [0-3.5 PC]{San Luis Potosi}2028-08') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (3, 1, N'51591084_BMW:BMW:3-Series:G20 [0-3.5 PC]{San Luis Potosi}2019-04') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (4, 1, N'255521084_BMW:BMW:3-Series:G50 [0-3.5 PC]{San Luis Potosi}2025-11') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (5, 1, N'91591146_Daimler:Mercedes-Benz:GLA:H248 [0-3.5 PC]{Aguascalientes}2025-01') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (6, 1, N'120741146_Daimler:Mercedes-Benz:GLB:X247 [0-3.5 PC]{Aguascalientes}2019-07') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (7, 1, N'241261146_Daimler:Mercedes-Benz:GLB:X248 [0-3.5 PC]{Aguascalientes}2027-01') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (8, 1, N'141270325_Ford:Ford:Bronco Sport:CX430 [0-3.5 PC]{Hermosillo}2020-10') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (9, 1, N'141280325_Ford:Ford:Bronco Sport:CX735 [0-3.5 PC]{Hermosillo}2026-10') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (10, 1, N'223220203_Ford:Ford:D-CUV EV:CDX746 [0-3.5 PC]{Cuautitlan}2023-04') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (11, 1, N'19850325_Ford:Ford:Fusion:CD391 [0-3.5 PC]{Hermosillo}2012-08') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (12, 1, N'220760325_Ford:Ford:Maverick:P758 [0-3.5 LCV]{Hermosillo}2021-07') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (13, 1, N'219670325_Ford:Ford:Maverick:P758(2) [0-3.5 LCV]{Hermosillo}2028-07') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (14, 1, N'212210203_Ford:Ford:Mustang Mach-E:CX727 [0-3.5 PC]{Cuautitlan}2020-10') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (15, 1, N'223250203_Ford:Lincoln:D-CUV EV:CDX747 [0-3.5 PC]{Cuautitlan}2023-04') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (16, 1, N'23730325_Ford:Lincoln:MKZ:CD533 [0-3.5 PC]{Hermosillo}2012-10') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (17, 1, N'218970621_General Motors:Chevrolet:Blazer:C1UC [0-3.5 PC]{Ramos Arizpe #2 (Factory Zero 3)}2018-11') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (18, 1, N'254170621_General Motors:Chevrolet:C-CUV EV:C223 [0-3.5 PC]{Ramos Arizpe #2 (Factory Zero 3)}2023-06') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (19, 1, N'239890621_General Motors:Chevrolet:D-CUV EV:C234 [0-3.5 PC]{Ramos Arizpe #2 (Factory Zero 3)}2023-06') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (20, 1, N'29450661_General Motors:Chevrolet:Equinox:D2UC [0-3.5 PC]{San Luis Potosi}2017-04') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (21, 1, N'29450621_General Motors:Chevrolet:Equinox:D2UC [0-3.5 PC]{Ramos Arizpe #2 (Factory Zero 3)}2017-05') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (22, 1, N'136190621_General Motors:Chevrolet:Equinox:D2UC-2 [0-3.5 PC]{Ramos Arizpe #2 (Factory Zero 3)}2024-05') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (23, 1, N'136190661_General Motors:Chevrolet:Equinox:D2UC-2 [0-3.5 PC]{San Luis Potosi}2024-11') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (24, 1, N'175680661_General Motors:Chevrolet:Onix:JBSC [0-3.5 PC]{San Luis Potosi}2019-12') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (25, 1, N'175690661_General Motors:Chevrolet:Onix:JBSC(2) [0-3.5 PC]{San Luis Potosi}2027-01') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (26, 1, N'111620723_General Motors:Chevrolet:Silverado:T1XCF [0-3.5 LCV]{Silao}2019-01') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (27, 1, N'111720723_General Motors:Chevrolet:Silverado:T1XCF-2 [0-3.5 LCV]{Silao}2026-01') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (28, 1, N'580661_General Motors:Chevrolet:Trax:G1UC [0-3.5 PC]{San Luis Potosi}2012-09') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (29, 1, N'111640723_General Motors:GMC:Sierra:T1XGF [0-3.5 LCV]{Silao}2019-01') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (30, 1, N'111740723_General Motors:GMC:Sierra:T1XGF-2 [0-3.5 LCV]{Silao}2026-01') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (31, 1, N'17460661_General Motors:GMC:Terrain:D2UG [0-3.5 PC]{San Luis Potosi}2017-06') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (32, 1, N'25540661_General Motors:GMC:Terrain:D2UG-2 [0-3.5 PC]{San Luis Potosi}2025-02') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (33, 1, N'143590621_General Motors:Holden:Equinox:D2UH [0-3.5 PC]{Ramos Arizpe #2 (Factory Zero 3)}2017-09') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (34, 1, N'252990621_General Motors:Honda:C-CUV EV:R232 [0-3.5 PC]{Ramos Arizpe #2 (Factory Zero 3)}2026-01') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (35, 1, N'253030621_General Motors:Honda:D-CUV EV:R233 [0-3.5 PC]{Ramos Arizpe #2 (Factory Zero 3)}2023-12') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (36, 1, N'79660980_Honda:Honda:Fit:2WF [0-3.5 PC]{Celaya}2014-01') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (37, 1, N'115040980_Honda:Honda:HR-V:2XP [0-3.5 PC]{Celaya}2015-01') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (38, 1, N'236020980_Honda:Honda:HR-V:2ZY [0-3.5 PC]{Celaya}2022-04') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (39, 1, N'236030980_Honda:Honda:HR-V:2ZY(2) [0-3.5 PC]{Celaya}2028-04') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (40, 1, N'244051116_Hyundai:Hyundai:Accent:BN7 [0-3.5 PC]{Monterrey}2023-07') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (41, 1, N'190361116_Hyundai:Hyundai:Accent:HC [0-3.5 PC]{Monterrey}2017-07') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (42, 1, N'133131116_Hyundai:Kia:Forte:BD [0-3.5 PC]{Monterrey}2018-07') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (43, 1, N'133141116_Hyundai:Kia:Forte:BD(2) [0-3.5 PC]{Monterrey}2023-07') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (44, 1, N'133151116_Hyundai:Kia:Forte:BD(3) [0-3.5 PC]{Monterrey}2028-07') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (45, 1, N'243991116_Hyundai:Kia:Rio:BL7 [0-3.5 PC]{Monterrey}2022-07') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (46, 1, N'244001116_Hyundai:Kia:Rio:BL7(2) [0-3.5 PC]{Monterrey}2028-01') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (47, 1, N'133101116_Hyundai:Kia:Rio:SC [0-3.5 PC]{Monterrey}2017-01') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (48, 1, N'242521357_Jianghuai:JAC:Frison T6:T6 [0-3.5 LCV]{Sahagun}2018-12') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (49, 1, N'243231357_Jianghuai:JAC:Frison T6:T6(2) [0-3.5 LCV]{Sahagun}2026-01') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (50, 1, N'242531357_Jianghuai:JAC:Frison T8:P622 [0-3.5 LCV]{Sahagun}2019-10') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (51, 1, N'243361357_Jianghuai:JAC:Frison T8:P622(2) [3.5-6 LCV]{Sahagun}2024-07') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (52, 1, N'242691357_Jianghuai:JAC:J4:A301 [0-3.5 PC]{Sahagun}2017-11') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (53, 1, N'243401357_Jianghuai:JAC:J7:A432 [0-3.5 PC]{Sahagun}2020-11') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (54, 1, N'243411357_Jianghuai:JAC:J7:A432(2) [0-3.5 PC]{Sahagun}2025-11') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (55, 1, N'242491357_Jianghuai:JAC:Sei 2:S201 [0-3.5 PC]{Sahagun}2017-04') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (56, 1, N'243301357_Jianghuai:JAC:Sei 2:S201(2) [0-3.5 PC]{Sahagun}2021-10') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (57, 1, N'243311357_Jianghuai:JAC:Sei 2:S201(3) [0-3.5 PC]{Sahagun}2027-10') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (58, 1, N'242501357_Jianghuai:JAC:Sei 3:S301 [0-3.5 PC]{Sahagun}2017-04') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (59, 1, N'242511357_Jianghuai:JAC:Sei 3:S301(2) [0-3.5 PC]{Sahagun}2021-06') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (60, 1, N'243321357_Jianghuai:JAC:Sei 3:S301(3) [0-3.5 PC]{Sahagun}2026-04') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (61, 1, N'242541357_Jianghuai:JAC:Sei 4:S4 [0-3.5 PC]{Sahagun}2019-10') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (62, 1, N'243381357_Jianghuai:JAC:Sei 4:X4(2) [0-3.5 PC]{Sahagun}2025-01') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (63, 1, N'256051357_Jianghuai:JAC:Sei 7 Pro:X7 [0-3.5 PC]{Sahagun}2020-11') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (64, 1, N'243341357_Jianghuai:JAC:Sei 7 Pro:X7(2) [0-3.5 PC]{Sahagun}2024-04') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (65, 1, N'242551357_Jianghuai:JAC:Sei 7:S7 [0-3.5 PC]{Sahagun}2018-09') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (66, 1, N'249871357_Jianghuai:JAC:X200:HFC6600/HFC6800(2) [3.5-6 LCV]{Sahagun}2019-12') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (67, 1, N'249881357_Jianghuai:JAC:X200:HFC6600/HFC6800(3) [3.5-6 LCV]{Sahagun}2023-08') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (68, 1, N'116430481_Mazda:Mazda:2:J03W [0-3.5 PC]{Salamanca}2014-10') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (69, 1, N'150650481_Mazda:Mazda:3:J59C [0-3.5 PC]{Salamanca}2019-01') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (70, 1, N'173100481_Mazda:Mazda:3:J59C(2) [0-3.5 PC]{Salamanca}2024-01') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (71, 1, N'228150481_Mazda:Mazda:CX-30:J59K [0-3.5 PC]{Salamanca}2019-09') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (72, 1, N'239920481_Mazda:Mazda:CX-30:J59K(2) [0-3.5 PC]{Salamanca}2025-09') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (73, 1, N'116400481_Mazda:Toyota:Yaris:J03G [0-3.5 PC]{Salamanca}2015-06') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (74, 1, N'151431146_Renault-Nissan-Mitsubishi:Infiniti:QX50:P71A [0-3.5 PC]{Aguascalientes}2017-11') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (75, 1, N'151441146_Renault-Nissan-Mitsubishi:Infiniti:QX50:P71A(2) [0-3.5 PC]{Aguascalientes}2023-11') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (76, 1, N'217831146_Renault-Nissan-Mitsubishi:Infiniti:QX55:N71A [0-3.5 PC]{Aguascalientes}2021-02') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (77, 1, N'218051146_Renault-Nissan-Mitsubishi:Infiniti:QX55:N71A(2) [0-3.5 PC]{Aguascalientes}2027-02') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (78, 1, N'10330206_Renault-Nissan-Mitsubishi:Nissan:Frontier:H60A [0-3.5 LCV]{Cuernavaca #2}2014-12') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (79, 1, N'148660206_Renault-Nissan-Mitsubishi:Nissan:Frontier:H70X [0-3.5 LCV]{Cuernavaca #2}2024-04') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (80, 1, N'172380005_Renault-Nissan-Mitsubishi:Nissan:Kicks:P02F [0-3.5 PC]{Aguascalientes (A1)}2016-05') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (81, 1, N'99910005_Renault-Nissan-Mitsubishi:Nissan:Kicks:P13C [0-3.5 PC]{Aguascalientes (A1)}2023-08') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (82, 1, N'31410005_Renault-Nissan-Mitsubishi:Nissan:Micra:B02A [0-3.5 PC]{Aguascalientes (A1)}2011-02') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (83, 1, N'76940205_Renault-Nissan-Mitsubishi:Nissan:NV200:X11M [0-3.5 LCV]{Cuernavaca #1}2013-01') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (84, 1, N'132011032_Renault-Nissan-Mitsubishi:Nissan:Sentra:L21B [0-3.5 PC]{Aguascalientes (A2)}2019-11') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (85, 1, N'132021032_Renault-Nissan-Mitsubishi:Nissan:Sentra:L21B(2) [0-3.5 PC]{Aguascalientes (A2)}2025-11') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (86, 1, N'7190205_Renault-Nissan-Mitsubishi:Nissan:Versa:L02B [0-3.5 PC]{Cuernavaca #1}2012-09') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (87, 1, N'60930205_Renault-Nissan-Mitsubishi:Nissan:Versa:L02D [0-3.5 PC]{Cuernavaca #1}2019-10') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (88, 1, N'60930005_Renault-Nissan-Mitsubishi:Nissan:Versa:L02D [0-3.5 PC]{Aguascalientes (A1)}2019-06') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (89, 1, N'100050205_Renault-Nissan-Mitsubishi:Nissan:Versa:L02D(2) [0-3.5 PC]{Cuernavaca #1}2026-02') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (90, 1, N'100050005_Renault-Nissan-Mitsubishi:Nissan:Versa:L02D(2) [0-3.5 PC]{Aguascalientes (A1)}2026-02') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (91, 1, N'128300206_Renault-Nissan-Mitsubishi:Renault:Alaskan:U60A [0-3.5 LCV]{Cuernavaca #2}2016-08') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (92, 1, N'42250828_Stellantis:Dodge:Journey:JC49 [0-3.5 PC]{Toluca}2008-01') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (93, 1, N'251400828_Stellantis:Jeep:Compass:551(2) [0-3.5 PC]{Toluca}2024-01') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (94, 1, N'107960828_Stellantis:Jeep:Compass:MP [0-3.5 PC]{Toluca}2017-01') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (95, 1, N'57130649_Stellantis:Ram:1500:DS [0-3.5 LCV]{Saltillo Truck}2009-09') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (96, 1, N'138110649_Stellantis:Ram:2500/3500:DJ [3.5-6 LCV]{Saltillo Truck}2009-09') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (97, 1, N'151660649_Stellantis:Ram:2500/3500:DK [3.5-6 LCV]{Saltillo Truck}2024-11') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (98, 1, N'229990649_Stellantis:Ram:Compact Pickup:MT [0-3.5 LCV]{Saltillo Truck}2023-07') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (99, 1, N'85781081_Stellantis:Ram:ProMaster:VF [3.5-6 LCV]{Saltillo Van}2013-07') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (100, 1, N'219691081_Stellantis:Ram:ProMaster:VF(2) [3.5-6 LCV]{Saltillo Van}2024-07') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (101, 1, N'237181085_Toyota:Toyota:4Runner:610L(2) [0-3.5 PC]{Guanajuato}2024-01') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (102, 1, N'238080820_Toyota:Toyota:Tacoma:920B [0-3.5 LCV]{Tijuana}2023-12') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (103, 1, N'238081085_Toyota:Toyota:Tacoma:920B [0-3.5 LCV]{Guanajuato}2023-12') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (104, 1, N'63400820_Toyota:Toyota:Tacoma:989A [0-3.5 LCV]{Tijuana}2015-08') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (105, 1, N'63401085_Toyota:Toyota:Tacoma:989A [0-3.5 LCV]{Guanajuato}2019-11') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (106, 1, N'220681046_Volkswagen:Audi:Q5 Sportback:AU426/1 [0-3.5 PC]{San Jose Chiapa}2021-01') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (107, 1, N'220691046_Volkswagen:Audi:Q5 Sportback:AU436/1 [0-3.5 PC]{San Jose Chiapa}2024-07') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (108, 1, N'60081046_Volkswagen:Audi:Q5:AU426 [0-3.5 PC]{San Jose Chiapa}2016-09') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (109, 1, N'55121046_Volkswagen:Audi:Q5:AU436 [0-3.5 PC]{San Jose Chiapa}2023-07') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (110, 1, N'45520602_Volkswagen:Volkswagen:Golf:VW370 [0-3.5 PC]{Puebla #1}2014-01') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (111, 1, N'19310602_Volkswagen:Volkswagen:Jetta:VW371 [0-3.5 PC]{Puebla #1}2017-12') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (112, 1, N'13880602_Volkswagen:Volkswagen:Jetta:VW381 [0-3.5 PC]{Puebla #1}2025-01') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (113, 1, N'148700602_Volkswagen:Volkswagen:Taos:VW316/5 [0-3.5 PC]{Puebla #1}2020-10') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (114, 1, N'230910602_Volkswagen:Volkswagen:Taos:VW326/5 [0-3.5 PC]{Puebla #1}2026-10') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (115, 1, N'63710602_Volkswagen:Volkswagen:Tiguan:VW326 [0-3.5 PC]{Puebla #1}2017-03') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (116, 1, N'43660602_Volkswagen:Volkswagen:Tiguan:VW336 [0-3.5 PC]{Puebla #1}2024-06') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (117, 1, N'Camiones') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (118, 1, N'Otros') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (119, 1, N'Refacciones_Daimler') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (120, 1, N'Refacciones_FCA') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (121, 1, N'Refacciones_Ford') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (122, 1, N'Refacciones_GM') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (123, 1, N'Refacciones_Honda') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (124, 1, N'Refacciones_Mazda') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (125, 1, N'Refacciones_Nissan') 
 
 INSERT [dbo].[SCDM_cat_ihs] ([id], [activo], [descripcion]) VALUES (126, 1, N'Refacciones_VW') 
 


GO

SET IDENTITY_INSERT [dbo].[SCDM_cat_ihs] OFF
	  
IF object_id(N'SCDM_cat_ihs',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< SCDM_cat_ihs en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla SCDM_cat_ihs  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
