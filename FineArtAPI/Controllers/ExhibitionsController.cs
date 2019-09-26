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
    public class ExhibitionsController : ApiController
    {
        private FineArtEntities db = new FineArtEntities();

        // GET: api/Exhibitions
        public IQueryable<Exhibition> GetExhibitions()
        {
            return db.Exhibitions;
        }

        // GET: api/Exhibitions/5
        [ResponseType(typeof(Exhibition))]
        public IHttpActionResult GetExhibition(int id)
        {
            Exhibition exhibition = db.Exhibitions.Find(id);
            if (exhibition == null)
            {
                return NotFound();
            }

            return Ok(exhibition);
        }

        // PUT: api/Exhibitions/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutExhibition(int id, Exhibition exhibition)
        {
            User user = db.Users.Where(u => u.Username == User.Identity.Name).FirstOrDefault();
            if (!ModelState.IsValid || user.RoleId != 2 )
            {
                return BadRequest(ModelState);
            }

            if (id != exhibition.ExhibitionId)
            {
                return BadRequest();
            }

            db.Entry(exhibition).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExhibitionExists(id))
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

        // POST: api/Exhibitions
        [ResponseType(typeof(Exhibition))]
        public IHttpActionResult PostExhibition(Exhibition exhibition)
        {
            User user = db.Users.Where(u => u.Username == User.Identity.Name).FirstOrDefault();

            if (!ModelState.IsValid || user.RoleId!= 2)
            {
                return BadRequest(ModelState);
            }

            db.Exhibitions.Add(exhibition);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = exhibition.ExhibitionId }, exhibition);
        }

        // DELETE: api/Exhibitions/5
        [ResponseType(typeof(Exhibition))]
        public IHttpActionResult DeleteExhibition(int id)
        {
            User user = db.Users.Where(u => u.Username == User.Identity.Name).FirstOrDefault();
            if(user.RoleId != 2)
            {
                return BadRequest();
            }
            Exhibition exhibition = db.Exhibitions.Find(id);
            if (exhibition == null)
            {
                return NotFound();
            }

            db.Exhibitions.Remove(exhibition);
            db.SaveChanges();

            return Ok(exhibition);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ExhibitionExists(int id)
        {
            return db.Exhibitions.Count(e => e.ExhibitionId == id) > 0;
        }
    }
}