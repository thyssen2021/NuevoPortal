
			ALTER TABLE mm_v3 ADD unidad_medida varchar(15);
			PRINT 'Se ha agregado columna unidad_medida'		
		
			ALTER TABLE mm_v3 ADD size_dimensions varchar(50);
			PRINT 'Se ha agregado columna size_dimensions'		
		
		    ALTER TABLE mm_v3 ADD material_descripcion_es varchar(40);	

			ALTER TABLE mm_v3 ADD angle_a float;
			ALTER TABLE mm_v3 ADD angle_b float;
			ALTER TABLE mm_v3 ADD real_net_weight float;
			ALTER TABLE mm_v3 ADD real_gross_weight float;
			ALTER TABLE mm_v3 ADD double_pieces varchar(1);
			ALTER TABLE mm_v3 ADD coil_position varchar(80);
			--ALTER TABLE mm_v3 ADD language_key varchar(2);
			ALTER TABLE mm_v3 ADD maximum_weight_tol_positive float;
			ALTER TABLE mm_v3 ADD maximum_weight_tol_negative float;
			ALTER TABLE mm_v3 ADD minimum_weight_tol_positive float;
			ALTER TABLE mm_v3 ADD minimum_weight_tol_negative float;

		

		select * from mm_v3


