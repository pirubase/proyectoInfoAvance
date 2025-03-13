
--********************************
--script_crea_tablas
--******************************

create table t001_usuario(
	f001_rowid int8 not null primary key GENERATED ALWAYS AS identity(START WITH 1 INCREMENT BY 1),
	f001_ts timestamp not null,
	f001_id_usuario varchar(80) not null,
	f001_clave varchar(512) not null,
	f001_nombres varchar(80) not null,
	f001_apellidos varchar(80) not null,
	f001_correo_electronico varchar(80) not null,
	f001_numero_celular varchar(100) not null,
	f001_rowid_empresa_o_persona_natural int8 not NULL,
	f001_rowid_estado int8 not null,
	f001_rowid_perfil int8 not null,
    f001_rowid_cliente int8 null,
    f001_sesion_id varchar(255) null,
    f001_ultima_actividad timestamp null
	
);	

create table t002_empresa_o_persona_natural
(
	f002_rowid int8 not null primary key GENERATED ALWAYS AS identity(START WITH 1 INCREMENT BY 1),
	f002_ts timestamp not null,
	f002_razon_social varchar(100) not null,
	f002_nit varchar(100) not null,
	f002_alcance varchar(50) not null,
	f002_cantidad_empleados int8 not null
	
);

create table t003_permisos(
	f003_rowid int8 not null primary key GENERATED ALWAYS AS identity(START WITH 1 INCREMENT BY 1),
	f003_ts timestamp not null,
	f003_id int8 not null,
	f003_permiso_consultar bool not null,
	f003_permiso_crear bool not null,
	f003_permiso_editar bool not null,
	f003_permiso_detalle bool not null,
	f003_permiso_eliminar bool not null,
	f003_permiso_uso_menu bool not null,
	f003_rowid_empresa_o_persona_natural int8 not null,
	f003_rowid_menu int8 not null,
	f003_rowid_perfil int8 not null
);


 create table t004_perfil
 (
	f004_rowid int8 not null primary key GENERATED ALWAYS AS identity(START WITH 1 INCREMENT BY 1),
	f004_ts timestamp not null,
	f004_id int8 not null,
	f004_nombre varchar(80) not null,
	f004_descripcion varchar(255) not null
	
 );
 
create table t005_menu
(
	f005_rowid int8 not null primary key GENERATED ALWAYS AS identity(START WITH 1 INCREMENT BY 1),
	f005_ts timestamp not null,
	f005_id int8 not null,
	f005_nombre varchar(80) not null,
	f005_descripcion varchar(255) not null
);


create table t006_mecanico
(
 f006_rowid int8 not null primary key GENERATED ALWAYS AS identity(START WITH 1 INCREMENT BY 1),
f006_ts timestamp not null,

f006_nombre varchar(255) not null,
f006_apellido varchar (255) not null,
f006_correo varchar (255) not null,
f006_telefono varchar (255) not null,
f006_direccion varchar(255) NOT NULL,
f006_años_experiencia int8 NOT NULL,
f006_tarifa_hora decimal (25,4) NOT NULL,
f006_rowid_empresa_o_persona_natural int8 NOT null

);


create table t007_cliente(
    f007_rowid int8 not null primary key GENERATED ALWAYS AS identity(START WITH 1 INCREMENT BY 1),
    f007_ts timestamp not null,
    f007_id int8 not null,
    f007_nombre varchar(255) not null,
    f007_apellido varchar (255) not null,
    f007_correo varchar (255) not null,
    f007_telefono varchar (255) not null,
    f007_direccion varchar(255) not null,
    f007_rowid_empresa_o_persona_natural int8 not null,
    f007_rowid_mecanico_familia int8 null


);

create table t008_estados_usuario
(
	f008_rowid int8 not null primary key GENERATED ALWAYS AS identity(START WITH 1 INCREMENT BY 1),
	f008_id int8 not null,
	f008_nombre_estado varchar(80) not null
);

