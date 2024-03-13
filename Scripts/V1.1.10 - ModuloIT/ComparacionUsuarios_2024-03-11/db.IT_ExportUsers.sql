--USE Portal_2_0
GO
IF object_id(N'IT_ExportUsers',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IT_ExportUsers]
      PRINT '<<< IT_ExportUsers en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar las IT_ExportUsers
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2024/03/11
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[IT_ExportUsers](
	[id] [int] IDENTITY(1,1) NOT NULL,
	C8ID [varchar](14) NULL,
	userPrincipalName [varchar](60) NULL,
	displayName [varchar](60) NULL,
	surname [varchar](60) NULL,
	mail [varchar](100) NULL,
	givenName [varchar](50) NULL,
	C_id [varchar](50) NULL,
	userType [varchar](12) NULL,
	jobTitle [varchar](60) NULL,
	department [varchar](25) NULL,
	accountEnabled [varchar](10) NULL,
	usageLocation [varchar](5) NULL,
	streetAddress [varchar](60) NULL,
	[state][varchar](20) NULL,
	country [varchar](5) NULL,
	officeLocation [varchar](20) NULL,
	city [varchar](30) NULL,
	postalCode [varchar](5) NULL,
	telephoneNumber [varchar](25) NULL,
	mobilePhone [varchar](25) NULL,
	alternateEmailAddress [varchar](120) NULL,
	ageGroup [varchar](10) NULL,
	consentProvidedForMinor [varchar](10) NULL,
	legalAgeGroupClassification [varchar](10) NULL,
	companyName [varchar](60) NULL,
	creationType [varchar](5) NULL,
	directorySynced [varchar](5) NULL,
	invitationState [varchar](5) NULL,
	identityIssuer [varchar](60) NULL,
	createdDateTime [varchar](30) NULL,
	updateTime datetime not null

 CONSTRAINT [PK_IT_ExportUsers] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


IF object_id(N'IT_ExportUsers',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IT_ExportUsers en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IT_ExportUsers  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END