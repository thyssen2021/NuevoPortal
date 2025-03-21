--====== INSERT CTZ_Clients  =====

/* copia de la BD de SCDM */
--select * from clientes
--select * from CTZ_Clients
--DELETE FROM dbo.CTZ_Clients;
--DBCC CHECKIDENT ('dbo.CTZ_Clients', RESEED, 0);

INSERT INTO dbo.CTZ_Clients (Client_Name, Clave_SAP, Pais, Direccion, Ciudad, Codigo_Postal, Calle, Estado, Automotriz, Active)
SELECT    
    descripcion,                -- Mapeo de descripcion a Client_Name
    claveSAP,                   -- Mapeo de claveSAP a Clave_SAP
    pais,                       -- Mapeo de pais a Pais
    direccion,                  -- Mapeo de direccion a Direccion
    ciudad,                     -- Mapeo de ciudad a Ciudad
    codigo_postal,              -- Mapeo de codigo_postal a Codigo_Postal
    calle,                      -- Mapeo de calle a Calle
    estado,                     -- Mapeo de estado a Estado
    automotriz,                 -- Mapeo de automotriz a Automotriz
    activo                      -- Mapeo de activo a Active
FROM dbo.clientes
WHERE CAST(claveSAP AS INT) IN (
    1956, 1997, 1280, 2410, 2200, 1728, 2125, 954, 946, 1561,
	2400, 2421, 2370, 2365, 2420, 2395, 2195, 1200, 1494, 1123,
	1293, 2021, 989, 2010, 2209, 1408, 2415, 1374, 2063, 1413,
	1953, 980, 2356, 2416, 2407, 2397, 2312, 1339, 2413, 2284,
	2350, 2342, 1546, 2343, 2221, 2274, 2386, 2407, 2313, 2328,
	2189, 1430, 1189, 1472, 2406, 2129, 2027, 2028, 2345, 1959, 
	1744, 1970, 1296, 2325, 1801, 1283, 1595, 2055, 2026, 1421, 
	2330, 2266, 2265, 1522, 945, 2327
);


--====== INSERT OEMClients  =====

--DELETE FROM dbo.CTZ_OEMClients;  -- Elimina todos los registros
--DBCC CHECKIDENT ('dbo.CTZ_OEMClients', RESEED, 0); -- Reinicia el índice IDENTITY a 1

INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('BMW', 1,1)
INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('BYD', 1,1)
INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('Chery', 1,1)
INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('COMPAS', 1,1)
INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('Ford', 1,1)
INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('Geely', 1,1)
INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('General Motors', 1,1)
INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('Giant Motors', 1,1)
INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('Honda', 1,1)
INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('Hyundai', 1,1)
INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('Lucid Motors', 1,1)
INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('Mazda', 1,1)
INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('Mazda Toyota', 1,1)
INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('Mercedes-Benz', 1,1)
INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('Multimatic', 1,1)
INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('Navistar', 1,1)
INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('Nissan', 1,1)
INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('Oshkosh', 1,1)
INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('Otros', 1,1)
INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('Rivian', 1,1)
INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('SIA', 1,1)
INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('Stellantis', 1,1)
INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('Tesla', 1,1)
INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('Toyota', 1,1)
INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('Volkswagen', 1,1)
INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('Zoox', 1,1)

--select * from [CTZ_OEMClients]

--====== INSERT [CTZ_plants]  =====
--DELETE FROM dbo.CTZ_plants;  -- Elimina todos los registros
--DBCC CHECKIDENT ('dbo.CTZ_plants', RESEED, 0); -- Reinicia el índice IDENTITY a 1

INSERT INTO [dbo].[CTZ_plants]([Description],[Codigo_SAP],[Active])VALUES('Puebla', '5190',1)
INSERT INTO [dbo].[CTZ_plants]([Description],[Codigo_SAP],[Active])VALUES('Silao', '5390',1)
INSERT INTO [dbo].[CTZ_plants]([Description],[Codigo_SAP],[Active])VALUES('Saltillo', '5490',1)
INSERT INTO [dbo].[CTZ_plants]([Description],[Codigo_SAP],[Active])VALUES('San Luis Potosí', '5890',1)

--select * from CTZ_plants

--====== INSERT [CTZ_Project_Status] =====

INSERT INTO [dbo].[CTZ_Project_Status]([Status_Percent],[Description])VALUES('25','Quotes')
INSERT INTO [dbo].[CTZ_Project_Status]([Status_Percent],[Description])VALUES('50','Carry Over')
INSERT INTO [dbo].[CTZ_Project_Status]([Status_Percent],[Description])VALUES('75','Casi Casi')
INSERT INTO [dbo].[CTZ_Project_Status]([Status_Percent],[Description])VALUES('100','POH')

--select * from [CTZ_Project_Status]

--====== INSERT [CTZ_Material_Owner] =====

INSERT INTO [dbo].[CTZ_Material_Owner]([Owner_Key],[Description],[Active])VALUES('OWN', 'Propiedad tkMM',1)
INSERT INTO [dbo].[CTZ_Material_Owner]([Owner_Key],[Description],[Active])VALUES('CM', 'Propiedad del Cliente',1)

--Select * from CTZ_Material_Owner

--======= INSERT CTZ_Temp_IHS =========

DELETE FROM dbo.CTZ_Temp_IHS;  -- Elimina todos los registros
DBCC CHECKIDENT ('dbo.CTZ_Temp_IHS', RESEED, 0); -- Reinicia el índice IDENTITY a 1

INSERT INTO [dbo].[CTZ_Temp_IHS] 
    (Vehicle, Program, SOP, EOP, Max_Production, Mnemonic_Vehicle_plant, Production_Plant)
