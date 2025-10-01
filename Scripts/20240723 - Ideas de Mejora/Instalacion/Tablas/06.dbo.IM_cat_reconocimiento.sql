GO
IF object_id(N'IM_cat_reconocimiento',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IM_cat_reconocimiento]
      PRINT '<<< IM_cat_reconocimiento en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los IM_cat_impacto
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2021/12/09
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE IM_cat_reconocimiento(
	[id] [int] IDENTITY(1,1) NOT NULL,
	[activo] [bit] NOT NULL,
	[descripcion] [varchar](50) NOT NULL,
 CONSTRAINT [PK_ideaMejoraReconocimiento] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

SET IDENTITY_INSERT IM_cat_reconocimiento ON 
GO
INSERT IM_cat_reconocimiento ([id], [activo], [descripcion]) VALUES (2, 1, N'Sin premio')
GO
INSERT IM_cat_reconocimiento ([id], [activo], [descripcion]) VALUES (3, 1, N'Tarjeta')
GO
INSERT IM_cat_reconocimiento ([id], [activo], [descripcion]) VALUES (4, 1, N'Comida')
GO
INSERT IM_cat_reconocimiento ([id], [activo], [descripcion]) VALUES (6, 1, N'Monto')
GO
SET IDENTITY_INSERT IM_cat_reconocimiento OFF

IF object_id(N'IM_cat_reconocimiento',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IM_cat_reconocimiento en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IM_cat_reconocimiento  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO

---Actualiza los montos de reconocimiento
update IM_cat_reconocimiento set activo = 0  where id <> 2
update IM_cat_reconocimiento set activo = 1  where id = 4
insert into IM_cat_reconocimiento (activo, descripcion) values (1,'Depósito Nómina')
insert into IM_cat_reconocimiento (activo, descripcion) values (1,'Souvenir')