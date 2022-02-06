using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;

using WebApiAspNet.Models;

namespace WebApiAspNet.Controllers
{
    [ApiController]
    [Route("api")]
    public class RootController : ControllerBase
    {
        private readonly IUrlHelper _urlHelper;

        public RootController(IUrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        /// <summary>
        /// Lista con los endpoints que podemos solicitar (HATEOAS)
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "GetRoot")]
        public ActionResult<IEnumerable<Enlace>> Get()
        {
            List<Enlace> enlaces = new List<Enlace>();

            enlaces.Add(new Enlace(href: _urlHelper.Link("GetRoot", new { }), rel: "self", metodo: "GET"));
            enlaces.Add(new Enlace(href: _urlHelper.Link("ObtenerAutores", new { }), rel: "autores", metodo: "GET"));
            enlaces.Add(new Enlace(href: _urlHelper.Link("CrearAutor", new { }), rel: "crear-autor", metodo: "POST"));
            enlaces.Add(new Enlace(href: _urlHelper.Link("ObtenerLibros", new { }), rel: "libros", metodo: "GET"));
            enlaces.Add(new Enlace(href: _urlHelper.Link("CrearLibro", new { }), rel: "crear-libro", metodo: "POST"));
            return enlaces;
        }
    }
}
