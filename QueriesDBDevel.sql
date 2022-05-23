-- crear la db
CREATE DATABASE DEVEL;
USE DEVEL;

--creacion de las tablas iniciales

create table encuesta (
Nombre varchar (60),
Descripcion varchar (150),
Link varchar(200),
primary key (Nombre)
)

create table campoEncuesta(
Nombre varchar (50),
Titulo varchar (50),
tipo varchar (20),
requerido varchar(2),
nombreEncuesta varchar (60) not null,
foreign key (nombreEncuesta) references encuesta(Nombre),
primary key (Nombre,nombreEncuesta)
)

create table respuesta(
nombreCampo varchar(50),
nombreEncuesta varchar(60),
respuesta varchar(100),
foreign key (nombreCampo,nombreEncuesta) references campoEncuesta(Nombre,nombreEncuesta),
)


-- **************************************************************************************************************************************
-- SP para los procesos de encuestas ****************************************************************************************************
-- **************************************************************************************************************************************


USE [Devel]
GO

/****** Object:  StoredProcedure [dbo].[CRUDEVEL]    Script Date: 5/23/2022 4:23:00 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO





CREATE PROCEDURE [dbo].[CRUDEVEL] 
	@opcion varchar(5),
	@nombre varchar(60)='',
	@nuevonombre varchar(60)='',
	@descripcion varchar(150)='',
	@nuevadescripcion varchar(60)='',
	@nombreCampo varchar(50)='',
	@tituloCampo varchar(50)='',
	@tipoCampo varchar(20)='',
	@requeridoCampo varchar(20)='',
	@respuesta varchar(60)=''
	as 
	DECLARE @err_message nvarchar(255)
	begin
	--primera opcion = INSERT ENCUESTA
		if (@opcion = 'CE')
		begin
			print('creando la encuesta');
			insert into encuesta (Nombre, Descripcion,Link) values (@nombre,@descripcion,'www.holamundo.com/'+@nombre)
		end
		if (@opcion = 'DE')
		begin
			print('eliminando la encuesta');
			delete from respuesta where nombreEncuesta = @nombre;
			delete from campoEncuesta where nombreEncuesta = @nombre;
			delete from encuesta where nombre = @nombre;
		end
		if (@opcion = 'UE')
		begin
		--where (dui = @dui or @dui = '')
			print('actualizar la encuesta');
			if exists(select * from encuesta where Nombre = @nombre)
				begin
				delete from respuesta where nombreEncuesta = @nombre;
				delete from campoEncuesta where nombreEncuesta = @nombre;
				delete from respuesta 
				update encuesta set Nombre = @nuevonombre, Descripcion = @nuevadescripcion, Link = 'www.holamundo.com/'+@nuevonombre
				where Nombre = @nombre;
				end
			else
				begin
					SET @err_message ='La encuesta: '+ @nombre + ' no ha sido creada.'
					RAISERROR (@err_message,10, 1) 
				end

		end
		--aqui va la opcion para crear campo de la encuesta
		if (@opcion = 'CC')
		begin
			if not exists(select * from campoEncuesta where Nombre = @nombre and Nombre = @nombreCampo)
				begin
				insert into campoEncuesta(Nombre,Titulo,tipo,nombreEncuesta,requerido) values (@nombreCampo, @tituloCampo, @tipoCampo, @nombre,@requeridoCampo)
				end
		end
		if (@opcion = 'GCE')
		begin
			print('obteniendo la encuesta');
			select * from campoEncuesta where nombreEncuesta = @nombre;
		end
		if (@opcion = 'CR')
		begin
			print('creando la respuesta');
			insert into respuesta (respuesta, nombreCampo, nombreEncuesta) values (@respuesta,@nombreCampo,@nombre);
		end
		if (@opcion = 'GRE')
		begin
			print('obteniendo los resultados de la encuesta');
			select * from respuesta where nombreEncuesta = @nombre order by nombreCampo;
		end
end
	
GO


