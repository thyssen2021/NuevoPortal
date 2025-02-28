--Agrega el mes a la tabla de tipo de cambio 
ALTER TABLE [dbo].[budget_rel_tipo_cambio_fy]
ADD [mes] INT NOT NULL DEFAULT (0);

--  Elimina la restricción única existente
ALTER TABLE [dbo].[budget_rel_tipo_cambio_fy]
DROP CONSTRAINT UQ_budget_rel_tipo_cambio_fy_anio_tipo_cambio;

--condicion para que no se repitan fy, mes y tipo de cambio
ALTER TABLE [dbo].[budget_rel_tipo_cambio_fy]
ADD CONSTRAINT UQ_tipo_cambio_fy_mes UNIQUE (id_budget_anio_fiscal, mes, id_tipo_cambio);

-- Inserta 12 registros nuevos por cada registro actual (con mes = 0)
INSERT INTO [dbo].[budget_rel_tipo_cambio_fy] (id_budget_anio_fiscal, id_tipo_cambio, cantidad, mes)
SELECT 
    t.id_budget_anio_fiscal,
    t.id_tipo_cambio,
    t.cantidad,
    v.mes
FROM [dbo].[budget_rel_tipo_cambio_fy] AS t
CROSS JOIN (VALUES (1),(2),(3),(4),(5),(6),(7),(8),(9),(10),(11),(12)) AS v(mes)
WHERE t.mes = 0 
  AND t.id_budget_anio_fiscal BETWEEN 1 AND 51;

-- Elimina los registros originales con mes = 0
DELETE FROM [dbo].[budget_rel_tipo_cambio_fy]
WHERE mes = 0
  AND id_budget_anio_fiscal BETWEEN 1 AND 51;

-- deja los TC vacios
update budget_rel_tipo_cambio_fy set cantidad = 0


--- SP cambio de TC
GO
CREATE PROCEDURE usp_ActualizarBudgetCantidad
    @id_fy INT,
    @month INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @usd_mxn FLOAT;
    DECLARE @eur_usd FLOAT;
    DECLARE @count INT;

    -- Se crea una tabla temporal para almacenar los registros a actualizar
    CREATE TABLE #stats_ddl(
        id INT, 
        id_budget_rel_fy_centro INT,
        id_cuenta_sap INT,
        mes INT,
        currency_iso VARCHAR(3),
        cantidad DECIMAL(14,2),
        comentario VARCHAR(150),
        moneda_local_usd BIT
    );

    -- Inserta los registros a partir de la condición indicada
    INSERT INTO #stats_ddl 
    SELECT c.*
    FROM budget_cantidad AS c 
    JOIN budget_rel_fy_centro AS yc ON yc.id = c.id_budget_rel_fy_centro
    WHERE id_anio_fiscal = @id_fy 
      AND moneda_local_usd = 1 
      AND mes = @month;

    SELECT @count = COUNT(*) FROM #stats_ddl;

    WHILE @count > 0
    BEGIN
        DECLARE @mes INT = (SELECT TOP(1) mes FROM #stats_ddl);
        DECLARE @cuenta_sap INT = (SELECT TOP(1) id_cuenta_sap FROM #stats_ddl);
        DECLARE @id_rel_fy_cc INT = (SELECT TOP(1) id_budget_rel_fy_centro FROM #stats_ddl);
        DECLARE @id_temp INT = (SELECT TOP(1) id FROM #stats_ddl);

        DECLARE @dato_mxn FLOAT = (SELECT TOP(1) cantidad 
                                   FROM budget_cantidad 
                                   WHERE mes = @mes 
                                     AND currency_iso = 'MXN' 
                                     AND id_cuenta_sap = @cuenta_sap 
                                     AND moneda_local_usd = 0 
                                     AND id_budget_rel_fy_centro = @id_rel_fy_cc);
        DECLARE @dato_usd FLOAT = (SELECT TOP(1) cantidad 
                                   FROM budget_cantidad 
                                   WHERE mes = @mes 
                                     AND currency_iso = 'USD' 
                                     AND id_cuenta_sap = @cuenta_sap 
                                     AND moneda_local_usd = 0 
                                     AND id_budget_rel_fy_centro = @id_rel_fy_cc);
        DECLARE @dato_eur FLOAT = (SELECT TOP(1) cantidad 
                                   FROM budget_cantidad 
                                   WHERE mes = @mes 
                                     AND currency_iso = 'EUR' 
                                     AND id_cuenta_sap = @cuenta_sap 
                                     AND moneda_local_usd = 0 
                                     AND id_budget_rel_fy_centro = @id_rel_fy_cc);

        -- Obtiene los factores de tipo de cambio
        SET @usd_mxn = (SELECT TOP(1) cantidad 
                        FROM budget_rel_tipo_cambio_fy 
                        WHERE mes = @mes 
                          AND id_budget_anio_fiscal = @id_fy 
                          AND id_tipo_cambio = 1);
        SET @eur_usd = (SELECT TOP(1) cantidad 
                        FROM budget_rel_tipo_cambio_fy 
                        WHERE mes = @mes 
                          AND id_budget_anio_fiscal = @id_fy 
                          AND id_tipo_cambio = 2);

        -- Calcula el nuevo valor en moneda local
        DECLARE @nuevo_valor FLOAT = (ISNULL(@dato_mxn, 0) / @usd_mxn)
                                     + ISNULL(@dato_usd, 0)
                                     + (ISNULL(@dato_eur, 0) * @eur_usd);

        -- Actualiza el registro correspondiente
        UPDATE budget_cantidad 
        SET cantidad = CONVERT(DECIMAL(10,2), @nuevo_valor)  
        WHERE id = @id_temp;

        -- Elimina el registro procesado de la tabla temporal
        DELETE TOP (1) FROM #stats_ddl;

        SELECT @count = COUNT(*) FROM #stats_ddl;
    END

    DROP TABLE #stats_ddl;
END
GO


