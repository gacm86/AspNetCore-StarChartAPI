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

        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject model)
        {
            _context.CelestialObjects.Add(model);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new { model.Id }, model);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject model)
        {
            var celestialObject = _context.CelestialObjects.Find(id);

            if (celestialObject == null) return NotFound();

            celestialObject.Name = model.Name;
            celestialObject.OrbitalPeriod = model.OrbitalPeriod;
            celestialObject.OrbitedObjectId = model.OrbitedObjectId;

            _context.CelestialObjects.Update(celestialObject);

            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var celestialObject = _context.CelestialObjects.Find(id);

            if (celestialObject == null) return NotFound();

            celestialObject.Name = name;

            _context.CelestialObjects.Update(celestialObject);

            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celestialObjects = _context.CelestialObjects.Where(x => x.Id == id || x.OrbitedObjectId == id);

            if (celestialObjects == null || celestialObjects.Count() == 0) return NotFound();

            _context.CelestialObjects.RemoveRange(celestialObjects);

            _context.SaveChanges();

            return NoContent();
        }
    }
}
