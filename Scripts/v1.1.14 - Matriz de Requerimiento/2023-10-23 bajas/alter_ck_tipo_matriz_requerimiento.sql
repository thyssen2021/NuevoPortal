USE [portal_2_0_calidad]
GO

ALTER TABLE [dbo].[IT_matriz_requerimientos] DROP CONSTRAINT [CK_it_matriz_requerimientos_tipo]
GO

ALTER TABLE [dbo].[IT_matriz_requerimientos]  WITH CHECK ADD  CONSTRAINT [CK_it_matriz_requerimientos_tipo] CHECK  (([tipo]='MODIFICACION' OR [tipo]='CREACION' OR [tipo]='BAJA'))
GO

ALTER TABLE [dbo].[IT_matriz_requerimientos] CHECK CONSTRAINT [CK_it_matriz_requerimientos_tipo]
GO


