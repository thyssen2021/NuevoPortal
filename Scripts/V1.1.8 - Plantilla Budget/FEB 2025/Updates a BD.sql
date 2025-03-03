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
IF OBJECT_ID('dbo.usp_ActualizarBudgetCantidad', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_ActualizarBudgetCantidad;
GO

CREATE PROCEDURE dbo.usp_ActualizarBudgetCantidad
    @id_fy   INT,  -- Año fiscal
    @month   INT   -- Mes a procesar
AS
BEGIN
    SET NOCOUNT ON;

    ----------------------------------------------------------------------------
    -- 1) Insertar fila local si NO existe y sí hay al menos 1 valor (sum > 0)
    ----------------------------------------------------------------------------
    INSERT INTO budget_cantidad (
        id_budget_rel_fy_centro,
        id_cuenta_sap,
        mes,
        currency_iso,
        cantidad,
        comentario,
        moneda_local_usd
    )
    SELECT 
        bc.id_budget_rel_fy_centro,
        bc.id_cuenta_sap,
        bc.mes,
        'USD' AS currency_iso,
        0     AS cantidad,
        NULL  AS comentario,
        1     AS moneda_local_usd
    FROM 
    (
        SELECT 
            c.id_budget_rel_fy_centro,
            c.id_cuenta_sap,
            c.mes
        FROM budget_cantidad c
        JOIN budget_rel_fy_centro fy ON fy.id = c.id_budget_rel_fy_centro
        WHERE 
            fy.id_anio_fiscal = @id_fy
            AND c.mes = @month
            -- Sólo filas donde la moneda sea MXN, USD o EUR y sea "no local"
            AND c.currency_iso IN ('MXN','USD','EUR')
            AND c.moneda_local_usd = 0
        GROUP BY 
            c.id_budget_rel_fy_centro,
            c.id_cuenta_sap,
            c.mes
        HAVING SUM(c.cantidad) > 0  -- al menos uno con cantidad > 0
    ) AS bc
    WHERE NOT EXISTS (
        SELECT 1
        FROM budget_cantidad bc2
        WHERE bc2.id_budget_rel_fy_centro = bc.id_budget_rel_fy_centro
          AND bc2.id_cuenta_sap = bc.id_cuenta_sap
          AND bc2.mes = bc.mes
          AND bc2.moneda_local_usd = 1
    );

    ----------------------------------------------------------------------------
    -- 2) Eliminar fila local si NO hay cantidades en MXN, USD o EUR > 0
    ----------------------------------------------------------------------------
    DELETE bcLocal
    FROM budget_cantidad bcLocal
    JOIN budget_rel_fy_centro fy 
         ON fy.id = bcLocal.id_budget_rel_fy_centro
    WHERE 
        fy.id_anio_fiscal = @id_fy
        AND bcLocal.mes = @month
        AND bcLocal.moneda_local_usd = 1
        -- No existe ninguna fila con MXN, USD o EUR que tenga cantidad > 0
        AND NOT EXISTS (
            SELECT 1 
            FROM budget_cantidad bcNon
            WHERE bcNon.id_budget_rel_fy_centro = bcLocal.id_budget_rel_fy_centro
              AND bcNon.id_cuenta_sap = bcLocal.id_cuenta_sap
              AND bcNon.mes = bcLocal.mes
              AND bcNon.moneda_local_usd = 0
              AND bcNon.currency_iso IN ('MXN','USD','EUR')
              AND bcNon.cantidad > 0
        );

    ----------------------------------------------------------------------------
    -- 3) Actualizar fila local sumando (MXN→USD) + USD + (EUR→USD)
    ----------------------------------------------------------------------------

    ;WITH cteLocal AS (
        SELECT 
            c.id,
            c.id_budget_rel_fy_centro,
            c.id_cuenta_sap,
            c.mes
        FROM budget_cantidad c
        JOIN budget_rel_fy_centro fy 
            ON fy.id = c.id_budget_rel_fy_centro
        WHERE 
            fy.id_anio_fiscal = @id_fy
            AND c.mes = @month
            AND c.moneda_local_usd = 1
    ),
    cteTC AS (
        -- Obtenemos en una sola fila los tipos de cambio USD/MXN (id_tipo_cambio=1) y EUR/USD (id_tipo_cambio=2)
        SELECT
            @month AS mes,
            MAX(CASE WHEN id_tipo_cambio = 1 THEN cantidad END) AS usd_mxn,
            MAX(CASE WHEN id_tipo_cambio = 2 THEN cantidad END) AS eur_usd
        FROM budget_rel_tipo_cambio_fy
        WHERE 
            id_budget_anio_fiscal = @id_fy
            AND mes = @month
    )
    UPDATE bcLocal
    SET bcLocal.cantidad = CONVERT(DECIMAL(10,2),
          (ISNULL(bcMXN.cantidad, 0) / cteTC.usd_mxn)
        +  ISNULL(bcUSD.cantidad, 0)
        +  (ISNULL(bcEUR.cantidad, 0) * cteTC.eur_usd)
    )
    FROM cteLocal
    JOIN budget_cantidad bcLocal 
        ON bcLocal.id = cteLocal.id
    CROSS JOIN cteTC
    LEFT JOIN budget_cantidad bcMXN 
        ON bcMXN.id_budget_rel_fy_centro = cteLocal.id_budget_rel_fy_centro
       AND bcMXN.id_cuenta_sap = cteLocal.id_cuenta_sap
       AND bcMXN.mes = cteLocal.mes
       AND bcMXN.currency_iso = 'MXN'
       AND bcMXN.moneda_local_usd = 0
    LEFT JOIN budget_cantidad bcUSD
        ON bcUSD.id_budget_rel_fy_centro = cteLocal.id_budget_rel_fy_centro
       AND bcUSD.id_cuenta_sap = cteLocal.id_cuenta_sap
       AND bcUSD.mes = cteLocal.mes
       AND bcUSD.currency_iso = 'USD'
       AND bcUSD.moneda_local_usd = 0
    LEFT JOIN budget_cantidad bcEUR
        ON bcEUR.id_budget_rel_fy_centro = cteLocal.id_budget_rel_fy_centro
       AND bcEUR.id_cuenta_sap = cteLocal.id_cuenta_sap
       AND bcEUR.mes = cteLocal.mes
       AND bcEUR.currency_iso = 'EUR'
       AND bcEUR.moneda_local_usd = 0;

