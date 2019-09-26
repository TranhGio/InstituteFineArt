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
    public class PostingsController : ApiController
    {
        private FineArtEntities db = new FineArtEntities();

        // GET: api/Postings
        public IQueryable<Posting> GetPostings()
        {
            return db.Postings;
        }

        // GET: api/Postings/5
        [ResponseType(typeof(Posting))]
        public IHttpActionResult GetPosting(int id)
        {
            Posting posting = db.Postings.Find(id);
            if (posting == null)
            {
                return NotFound();
            }

            return Ok(posting);
        }

        // PUT: api/Postings/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutPosting(int id, Posting posting)
        {
            User user = db.Users.Where(u => u.Username == User.Identity.Name).FirstOrDefault();
            if (!ModelState.IsValid || user.UserId != 4)
            {
                return BadRequest(ModelState);
            }

            if (id != posting.PostingId)
            {
                return BadRequest();
            }

            db.Entry(posting).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostingExists(id))
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

        // POST: api/Postings
        [ResponseType(typeof(Posting))]
        public IHttpActionResult PostPosting(Posting posting)
        {
            User user = db.Users.Where(u => u.Username == User.Identity.Name).FirstOrDefault();
            if (!ModelState.IsValid || user.RoleId != 4)
            {
                return BadRequest(ModelState);
            }

            db.Postings.Add(posting);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = posting.PostingId }, posting);
        }

        // DELETE: api/Postings/5
        [ResponseType(typeof(Posting))]
        public IHttpActionResult DeletePosting(int id)
        {
            User user = db.Users.Where(u => u.Username == User.Identity.Name).FirstOrDefault();
            if (user.RoleId == 2 || user.RoleId == 1)
            {
                BadRequest();
            }
            Posting posting = db.Postings.Find(id);
            if (posting == null)
            {
                return NotFound();
            }
            
            posting.isActive = false;
            //db.Postings.Remove(posting);
            db.SaveChanges();
            return Ok(posting);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PostingExists(int id)
        {
            return db.Postings.Count(e => e.PostingId == id) > 0;
        }
    }
}