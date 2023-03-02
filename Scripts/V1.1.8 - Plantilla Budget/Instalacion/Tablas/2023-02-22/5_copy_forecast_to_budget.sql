USE [Portal_2_0]
GO

--borra los valores previos de la tabla
IF object_id(N'budget_cantidad_budget_historico',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[budget_cantidad_budget_historico]
      PRINT '<<< budget_cantidad_budget_historico en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

-- copia los valores a la tabla
SELECT * INTO [budget_cantidad_budget_historico] FROM [budget_cantidad]

-- verifica si se copiaron los datos
select * from budget_cantidad_budget_historico