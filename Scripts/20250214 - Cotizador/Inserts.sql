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
--delete from CTZ_OEMClients where ID_OEM>25

--====== INSERT [CTZ_plants]  =====
--DELETE FROM dbo.CTZ_plants;  -- Elimina todos los registros
--DBCC CHECKIDENT ('dbo.CTZ_plants', RESEED, 0); -- Reinicia el índice IDENTITY a 1

INSERT INTO [dbo].[CTZ_plants]([Description],[Codigo_SAP],[Active])VALUES('Puebla', '5190',1)
INSERT INTO [dbo].[CTZ_plants]([Description],[Codigo_SAP],[Active])VALUES('Silao', '5390',1)
INSERT INTO [dbo].[CTZ_plants]([Description],[Codigo_SAP],[Active])VALUES('Saltillo', '5490',1)
INSERT INTO [dbo].[CTZ_plants]([Description],[Codigo_SAP],[Active])VALUES('San Luis Potosí', '5890',1)

--select * from CTZ_plants
--delete From CTZ_plants where ID_Plant>4

--====== INSERT [CTZ_Project_Status] =====

INSERT INTO [dbo].[CTZ_Project_Status]([Status_Percent],[Description])VALUES('25','Quotes')
INSERT INTO [dbo].[CTZ_Project_Status]([Status_Percent],[Description])VALUES('50','Carry Over')
INSERT INTO [dbo].[CTZ_Project_Status]([Status_Percent],[Description])VALUES('75','Casi Casi')
INSERT INTO [dbo].[CTZ_Project_Status]([Status_Percent],[Description])VALUES('100','POH')


--select * from [CTZ_Project_Status]
delete from CTZ_Project_Status where ID_Status>4

--====== INSERT [CTZ_Material_Owner] =====

INSERT INTO [dbo].[CTZ_Material_Owner]([Owner_Key],[Description],[Active])VALUES('OWN', 'Propiedad tkMM',1)
INSERT INTO [dbo].[CTZ_Material_Owner]([Owner_Key],[Description],[Active])VALUES('CM', 'Propiedad del Cliente',1)

--Select * from CTZ_Material_Owner
--delete from CTZ_Material_Owner where ID_Owner>2

--======= INSERT CTZ_Temp_IHS =========
DELETE FROM dbo.CTZ_Temp_IHS;  -- Elimina todos los registros
DBCC CHECKIDENT ('dbo.CTZ_Temp_IHS', RESEED, 0); -- Reinicia el índice IDENTITY a 1

SET IDENTITY_INSERT CTZ_Temp_IHS ON;

INSERT INTO [dbo].[CTZ_Temp_IHS] 
    (ID_IHS, Vehicle, Program, SOP, EOP, Max_Production, Mnemonic_Vehicle_plant, Production_Plant)
SELECT 
	i.id,
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

	SET IDENTITY_INSERT CTZ_Temp_IHS OFF;

	select * from CTZ_Temp_IHS

--======== CTZ CTZ_Temp_IHS_Production =========

-- Limpia la tabla y reinicia el contador de identidad
DELETE FROM dbo.CTZ_Temp_IHS_Production;
DBCC CHECKIDENT ('dbo.CTZ_Temp_IHS_Production', RESEED, 0);

-- Inserta los datos agrupados por ID_IHS, año y mes
INSERT INTO dbo.CTZ_Temp_IHS_Production (ID_IHS, Production_Year, Production_Month, Production_Amount)
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
    ON  i.id = t.ID_IHS 
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
select * from dbo.CTZ_Project_Materials
--delete from CTZ_Project_Materials where ID_Route is not null

DELETE FROM [CTZ_Route];
DBCC CHECKIDENT ('[CTZ_Route]', RESEED, 0);

INSERT INTO [dbo].[CTZ_Route]([ID_Material_Owner],[Route_Name],[Active])VALUES(1,'BLK', 1)
INSERT INTO [dbo].[CTZ_Route]([ID_Material_Owner],[Route_Name],[Active])VALUES(1,'BLK + RP', 1)
INSERT INTO [dbo].[CTZ_Route]([ID_Material_Owner],[Route_Name],[Active])VALUES(1,'BLK + RPLTZ', 0)
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

select * from CTZ_Project_Status
select * from CTZ_Production_Lines
select * from CTZ_Fiscal_Years
delete from CTZ_Hours_By_Line