SELECT 
    i.vehicle,
    i.program,
    CAST(i.sop_start_of_production AS DATE) AS SOP,
    CAST(i.eop_end_of_production AS DATE) AS EOP,
    d.MaxProduction,
    i.mnemonic_vehicle_plant,
    i.production_plant
FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_IHS_item] i
INNER JOIN [Portal_2_0_budget_desarrollo].[dbo].[BG_IHS_versiones] v 
    ON i.id_ihs_version = v.id
CROSS APPLY (
    SELECT TOP 1 
           SUM(r.cantidad) AS MaxProduction
    FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_IHS_rel_demanda] r
    WHERE r.id_ihs_item = i.id
      AND r.tipo = 'ORIGINAL'
    GROUP BY YEAR(r.fecha)
    ORDER BY SUM(r.cantidad) DESC
) d
WHERE 
    v.periodo = (SELECT MAX(periodo) FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_IHS_versiones])
    AND i.origen = 'IHS';


	select * from CTZ_Temp_IHS

--======== CTZ CTZ_Temp_IHS_Production =========

-- Limpia la tabla y reinicia el contador de identidad
DELETE FROM dbo.CTZ_Temp_IHS_Production;
DBCC CHECKIDENT ('dbo.CTZ_Temp_IHS_Production', RESEED, 0);

-- Inserta los datos agrupados por ID_IHS, año y mes
INSERT INTO dbo.CTZ_Temp_IHS_Production (ID_IHS, Production_Year, Production_Month, Production_Sum)
SELECT 
    t.ID_IHS,
    YEAR(r.fecha) AS Production_Year,
    MONTH(r.fecha) AS Production_Month,  -- Obtiene el número de mes (1 a 12)
    SUM(r.cantidad) AS Production_Amount
FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_IHS_rel_demanda] r
INNER JOIN [Portal_2_0_budget_desarrollo].[dbo].[BG_IHS_item] i 
    ON r.id_ihs_item = i.id
INNER JOIN [Portal_2_0_budget_desarrollo].[dbo].[BG_IHS_versiones] v
    ON i.id_ihs_version = v.id
INNER JOIN dbo.CTZ_Temp_IHS t
    ON i.vehicle = t.Vehicle  -- Se asume que 'Vehicle' es único en CTZ_Temp_IHS
WHERE 
    v.periodo = (SELECT MAX(periodo) FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_IHS_versiones])
    AND i.origen = 'IHS'
    AND r.tipo = 'ORIGINAL'
GROUP BY t.ID_IHS, YEAR(r.fecha), MONTH(r.fecha);


select * from CTZ_Temp_IHS  where ID_IHS = 1
select * from CTZ_Temp_IHS_Production where ID_IHS = 1

--======== CTZ Route =========

select * from CTZ_Route
select * from CTZ_Material_Owner

INSERT INTO [dbo].[CTZ_Route]([ID_Material_Owner],[Route_Name],[Active])VALUES(1,'BLK', 1)
INSERT INTO [dbo].[CTZ_Route]([ID_Material_Owner],[Route_Name],[Active])VALUES(1,'BLK + RP', 1)
INSERT INTO [dbo].[CTZ_Route]([ID_Material_Owner],[Route_Name],[Active])VALUES(1,'BLK + RPLTZ', 1)
INSERT INTO [dbo].[CTZ_Route]([ID_Material_Owner],[Route_Name],[Active])VALUES(1,'BLK + SH', 1)
INSERT INTO [dbo].[CTZ_Route]([ID_Material_Owner],[Route_Name],[Active])VALUES(1,'BLK + WLD', 1)
INSERT INTO [dbo].[CTZ_Route]([ID_Material_Owner],[Route_Name],[Active])VALUES(1,'COIL TO COIL', 1)
INSERT INTO [dbo].[CTZ_Route]([ID_Material_Owner],[Route_Name],[Active])VALUES(1,'REWINDED', 1)
INSERT INTO [dbo].[CTZ_Route]([ID_Material_Owner],[Route_Name],[Active])VALUES(1,'SLT', 1)
INSERT INTO [dbo].[CTZ_Route]([ID_Material_Owner],[Route_Name],[Active])VALUES(1,'SLT + BLK', 1)
INSERT INTO [dbo].[CTZ_Route]([ID_Material_Owner],[Route_Name],[Active])VALUES(1,'SLT + BLK + WLD', 1)
INSERT INTO [dbo].[CTZ_Route]([ID_Material_Owner],[Route_Name],[Active])VALUES(1,'WAREHOUSING', 1)
INSERT INTO [dbo].[CTZ_Route]([ID_Material_Owner],[Route_Name],[Active])VALUES(1,'WAREHOUSING / REPALLETIZING', 1)
INSERT INTO [dbo].[CTZ_Route]([ID_Material_Owner],[Route_Name],[Active])VALUES(1,'WEIGHT DIVISION', 1)

--======== CTZ_Vehicle_types ===========
Select * from CTZ_Vehicle_types

SET IDENTITY_INSERT CTZ_Vehicle_Types ON;
INSERT INTO [dbo].[CTZ_Vehicle_Types]([ID_VehicleType],[VehicleType_Name],[Active])VALUES (1,'Automotriz',1)
INSERT INTO [dbo].[CTZ_Vehicle_Types]([ID_VehicleType],[VehicleType_Name],[Active])VALUES (2,'No automotriz',1)
INSERT INTO [dbo].[CTZ_Vehicle_Types]([ID_VehicleType],[VehicleType_Name],[Active])VALUES (3,'Camiones',1)
INSERT INTO [dbo].[CTZ_Vehicle_Types]([ID_VehicleType],[VehicleType_Name],[Active])VALUES (4,'Construcción',1)
SET IDENTITY_INSERT CTZ_Vehicle_Types OFF;

