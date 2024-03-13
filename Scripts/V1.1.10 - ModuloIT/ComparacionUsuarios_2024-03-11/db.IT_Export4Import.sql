--USE Portal_2_0
GO
IF object_id(N'IT_Export4Import',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IT_Export4Import]
      PRINT '<<< IT_Export4Import en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar las IT_Export4Import
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2024/03/11
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[IT_Export4Import](
	[id] [int] IDENTITY(1,1) NOT NULL,
	C8ID varchar(10) NULL, --8id
	tksir varchar(5) NULL,
	tktitle varchar(5) NULL,
	tknameprefix varchar(10) NULL,
	lastName varchar(100) NULL,
	firstName varchar(100) NULL,
	tkbirth varchar(15) NULL,
	tksex varchar(1) NULL,
	tkstreet varchar(100) NULL,
	tkpostalcode varchar(100) NULL,
	tkpostaladdress varchar(100) NULL,
	tkaddaddon varchar(100) NULL,
	tkfedst varchar(100) NULL,
	tkcountry varchar(100) NULL,
	tkcountrykey varchar(5) NULL,
	tknationality varchar(3) NULL, 
	tkpreflang varchar(5) NULL,
	tkempno varchar(10) NULL,
	tkfkz6 varchar(6) NULL,
	tkfkzext varchar(6) NULL,
	tkuniqueid varchar(10) NULL,
	tkpstatus varchar(5) NULL,
	tkcostcenter varchar(4) NULL,
	tkdepartment varchar(50) NULL,
	tkfunction varchar(60) NULL,
	tkorgstreet varchar(50) NULL,
	tkorgpostalcode varchar(5) NULL,
	tkorgpostaladdress varchar(50) NULL,
	tkorgaddonaddr varchar(50) NULL,
	tkorgfedst varchar(20) NULL,
	tkorgcountry varchar(40) NULL,
	tkorgcountrykey varchar(5) NULL,
	tkapsite varchar(20) NULL,
	tkorgkey varchar(10) NULL,
	tkbuilding varchar(10) NULL,
	email varchar(120) NULL,
	tkareacode varchar(20) NULL,
	tkphoneext varchar(20) NULL,
	tkorgfax varchar(20) NULL,
	tkmobile varchar(20) NULL,
	tkgodfather varchar(8) NULL,
	tkprefdelmethod varchar(1) NULL,
	tkedateorg varchar(15) NULL,
	tkedatetrust varchar(15) NULL,
	tkldateorg varchar(15) NULL,
	tklreason varchar(10) NULL,
	shares varchar(1) NULL,
	supervisoryboardelection varchar(1) NULL,
	tkbkz varchar(5) NULL,
	tkinside varchar(1) NULL,
	updateTime datetime not null

 CONSTRAINT [PK_IT_Export4Import] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


IF object_id(N'IT_Export4Import',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IT_Export4Import en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IT_Export4Import  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END