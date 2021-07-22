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
    public class SpeciesDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Returns all Species in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all Speciess in the database, including their associated species.
        /// </returns>
        /// <example>
        /// GET: api/SpeciesData/ListSpecies
        /// </example>
        [HttpGet]
        [ResponseType(typeof(SpeciesDto))]
        public IHttpActionResult ListSpecies()
        {
            List<Species> Species = db.Species.ToList();
            List<SpeciesDto> SpeciesDtos = new List<SpeciesDto>();

            Species.ForEach(s => SpeciesDtos.Add(new SpeciesDto()
            {
                SpeciesID = s.SpeciesID,
                SpeciesName = s.SpeciesName,
                SpeciesEndangered = s.SpeciesEndangered
            }));

            return Ok(SpeciesDtos);
        }

        /// <summary>
        /// Returns all Species in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: An Species in the system matching up to the Species ID primary key
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <param name="id">The primary key of the Species</param>
        /// <example>
        /// GET: api/SpeciesData/FindSpecies/5
        /// </example>
        [ResponseType(typeof(SpeciesDto))]
        [HttpGet]
        public IHttpActionResult FindSpecies(int id)
        {
            Species Species = db.Species.Find(id);
            SpeciesDto SpeciesDto = new SpeciesDto()
            {
                SpeciesID = Species.SpeciesID,
                SpeciesName = Species.SpeciesName,
                SpeciesEndangered = Species.SpeciesEndangered
            };
            if (Species == null)
            {
                return NotFound();
            }

            return Ok(SpeciesDto);
        }

        /// <summary>
        /// Updates a particular Species in the system with POST Data input
        /// </summary>
        /// <param name="id">Represents the Species ID primary key</param>
        /// <param name="Species">JSON FORM DATA of an Species</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
        /// <example>
        /// POST: api/SpeciesData/UpdateSpecies/5
        /// FORM DATA: Species JSON Object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateSpecies(int id, Species Species)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != Species.SpeciesID)
            {

                return BadRequest();
            }

            db.Entry(Species).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SpeciesExists(id))
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
        /// Adds an Species to the system
        /// </summary>
        /// <param name="Species">JSON FORM DATA of an Species</param>
        /// <returns>
        /// HEADER: 201 (Created)
        /// CONTENT: Species ID, Species Data
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/SpeciesData/AddSpecies
        /// FORM DATA: Species JSON Object
        /// </example>
        [ResponseType(typeof(Species))]
        [HttpPost]
        public IHttpActionResult AddSpecies(Species Species)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Species.Add(Species);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = Species.SpeciesID }, Species);
        }

        /// <summary>
        /// Deletes an Species from the system by it's ID.
        /// </summary>
        /// <param name="id">The primary key of the Species</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST: api/SpeciesData/DeleteSpecies/5
        /// FORM DATA: (empty)
        /// </example>
        [ResponseType(typeof(Species))]
        [HttpPost]
        public IHttpActionResult DeleteSpecies(int id)
        {
            Species Species = db.Species.Find(id);
            if (Species == null)
            {
                return NotFound();
            }

            db.Species.Remove(Species);
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

        private bool SpeciesExists(int id)
        {
            return db.Species.Count(e => e.SpeciesID == id) > 0;
        }
    }
}