--======== CTZ_Production_Lines ===========
select * from ctz_plants
Select * From CTZ_Production_Lines

SET IDENTITY_INSERT CTZ_Production_Lines ON;
-- Puebla
INSERT INTO [dbo].[CTZ_Production_Lines]([ID_Line], ID_Plant,[Line_Name],[Description],[Aluminum],[Steel],[Active])
     VALUES(1,1,'Blanking 1', 'Puebla - Blanking 1',0,1,1)
INSERT INTO [dbo].[CTZ_Production_Lines]([ID_Line], ID_Plant,[Line_Name],[Description],[Aluminum],[Steel],[Active])
     VALUES(2,1,'Blanking 2', 'Puebla - Blanking 2',0,1,1)
INSERT INTO [dbo].[CTZ_Production_Lines]([ID_Line], ID_Plant,[Line_Name],[Description],[Aluminum],[Steel],[Active])
     VALUES(3,1,'Blanking 3', 'Puebla - Blanking 3',0,1,1)

-- Silao
INSERT INTO [dbo].[CTZ_Production_Lines]([ID_Line], ID_Plant,[Line_Name],[Description],[Aluminum],[Steel],[Active])
     VALUES(4,2,'Blanking 1', 'Silao - Blanking 1',0,1,1)
INSERT INTO [dbo].[CTZ_Production_Lines]([ID_Line], ID_Plant,[Line_Name],[Description],[Aluminum],[Steel],[Active])
     VALUES(5,2,'Blanking 2', 'Silao - Blanking 2',0,1,1)
INSERT INTO [dbo].[CTZ_Production_Lines]([ID_Line], ID_Plant,[Line_Name],[Description],[Aluminum],[Steel],[Active])
     VALUES(6,2,'Blanking 3', 'Silao - Blanking 3',1,1,1)

--SLP
INSERT INTO [dbo].[CTZ_Production_Lines]([ID_Line], ID_Plant,[Line_Name],[Description],[Aluminum],[Steel],[Active])
     VALUES(7,4,'Blanking 1', 'SLP - Blanking 1',0,1,1)

SET IDENTITY_INSERT CTZ_Production_Lines OFF;

--======== CTZ_Material_Type ===========
Select * From CTZ_Material_Type

SET IDENTITY_INSERT CTZ_Material_Type ON;

INSERT INTO [dbo].[CTZ_Material_Type](ID_Material_Type,[Material_Name],[Tipo],[Active])
     VALUES(1,'E ALU','E ALU',1)
INSERT INTO [dbo].[CTZ_Material_Type](ID_Material_Type,[Material_Name],[Tipo],[Active])
     VALUES(2,'UE ALU','UE ALU',1)
INSERT INTO [dbo].[CTZ_Material_Type](ID_Material_Type,[Material_Name],[Tipo],[Active])
     VALUES(3,'UE STEEL','UE STEEL',1)
INSERT INTO [dbo].[CTZ_Material_Type](ID_Material_Type,[Material_Name],[Tipo],[Active])
     VALUES(4,'E STEEL','E STEEL',1)
INSERT INTO [dbo].[CTZ_Material_Type](ID_Material_Type,[Material_Name],[Tipo],[Active])
     VALUES(5,'HS STEEL','HS STEEL',1)

SET IDENTITY_INSERT CTZ_Material_Type OFF;

--======== CTZ_Material_Type_Lines ===========
Select * From CTZ_Production_Lines
Select * From CTZ_Material_Type
Select * From CTZ_Material_Type_Lines

SET IDENTITY_INSERT [CTZ_Material_Type_Lines] ON;
--Puebla blk1
INSERT INTO [dbo].[CTZ_Material_Type_Lines](ID_Material_Type_Line,[ID_Material_Type],[ID_Line])VALUES(1,3,1)
INSERT INTO [dbo].[CTZ_Material_Type_Lines](ID_Material_Type_Line,[ID_Material_Type],[ID_Line])VALUES(2,4,1)
INSERT INTO [dbo].[CTZ_Material_Type_Lines](ID_Material_Type_Line,[ID_Material_Type],[ID_Line])VALUES(3,5,1)
--Puebla blk2
INSERT INTO [dbo].[CTZ_Material_Type_Lines](ID_Material_Type_Line,[ID_Material_Type],[ID_Line])VALUES(4,3,2)
INSERT INTO [dbo].[CTZ_Material_Type_Lines](ID_Material_Type_Line,[ID_Material_Type],[ID_Line])VALUES(5,4,2)
INSERT INTO [dbo].[CTZ_Material_Type_Lines](ID_Material_Type_Line,[ID_Material_Type],[ID_Line])VALUES(6,5,2)
--Puebla blk3
INSERT INTO [dbo].[CTZ_Material_Type_Lines](ID_Material_Type_Line,[ID_Material_Type],[ID_Line])VALUES(7,3,3)
INSERT INTO [dbo].[CTZ_Material_Type_Lines](ID_Material_Type_Line,[ID_Material_Type],[ID_Line])VALUES(8,4,3)
INSERT INTO [dbo].[CTZ_Material_Type_Lines](ID_Material_Type_Line,[ID_Material_Type],[ID_Line])VALUES(9,5,3)