-- =================== INSERTS CTZ_Hours_By_Line ===============================
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (1,4,13,1490.72588052213)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (2,4,13,2760.15130933306)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (3,4,13,6565.25907922238)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (4,4,13,5459.19266685441)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (5,4,13,3360.41751807855)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (6,4,13,4189.86826609312)

Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (1,4,14,1490.72588052213)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (2,4,14,5031.0996505226)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (3,4,14,7037.01562479024)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (4,4,14,7987.85566691715)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (5,4,14,4845.12044639369)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (6,4,14,6925.41424459403)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (7,4,14,1148.56617638358)

Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (1,4,15,5075.48011055099)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (1,1,15,2481.38247540515)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (2,4,15,5525.69421977146)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (2,3,15,95.148290652766)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (2,1,15,204.219765037594)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (3,4,15,6233.88061815589)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (3,1,15,60.3195154160067)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (4,4,15,6245.4005924425)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (4,1,15,39.2234578192731)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (5,4,15,6860.24142878148)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (5,1,15,468.148319294738)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (6,4,15,6407.43868761262)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (6,1,15,248.754304784099)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (7,4,15,7665.95688913712)

Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (1,4,16,4215.86037371111)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (1,1,16,2332.46057577206)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (2,4,16,6949.24562969693)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (2,3,16,133.033355195819)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (2,1,16,349.891703200909)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (3,4,16,6030.51798176305)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (3,1,16,84.3368542358993)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (4,4,16,5256.25842159487)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (4,1,16,219.077634211404)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (5,4,16,6141.4298117781)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (5,1,16,832.576791661705)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (6,4,16,6383.8934229155)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (6,1,16,1098.46556330974)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (7,4,16,8921.22421981006)

Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (1,4,17,4269.57329702826)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (1,1,17,2311.97222796837)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (2,4,17,5310.98943878694)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (2,3,17,155.446237694868)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (2,1,17,535.238438337582)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (3,4,17,4705.18426335917)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (3,1,17,98.5455615299933)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (4,4,17,1280.09699971828)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (4,2,17,2880.84932694265)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (4,1,17,223.524836589664)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (5,4,17,3648.35107209432)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (5,2,17,753.231892614558)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (5,1,17,1039.50758724962)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (6,4,17,3438.16718966536)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (6,2,17,267.619192909764)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (6,1,17,1945.52163281417)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (7,4,17,8738.9603721978)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (7,2,17,95.9509345164013)

Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (1,4,18,3533.45590033168)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (1,1,18,2294.04026591843)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (2,4,18,4808.42033775956)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (2,3,18,164.895477630316)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (2,2,18,766.13685964586)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (2,1,18,526.590733379952)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (3,4,18,2196.45243646594)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (3,2,18,786.092567202027)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (3,1,18,104.535932665885)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (4,4,18,307.188457815689)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (4,2,18,2827.40579396036)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (4,1,18,178.396358284775)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (5,4,18,2558.60445948967)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (5,2,18,742.416883819565)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (5,1,18,1254.83790403131)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (6,4,18,2742.74106860903)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (6,2,18,263.958057856175)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (6,1,18,1505.02638088179)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (7,4,18,7072.46162177701)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (7,2,18,371.105302437506)

Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (1,4,19,3257.02425690149)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (1,1,19,1881.18341705009)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (2,4,19,4520.4547607162)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (2,3,19,167.900830295617)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (2,2,19,803.634251791589)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (2,1,19,481.416670910406)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (3,4,19,1236.43454251961)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (3,2,19,824.566660810893)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (3,1,19,106.441184091649)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (4,4,19,294.203259911397)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (4,2,19,2577.59266105532)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (4,1,19,160.093638239313)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (5,4,19,2182.90297731782)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (5,2,19,677.873961367213)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (5,1,19,1084.38751030614)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (6,4,19,2444.65618686831)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (6,2,19,241.07073728866)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (6,1,19,1112.2897685465)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (7,4,19,6140.69416824033)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (7,2,19,518.902783440572)

Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (1,4,20,3019.60859288007)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (1,1,20,726.475928100152)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (2,4,20,3712.93345118627)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (2,3,20,174.322691365447)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (2,2,20,755.477896417476)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (2,1,20,424.479219485607)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (3,4,20,807.780410279668)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (3,2,20,775.155967999914)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (3,1,20,110.512340232696)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (4,4,20,275.690561532308)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (4,2,20,2405.0161619113)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (4,1,20,177.13939131274)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (5,4,20,1695.76906178258)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (5,2,20,634.633563422426)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (5,1,20,1144.38032696873)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (6,4,20,1781.72799419871)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (6,2,20,225.815722428139)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (6,1,20,956.08087064446)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (7,4,20,5396.1386240322)
Insert into CTZ_Hours_By_Line (ID_Line,ID_Status,ID_Fiscal_Year,Hours) Values (7,2,20,571.057072627408)

