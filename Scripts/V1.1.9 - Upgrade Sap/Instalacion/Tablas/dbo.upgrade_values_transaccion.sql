USE Portal_2_0
GO
IF object_id(N'upgrade_values_transaccion',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[upgrade_values_transaccion]
      PRINT '<<< upgrade_values_transaccion en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los upgrade_values_transaccion
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/03/25
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [upgrade_values_transaccion](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_revision] [int] NOT NULL,
	[transaccion][varchar](30) NOT NULL,
	[estatus][varchar](20) NOT NULL,
	[nota][varchar](250) NULL,
	[activo] [bit] NOT NULL	
 CONSTRAINT [PK_upgrade_values_transaccion] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restriccion de clave foranea
  alter table [upgrade_values_transaccion]
 add constraint FK_upgrade_values_transaccion_revision
  foreign key (id_revision)
  references upgrade_revision(id);

  -- restricion check
ALTER TABLE [upgrade_values_transaccion] ADD CONSTRAINT CK_upgrade_values_transaccion_Estatus CHECK ([estatus] IN 
('PENDIENTE','OK','NO OK','N/A')
)
 	  
IF object_id(N'upgrade_values_transaccion',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< upgrade_values_transaccion en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla upgrade_values_transaccion  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
