using AutoMapper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using WebApiAspNet.Context;
using WebApiAspNet.Entities;
using WebApiAspNet.Models;

namespace WebApiAspNet.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v1/[controller]")]
    public class LibrosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUrlHelper _urlHelper;

        public LibrosController(ApplicationDbContext context, IMapper mapper, IUrlHelper urlHelper)
        {
            _context = context;
            _mapper = mapper;
            _urlHelper = urlHelper;
        }


        // GET: api/Libros
        /// <summary>
        /// Obtener lista de todos los libros con su autor
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "ObtenerLibros")]
        public async Task<ActionResult<IEnumerable<Libro>>> GetAsync()
        {
            return await _context.Libros.Include(x => x.Autor).ToListAsync();
        }

        /// <summary>
        /// Obtiene un libro en especifico
        /// </summary>
        /// <param name="id">Id del libro a obtener</param>
        /// <returns></returns>
        // GET: api/Libros/1
        [HttpGet("{id}", Name = "ObtenerLibro")]
        public async Task<ActionResult<LibroDTO>> GetAsync(int id)
        {
            var libro = await _context.Libros.Include(x => x.Autor).FirstOrDefaultAsync(x => x.Id == id);

            if (libro == null)
            {
                return NotFound();
            }

            var LibroDTO = _mapper.Map<LibroDTO>(libro);

            GenerarEnlaces(LibroDTO);

            return LibroDTO;
        }

        /// <summary>
        /// Crea un libro
        /// </summary>
        /// <param name="libro">Modelo del libro a enviar</param>
        /// <returns></returns>
        // POST: api/Libros
        [HttpPost(Name = "CrearLibro")]
        public async Task<ActionResult> PostAsync([FromBody] Libro libro)
        {
            await _context.Libros.AddAsync(libro);
            await _context.SaveChangesAsync();

            return new CreatedAtRouteResult("ObtenerLibro", new { id = libro.Id }, libro);
        }

        /// <summary>
        /// Actualiza un libro en especifico
        /// </summary>
        /// <param name="id">Id del libro a actualizar</param>
        /// <param name="value">Modelo del libro a actualizar</param>
        /// <returns></returns>
        // PUT: api/Libros/1
        [HttpPut("{id}", Name = "ActualiarLibro")]
        public ActionResult Put(int id, [FromBody] Libro value)
        {
            if (id != value.Id)
            {
                return BadRequest();
            }

            _context.Entry(value).State = EntityState.Modified;

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException) when (!LibroExiste(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        /// <summary>
        /// Modifica un libro en especifico
        /// </summary>
        /// <param name="id">>Id del libro a modificar</param>
        /// <param name="patchDocument">Modelo de libro ejemplo: [{"op":"replace","path":"/Titulo","value": ""}]</param>
        /// <returns></returns>
        // PATCH: api/Autores/1
        // [{"op":"replace","path":"/Titulo","value": ""}]
        [HttpPatch("{id}", Name = "ModificarLibro")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<LibroDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var LibroDeLaDB = await _context.Libros.FirstOrDefaultAsync(x => x.Id == id);

            if (LibroDeLaDB == null)
            {
                return NotFound();
            }

            var LibroDTO = _mapper.Map<LibroDTO>(LibroDeLaDB);

            patchDocument.ApplyTo(LibroDTO, ModelState);

            var isValid = TryValidateModel(LibroDeLaDB);

            if (!isValid)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(LibroDTO, LibroDeLaDB);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Borra un libro en especifico
        /// </summary>
        /// <param name="id">Id del libro a borrar</param>
        /// <returns></returns>
        // DELETE: api/Libros/1
        [HttpDelete("{id}", Name = "BorrarLibro")]
        public async Task<ActionResult<Libro>> DeleteAsync(int id)
        {
            var libro = await _context.Libros.FindAsync(id);

            if (libro == null)
            {
                return NotFound();
            }

            _context.Libros.Remove(libro);
            await _context.SaveChangesAsync();

            return libro;
        }


        private bool LibroExiste(long id) =>
            _context.Libros.Any(a => a.Id == id);

        private void GenerarEnlaces(LibroDTO libro)
        {
            libro.Enlaces.Add(new Enlace(href: _urlHelper.Link("ObtenerLibro", new { id = libro.Id }), rel: "self", metodo: "GET"));
            libro.Enlaces.Add(new Enlace(href: _urlHelper.Link("ActualiarLibro", new { id = libro.Id }), rel: "actualizar-libro", metodo: "PUT"));
            libro.Enlaces.Add(new Enlace(href: _urlHelper.Link("ModificarLibro", new { id = libro.Id }), rel: "modificar-libro", metodo: "PATCH"));
            libro.Enlaces.Add(new Enlace(href: _urlHelper.Link("BorrarLibro", new { id = libro.Id }), rel: "borrar-libro", metodo: "DELETE"));
        }
    }
}
