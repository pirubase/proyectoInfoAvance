-- ================================================================
-- Ubicar aqui los cambios que se desean aplicar a la base de datos
-- ================================================================

--- se ejecutaron?---
UPDATE t005_menu
	SET f005_nombre='historiales'
	WHERE f005_rowid=7;
UPDATE t005_menu
	SET f005_nombre='vehiculos'
	WHERE f005_rowid=8;

create table t017_gestion_cliente (

f017_rowid int8 not null primary key GENERATED ALWAYS AS identity(START WITH 1 INCREMENT BY 1),
f017_ts  timestamp not null,
f017_plan_basic bool not null,
f017_plan_estandar bool not null,
f017_plan_pro bool not null,
f017_plan_enterprise bool not null,
f017_suscripcion_mensual_pagada bool not null,
f017_mensaje_cliente_aviso_suspencion varchar(8000) null,
f017_mensaje_aviso_cliente varchar(8000) null,
f017_numero_usuarios int8 null,
f017_rowid_empresa_o_persona_natural int8 not null



);

	--t017 t002
alter table t017_gestion_cliente  add  constraint fk_t017_t002 
foreign key(f017_rowid_empresa_o_persona_natural)
references t002_empresa_o_persona_natural (f002_rowid);

-- nuevo si ya se ejecuto "se ejecutaron?"--

-- Menus
INSERT INTO t005_menu (f005_ts, f005_id, f005_nombre, f005_descripcion)
VALUES 
--    (current_date, 1, 'usuarios', 'Gestion de usuarios'),
--    (current_date, 2, 'perfiles', 'Gestion de perfiles'),
--    (current_date, 3, 'permisos', 'Gestion de permisos'),
--    (current_date, 4, 'mecanicos', 'Gestion de mecanicos'),
--    (current_date, 5, 'menus', 'Gestion de menus'),
--    (current_date, 6, 'clientes', 'Gestion de clientes'),
 --   (current_date, 7, 'historiales', 'Gestion de historiales'),
--    (current_date, 8, 'vehiculos', 'Gestion de vehiculos'),
 --   (current_date, 9, 'citas', 'Gestion de citas'),
    (current_date, 10, 'calendarios', 'Gestion de calendarios'),
    (current_date, 11, 'estado reparaciones', 'Gestion de reparaciones'),
    (current_date, 12, 'inventarios', 'Inventarios'),
    (current_date, 13, 'servicios', 'Gestion de servicios'),
    (current_date, 14, 'servicios', 'Gestion de servicios'),
    (current_date, 15, 'gestion inventarios', 'Gestion de inventarios'),
    (current_date, 16, 'estadisticas', 'Gestion de estadisticas'),
    (current_date, 17, 'estadisticas', 'Gestion de estadisticas'),
    (current_date, 18, 'calendario mecanicos', 'Gestion de calendario mecanicos'),
    (current_date, 19, 'facturaciones', 'Gestion de facturaciones'),
    (current_date, 20, 'cotizaciones', 'Gestion de cotizaciones'),
    (current_date, 21, 'suscripciones', 'Gestion de cotizaciones');