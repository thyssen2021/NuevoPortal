--USE Portal_2_0
GO
IF object_id(N'IT_UsuariosActivos',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IT_UsuariosActivos]
      PRINT '<<< IT_UsuariosActivos en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar las IT_UsuariosActivos
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2024/03/11
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[IT_UsuariosActivos](
	[id] [int] IDENTITY(1,1) NOT NULL,
	Numero [varchar](6) NULL,
	ApellidoPaterno [varchar](50) NULL,
	ApellidoMaterno [varchar](50) NULL,
	Nombre [varchar](50) NULL,
	Antiguedad [varchar](15) NULL,
	Puesto [varchar](50) NULL,
	Departamento [varchar](50) NULL,
	Planta [varchar](5) NULL,
	Area [varchar](5) NULL,
	DepartamentoNum [varchar](6) NULL,
	Genero [varchar](1) NULL,
	C8ID [varchar](8) NULL,
	updateTime datetime not null

 CONSTRAINT [PK_IT_UsuariosActivos] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


IF object_id(N'IT_UsuariosActivos',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IT_UsuariosActivos en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IT_UsuariosActivos  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END