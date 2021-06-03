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

        /// <summary>
        /// Returns all animals in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all animals in the database, including their associated species.
        /// </returns>
        /// <example>
        /// GET: api/AnimalData/ListAnimals
        /// </example>
        [HttpGet]
        [ResponseType(typeof(AnimalDto))]
        public IHttpActionResult ListAnimals()
        {
            List<Animal> Animals = db.Animals.ToList();
            List<AnimalDto> AnimalDtos = new List<AnimalDto>();

            Animals.ForEach(a => AnimalDtos.Add(new AnimalDto(){ 
                AnimalID = a.AnimalID,
                AnimalName = a.AnimalName,
                AnimalWeight = a.AnimalWeight,
                SpeciesID = a.Species.SpeciesID,
                SpeciesName = a.Species.SpeciesName
            }));

            return Ok(AnimalDtos);
        }

        /// <summary>
        /// Gathers information about all animals related to a particular species ID
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all animals in the database, including their associated species matched with a particular species ID
        /// </returns>
        /// <param name="id">Species ID.</param>
        /// <example>
        /// GET: api/AnimalData/ListAnimalsForSpecies/3
        /// </example>
        [HttpGet]
        [ResponseType(typeof(AnimalDto))]
        public IHttpActionResult ListAnimalsForSpecies(int id)
        {
            List<Animal> Animals = db.Animals.Where(a=>a.SpeciesID==id).ToList();
            List<AnimalDto> AnimalDtos = new List<AnimalDto>();

            Animals.ForEach(a => AnimalDtos.Add(new AnimalDto()
            {
                AnimalID = a.AnimalID,
                AnimalName = a.AnimalName,
                AnimalWeight = a.AnimalWeight,
                SpeciesID = a.Species.SpeciesID,
                SpeciesName = a.Species.SpeciesName
            }));

            return Ok(AnimalDtos);
        }

        /// <summary>
        /// Gathers information about animals related to a particular keeper
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all animals in the database, including their associated species that match to a particular keeper id
        /// </returns>
        /// <param name="id">Keeper ID.</param>
        /// <example>
        /// GET: api/AnimalData/ListAnimalsForKeeper/1
        /// </example>
        [HttpGet]
        [ResponseType(typeof(AnimalDto))]
        public IHttpActionResult ListAnimalsForKeeper(int id)
        {
            //all animals that have keepers which match with our ID
            List<Animal> Animals = db.Animals.Where(
                a=>a.Keepers.Any(
                    k=>k.KeeperID==id
                )).ToList();
            List<AnimalDto> AnimalDtos = new List<AnimalDto>();

            Animals.ForEach(a => AnimalDtos.Add(new AnimalDto()
            {
                AnimalID = a.AnimalID,
                AnimalName = a.AnimalName,
                AnimalWeight = a.AnimalWeight,
                SpeciesID = a.Species.SpeciesID,
                SpeciesName = a.Species.SpeciesName
            }));

            return Ok(AnimalDtos);
        }


        /// <summary>
        /// Associates a particular keeper with a particular animal
        /// </summary>
        /// <param name="animalid">The animal ID primary key</param>
        /// <param name="keeperid">The keeper ID primary key</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST api/AnimalData/AssociateAnimalWithKeeper/9/1
        /// </example>
        [HttpPost]
        [Route("api/AnimalData/AssociateAnimalWithKeeper/{animalid}/{keeperid}")]
        [Authorize]
        public IHttpActionResult AssociateAnimalWithKeeper(int animalid, int keeperid)
        {
            
            Animal SelectedAnimal = db.Animals.Include(a=>a.Keepers).Where(a=>a.AnimalID==animalid).FirstOrDefault();
            Keeper SelectedKeeper = db.Keepers.Find(keeperid);

            if(SelectedAnimal==null || SelectedKeeper == null)
            {
                return NotFound();
            }

            Debug.WriteLine("input animal id is: " + animalid);
            Debug.WriteLine("selected animal name is: "+ SelectedAnimal.AnimalName);
            Debug.WriteLine("input keeper id is: " + keeperid);
            Debug.WriteLine("selected keeper name is: " + SelectedKeeper.KeeperFirstName);


            SelectedAnimal.Keepers.Add(SelectedKeeper);
            db.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Removes an association between a particular keeper and a particular animal
        /// </summary>
        /// <param name="animalid">The animal ID primary key</param>
        /// <param name="keeperid">The keeper ID primary key</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST api/AnimalData/AssociateAnimalWithKeeper/9/1
        /// </example>
        [HttpPost]
        [Route("api/AnimalData/UnAssociateAnimalWithKeeper/{animalid}/{keeperid}")]
        [Authorize]
        public IHttpActionResult UnAssociateAnimalWithKeeper(int animalid, int keeperid)
        {

            Animal SelectedAnimal = db.Animals.Include(a => a.Keepers).Where(a => a.AnimalID == animalid).FirstOrDefault();
            Keeper SelectedKeeper = db.Keepers.Find(keeperid);

            if (SelectedAnimal == null || SelectedKeeper == null)
            {
                return NotFound();
            }

            Debug.WriteLine("input animal id is: " + animalid);
            Debug.WriteLine("selected animal name is: " + SelectedAnimal.AnimalName);
            Debug.WriteLine("input keeper id is: " + keeperid);
            Debug.WriteLine("selected keeper name is: " + SelectedKeeper.KeeperFirstName);


            SelectedAnimal.Keepers.Remove(SelectedKeeper);
            db.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Returns all animals in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: An animal in the system matching up to the animal ID primary key
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <param name="id">The primary key of the animal</param>
        /// <example>
        /// GET: api/AnimalData/FindAnimal/5
        /// </example>
        [ResponseType(typeof(AnimalDto))]
        [HttpGet]
        public IHttpActionResult FindAnimal(int id)
        {
            Animal Animal = db.Animals.Find(id);
            AnimalDto AnimalDto = new AnimalDto()
            {
                AnimalID = Animal.AnimalID,
                AnimalName = Animal.AnimalName,
                AnimalWeight = Animal.AnimalWeight,
                SpeciesID = Animal.Species.SpeciesID,
                SpeciesName = Animal.Species.SpeciesName
            };
            if (Animal == null)
            {
                return NotFound();
            }

            return Ok(AnimalDto);
        }

        /// <summary>
        /// Updates a particular animal in the system with POST Data input
        /// </summary>
        /// <param name="id">Represents the Animal ID primary key</param>
        /// <param name="animal">JSON FORM DATA of an animal</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
        /// <example>
        /// POST: api/AnimalData/UpdateAnimal/5
        /// FORM DATA: Animal JSON Object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        [Authorize]
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

        /// <summary>
        /// Adds an animal to the system
        /// </summary>
        /// <param name="animal">JSON FORM DATA of an animal</param>
        /// <returns>
        /// HEADER: 201 (Created)
        /// CONTENT: Animal ID, Animal Data
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/AnimalData/AddAnimal
        /// FORM DATA: Animal JSON Object
        /// </example>
        [ResponseType(typeof(Animal))]
        [HttpPost]
        [Authorize]
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

        /// <summary>
        /// Deletes an animal from the system by it's ID.
        /// </summary>
        /// <param name="id">The primary key of the animal</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST: api/AnimalData/DeleteAnimal/5
        /// FORM DATA: (empty)
        /// </example>
        [ResponseType(typeof(Animal))]
        [HttpPost]
        [Authorize]
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