--Silao blk1
INSERT INTO [dbo].[CTZ_Material_Type_Lines](ID_Material_Type_Line,[ID_Material_Type],[ID_Line])VALUES(10,3,4)
INSERT INTO [dbo].[CTZ_Material_Type_Lines](ID_Material_Type_Line,[ID_Material_Type],[ID_Line])VALUES(11,4,4)
INSERT INTO [dbo].[CTZ_Material_Type_Lines](ID_Material_Type_Line,[ID_Material_Type],[ID_Line])VALUES(12,5,4)
--Silao blk2
INSERT INTO [dbo].[CTZ_Material_Type_Lines](ID_Material_Type_Line,[ID_Material_Type],[ID_Line])VALUES(13,3,5)
INSERT INTO [dbo].[CTZ_Material_Type_Lines](ID_Material_Type_Line,[ID_Material_Type],[ID_Line])VALUES(14,4,5)
INSERT INTO [dbo].[CTZ_Material_Type_Lines](ID_Material_Type_Line,[ID_Material_Type],[ID_Line])VALUES(15,5,5)
--Silao blk3
INSERT INTO [dbo].[CTZ_Material_Type_Lines](ID_Material_Type_Line,[ID_Material_Type],[ID_Line])VALUES(16,1,6)
INSERT INTO [dbo].[CTZ_Material_Type_Lines](ID_Material_Type_Line,[ID_Material_Type],[ID_Line])VALUES(17,2,6)
INSERT INTO [dbo].[CTZ_Material_Type_Lines](ID_Material_Type_Line,[ID_Material_Type],[ID_Line])VALUES(18,3,6)
INSERT INTO [dbo].[CTZ_Material_Type_Lines](ID_Material_Type_Line,[ID_Material_Type],[ID_Line])VALUES(19,4,6)
INSERT INTO [dbo].[CTZ_Material_Type_Lines](ID_Material_Type_Line,[ID_Material_Type],[ID_Line])VALUES(20,5,6)
--SLP blk1
INSERT INTO [dbo].[CTZ_Material_Type_Lines](ID_Material_Type_Line,[ID_Material_Type],[ID_Line])VALUES(21,1,7)
INSERT INTO [dbo].[CTZ_Material_Type_Lines](ID_Material_Type_Line,[ID_Material_Type],[ID_Line])VALUES(22,2,7)
INSERT INTO [dbo].[CTZ_Material_Type_Lines](ID_Material_Type_Line,[ID_Material_Type],[ID_Line])VALUES(23,3,7)
INSERT INTO [dbo].[CTZ_Material_Type_Lines](ID_Material_Type_Line,[ID_Material_Type],[ID_Line])VALUES(24,4,7)
INSERT INTO [dbo].[CTZ_Material_Type_Lines](ID_Material_Type_Line,[ID_Material_Type],[ID_Line])VALUES(25,5,7)

SET IDENTITY_INSERT [CTZ_Material_Type_Lines] OFF;

--======== Insert CTZ_Engineering_Criteria ========
DELETE FROM dbo.CTZ_Engineering_Criteria;  -- Elimina todos los registros
DBCC CHECKIDENT ('dbo.CTZ_Engineering_Criteria', RESEED, 0); -- Reinicia el índice IDENTITY a 1

SET IDENTITY_INSERT CTZ_Engineering_Criteria ON;

INSERT INTO [dbo].[CTZ_Engineering_Criteria] (ID_Criteria, [CriteriaName], [Active])
VALUES
(1, 'Gauge - Metric', 1),
(2, 'Longitudinal width [mm]', 1),
(3, 'Longitudinal pitch [mm]', 1),
(4, 'Tensile STRENGTH (Rm)', 1)

SET IDENTITY_INSERT CTZ_Engineering_Criteria OFF;

select * from CTZ_Engineering_Criteria
select * from CTZ_Production_Lines
--======== Insert CTZ_Engineering_Dimension ========
DELETE FROM dbo.CTZ_Engineering_Dimension;  -- Elimina todos los registros
DBCC CHECKIDENT ('dbo.CTZ_Engineering_Dimension', RESEED, 0); -- Reinicia el índice IDENTITY a 1
-------------------------------------------------------------------------------
-- PUEBLA-BLK1 => ID_Line = 1
-------------------------------------------------------------------------------
INSERT INTO [dbo].[CTZ_Engineering_Dimension]
       ([ID_Line], [ID_Criteria], [Min_Value], [Max_Value], [Active])
VALUES
(1, 1, 0.50,  3.00, 1),   -- Gauge - Metric
(1, 2, 300.0, 1680, 1), -- Longitudinal width [mm]
(1, 3, 200, 2500.0, 1), -- Longitudinal pitch [mm]
(1, 4, 270.0, 600.0, 1)  -- Tensile STRENGTH (Rm)

-------------------------------------------------------------------------------
-- PUEBLA-BLK2 => ID_Line = 2
-------------------------------------------------------------------------------
INSERT INTO [dbo].[CTZ_Engineering_Dimension]
       ([ID_Line], [ID_Criteria], [Min_Value], [Max_Value], [Active])
VALUES
(2, 1, 0.50,  3.80, 1),   -- Gauge - Metric
(2, 2, 400.0, 1800.0, 1), -- Longitudinal width [mm]
(2, 3, 170.0, 1000.0, 1), -- Longitudinal pitch [mm]
(2, 4, 270.0, 600.0,  1) -- Tensile STRENGTH (Rm)

-------------------------------------------------------------------------------
-- PUEBLA-BLK3 => ID_Line = 3
-------------------------------------------------------------------------------
INSERT INTO [dbo].[CTZ_Engineering_Dimension]
       ([ID_Line], [ID_Criteria], [Min_Value], [Max_Value], [Active])
VALUES
(3, 1, 0.50,  2.50, 1),   -- Gauge - Metric
(3, 2, 400.0, 2000.0, 1), -- Longitudinal width [mm]
(3, 3, 400.0, 3700.0, 1), -- Longitudinal pitch [mm]
(3, 4, 270.0, 600.0,  1) -- Tensile STRENGTH (Rm)

