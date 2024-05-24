
--Crea las funciones para obtener el peso del historico de pesos

IF OBJECT_ID (N'GetNetWeight', N'IF') IS NOT NULL
-- deletes function
    DROP FUNCTION GetNetWeight;

GO
CREATE FUNCTION GetNetWeight(@DateTimeUTC datetime2(7), @material varchar(20))
RETURNS TABLE AS RETURN
SELECT top 1 net_weight FROM bom_pesos FOR SYSTEM_TIME AS OF @DateTimeUTC where material = @material
GO


IF OBJECT_ID (N'GetGrossWeight', N'IF') IS NOT NULL
-- deletes function
    DROP FUNCTION GetGrossWeight;

GO
CREATE FUNCTION GetGrossWeight(@DateTimeUTC datetime2(7), @material varchar(20))
RETURNS TABLE AS RETURN
SELECT top 1 gross_weight FROM bom_pesos FOR SYSTEM_TIME AS OF @DateTimeUTC where material = @material
GO

IF OBJECT_ID (N'GetBOM', N'IF') IS NOT NULL
-- deletes function
    DROP FUNCTION GetBOM;

GO
CREATE FUNCTION GetBOM(@DateTimeUTC datetime2(7), @material varchar(20), @plant varchar(20))
RETURNS TABLE AS RETURN
SELECT top 1 * FROM bom_pesos FOR SYSTEM_TIME AS OF @DateTimeUTC where material = @material AND plant = @plant
GO


select * from view_datos_base_reporte_pesadas

select * from bom_pesos