--======== CTZ_Total_Time_Per_Fiscal_Year ==========
select * from CTZ_Total_Time_Per_Fiscal_Year

select * from CTZ_Fiscal_Years

Insert into CTZ_Total_Time_Per_Fiscal_Year (ID_Fiscal_Year, [Value]) Values (13,7629)
Insert into CTZ_Total_Time_Per_Fiscal_Year (ID_Fiscal_Year, [Value]) Values (14,7650)
Insert into CTZ_Total_Time_Per_Fiscal_Year (ID_Fiscal_Year, [Value]) Values (15,7629)
Insert into CTZ_Total_Time_Per_Fiscal_Year (ID_Fiscal_Year, [Value]) Values (16,7629)
Insert into CTZ_Total_Time_Per_Fiscal_Year (ID_Fiscal_Year, [Value]) Values (17,7629)
Insert into CTZ_Total_Time_Per_Fiscal_Year (ID_Fiscal_Year, [Value]) Values (18,7650)--27/28
Insert into CTZ_Total_Time_Per_Fiscal_Year (ID_Fiscal_Year, [Value]) Values (19,7629)
Insert into CTZ_Total_Time_Per_Fiscal_Year (ID_Fiscal_Year, [Value]) Values (20,7629)
Insert into CTZ_Total_Time_Per_Fiscal_Year (ID_Fiscal_Year, [Value]) Values (21,7629)
Insert into CTZ_Total_Time_Per_Fiscal_Year (ID_Fiscal_Year, [Value]) Values (22,7650)

--======== CTZ_Import_Business_Model ==========
Insert into CTZ_Import_Business_Model (Description, Active) values ('Definitive Import',1)
Insert into CTZ_Import_Business_Model (Description, Active) values ('Domestic Sale',1)
Insert into CTZ_Import_Business_Model (Description, Active) values ('Consignment',1)

--======== CTZ_Incoterms ==========
INSERT INTO [dbo].[CTZ_Incoterms]([key],[Description_ES],[Description_EN],Active)VALUES
('EXW','En Fábrica (Lugar Convenido)','Ex Works (named place)',1),
('FCA','Franco Transportista (Lugar Designado)','Free Carrier (named place)',1),
('FAS','Franco Al Costado Del Buque (Puerto De Carga Convenido)','Free Alongside Ship (named port of shipment)',1),
('FOB','Franco A Bordo (Puerto De Carga Convenido)','Free On Board (named port of shipment)',1),
('CFR','Coste Y Flete (Puerto De Destino Convenido)','Cost and Freight (named port of destination)',1),
('CIF','Coste, Seguro Y Flete (Puerto De Destino Convenido)','Cost, Insurance And Freight (named port of destination)',1),
('CPT','Transporte Pagado Hasta (Lugar De Destino Convenido)','Carriage Paid To (named place of destination)',1),
('CIP','Transporte Y Seguro Pagados Hasta (Lugar De Destino Convenido)','Carriage And Insurance Paid To (named place of destination)',1),
('DAF','Entregada En Frontera (Lugar Convenido)','Delivered At Frontier (named place)',1),
('DES','Entregada Sobre Buque (Puerto De Destino Convenido)','Delivered Ex Ship (named port of destination)',1),
('DEQ','Entregada En Muelle (Puerto De Destino Convenido)','Delivered Ex Quay (named port of destination)',1),
('DAP','Entregada En Lugar (Lugar De Destino Convenido)','Delivered At Place (named place of destination)',1),
('DAT','Entregada En Terminal (Terminal De Destino Convenido)','Delivered At Terminal (named terminal at destination)',1),
('DDU','Entregada Derechos No Pagados (Lugar De Destino Convenido)','Delivered Duty Unpaid (named place of destination)',1),
('DDP','Entregada Derechos Pagados (Lugar De Destino Convenido)','Delivered Duty Paid (named place of destination)',1);


