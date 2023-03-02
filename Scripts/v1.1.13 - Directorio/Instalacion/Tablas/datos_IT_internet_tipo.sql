--borra el tipo de internet y solo deja las opciones s/n
delete from IT_internet_tipo;  DBCC checkident ('IT_internet_tipo', reseed,0);

--inserta nuevos valores
INSERT INTO IT_internet_tipo (descripcion, activo) VALUES ('SI', 1)
INSERT INTO IT_internet_tipo (descripcion, activo) VALUES ('NO', 2)

select * from IT_internet_tipo

-- indica que tipos de hardware pueden asignarse
update IT_inventory_hardware_type set disponible_en_matriz_rh= 1 where id in(1,2,6,8,9,11,12,13)
--indica que campo require más detalles
update IT_inventory_hardware_type set aplica_descripcion =1 where id in (13)

--actualiza la lista de software
update IT_inventory_software set disponible_en_matriz_rh=1 --todos 
update IT_inventory_software set disponible_en_matriz_rh=0 where id in (11,24,6,25,26)
update IT_inventory_software set aplica_descripcion =1 where id in (3) --SAP
--Se agrega otro
insert IT_inventory_software (descripcion, disponible_en_matriz_rh, aplica_descripcion, activo) VALUES ('Otro',1,1,1)