-------------------------------------------------------------------------------
-- SILAO-BLK1 => ID_Line = 4
-------------------------------------------------------------------------------
INSERT INTO [dbo].[CTZ_Engineering_Dimension]
       ([ID_Line], [ID_Criteria], [Min_Value], [Max_Value], [Active])
VALUES
(4, 1, 0.05,  2.50, 1),   -- Gauge - Metric
(4, 2, 300.0, 2000.0, 1), -- Longitudinal width [mm]
(4, 3, 300.0, 4000.0, 1), -- Longitudinal pitch [mm]
(4, 4, 450.0, 450.0,  1) -- Tensile STRENGTH (Rm)

-------------------------------------------------------------------------------
-- SILAO-BLK2 => ID_Line = 5
-------------------------------------------------------------------------------
INSERT INTO [dbo].[CTZ_Engineering_Dimension]
       ([ID_Line], [ID_Criteria], [Min_Value], [Max_Value], [Active])
VALUES
(5, 1, 0.05,  2.50, 1),   -- Gauge - Metric
(5, 2, 300.0, 2000.0, 1), -- Longitudinal width [mm]
(5, 3, 300.0, 4000.0, 1), -- Longitudinal pitch [mm]
(5, 4, 600.0, 600.0,  1) -- Tensile STRENGTH (Rm)

-------------------------------------------------------------------------------
-- SILAO-BLK3 => ID_Line = 6
-------------------------------------------------------------------------------
INSERT INTO [dbo].[CTZ_Engineering_Dimension]
       ([ID_Line], [ID_Criteria], [Min_Value], [Max_Value], [Active])
VALUES
(6, 1, 0.05,  3.00, 1),   -- Gauge - Metric
(6, 2, 300.0, 2150.0, 1), -- Longitudinal width [mm]
(6, 3, 300.0, 4500.0, 1), -- Longitudinal pitch [mm]
(6, 4, 270.0, 600.0,  1) -- Tensile STRENGTH (Rm)

-------------------------------------------------------------------------------
-- SLP-BLK1 => ID_Line = 7
-------------------------------------------------------------------------------
INSERT INTO [dbo].[CTZ_Engineering_Dimension]
       ([ID_Line], [ID_Criteria], [Min_Value], [Max_Value], [Active])
VALUES
(7, 1, 0.50,  2.50, 1),   -- Gauge - Metric
(7, 2, 400.0, 2000.0, 1), -- Longitudinal width [mm]
(7, 3, 400.0, 3700.0, 1) -- Longitudinal pitch [mm]


-- ======== Inserts CTZ Line Manufacturer ==================--

GO
SET IDENTITY_INSERT [dbo].[CTZ_Line_Manufacturer] ON 
GO
INSERT [dbo].[CTZ_Line_Manufacturer] ([ID_Manufacturer], [Manufacter_Name], [Active]) VALUES (1, N'FAGOR', 1)
GO
INSERT [dbo].[CTZ_Line_Manufacturer] ([ID_Manufacturer], [Manufacter_Name], [Active]) VALUES (2, N'SCHULER', 1)
GO

--- Update Lineas
select * from CTZ_Production_Lines
update CTZ_Production_Lines set ID_Manufacturer = 1 where ID_Line in (2,3,4,5)
update CTZ_Production_Lines set ID_Manufacturer = 2 where ID_Line in (1,6,7)

