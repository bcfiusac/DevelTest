using DevelWebApp.Data;
using DevelWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;  
using static DevelWebApp.Models.Encuesta;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace DevelWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpPost("NuevaEncuesta")]
        [Authorize]
        public IActionResult NuevaEncuestaEndpoint([FromBody] Encuesta encuesta)
        {

            string sqlDataSource = _configuration.GetConnectionString("ConexionDBDevel");
            using (SqlConnection conectando = new SqlConnection(sqlDataSource))
            {
                SqlCommand cmd = new SqlCommand("CRUDEVEL", conectando);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@opcion", "CE");
                cmd.Parameters.AddWithValue("@nombre", encuesta.Nombre);
                cmd.Parameters.AddWithValue("@descripcion", encuesta.Descripcion);
                conectando.Open();
                try
                {
                    cmd.ExecuteNonQuery();
                    conectando.Close();
                    
                }
                catch(Exception ex)
                {
                    conectando.Close();
                    return StatusCode(409, $"La encuesta '{encuesta.Nombre}' ya existe.");
                }
                //cmd.ExecuteNonQuery();
                conectando.Close();

                List<Campo> campos = new List<Campo>();
                campos = encuesta.Campos;
                foreach (var v in campos)
                {
                    using (SqlConnection conectandoNuevamente = new SqlConnection(sqlDataSource))
                    {
                        SqlCommand cmd2 = new SqlCommand("CRUDEVEL", conectandoNuevamente);
                        cmd2.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd2.Parameters.AddWithValue("@opcion", "CC");
                        cmd2.Parameters.AddWithValue("@nombre", encuesta.Nombre);
                        cmd2.Parameters.AddWithValue("@nombreCampo", v.NombreCampo);
                        cmd2.Parameters.AddWithValue("@tituloCampo", v.TituloCampo);
                        cmd2.Parameters.AddWithValue("@tipoCampo", v.tipoCampo);
                        cmd2.Parameters.AddWithValue("@requeridoCampo", v.EsRequerido);
                        
                        conectandoNuevamente.Open();
                        cmd2.ExecuteNonQuery();
                        conectandoNuevamente.Close();
                        //    try
                        //{

                        //}
                        //catch (Exception ex)
                        //{
                        //    string quedice = ex.ToString();

                        //}

                    }
                }
                return Ok("Encuesta ingresada exitosamente");

            }

        }

        [HttpPut("ActualizarEncuesta")]
        [Authorize]
        public IActionResult ActualizarEncuestaEndpoint([FromBody] Encuesta encuesta)
        {

            string sqlDataSource = _configuration.GetConnectionString("ConexionDBDevel");
            using (SqlConnection conectando = new SqlConnection(sqlDataSource))
            {
                SqlCommand cmd = new SqlCommand("CRUDEVEL", conectando);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@opcion", "UE");
                cmd.Parameters.AddWithValue("@nombre", encuesta.Nombre);
                cmd.Parameters.AddWithValue("@nuevonombre", encuesta.NuevoNombre);
                cmd.Parameters.AddWithValue("@nuevadescripcion", encuesta.NuevaDescripcion);
                conectando.Open();
                try
                {
                    cmd.ExecuteNonQuery();
                    conectando.Close();

                }
                catch (Exception ex)
                {
                    conectando.Close();
                    return StatusCode(409, $"La encuesta '{encuesta.Nombre}' no existe.");
                }
                //cmd.ExecuteNonQuery();
                conectando.Close();

                List<Campo> campos = new List<Campo>();
                campos = encuesta.Campos;
                foreach (var v in campos)
                {
                    using (SqlConnection conectandoNuevamente = new SqlConnection(sqlDataSource))
                    {
                        SqlCommand cmd2 = new SqlCommand("CRUDEVEL", conectandoNuevamente);
                        cmd2.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd2.Parameters.AddWithValue("@opcion", "CC");
                        cmd2.Parameters.AddWithValue("@nombre", encuesta.NuevoNombre);
                        cmd2.Parameters.AddWithValue("@nombreCampo", v.NombreCampo);
                        cmd2.Parameters.AddWithValue("@tituloCampo", v.TituloCampo);
                        cmd2.Parameters.AddWithValue("@tipoCampo", v.tipoCampo);
                        cmd2.Parameters.AddWithValue("@requeridoCampo", v.EsRequerido);

                        conectandoNuevamente.Open();
                        
                        //conectandoNuevamente.Close();
                        try
                        {
                            cmd2.ExecuteNonQuery();
                            conectandoNuevamente.Close();
                        }
                        catch (Exception ex)
                        {
                            conectandoNuevamente.Close();
                            return StatusCode(409, $"El campo '{v.NombreCampo}' ya existe en la encuesta '{encuesta.NuevoNombre}'.");

                        }

                    }
                }
                return Ok("Encuesta actualizada exitosamente");

            }

        }
        [HttpGet("obtenerEncuesta")]
        [Authorize]
        public string obtenerEncuesta([FromBody] Encuesta encuesta)
        {
            List<Campo> listaCampos = new List<Campo>();
            StringBuilder pagina = new StringBuilder();
            pagina.AppendLine("<!DOCTYPE html>");
            pagina.AppendLine("<html><body>");
            pagina.AppendLine("<form action=\"https://localhost:44398/api/user/nuevarespuesta\" method =\"POST\" name =\"myForm\" > ");


            string sqlDataSource = _configuration.GetConnectionString("ConexionDBDevel");
            using (SqlConnection conectando = new SqlConnection(sqlDataSource))
            {
                SqlCommand cmd = new SqlCommand("CRUDEVEL", conectando);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@nombre", encuesta.Nombre);
                cmd.Parameters.AddWithValue("@opcion", "GCE");
                try
                {
                    conectando.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Campo temp = new Campo();
                            {
                                pagina.AppendLine("<label for=\"" + dr["Nombre"].ToString() + "\">" + dr["Titulo"].ToString() + ":" + "</label><br>");
                                pagina.AppendLine("<input type=\""+ dr["tipo"].ToString() + "\" id =\"" + dr["Nombre"].ToString() + "\" name =\"" + dr["Nombre"].ToString() + "\"><br>");


                            };
                        }
                        pagina.AppendLine("<input type=\"submit\" value =\"Guardar\" > ");
                        pagina.AppendLine("</form> ");
                        pagina.AppendLine("</body></html>");
                        
                    }
                    string salida = pagina.ToString();
                    return salida;
                }
                catch(Exception ex)
                {
                    
                    
                }

            }
            return "error en la salida";
        }

        [HttpGet("obtenerResultados")]
        [Authorize]
        public List<Respuesta> obtenerResultados([FromBody] Encuesta encuesta)
        {
            List<Respuesta> listaCampos = new List<Respuesta>();
            string sqlDataSource = _configuration.GetConnectionString("ConexionDBDevel");
            using (SqlConnection conectando = new SqlConnection(sqlDataSource))
            {
                SqlCommand cmd = new SqlCommand("CRUDEVEL", conectando);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@nombre", encuesta.Nombre);
                cmd.Parameters.AddWithValue("@opcion", "GRE");
                try
                {
                    conectando.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Respuesta temp = new Respuesta();
                            {

                                
                                temp.respuesta = dr["respuesta"].ToString();
                                temp.NombreCampo = dr["nombreCampo"].ToString();
                                listaCampos.Add(temp);

                            };
                        }
                      
                    }
                    
                    return listaCampos;
                }
                catch (Exception ex)
                {

                    return listaCampos;
                }

            }
        }

        [HttpPost("NuevaRespuesta")]
        [AllowAnonymous]
        public IActionResult NuevaRespuestaEndpoint([FromBody] Encuesta encuesta)
        {

            List<Campo> campos = new List<Campo>();
            campos = encuesta.Campos;
            foreach (var v in campos)
            {
                string sqlDataSource = _configuration.GetConnectionString("ConexionDBDevel");
                using (SqlConnection conectandoNuevamente = new SqlConnection(sqlDataSource))
                {
                    SqlCommand cmd2 = new SqlCommand("CRUDEVEL", conectandoNuevamente);
                    cmd2.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd2.Parameters.AddWithValue("@opcion", "CR");
                    cmd2.Parameters.AddWithValue("@nombre", encuesta.Nombre);
                    cmd2.Parameters.AddWithValue("@nombreCampo", v.NombreCampo);
                    cmd2.Parameters.AddWithValue("@respuesta", v.respuesta);

                    
                    try
                    {
                        conectandoNuevamente.Open();
                        cmd2.ExecuteNonQuery();
                        conectandoNuevamente.Close();
                        
                    }
                    catch (Exception ex)
                    {
                        string quedice = ex.ToString();
                        conectandoNuevamente.Close();
                        return StatusCode(409, $"No se pudo ingresar la respuesta, revise los campos a ingresar y el nombre de la encuesta.");

                    }
                    
                }
                
            }
            return Ok("Respuesta ingresada exitosamente");


        }





        [HttpDelete("eliminarEncuesta")]
        [Authorize]
        public IActionResult eliminarEncuestaEndpoint([FromBody] Encuesta encuesta)
        {
            string sqlDataSource = _configuration.GetConnectionString("ConexionDBDevel");
            using (SqlConnection conectando = new SqlConnection(sqlDataSource))
            {
                SqlCommand cmd = new SqlCommand("CRUDEVEL", conectando);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@opcion", "DE");
                cmd.Parameters.AddWithValue("@nombre", encuesta.Nombre);
                conectando.Open();
                try
                {
                    cmd.ExecuteNonQuery();
                    conectando.Close();

                }
                catch (Exception ex)
                {
                    conectando.Close();
                    return StatusCode(409, $"La encuesta '{encuesta.Nombre}' no existe.");
                }
                conectando.Close();

                return Ok("Encuesta eliminada exitosamente");

            }

        }
      
        
    }
}
