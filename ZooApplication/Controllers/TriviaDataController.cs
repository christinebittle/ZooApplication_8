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
    public class TriviaDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Returns all Trivias in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all Trivias in the database, including their associated species.
        /// </returns>
        /// <example>
        /// GET: api/TriviaData/ListTrivias
        /// </example>
        [HttpGet]
        [ResponseType(typeof(TriviaDto))]
        public IHttpActionResult ListTrivias()
        {
            List<Trivia> Trivias = db.Trivias.ToList();
            List<TriviaDto> TriviaDtos = new List<TriviaDto>();

            Trivias.ForEach(t => TriviaDtos.Add(new TriviaDto()
            {
                TriviaID = t.TriviaID,
                TriviaTitle = t.TriviaTitle,
                TriviaDesc = t.TriviaDesc,
                SpeciesName=t.Species.SpeciesName
            }));

            return Ok(TriviaDtos);
        }

        /// <summary>
        /// Returns all Trivias in the system associated with a particular Species.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all Trivias in the database taking care of a particular Species
        /// </returns>
        /// <param name="id">Species Primary Key</param>
        /// <example>
        /// GET: api/TriviaData/ListTriviasForSpecies/1
        /// </example>
        [HttpGet]
        [ResponseType(typeof(TriviaDto))]
        public IHttpActionResult ListTriviasForSpecies(int id)
        {
            List<Trivia> Trivias = db.Trivias.Where(
                t => t.SpeciesID==id
                ).ToList();
            List<TriviaDto> TriviaDtos = new List<TriviaDto>();

            Trivias.ForEach(t => TriviaDtos.Add(new TriviaDto()
            {
                TriviaID = t.TriviaID,
                TriviaTitle = t.TriviaTitle,
                TriviaDesc = t.TriviaDesc
            }));

            return Ok(TriviaDtos);
        }


        

        /// <summary>
        /// Returns all Trivias in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: An Trivia in the system matching up to the Trivia ID primary key
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <param name="id">The primary key of the Trivia</param>
        /// <example>
        /// GET: api/TriviaData/FindTrivia/5
        /// </example>
        [ResponseType(typeof(TriviaDto))]
        [HttpGet]
        public IHttpActionResult FindTrivia(int id)
        {
            Trivia Trivia = db.Trivias.Find(id);
            TriviaDto TriviaDto = new TriviaDto()
            {
                TriviaID = Trivia.TriviaID,
                TriviaTitle = Trivia.TriviaTitle,
                TriviaDesc = Trivia.TriviaDesc,
                SpeciesName = Trivia.Species.SpeciesName
            };
            if (Trivia == null)
            {
                return NotFound();
            }

            return Ok(TriviaDto);
        }

        /// <summary>
        /// Updates a particular Trivia in the system with POST Data input
        /// </summary>
        /// <param name="id">Represents the Trivia ID primary key</param>
        /// <param name="Trivia">JSON FORM DATA of an Trivia</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
        /// <example>
        /// POST: api/TriviaData/UpdateTrivia/5
        /// FORM DATA: Trivia JSON Object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateTrivia(int id, Trivia Trivia)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != Trivia.TriviaID)
            {

                return BadRequest();
            }

            db.Entry(Trivia).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TriviaExists(id))
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
        /// Adds an Trivia to the system
        /// </summary>
        /// <param name="Trivia">JSON FORM DATA of an Trivia</param>
        /// <returns>
        /// HEADER: 201 (Created)
        /// CONTENT: Trivia ID, Trivia Data
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/TriviaData/AddTrivia
        /// FORM DATA: Trivia JSON Object
        /// </example>
        [ResponseType(typeof(Trivia))]
        [HttpPost]
        public IHttpActionResult AddTrivia(Trivia Trivia)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Trivias.Add(Trivia);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = Trivia.TriviaID }, Trivia);
        }

        /// <summary>
        /// Deletes an Trivia from the system by it's ID.
        /// </summary>
        /// <param name="id">The primary key of the Trivia</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST: api/TriviaData/DeleteTrivia/5
        /// FORM DATA: (empty)
        /// </example>
        [ResponseType(typeof(Trivia))]
        [HttpPost]
        public IHttpActionResult DeleteTrivia(int id)
        {
            Trivia Trivia = db.Trivias.Find(id);
            if (Trivia == null)
            {
                return NotFound();
            }

            db.Trivias.Remove(Trivia);
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

        private bool TriviaExists(int id)
        {
            return db.Trivias.Count(e => e.TriviaID == id) > 0;
        }

        
    }
}