SET IDENTITY_INSERT [dbo].[CTZ_Line_Manufacturer] OFF
GO
SET IDENTITY_INSERT [dbo].[CTZ_Line_Stroke_Settings] ON 
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (1, 1, 0, 0, 50)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (2, 1, 0, 500, 50)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (3, 1, 0, 700, 50)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (4, 1, 0, 1000, 43)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (5, 1, 0, 2000, 29)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (6, 1, 0, 3000, 22)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (7, 1, 0, 4500, 17)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (8, 1, 5, 0, 50)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (9, 1, 5, 500, 50)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (10, 1, 5, 700, 50)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (11, 1, 5, 1000, 43)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (12, 1, 5, 2000, 29)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (13, 1, 5, 3000, 22)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (14, 1, 5, 4500, 17)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (15, 1, 10, 0, 49)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (16, 1, 10, 500, 49)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (17, 1, 10, 700, 47)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (18, 1, 10, 1000, 43)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (19, 1, 10, 2000, 29)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (20, 1, 10, 3000, 22)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (21, 1, 10, 4500, 17)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (22, 1, 20, 0, 41)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (23, 1, 20, 500, 41)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (24, 1, 20, 700, 39)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (25, 1, 20, 1000, 37)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (26, 1, 20, 2000, 29)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (27, 1, 20, 3000, 22)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (28, 1, 20, 4500, 17)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (29, 1, 30, 0, 35)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (30, 1, 30, 500, 35)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (31, 1, 30, 700, 34)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (32, 1, 30, 1000, 32)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (33, 1, 30, 2000, 28)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (34, 1, 30, 3000, 22)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (35, 1, 30, 4500, 17)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (36, 2, 0, 0, 75)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (37, 2, 0, 300, 75)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (38, 2, 0, 500, 75)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (39, 2, 0, 750, 67)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (40, 2, 0, 1000, 60)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (41, 2, 0, 1500, 50)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (42, 2, 0, 2000, 41)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (43, 2, 0, 2500, 40)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (44, 2, 0, 3000, 30)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (45, 2, 0, 3500, 30)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (46, 2, 0, 4000, 27)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (47, 2, 0, 4500, 24)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (48, 2, 5, 0, 72)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (49, 2, 5, 300, 72)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (50, 2, 5, 500, 69)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (51, 2, 5, 750, 62)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (52, 2, 5, 1000, 56)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (53, 2, 5, 1500, 49)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (54, 2, 5, 2000, 41)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (55, 2, 5, 2500, 38)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (56, 2, 5, 3000, 30)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (57, 2, 5, 3500, 30)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (58, 2, 5, 4000, 27)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (59, 2, 10, 0, 60)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (60, 2, 10, 300, 60)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (61, 2, 10, 500, 60)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (62, 2, 10, 750, 57)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (63, 2, 10, 1000, 50)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (64, 2, 10, 1500, 41)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (65, 2, 10, 2000, 41)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (66, 2, 10, 2500, 30)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (67, 2, 10, 3000, 30)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (68, 2, 10, 3500, 30)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (69, 2, 10, 4000, 27)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (70, 2, 15, 0, 53)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (71, 2, 15, 300, 53)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (72, 2, 15, 500, 53)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (73, 2, 15, 750, 50)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (74, 2, 15, 1000, 50)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (75, 2, 15, 1500, 41)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (76, 2, 15, 2000, 39)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (77, 2, 15, 2500, 30)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (78, 2, 15, 3000, 30)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (79, 2, 15, 3500, 30)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (80, 2, 20, 0, 48)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (81, 2, 20, 300, 48)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (82, 2, 20, 500, 48)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (83, 2, 20, 750, 48)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (84, 2, 20, 1000, 47)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (85, 2, 20, 1500, 41)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (86, 2, 20, 2000, 30)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (87, 2, 20, 2500, 30)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (88, 2, 20, 3000, 30)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (89, 2, 20, 3500, 29)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (90, 2, 25, 0, 43)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (91, 2, 25, 300, 43)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (92, 2, 25, 500, 43)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (93, 2, 25, 750, 43)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (94, 2, 25, 1000, 41)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (95, 2, 25, 1500, 40)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (96, 2, 25, 2000, 30)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (97, 2, 25, 2500, 30)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (98, 2, 25, 3000, 30)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (99, 2, 30, 0, 40)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (100, 2, 30, 300, 40)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (101, 2, 30, 500, 40)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (102, 2, 30, 750, 40)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (103, 2, 30, 1000, 40)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (104, 2, 30, 1500, 30)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (105, 2, 30, 2000, 30)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (106, 2, 30, 2500, 30)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (107, 2, 30, 3000, 30)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (108, 2, 35, 0, 37)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (109, 2, 35, 300, 37)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (110, 2, 35, 500, 37)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (111, 2, 35, 750, 37)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (112, 2, 35, 1000, 37)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (113, 2, 35, 1500, 30)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (114, 2, 35, 2000, 30)
GO
INSERT [dbo].[CTZ_Line_Stroke_Settings] ([ID_Stroke_setting], [ID_Machine_Manufacturer], [Rotation_degrees], [Advance_mm], [Strokes]) VALUES (115, 2, 35, 2500, 30)
GO
SET IDENTITY_INSERT [dbo].[CTZ_Line_Stroke_Settings] OFF
GO


-- =================== INSERTS CTZ_Fiscal_Years ===============================
SET IDENTITY_INSERT [dbo].[CTZ_Fiscal_Years] ON;