--======== CTZ_Countries ==========

-- Inserta TODOS los países ISO-3166-1
INSERT INTO [dbo].[CTZ_Countries]([ISO],[Name],[Nicename],[ISO3],[Active])VALUES
('AF','Afghanistan','Afghanistan','AFG',1),
('AL','Albania','Albania','ALB',1),
('DZ','Algeria','Algeria','DZA',1),
('AS','American Samoa','American Samoa','ASM',1),
('AD','Andorra','Andorra','AND',1),
('AO','Angola','Angola','AGO',1),
('AI','Anguilla','Anguilla','AIA',1),
('AQ','Antarctica','Antarctica','ATA',1),
('AG','Antigua And Barbuda','Antigua And Barbuda','ATG',1),
('AR','Argentina','Argentina','ARG',1),
('AM','Armenia','Armenia','ARM',1),
('AW','Aruba','Aruba','ABW',1),
('AU','Australia','Australia','AUS',1),
('AT','Austria','Austria','AUT',1),
('AZ','Azerbaijan','Azerbaijan','AZE',1),
('BS','Bahamas','Bahamas','BHS',1),
('BH','Bahrain','Bahrain','BHR',1),
('BD','Bangladesh','Bangladesh','BGD',1),
('BB','Barbados','Barbados','BRB',1),
('BY','Belarus','Belarus','BLR',1),
('BE','Belgium','Belgium','BEL',1),
('BZ','Belize','Belize','BLZ',1),
('BJ','Benin','Benin','BEN',1),
('BM','Bermuda','Bermuda','BMU',1),
('BT','Bhutan','Bhutan','BTN',1),
('BO','Bolivia','Bolivia','BOL',1),
('BQ','Bonaire, Sint Eustatius And Saba','Bonaire, Sint Eustatius And Saba','BES',1),
('BA','Bosnia And Herzegovina','Bosnia And Herzegovina','BIH',1),
('BW','Botswana','Botswana','BWA',1),
('BV','Bouvet Island','Bouvet Island','BVT',1),
('BR','Brazil','Brazil','BRA',1),
('IO','British Indian Ocean Territory','British Indian Ocean Territory','IOT',1),
('BN','Brunei Darussalam','Brunei Darussalam','BRN',1),
('BG','Bulgaria','Bulgaria','BGR',1),
('BF','Burkina Faso','Burkina Faso','BFA',1),
('BI','Burundi','Burundi','BDI',1),
('KH','Cambodia','Cambodia','KHM',1),
('CM','Cameroon','Cameroon','CMR',1),
('CA','Canada','Canada','CAN',1),
('CV','Cabo Verde','Cabo Verde','CPV',1),
('KY','Cayman Islands','Cayman Islands','CYM',1),
('CF','Central African Republic','Central African Republic','CAF',1),
('TD','Chad','Chad','TCD',1),
('CL','Chile','Chile','CHL',1),
('CN','China','China','CHN',1),
('CX','Christmas Island','Christmas Island','CXR',1),
('CC','Cocos (Keeling) Islands','Cocos (Keeling) Islands','CCK',1),
('CO','Colombia','Colombia','COL',1),
('KM','Comoros','Comoros','COM',1),
('CG','Congo','Congo','COG',1),
('CD','Congo, Democratic Republic Of The','Congo, Democratic Republic Of The','COD',1),
('CK','Cook Islands','Cook Islands','COK',1),
('CR','Costa Rica','Costa Rica','CRI',1),
('CI','Côte D''Ivoire','Côte D''Ivoire','CIV',1),
('HR','Croatia','Croatia','HRV',1),
('CU','Cuba','Cuba','CUB',1),
('CW','Curaçao','Curaçao','CUW',1),
('CY','Cyprus','Cyprus','CYP',1),
('CZ','Czechia','Czechia','CZE',1),
('DK','Denmark','Denmark','DNK',1),
('DJ','Djibouti','Djibouti','DJI',1),
('DM','Dominica','Dominica','DMA',1),
('DO','Dominican Republic','Dominican Republic','DOM',1),
('EC','Ecuador','Ecuador','ECU',1),
('EG','Egypt','Egypt','EGY',1),
('SV','El Salvador','El Salvador','SLV',1),
('GQ','Equatorial Guinea','Equatorial Guinea','GNQ',1),
('ER','Eritrea','Eritrea','ERI',1),
('EE','Estonia','Estonia','EST',1),
('SZ','Eswatini','Eswatini','SWZ',1),
('ET','Ethiopia','Ethiopia','ETH',1),
('FK','Falkland Islands','Falkland Islands','FLK',1),
('FO','Faroe Islands','Faroe Islands','FRO',1),
('FJ','Fiji','Fiji','FJI',1),
('FI','Finland','Finland','FIN',1),
('FR','France','France','FRA',1),
('GF','French Guiana','French Guiana','GUF',1),
('PF','French Polynesia','French Polynesia','PYF',1),
('TF','French Southern Territories','French Southern Territories','ATF',1),
('GA','Gabon','Gabon','GAB',1),
('GM','Gambia','Gambia','GMB',1),
('GE','Georgia','Georgia','GEO',1),
('DE','Germany','Germany','DEU',1),
('GH','Ghana','Ghana','GHA',1),
('GI','Gibraltar','Gibraltar','GIB',1),
('GR','Greece','Greece','GRC',1),
('GL','Greenland','Greenland','GRL',1),
('GD','Grenada','Grenada','GRD',1),
('GP','Guadeloupe','Guadeloupe','GLP',1),
('GU','Guam','Guam','GUM',1),
('GT','Guatemala','Guatemala','GTM',1),
('GG','Guernsey','Guernsey','GGY',1),
('GN','Guinea','Guinea','GIN',1),
('GW','Guinea-Bissau','Guinea-Bissau','GNB',1),
('GY','Guyana','Guyana','GUY',1),
('HT','Haiti','Haiti','HTI',1),
('HM','Heard Island And Mcdonald Islands','Heard Island And Mcdonald Islands','HMD',1),
('VA','Holy See','Holy See','VAT',1),
('HN','Honduras','Honduras','HND',1),
('HK','Hong Kong','Hong Kong','HKG',1),
('HU','Hungary','Hungary','HUN',1),
('IS','Iceland','Iceland','ISL',1),
('IN','India','India','IND',1),
('ID','Indonesia','Indonesia','IDN',1),
('IR','Iran','Iran','IRN',1),
('IQ','Iraq','Iraq','IRQ',1),
('IE','Ireland','Ireland','IRL',1),
('IM','Isle Of Man','Isle Of Man','IMN',1),
('IL','Israel','Israel','ISR',1),
('IT','Italy','Italy','ITA',1),
('JM','Jamaica','Jamaica','JAM',1),
('JP','Japan','Japan','JPN',1),
('JE','Jersey','Jersey','JEY',1),
('JO','Jordan','Jordan','JOR',1),
('KZ','Kazakhstan','Kazakhstan','KAZ',1),
('KE','Kenya','Kenya','KEN',1),
('KI','Kiribati','Kiribati','KIR',1),
('KP','Korea, Democratic People''s Republic Of','Korea, Democratic People''s Republic Of','PRK',1),
('KR','Korea, Republic Of','Korea, Republic Of','KOR',1),
('KW','Kuwait','Kuwait','KWT',1),
('KG','Kyrgyzstan','Kyrgyzstan','KGZ',1),
('LA','Lao People''s Democratic Republic','Lao People''s Democratic Republic','LAO',1),
('LV','Latvia','Latvia','LVA',1),
('LB','Lebanon','Lebanon','LBN',1),
('LS','Lesotho','Lesotho','LSO',1),
('LR','Liberia','Liberia','LBR',1),
('LY','Libya','Libya','LBY',1),
('LI','Liechtenstein','Liechtenstein','LIE',1),
('LT','Lithuania','Lithuania','LTU',1),
('LU','Luxembourg','Luxembourg','LUX',1),
('MO','Macao','Macao','MAC',1),
('MG','Madagascar','Madagascar','MDG',1),
('MW','Malawi','Malawi','MWI',1),
('MY','Malaysia','Malaysia','MYS',1),
('MV','Maldives','Maldives','MDV',1),
('ML','Mali','Mali','MLI',1),
('MT','Malta','Malta','MLT',1),
('MH','Marshall Islands','Marshall Islands','MHL',1),
('MQ','Martinique','Martinique','MTQ',1),
('MR','Mauritania','Mauritania','MRT',1),
('MU','Mauritius','Mauritius','MUS',1),
('YT','Mayotte','Mayotte','MYT',1),
('MX','Mexico','Mexico','MEX',1),
('FM','Micronesia (Federated States Of)','Micronesia (Federated States Of)','FSM',1),
('MD','Moldova','Moldova','MDA',1),
('MC','Monaco','Monaco','MCO',1),
('MN','Mongolia','Mongolia','MNG',1),
('ME','Montenegro','Montenegro','MNE',1),
('MS','Montserrat','Montserrat','MSR',1),
('MA','Morocco','Morocco','MAR',1),
('MZ','Mozambique','Mozambique','MOZ',1),
('MM','Myanmar','Myanmar','MMR',1),
('NA','Namibia','Namibia','NAM',1),
('NR','Nauru','Nauru','NRU',1),
('NP','Nepal','Nepal','NPL',1),
('NL','Netherlands','Netherlands','NLD',1),
('NC','New Caledonia','New Caledonia','NCL',1),
('NZ','New Zealand','New Zealand','NZL',1),
('NI','Nicaragua','Nicaragua','NIC',1),
('NE','Niger','Niger','NER',1),
('NG','Nigeria','Nigeria','NGA',1),
('NU','Niue','Niue','NIU',1),
('NF','Norfolk Island','Norfolk Island','NFK',1),
('MK','North Macedonia','North Macedonia','MKD',1),
('MP','Northern Mariana Islands','Northern Mariana Islands','MNP',1),
('NO','Norway','Norway','NOR',1),
('OM','Oman','Oman','OMN',1),
('PK','Pakistan','Pakistan','PAK',1),
('PW','Palau','Palau','PLW',1),
('PS','State Of Palestine','State Of Palestine','PSE',1),
('PA','Panama','Panama','PAN',1),
('PG','Papua New Guinea','Papua New Guinea','PNG',1),
('PY','Paraguay','Paraguay','PRY',1),
('PE','Peru','Peru','PER',1),
('PH','Philippines','Philippines','PHL',1),
('PN','Pitcairn','Pitcairn','PCN',1),
('PL','Poland','Poland','POL',1),
('PT','Portugal','Portugal','PRT',1),
('PR','Puerto Rico','Puerto Rico','PRI',1),
('QA','Qatar','Qatar','QAT',1),
('RE','Réunion','Réunion','REU',1),
('RO','Romania','Romania','ROU',1),
('RU','Russian Federation','Russian Federation','RUS',1),
('RW','Rwanda','Rwanda','RWA',1),
('BL','Saint Barthélemy','Saint Barthélemy','BLM',1),
('SH','Saint Helena, Ascension And Tristan Da Cunha','Saint Helena, Ascension And Tristan Da Cunha','SHN',1),
('KN','Saint Kitts And Nevis','Saint Kitts And Nevis','KNA',1),
('LC','Saint Lucia','Saint Lucia','LCA',1),
('MF','Saint Martin (French Part)','Saint Martin (French Part)','MAF',1),
('PM','Saint Pierre And Miquelon','Saint Pierre And Miquelon','SPM',1),
('VC','Saint Vincent And The Grenadines','Saint Vincent And The Grenadines','VCT',1),
('WS','Samoa','Samoa','WSM',1),
('SM','San Marino','San Marino','SMR',1),
('ST','Sao Tome And Principe','Sao Tome And Principe','STP',1),
('SA','Saudi Arabia','Saudi Arabia','SAU',1),
('SN','Senegal','Senegal','SEN',1),
('RS','Serbia','Serbia','SRB',1),
('SC','Seychelles','Seychelles','SYC',1),
('SL','Sierra Leone','Sierra Leone','SLE',1),
('SG','Singapore','Singapore','SGP',1),
('SX','Sint Maarten (Dutch Part)','Sint Maarten (Dutch Part)','SXM',1),
('SK','Slovakia','Slovakia','SVK',1),
('SI','Slovenia','Slovenia','SVN',1),
('SB','Solomon Islands','Solomon Islands','SLB',1),
('SO','Somalia','Somalia','SOM',1),
('ZA','South Africa','South Africa','ZAF',1),
('GS','South Georgia And The South Sandwich Islands','South Georgia And The South Sandwich Islands','SGS',1),
('SS','South Sudan','South Sudan','SSD',1),
('ES','Spain','Spain','ESP',1),
('LK','Sri Lanka','Sri Lanka','LKA',1),
('SD','Sudan','Sudan','SDN',1),
('SR','Suriname','Suriname','SUR',1),
('SJ','Svalbard And Jan Mayen','Svalbard And Jan Mayen','SJM',1),
('SE','Sweden','Sweden','SWE',1),
('CH','Switzerland','Switzerland','CHE',1),
('SY','Syrian Arab Republic','Syrian Arab Republic','SYR',1),
('TW','Taiwan','Taiwan','TWN',1),
('TJ','Tajikistan','Tajikistan','TJK',1),
('TZ','Tanzania','Tanzania','TZA',1),
('TH','Thailand','Thailand','THA',1),
('TL','Timor-Leste','Timor-Leste','TLS',1),
('TG','Togo','Togo','TGO',1),
('TK','Tokelau','Tokelau','TKL',1),
('TO','Tonga','Tonga','TON',1),
('TT','Trinidad And Tobago','Trinidad And Tobago','TTO',1),
('TN','Tunisia','Tunisia','TUN',1),
('TR','Turkey','Turkey','TUR',1),
('TM','Turkmenistan','Turkmenistan','TKM',1),
('TC','Turks And Caicos Islands','Turks And Caicos Islands','TCA',1),
('TV','Tuvalu','Tuvalu','TUV',1),
('UG','Uganda','Uganda','UGA',1),
('UA','Ukraine','Ukraine','UKR',1),
('AE','United Arab Emirates','United Arab Emirates','ARE',1),
('GB','United Kingdom','United Kingdom','GBR',1),
('US','United States','United States','USA',1),
('UM','United States Minor Outlying Islands','United States Minor Outlying Islands','UMI',1),
('UY','Uruguay','Uruguay','URY',1),
('UZ','Uzbekistan','Uzbekistan','UZB',1),
('VU','Vanuatu','Vanuatu','VUT',1),
('VE','Venezuela','Venezuela','VEN',1),
('VN','Viet Nam','Viet Nam','VNM',1),
('VG','Virgin Islands (British)','Virgin Islands (British)','VGB',1),
('VI','Virgin Islands (U.S.)','Virgin Islands (U.S.)','VIR',1),
('WF','Wallis And Futuna','Wallis And Futuna','WLF',1),
('EH','Western Sahara','Western Sahara','ESH',1),
('YE','Yemen','Yemen','YEM',1),
('ZM','Zambia','Zambia','ZMB',1),
('ZW','Zimbabwe','Zimbabwe','ZWE',1);