create table t009_cita (
f009_rowid int8 not null primary key GENERATED ALWAYS AS identity(START WITH 1 INCREMENT BY 1),
f009_ts timestamp not null,
f009_rowid_servicio int8 not null,
f009_descripcion  varchar(500) not null,
f009_fecha_inicio timestamp not null,
f009_fecha_finalizacion timestamp not null,
f009_estado varchar(255) not null,
f009_rowid_mecanico int8 not null,
f009_rowid_especialidad int8 not null,
f009_rowid_usuario_creador int8 not null,
f009_rowid_usuario_aprobador int8 null,
f009_hora timestamp not null,
f009_rowid_cliente int8 not null,
f009_rowid_empresa_o_persona_natural int8 not null

);

create table t010_vehiculo (
f010_rowid int8 not null primary key GENERATED ALWAYS AS identity(START WITH 1 INCREMENT BY 1),
f010_ts timestamp not null,
f010_id int8 not null,
f010_nombre varchar(255) not null,
f010_descripcion varchar(500) null,
f010_rowid_empresa_o_persona_natural int8 not null,
f010_rowid_cliente int8 not null

);



create table t012_token (
f012_rowid int8 not null primary key GENERATED ALWAYS AS identity(START WITH 1 INCREMENT BY 1),
f012_ts timestamp not null,
f012_token varchar(255) not null,
f012_rowid_empresa_o_persona_natural int8 not null,
f012_expiracion timestamp not null,
f012_rowid_cita int8 null

);

create table t013_documento(
f013_rowid int8 not null primary key GENERATED ALWAYS AS identity(START WITH 1 INCREMENT BY 1),
f013_ts timestamp not null,
f013_id int8 not null,
f013_nombre varchar(255) not null,
f013_docuemnto bytea null,
f013_descripcion varchar(255) not null,
f013_categoria varchar(255) not null,
f013_rowid_empresa_o_persona_natural int8 not null,
f013_rowid_historial_medico int8 not null

);

create table t014_servicio (

f014_rowid int8 not null primary key GENERATED ALWAYS AS identity(START WITH 1 INCREMENT BY 1),
f014_ts timestamp not null,
f014_id int8 not null,
f014_nombre varchar(255) not null,
f014_valor decimal(24,4) not null,
f014_rowid_empresa_o_persona_natural int8 not null

);


create table t016_auditoria_mecanico (

f016_rowid int8 not null primary key GENERATED ALWAYS AS identity(START WITH 1 INCREMENT BY 1),
f016_ts timestamp not null,
f016_fecha_inicio timestamp not null,
f016_fecha_finalizacion timestamp not null,
f016_descripcion varchar(255)  null,
f016_rowid_mecanico int8 not null,
f016_rowid_empresa_o_persona_natural int8 not null

);


--*********************************************************
--				FORANEAS								  *
--*********************************************************

--****************************USUARIOS******************************--


--t001 t002
alter table t001_usuario add  constraint fk_t001_t002 
foreign key(f001_rowid_empresa_o_persona_natural)
references t002_empresa_o_persona_natural (f002_rowid); 

--t001_t004
alter table t001_usuario add  constraint fk_t001_t004 
foreign key(f001_rowid_perfil)
references t004_perfil (f004_rowid); 



--t001_t007
alter table t001_usuario add constraint fk_t001_t007
foreign key (f001_rowid_cliente)
references t007_cliente (f007_rowid);

--t001_t008
alter table t001_usuario add  constraint fk_t001_t008 
foreign key(f001_rowid_estado)
references t008_estados_usuario (f008_rowid); 

--t003 t002
alter table t003_permisos add  constraint fk_t003_t002 
foreign key(f003_rowid_empresa_o_persona_natural)
references t002_empresa_o_persona_natural (f002_rowid); 








--t003 t004

alter table t003_permisos add  constraint fk_t003_t004 
foreign key(f003_rowid_perfil)
references t004_perfil (f004_rowid); 


--t003 t005

alter table t003_permisos add  constraint fk_t003_t005 
foreign key(f003_rowid_menu)
references t005_menu (f005_rowid); 

--t006 t002
alter table t006_mecanico  add  constraint fk_t006_t002 
foreign key(f006_rowid_empresa_o_persona_natural)
references t002_empresa_o_persona_natural (f002_rowid); 