INSERT INTO [dbo].[CTZ_Fiscal_Years] (ID_Fiscal_Year, Fiscal_Year_Name, Start_Date, End_Date) VALUES (1, 'FY 10/11', '2010-10-01', '2011-09-30');
INSERT INTO [dbo].[CTZ_Fiscal_Years] (ID_Fiscal_Year, Fiscal_Year_Name, Start_Date, End_Date) VALUES (2, 'FY 11/12', '2011-10-01', '2012-09-30');
INSERT INTO [dbo].[CTZ_Fiscal_Years] (ID_Fiscal_Year, Fiscal_Year_Name, Start_Date, End_Date) VALUES (3, 'FY 12/13', '2012-10-01', '2013-09-30');
INSERT INTO [dbo].[CTZ_Fiscal_Years] (ID_Fiscal_Year, Fiscal_Year_Name, Start_Date, End_Date) VALUES (4, 'FY 13/14', '2013-10-01', '2014-09-30');
INSERT INTO [dbo].[CTZ_Fiscal_Years] (ID_Fiscal_Year, Fiscal_Year_Name, Start_Date, End_Date) VALUES (5, 'FY 14/15', '2014-10-01', '2015-09-30');
INSERT INTO [dbo].[CTZ_Fiscal_Years] (ID_Fiscal_Year, Fiscal_Year_Name, Start_Date, End_Date) VALUES (6, 'FY 15/16', '2015-10-01', '2016-09-30');
INSERT INTO [dbo].[CTZ_Fiscal_Years] (ID_Fiscal_Year, Fiscal_Year_Name, Start_Date, End_Date) VALUES (7, 'FY 16/17', '2016-10-01', '2017-09-30');
INSERT INTO [dbo].[CTZ_Fiscal_Years] (ID_Fiscal_Year, Fiscal_Year_Name, Start_Date, End_Date) VALUES (8, 'FY 17/18', '2017-10-01', '2018-09-30');
INSERT INTO [dbo].[CTZ_Fiscal_Years] (ID_Fiscal_Year, Fiscal_Year_Name, Start_Date, End_Date) VALUES (9, 'FY 18/19', '2018-10-01', '2019-09-30');
INSERT INTO [dbo].[CTZ_Fiscal_Years] (ID_Fiscal_Year, Fiscal_Year_Name, Start_Date, End_Date) VALUES (10, 'FY 19/20', '2019-10-01', '2020-09-30');
INSERT INTO [dbo].[CTZ_Fiscal_Years] (ID_Fiscal_Year, Fiscal_Year_Name, Start_Date, End_Date) VALUES (11, 'FY 20/21', '2020-10-01', '2021-09-30');
INSERT INTO [dbo].[CTZ_Fiscal_Years] (ID_Fiscal_Year, Fiscal_Year_Name, Start_Date, End_Date) VALUES (12, 'FY 21/22', '2021-10-01', '2022-09-30');
INSERT INTO [dbo].[CTZ_Fiscal_Years] (ID_Fiscal_Year, Fiscal_Year_Name, Start_Date, End_Date) VALUES (13, 'FY 22/23', '2022-10-01', '2023-09-30');
INSERT INTO [dbo].[CTZ_Fiscal_Years] (ID_Fiscal_Year, Fiscal_Year_Name, Start_Date, End_Date) VALUES (14, 'FY 23/24', '2023-10-01', '2024-09-30');
INSERT INTO [dbo].[CTZ_Fiscal_Years] (ID_Fiscal_Year, Fiscal_Year_Name, Start_Date, End_Date) VALUES (15, 'FY 24/25', '2024-10-01', '2025-09-30');
INSERT INTO [dbo].[CTZ_Fiscal_Years] (ID_Fiscal_Year, Fiscal_Year_Name, Start_Date, End_Date) VALUES (16, 'FY 25/26', '2025-10-01', '2026-09-30');
INSERT INTO [dbo].[CTZ_Fiscal_Years] (ID_Fiscal_Year, Fiscal_Year_Name, Start_Date, End_Date) VALUES (17, 'FY 26/27', '2026-10-01', '2027-09-30');
INSERT INTO [dbo].[CTZ_Fiscal_Years] (ID_Fiscal_Year, Fiscal_Year_Name, Start_Date, End_Date) VALUES (18, 'FY 27/28', '2027-10-01', '2028-09-30');
INSERT INTO [dbo].[CTZ_Fiscal_Years] (ID_Fiscal_Year, Fiscal_Year_Name, Start_Date, End_Date) VALUES (19, 'FY 28/29', '2028-10-01', '2029-09-30');
INSERT INTO [dbo].[CTZ_Fiscal_Years] (ID_Fiscal_Year, Fiscal_Year_Name, Start_Date, End_Date) VALUES (20, 'FY 29/30', '2029-10-01', '2030-09-30');
INSERT INTO [dbo].[CTZ_Fiscal_Years] (ID_Fiscal_Year, Fiscal_Year_Name, Start_Date, End_Date) VALUES (21, 'FY 30/31', '2030-10-01', '2031-09-30');
INSERT INTO [dbo].[CTZ_Fiscal_Years] (ID_Fiscal_Year, Fiscal_Year_Name, Start_Date, End_Date) VALUES (22, 'FY 31/32', '2031-10-01', '2032-09-30');
INSERT INTO [dbo].[CTZ_Fiscal_Years] (ID_Fiscal_Year, Fiscal_Year_Name, Start_Date, End_Date) VALUES (23, 'FY 32/33', '2032-10-01', '2033-09-30');
INSERT INTO [dbo].[CTZ_Fiscal_Years] (ID_Fiscal_Year, Fiscal_Year_Name, Start_Date, End_Date) VALUES (24, 'FY 33/34', '2033-10-01', '2034-09-30');
INSERT INTO [dbo].[CTZ_Fiscal_Years] (ID_Fiscal_Year, Fiscal_Year_Name, Start_Date, End_Date) VALUES (25, 'FY 34/35', '2034-10-01', '2035-09-30');
INSERT INTO [dbo].[CTZ_Fiscal_Years] (ID_Fiscal_Year, Fiscal_Year_Name, Start_Date, End_Date) VALUES (26, 'FY 35/36', '2035-10-01', '2036-09-30');
INSERT INTO [dbo].[CTZ_Fiscal_Years] (ID_Fiscal_Year, Fiscal_Year_Name, Start_Date, End_Date) VALUES (27, 'FY 36/37', '2036-10-01', '2037-09-30');
INSERT INTO [dbo].[CTZ_Fiscal_Years] (ID_Fiscal_Year, Fiscal_Year_Name, Start_Date, End_Date) VALUES (28, 'FY 37/38', '2037-10-01', '2038-09-30');
INSERT INTO [dbo].[CTZ_Fiscal_Years] (ID_Fiscal_Year, Fiscal_Year_Name, Start_Date, End_Date) VALUES (29, 'FY 38/39', '2038-10-01', '2039-09-30');
INSERT INTO [dbo].[CTZ_Fiscal_Years] (ID_Fiscal_Year, Fiscal_Year_Name, Start_Date, End_Date) VALUES (30, 'FY 39/40', '2039-10-01', '2040-09-30');
INSERT INTO [dbo].[CTZ_Fiscal_Years] (ID_Fiscal_Year, Fiscal_Year_Name, Start_Date, End_Date) VALUES (31, 'FY 40/41', '2040-10-01', '2041-09-30');
INSERT INTO [dbo].[CTZ_Fiscal_Years] (ID_Fiscal_Year, Fiscal_Year_Name, Start_Date, End_Date) VALUES (32, 'FY 41/42', '2041-10-01', '2042-09-30');
INSERT INTO [dbo].[CTZ_Fiscal_Years] (ID_Fiscal_Year, Fiscal_Year_Name, Start_Date, End_Date) VALUES (33, 'FY 42/43', '2042-10-01', '2043-09-30');
INSERT INTO [dbo].[CTZ_Fiscal_Years] (ID_Fiscal_Year, Fiscal_Year_Name, Start_Date, End_Date) VALUES (34, 'FY 43/44', '2043-10-01', '2044-09-30');
INSERT INTO [dbo].[CTZ_Fiscal_Years] (ID_Fiscal_Year, Fiscal_Year_Name, Start_Date, End_Date) VALUES (35, 'FY 44/45', '2044-10-01', '2045-09-30');
INSERT INTO [dbo].[CTZ_Fiscal_Years] (ID_Fiscal_Year, Fiscal_Year_Name, Start_Date, End_Date) VALUES (36, 'FY 45/46', '2045-10-01', '2046-09-30');
INSERT INTO [dbo].[CTZ_Fiscal_Years] (ID_Fiscal_Year, Fiscal_Year_Name, Start_Date, End_Date) VALUES (37, 'FY 46/47', '2046-10-01', '2047-09-30');
INSERT INTO [dbo].[CTZ_Fiscal_Years] (ID_Fiscal_Year, Fiscal_Year_Name, Start_Date, End_Date) VALUES (38, 'FY 47/48', '2047-10-01', '2048-09-30');
INSERT INTO [dbo].[CTZ_Fiscal_Years] (ID_Fiscal_Year, Fiscal_Year_Name, Start_Date, End_Date) VALUES (39, 'FY 48/49', '2048-10-01', '2049-09-30');
INSERT INTO [dbo].[CTZ_Fiscal_Years] (ID_Fiscal_Year, Fiscal_Year_Name, Start_Date, End_Date) VALUES (40, 'FY 49/50', '2049-10-01', '2050-09-30');

