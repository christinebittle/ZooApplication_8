using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using ZooApplication.Models;
using System.Diagnostics;

namespace ZooApplication.Controllers
{
    public class AnimalDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/AnimalData/ListAnimals
        [HttpGet]
        public IEnumerable<AnimalDto> ListAnimals()
        {
            List<Animal> Animals = db.Animals.ToList();
            List<AnimalDto> AnimalDtos = new List<AnimalDto>();

            Animals.ForEach(a => AnimalDtos.Add(new AnimalDto(){ 
                AnimalID = a.AnimalID,
                AnimalName = a.AnimalName,
                AnimalWeight = a.AnimalWeight,
                SpeciesName = a.Species.SpeciesName
            }));

            return AnimalDtos;
        }

        // GET: api/AnimalData/FindAnimal/5
        [ResponseType(typeof(Animal))]
        [HttpGet]
        public IHttpActionResult FindAnimal(int id)
        {
            Animal Animal = db.Animals.Find(id);
            AnimalDto AnimalDto = new AnimalDto()
            {
                AnimalID = Animal.AnimalID,
                AnimalName = Animal.AnimalName,
                AnimalWeight = Animal.AnimalWeight,
                SpeciesName = Animal.Species.SpeciesName
            };
            if (Animal == null)
            {
                return NotFound();
            }

            return Ok(AnimalDto);
        }

        // POST: api/AnimalData/UpdateAnimal/5
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateAnimal(int id, Animal animal)
        { 
            if (!ModelState.IsValid)
            {      
                return BadRequest(ModelState);
            }

            if (id != animal.AnimalID)
            {
                
                return BadRequest();
            }

            db.Entry(animal).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AnimalExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/AnimalData/AddAnimal
        [ResponseType(typeof(Animal))]
        [HttpPost]
        public IHttpActionResult AddAnimal(Animal animal)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Animals.Add(animal);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = animal.AnimalID }, animal);
        }

        // POST: api/AnimalData/DeleteAnimal/5
        [ResponseType(typeof(Animal))]
        [HttpPost]
        public IHttpActionResult DeleteAnimal(int id)
        {
            Animal animal = db.Animals.Find(id);
            if (animal == null)
            {
                return NotFound();
            }

            db.Animals.Remove(animal);
            db.SaveChanges();

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AnimalExists(int id)
        {
            return db.Animals.Count(e => e.AnimalID == id) > 0;
        }
    }
}