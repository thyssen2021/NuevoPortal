USE Portal_2_0
GO
IF object_id(N'PFA_Reason',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[PFA_Reason]
      PRINT '<<< PFA_Reason en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los PFA_Reason
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2021/11/01
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [PFA_Reason](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[descripcion] [varchar](30) NOT NULL,
	[activo] [bit] NOT NULL	
 CONSTRAINT [PK_PFA_Reason] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restricción default
ALTER TABLE [PFA_Reason] ADD  CONSTRAINT [DF_PFA_Reason_activo]  DEFAULT (1) FOR [activo]
GO


SET IDENTITY_INSERT [PFA_Reason] ON 

INSERT [PFA_Reason] ([id], [descripcion], [activo]) VALUES (1, N'Supplier delay', 1)
INSERT [PFA_Reason] ([id], [descripcion], [activo]) VALUES (2, N'tkMM increase', 1)
INSERT [PFA_Reason] ([id], [descripcion], [activo]) VALUES (3, N'Customer increase', 1)

SET IDENTITY_INSERT [PFA_Reason] OFF
GO




 	  
IF object_id(N'PFA_Reason',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< PFA_Reason en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla PFA_Reason  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
