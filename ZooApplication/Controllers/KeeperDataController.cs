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
    public class KeeperDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Returns all Keepers in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all Keepers in the database, including their associated species.
        /// </returns>
        /// <example>
        /// GET: api/KeeperData/ListKeepers
        /// </example>
        [HttpGet]
        [ResponseType(typeof(KeeperDto))]
        public IHttpActionResult ListKeepers()
        {
            List<Keeper> Keepers = db.Keepers.ToList();
            List<KeeperDto> KeeperDtos = new List<KeeperDto>();

            Keepers.ForEach(k => KeeperDtos.Add(new KeeperDto()
            {
                KeeperID = k.KeeperID,
                KeeperFirstName = k.KeeperFirstName,
                KeeperLastName=k.KeeperLastName
            }));

            return Ok(KeeperDtos);
        }

        /// <summary>
        /// Returns all Keepers in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: An Keeper in the system matching up to the Keeper ID primary key
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <param name="id">The primary key of the Keeper</param>
        /// <example>
        /// GET: api/KeeperData/FindKeeper/5
        /// </example>
        [ResponseType(typeof(KeeperDto))]
        [HttpGet]
        public IHttpActionResult FindKeeper(int id)
        {
            Keeper Keeper = db.Keepers.Find(id);
            KeeperDto KeeperDto = new KeeperDto()
            {
                KeeperID = Keeper.KeeperID,
                KeeperFirstName = Keeper.KeeperFirstName,
                KeeperLastName = Keeper.KeeperLastName
            };
            if (Keeper == null)
            {
                return NotFound();
            }

            return Ok(KeeperDto);
        }

        /// <summary>
        /// Updates a particular Keeper in the system with POST Data input
        /// </summary>
        /// <param name="id">Represents the Keeper ID primary key</param>
        /// <param name="Keeper">JSON FORM DATA of an Keeper</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
        /// <example>
        /// POST: api/KeeperData/UpdateKeeper/5
        /// FORM DATA: Keeper JSON Object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateKeeper(int id, Keeper Keeper)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != Keeper.KeeperID)
            {

                return BadRequest();
            }

            db.Entry(Keeper).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KeeperExists(id))
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
        /// Adds an Keeper to the system
        /// </summary>
        /// <param name="Keeper">JSON FORM DATA of an Keeper</param>
        /// <returns>
        /// HEADER: 201 (Created)
        /// CONTENT: Keeper ID, Keeper Data
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/KeeperData/AddKeeper
        /// FORM DATA: Keeper JSON Object
        /// </example>
        [ResponseType(typeof(Keeper))]
        [HttpPost]
        public IHttpActionResult AddKeeper(Keeper Keeper)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Keepers.Add(Keeper);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = Keeper.KeeperID }, Keeper);
        }

        /// <summary>
        /// Deletes an Keeper from the system by it's ID.
        /// </summary>
        /// <param name="id">The primary key of the Keeper</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST: api/KeeperData/DeleteKeeper/5
        /// FORM DATA: (empty)
        /// </example>
        [ResponseType(typeof(Keeper))]
        [HttpPost]
        public IHttpActionResult DeleteKeeper(int id)
        {
            Keeper Keeper = db.Keepers.Find(id);
            if (Keeper == null)
            {
                return NotFound();
            }

            db.Keepers.Remove(Keeper);
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

        private bool KeeperExists(int id)
        {
            return db.Keepers.Count(e => e.KeeperID == id) > 0;
        }
    }
}