--t006 t010
--alter table t006_mecanico  add  constraint fk_t006_t010 
--foreign key(f006_rowid_especialidad)
--references t010_especialidad (f010_rowid);

--t007 t002
alter table t007_cliente  add  constraint fk_t007_t002 
foreign key(f007_rowid_empresa_o_persona_natural)
references t002_empresa_o_persona_natural (f002_rowid); 

--t007 t006
alter table t007_cliente  add  constraint fk_t007_t006 
foreign key(f007_rowid_mecanico_familia)
references t006_mecanico (f006_rowid); 




--t009 t002
alter table t009_cita  add  constraint fk_t009_t002 
foreign key(f009_rowid_empresa_o_persona_natural)
references t002_empresa_o_persona_natural (f002_rowid);

--t009 t006
alter table t009_cita  add  constraint fk_t009_t006 
foreign key(f009_rowid_mecanico)
references t006_mecanico (f006_rowid);

--t009 t007
alter table t009_cita  add  constraint fk_t009_t007 
foreign key(f009_rowid_cliente)
references t007_cliente (f007_rowid);

--t009 t0014
alter table t009_cita  add  constraint fk_t009_t014 
foreign key(f009_rowid_servicio)
references t014_servicio (f014_rowid);

--t009 t010
alter table t009_cita  add  constraint fk_t009_t010 
foreign key(f009_rowid_especialidad)
references t010_vehiculo (f010_rowid);

--t009 t001 creador
alter table t009_cita  add  constraint fk_t009_t001_creador 
foreign key(f009_rowid_usuario_creador)
references t001_usuario (f001_rowid);

--t009 t001 aprobador
alter table t009_cita  add  constraint fk_t009_t001_aprobador
foreign key(f009_rowid_usuario_aprobador)
references t001_usuario (f001_rowid);



--t010 t002
alter table t010_vehiculo  add  constraint fk_t010_t002 
foreign key(f010_rowid_empresa_o_persona_natural)
references t002_empresa_o_persona_natural (f002_rowid);



--t010 t007
alter table t010_vehiculo  add  constraint fk_t010_t007 
foreign key(f010_rowid_cliente)
references t007_cliente (f007_rowid);


--t012 t002
alter table t012_token  add  constraint fk_t012_t002 
foreign key(f012_rowid_empresa_o_persona_natural)
references t002_empresa_o_persona_natural (f002_rowid);

alter table t012_token  add  constraint fk_t012_t009 
foreign key(f012_rowid_cita)
references t009_cita (f009_rowid);

--t013 t002
alter table t013_documento  add  constraint fk_t013_t002 
foreign key(f013_rowid_empresa_o_persona_natural)
references t002_empresa_o_persona_natural (f002_rowid);




--t007 t013 examenes laboratorio
--alter table t007_paciente  add  constraint fk_t07_t013_examenes_laboratorio
--foreign key(f007_rowid_examenes_laboratorio)
--references t013_documento (f013_rowid);

--t007 t013 estudios imagenes
--alter table t007_paciente  add  constraint fk_t07_t013_estudios_imagenes
--foreign key(f007_rowid_estudios_imagen)
--references t013_documento (f013_rowid);

--t007 t013 estudios especializados
--alter table t007_paciente  add  constraint fk_t07_t013_estudios_especializados
--foreign key(f007_rowid_estudios_especializados)
--references t013_documento (f013_rowid);


--t014 t002
alter table t014_servicio  add  constraint fk_t014_t002 
foreign key(f014_rowid_empresa_o_persona_natural)
references t002_empresa_o_persona_natural (f002_rowid);



--t016 t002
alter table t016_auditoria_mecanico  add  constraint fk_t016_t002 
foreign key(f016_rowid_empresa_o_persona_natural)
references t002_empresa_o_persona_natural (f002_rowid);

--t016 t006
alter table t016_auditoria_mecanico  add  constraint fk_t016_t006 
foreign key(f016_rowid_mecanico)
references t006_mecanico (f006_rowid);


