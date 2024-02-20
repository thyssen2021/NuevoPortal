--USE Portal_2_0
DECLARE @count INT;

CREATE TABLE #stats_ddl(
    [id_budget_rel_fy_centro] [int] NOT NULL,
	[id_cuenta_sap] [int] NOT NULL,
	[mes] [int] NOT NULL,
	[currency_iso] [varchar](3) NOT NULL,
	[cantidad] [decimal](14, 2) NOT NULL,
	[comentario] [varchar](150) NULL,
	[moneda_local_usd] [bit] NOT NULL,
);

INSERT INTO #stats_ddl 
SELECT id_budget_rel_fy_centro, id_cuenta_sap, mes, currency_iso, cantidad, comentario, moneda_local_usd FROM budget_cantidad

SELECT @count = COUNT(*) FROM #stats_ddl;

WHILE @count > 0
BEGIN
  --  DECLARE @fechaMov VARCHAR(50) = (SELECT TOP(1) cantidad + currency_iso + moneda_local_usd FROM #stats_ddl);
	DECLARE @cantidad decimal(14,2) = (select top (1) cantidad from #stats_ddl) 

	--INSERT nuevo valor
INSERT INTO [dbo].[budget_cantidad]
           ([id_budget_rel_fy_centro]
           ,[id_cuenta_sap]
           ,[mes]
           ,[currency_iso]
           ,[cantidad]
           ,[comentario]
           ,[moneda_local_usd])
     VALUES
           ((select top (1) id_budget_rel_fy_centro from #stats_ddl) 
           ,(select top (1) id_cuenta_sap from #stats_ddl) 
           ,(select top (1) mes from #stats_ddl) 
           ,'USD'  --Valor por defecto
           ,(select top (1) cantidad from #stats_ddl) 
           , null -- no necesario, el comentario ya se encuentra en la moneda local
           ,0  -- no es la moneda local
		   )



    DELETE TOP (1) FROM #stats_ddl
    SELECT @count = COUNT(*) FROM #stats_ddl;
END

DROP TABLE #stats_ddl;

--select count(*) from budget_cantidad