END
GO

-- ================ Table budget_target =====================
--- 
CREATE TABLE [dbo].[budget_target](
    [id] [int] IDENTITY(1,1) NOT NULL,
    [id_cuenta_sap] [int] NOT NULL,       -- FK a budget_cuenta_sap
    [id_centro_costo] [int] NOT NULL,     -- FK a budget_centro_costo
    [id_anio_fiscal] [int] NOT NULL,      -- FK a budget_anio_fiscal
    [target] [float] NOT NULL,            -- Valor del target
    [activado] [bit] NOT NULL,            -- Indica si está activado o no
 CONSTRAINT [PK_budget_target] PRIMARY KEY CLUSTERED 
(
    [id] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[budget_target] WITH CHECK ADD CONSTRAINT [FK_budget_target_cuenta_sap] FOREIGN KEY([id_cuenta_sap])
REFERENCES [dbo].[budget_cuenta_sap] ([id])
GO
ALTER TABLE [dbo].[budget_target] CHECK CONSTRAINT [FK_budget_target_cuenta_sap]
GO

ALTER TABLE [dbo].[budget_target] WITH CHECK ADD CONSTRAINT [FK_budget_target_centro_costo] FOREIGN KEY([id_centro_costo])
REFERENCES [dbo].[budget_centro_costo] ([id])
GO
ALTER TABLE [dbo].[budget_target] CHECK CONSTRAINT [FK_budget_target_centro_costo]
GO

ALTER TABLE [dbo].[budget_target] WITH CHECK ADD CONSTRAINT [FK_budget_target_anio_fiscal] FOREIGN KEY([id_anio_fiscal])
REFERENCES [dbo].[budget_anio_fiscal] ([id])
GO
ALTER TABLE [dbo].[budget_target] CHECK CONSTRAINT [FK_budget_target_anio_fiscal]
GO



