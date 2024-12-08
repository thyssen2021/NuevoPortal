USE Portal_2_0
GO
IF object_id(N'IT_asignacion_software',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IT_asignacion_software]
      PRINT '<<< IT_asignacion_software en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO
GO
IF object_id(N'IT_inventory_software_versions',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IT_inventory_software_versions]
      PRINT '<<< IT_inventory_software_versions en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Borra la tabla de versiones de software
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/06/06
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/
