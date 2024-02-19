/*****************************************
* Se dehabilita la edición de todos los centros de costo, se habilitarán
* el año fiscal según las indicaciones de Carmen
******************************************/

--Habilita CeCo
update budget_rel_fy_centro set estatus=1
--Deshabilita CeCo
update budget_rel_fy_centro set estatus=0