SET IDENTITY_INSERT [dbo].[CTZ_Fiscal_Years] OFF;

-- =================== INSERTS CTZ_Hours_By_Line ===============================
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (6,4,13,4189.86826609312)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (5,4,13,3360.41751807855)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (4,4,13,5459.19266685441)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (3,4,13,6565.25907922238)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (2,4,13,2760.15130933306)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (1,4,13,1490.72588052213)

Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (7,4,14,1148.56617638358)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (6,4,14,6925.41424459403)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (5,4,14,4845.12044639369)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (4,4,14,7987.85566691715)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (3,4,14,7037.01562479024)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (2,4,14,5031.0996505226)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (1,4,14,1490.72588052213)

Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (7,4,15,7665.95688913712)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (6,4,15,8317.32297344404)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (6,1,15,680.11694085683)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (5,4,15,6530.73749933632)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (5,1,15,951.293188011087)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (4,4,15,7669.50433139236)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (4,1,15,128.579220004011)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (3,4,15,5766.83384625498)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (3,1,15,100.721443699542)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (2,4,15,4489.75298115803)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (2,3,15,142.260493632162)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (2,1,15,720.45516321863)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (1,4,15,5024.72440779638)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (1,1,15,1914.9086006212)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (7,4,16,8921.22421981006)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (6,4,16,7255.81310513397)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (6,1,16,1688.6549798833)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (5,4,16,5151.87644888802)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (5,1,16,1506.60510631343)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (4,4,16,6627.98459567938)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (4,1,16,384.967312270541)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (3,4,16,5756.59880507628)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (3,1,16,83.354863915501)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (2,4,16,5731.30090446394)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (2,3,16,127.52722571419)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (2,1,16,3088.43793340193)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (1,4,16,4439.16490547877)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (1,1,16,1739.77568688317)

Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (7,4,17,8738.9603721978)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (7,2,17,95.9509345164013)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (6,4,17,4609.64256135387)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (6,1,17,1209.9835630594)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (5,4,17,2958.64520100628)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (5,2,17,673.092704992509)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (5,1,17,1334.01816400085)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (4,4,17,2522.71191160163)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (4,2,17,2309.64609393949)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (4,1,17,753.653699084226)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (3,4,17,4111.3556317296)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (3,1,17,97.3981297409205)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (2,4,17,3834.7225931101)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (2,3,17,149.012459407309)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (2,1,17,3497.86134630571)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (1,4,17,4500.57000728612)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (1,1,17,1757.25142241282)

Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (7,4,18,7072.46162177701)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (7,2,18,371.105302437506)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (6,4,18,3908.93474764908)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (6,1,18,1009.93843292433)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (5,4,18,2587.25484174806)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (5,2,18,724.731606278643)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (5,1,18,469.1321578522)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (4,4,18,632.665564819145)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (4,2,18,2502.60912857985)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (4,1,18,800.501578604912)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (3,4,18,2158.92313193415)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (3,2,18,776.939566430025)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (3,1,18,103.318750984854)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (2,4,18,2878.54677606146)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (2,3,18,158.070603902737)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (2,2,18,734.427152379968)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (2,1,18,2506.10656363874)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (1,4,18,3795.01155549138)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (1,1,18,1758.42309925176)

Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (7,4,19,6140.69416824033)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (7,2,19,518.902783440572)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (6,4,19,3485.89829362247)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (6,1,19,755.5623420917)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (5,4,19,1989.47904385913)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (5,2,19,690.226625823521)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (5,1,19,383.761492546212)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (4,4,19,519.674402890923)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (4,2,19,2382.40868357496)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (4,1,19,632.032404945893)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (3,4,19,1222.17301731397)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (3,2,19,814.965680471093)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (3,1,19,105.201818295797)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (2,4,19,2606.60113491661)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (2,3,19,160.951567756761)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (2,2,19,770.372561595748)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (2,1,19,724.547116431947)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (1,4,19,3516.47957612847)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (1,1,19,1487.36005593898)

Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (7,4,20,5396.1386240322)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (7,2,20,571.057072627408)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (6,4,20,2985.84659843142)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (6,1,20,619.258128766581)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (5,4,20,1620.49972757847)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (5,2,20,645.400095574993)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (5,1,20,313.7621602151)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (4,4,20,444.543804483027)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (4,2,20,2215.38819538287)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (4,1,20,447.118331173247)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (3,4,20,798.487489648184)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (3,2,20,766.13030935671)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (3,1,20,109.225571246867)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (2,4,20,1857.09221805556)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (2,3,20,167.107633842232)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (2,2,20,724.209353937581)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (2,1,20,269.72130167212)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (1,4,20,3266.98084954386)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (1,1,20,726.475928100152)

