--USE Portal_2_0
GO
IF object_id(N'SCDM_cat_dias_feriados',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[SCDM_cat_dias_feriados]
      PRINT '<<< SCDM_cat_dias_feriados en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:     Table
*  Funcion: Crea una tabla para guardar los SCDM_cat_dias_feriados
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[SCDM_cat_dias_feriados](
	[id] [int] IDENTITY(1,1) NOT NULL,	
	[fecha] [datetime] NOT NULL,
	[motivo] [varchar](50) NOT NULL,
 CONSTRAINT [PK_SCDM_cat_dias_feriados_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

--Inserta valores

SET IDENTITY_INSERT [dbo].[SCDM_cat_dias_feriados] ON 
SET IDENTITY_INSERT [dbo].[SCDM_cat_dias_feriados] OFF
	  
IF object_id(N'SCDM_cat_dias_feriados',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< SCDM_cat_dias_feriados en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla SCDM_cat_dias_feriados  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