update CTZ_Countries set warning = 1 where ID_Country = 45 --china
update CTZ_Countries set warning = 1 where ID_Country = 241 --vietnam
update CTZ_Countries set warning = 1 where ID_Country = 103 --india

--======== CTZ_Departments==========
select * from CTZ_Departments

insert into CTZ_Departments values ('Sales')
insert into CTZ_Departments values ('Data Management')
insert into CTZ_Departments values ('Engineering')
insert into CTZ_Departments values ('Foreign Trade')
insert into CTZ_Departments values ('Disposition')

select * from CTZ_Employee_Plants
select * from CTZ_Employee_Departments

--======== CTZ_Assigment_Status ==========
Select * from CTZ_Assignment_Status
insert Into CTZ_Assignment_Status values ('PENDING', 1)   --	La cotización fue asignada pero el usuario aún no la ha revisado ni iniciado ninguna actividad.
insert Into CTZ_Assignment_Status values ('IN_PROGRESS', 1) -- El usuario ya comenzó a trabajar o revisar la cotización Clic en Start.
insert Into CTZ_Assignment_Status values ('ON_HOLD', 1) -- La asignación está detenida, por ejemplo a la espera de información o aprobación de terceros.
insert Into CTZ_Assignment_Status values ('REJECTED', 1) -- Rechazado
insert Into CTZ_Assignment_Status values ('ON_REVIEWED', 1) -- Revisada (más detalle)
insert Into CTZ_Assignment_Status values ('APPROVED', 1) -- Aprobada