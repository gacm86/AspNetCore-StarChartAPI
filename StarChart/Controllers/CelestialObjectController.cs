using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.Find(id);

            if (celestialObject == null) return NotFound();

            var satellites = _context.CelestialObjects.Where(x => x.OrbitedObjectId == id).ToList();

            if (satellites.Any())
            {
                celestialObject.Satellites = satellites;
            }

            return Ok(celestialObject);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObject = _context.CelestialObjects.Where(x => x.Name == name);

            if (celestialObject == null || celestialObject.Count() == 0) return NotFound();

            foreach (var item in celestialObject)
            {
                item.Satellites = _context.CelestialObjects.Where(x => x.OrbitedObjectId == item.Id).ToList();
            }

            return Ok(celestialObject);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects;
            foreach (var item in celestialObjects)
            {
                item.Satellites = _context.CelestialObjects.Where(x => x.OrbitedObjectId == item.Id).ToList();
            }

            return Ok(celestialObjects);
        }
    }
}
