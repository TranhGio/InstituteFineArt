using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using FineArtAPI.Models;

namespace FineArtAPI.Controllers
{
    [Authorize]
    public class AwardsController : ApiController
    {
        private FineArtEntities db = new FineArtEntities();

        // GET: api/Awards
        public IQueryable<Award> GetAwards()
        {
            return db.Awards;
        }

        // GET: api/Awards/5
        [ResponseType(typeof(Award))]
        public IHttpActionResult GetAward(int id)
        {
            Award award = db.Awards.Find(id);
            if (award == null)
            {
                return NotFound();
            }

            return Ok(award);
        }

        // PUT: api/Awards/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutAward(int id, Award award)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != award.AwardId)
            {
                return BadRequest();
            }

            db.Entry(award).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AwardExists(id))
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

        // POST: api/Awards
        [ResponseType(typeof(Award))]
        public IHttpActionResult PostAward(Award award)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Awards.Add(award);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = award.AwardId }, award);
        }

        // DELETE: api/Awards/5
        [ResponseType(typeof(Award))]
        public IHttpActionResult DeleteAward(int id)
        {
            Award award = db.Awards.Find(id);
            if (award == null)
            {
                return NotFound();
            }

            db.Awards.Remove(award);
            db.SaveChanges();

            return Ok(award);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AwardExists(int id)
        {
            return db.Awards.Count(e => e.AwardId == id) > 0;
        }
    }
}