using AutoMapper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using WebApiAspNet.Context;
using WebApiAspNet.Entities;
using WebApiAspNet.Helpers;
using WebApiAspNet.Models;

namespace WebApiAspNet.Controllers
{

    [ApiController]
    [Authorize]
    [Route("api/v1/[controller]")]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AutoresController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        ///  Obtener lista de todos los autores con sus libros
        /// </summary>
        /// <param>Agregar en el Header de la peticion [Key:IncluirHATEOA,Value:Y] para optener endpoints de cada autor </param>
        /// <param name="numeroDePagina">Numero de pagina a mostrar</param>
        /// <param name="cantidadDeRegistro">Cantidad de registros a mostrasr por pagina (Min 5 - Max 100)</param>
        /// <returns></returns>
        // GET:api/Autores
        [HttpGet(Name = "ObtenerAutores")]
        [ServiceFilter(typeof(HATEOASAuthorsFilterAttribute))]
        public async Task<ActionResult<IEnumerable<AutorDTO>>> GetAsync(int numeroDePagina = 1, int cantidadDeRegistro = 5)
        {
            numeroDePagina = numeroDePagina < 1 ? 1 : numeroDePagina;
            cantidadDeRegistro = cantidadDeRegistro < 5 ? 5 : cantidadDeRegistro;
            cantidadDeRegistro = cantidadDeRegistro > 100 ? 100 : cantidadDeRegistro;

            var query = _context.Autores.AsQueryable();

            var totalDeRegistro = query.Count();

            var autores = await query
                .Skip(cantidadDeRegistro * (numeroDePagina - 1))
                .Take(cantidadDeRegistro)
                .Include(x => x.Libros).ToListAsync();

            Response.Headers["X-Total-Registro"] = totalDeRegistro.ToString();
            Response.Headers["X-Cantidad-Paginas"] = ((int)Math.Ceiling((double)totalDeRegistro / cantidadDeRegistro)).ToString();

            var autoresDTO = _mapper.Map<List<AutorDTO>>(autores);
            return autoresDTO;
        }

        /// <summary>
        /// Obtiene un autor en especifico
        /// </summary>
        /// <param name="id">Id del autor a obtener</param>
        /// <param>Agregar en el Header de la peticion [Key:IncluirHATEOA,Value:Y] para optener endpoints del autor </param>
        /// <returns></returns>
        // GET: api/Autores/1
        [HttpGet("{id}", Name = "ObtenerAutor")]
        [ServiceFilter(typeof(HATEOASAuthorFilterAttribute))]
        public async Task<ActionResult<AutorDTO>> GetAsync(int id)
        {
            var autor = await _context.Autores.Include(x => x.Libros).FirstOrDefaultAsync(x => x.Id == id);
            if (autor == null)
            {
                return NotFound();
            }

            var autorDTO = _mapper.Map<AutorDTO>(autor);

            return autorDTO;
        }
        /// <summary>
        /// Crea un autor
        /// </summary>
        /// <param name="autorCreacion">Modelo del Autor</param>
        /// <returns></returns>
        // POST: api/Autores
        [HttpPost(Name = "CrearAutor")]
        public async Task<IActionResult> PostAsync([FromBody] AutorCreacionDTO autorCreacion)
        {
            var autor = _mapper.Map<Autor>(autorCreacion);
            await _context.Autores.AddAsync(autor);
            await _context.SaveChangesAsync();
            var autorDTO = _mapper.Map<AutorDTO>(autor);
            return new CreatedAtRouteResult("ObtenerAutor", new { id = autor.Id }, autorDTO);
        }

        /// <summary>
        /// Actualiza un autor en especifico
        /// </summary>
        /// <param name="id">>Id del autor a actualizar</param>
        /// <param name="autorActualizacion">Modelo del Autor a enviar</param>
        /// <returns></returns>
        // PUT: api/Autores/1
        [HttpPut("{id}", Name = "ActualizarAutor")]
        public async Task<IActionResult> PutAsync(int id, [FromBody] AutorCreacionDTO autorActualizacion)
        {
            var autor = _mapper.Map<Autor>(autorActualizacion);
            autor.Id = id;
            _context.Entry(autor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!AutorExiste(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        /// <summary>
        /// Modifica un autor en especifico
        /// </summary>
        /// <param name="id">Id del autor a modificar</param>
        /// <param name="patchDocument">Modelo de Autor ejemplo: [{"op":"replace","path":"/fechaNacimiento","value": "0001-01-01"}]</param>
        /// <returns></returns>
        // PATCH: api/Autores/1
        // [{"op":"replace","path":"/fechaNacimiento","value": "0001-01-01"}]
        [HttpPatch("{id}", Name = "ModificarAutor")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<AutorCreacionDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var autorDeLaDB = await _context.Autores.FirstOrDefaultAsync(x => x.Id == id);

            if (autorDeLaDB == null)
            {
                return NotFound();
            }

            var autorDTO = _mapper.Map<AutorCreacionDTO>(autorDeLaDB);

            patchDocument.ApplyTo(autorDTO, ModelState);

            var isValid = TryValidateModel(autorDeLaDB);

            if (!isValid)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(autorDTO, autorDeLaDB);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Borra un autor en especifico
        /// </summary>
        /// <param name="id">Id del autor a borrar</param>
        /// <returns></returns>
        // DELETE: api/Autores/1
        [HttpDelete("{id}", Name = "BorrarAutor")]
        public async Task<ActionResult<Autor>> DeleteAsync(int id)
        {
            var autor = await _context.Autores.FindAsync(id);

            if (autor == null)
            {
                return NotFound();
            }

            _context.Autores.Remove(autor);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool AutorExiste(long id) =>
            _context.Autores.Any(a => a.Id == id);
    }
}
