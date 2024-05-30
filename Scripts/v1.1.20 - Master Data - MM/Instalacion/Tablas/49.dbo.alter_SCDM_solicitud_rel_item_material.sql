use Portal_2_0
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*****************************************************************************
*  Tipo de objeto:     Alter table
*  Autor :  Alfredo Xochitemol
*  Fecha de Creación: 
*  Log de Mantenimiento: 
*  Date          Modified By             Description            
*  ----------    --------------------    -------------------------------------
*  21/03/2023	Alfredo Xochitemol		 Se aumenta tamaño de campos
******************************************************************************/

--BEGIN TRANSACTION
IF object_id(N'SCDM_solicitud_rel_item_material','U') IS NOT NULL
BEGIN			
	--platinas/platina_soldada/shearing
	ALTER TABLE SCDM_solicitud_rel_item_material ADD avance_mm float null;
	ALTER TABLE SCDM_solicitud_rel_item_material ADD avance_tolerancia_negativa_mm float null;
	ALTER TABLE SCDM_solicitud_rel_item_material ADD avance_tolerancia_positiva_mm float null;
	ALTER TABLE SCDM_solicitud_rel_item_material ADD id_forma int null; --FK
	ALTER TABLE SCDM_solicitud_rel_item_material ADD piezas_por_golpe int null;
	ALTER TABLE SCDM_solicitud_rel_item_material ADD piezas_por_paquete int null;
	ALTER TABLE SCDM_solicitud_rel_item_material ADD peso_bruto float null;
	ALTER TABLE SCDM_solicitud_rel_item_material ADD peso_neto float null;
	ALTER TABLE SCDM_solicitud_rel_item_material ADD piezas_por_auto int null;
	ALTER TABLE SCDM_solicitud_rel_item_material ADD peso_inicial float null;
	ALTER TABLE SCDM_solicitud_rel_item_material ADD porcentaje_scrap_puntas_colas float null;
	ALTER TABLE SCDM_solicitud_rel_item_material ADD conciliacion_scrap_ingenieria bit null;
	ALTER TABLE SCDM_solicitud_rel_item_material ADD angulo_a int null;
	ALTER TABLE SCDM_solicitud_rel_item_material ADD angulo_b int null;
	
	  -- restriccion de clave foranea
	alter table [dbo].[SCDM_solicitud_rel_item_material]
	add constraint FK_SCDM_solicitud_rel_item_material_id_forma
	foreign key (id_forma)
	references SCDM_cat_forma_material(id);

	-- C&B  
	--ALTER TABLE SCDM_solicitud_rel_item_material ADD material_compra_tkmm bit null;
	--ALTER TABLE SCDM_solicitud_rel_item_material ADD id_materia_prima_producto_terminado int null; --FK
	--ALTER TABLE SCDM_solicitud_rel_item_material ADD id_tipo_metal_cb int null; --FK
	--ALTER TABLE SCDM_solicitud_rel_item_material ADD id_aleacion int null; --FK
	--ALTER TABLE SCDM_solicitud_rel_item_material ADD precio float null; 
	--ALTER TABLE SCDM_solicitud_rel_item_material ADD id_moneda int null; -- FK
	--ALTER TABLE SCDM_solicitud_rel_item_material ADD id_incoterm int null; -- FK
	--ALTER TABLE SCDM_solicitud_rel_item_material ADD id_terminos_pago int null; -- FK
	--ALTER TABLE SCDM_solicitud_rel_item_material ADD aplica_tasa_iva bit null; 

	--Archivo excel
	ALTER TABLE SCDM_solicitud_rel_item_material ADD tratamiento_superficial varchar(30) null; 


	 -- restriccion de clave foranea
	alter table [dbo].[SCDM_solicitud_rel_item_material]
	add constraint FK_SCDM_solicitud_rel_item_material_id_materia_prima_producto_terminado
	foreign key (id_materia_prima_producto_terminado)
	references SCDM_cat_materia_prima_producto_terminado(id);

	alter table [dbo].[SCDM_solicitud_rel_item_material]
	add constraint FK_SCDM_solicitud_rel_item_material_id_tipo_metal_cb
	foreign key (id_tipo_metal_cb)
	references SCDM_cat_tipo_metal_cb(id);
	
	alter table [dbo].[SCDM_solicitud_rel_item_material]
	add constraint FK_SCDM_solicitud_rel_item_material_id_aleacion
	foreign key (id_aleacion)
	references SCDM_cat_aleacion(id);
	
	alter table [dbo].[SCDM_solicitud_rel_item_material]
	add constraint FK_SCDM_solicitud_rel_item_material_id_moneda
	foreign key (id_moneda)
	references SCDM_cat_moneda(id);

	alter table [dbo].[SCDM_solicitud_rel_item_material]
	add constraint FK_SCDM_solicitud_rel_item_material_id_incoterm
	foreign key (id_incoterm)
	references SCDM_cat_incoterm(id);

	alter table [dbo].[SCDM_solicitud_rel_item_material]
	add constraint FK_SCDM_solicitud_rel_item_material_id_terminos_pago
	foreign key (id_terminos_pago)
	references SCDM_cat_terminos_pago(id);

	

END
ELSE
BEGIN
	PRINT 'La tabla SCDM_solicitud_rel_item_material NO EXISTE, no se puede crear las columnas'
END

--COMMIT TRANSACTION

GO

