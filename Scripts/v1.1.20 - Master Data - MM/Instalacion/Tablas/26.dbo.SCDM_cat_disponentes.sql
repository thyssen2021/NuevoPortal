--USE Portal_2_0
GO
IF object_id(N'SCDM_cat_disponentes',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[SCDM_cat_disponentes]
      PRINT '<<< SCDM_cat_disponentes en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los SCDM_cat_disponentes
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[SCDM_cat_disponentes](
	[id] [int] IDENTITY(1,1) NOT NULL,		
	[id_empleado][int] NOT NULL,	
	[activo] [bit]  NOT NULL DEFAULT 1,	
 CONSTRAINT [PK_SCDM_cat_disponentes_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


    -- restriccion de clave foranea
alter table [dbo].[SCDM_cat_disponentes]
 add constraint FK_SCDM_cat_disponentes_id_empleado
  foreign key (id_empleado)
  references empleados(id);


SET IDENTITY_INSERT [dbo].[SCDM_cat_disponentes] ON 

INSERT INTO [dbo].[SCDM_cat_disponentes]([id],[id_empleado],[activo])VALUES(1,153,1) --karina
INSERT INTO [dbo].[SCDM_cat_disponentes]([id],[id_empleado],[activo])VALUES(2,101,1) --humberto d
INSERT INTO [dbo].[SCDM_cat_disponentes]([id],[id_empleado],[activo])VALUES(3,151,1) --fernando r
 
SET IDENTITY_INSERT [dbo].[SCDM_cat_disponentes] OFF

	  
IF object_id(N'SCDM_cat_disponentes',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< SCDM_cat_disponentes en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla SCDM_cat_disponentes  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
