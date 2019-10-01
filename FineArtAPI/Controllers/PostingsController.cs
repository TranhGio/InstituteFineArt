using System;
using System.Collections;
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
        public IHttpActionResult GetPostings()
        {
            
            return Ok(db.Postings);
        }

        // GET: api/Postings/5
        [ResponseType(typeof(Posting))]
        public IHttpActionResult GetPosting(int id)
        {
            Posting posting = db.Postings.Find(id);
            Competition competition = db.Competitions.Where(c => c.CompetitionId == posting.CompetitionId).FirstOrDefault();
            User user = db.Users.Where(u => u.UserId == posting.UserId).FirstOrDefault();
            if (posting == null)
            {
                return NotFound();
            }

            return Ok(new {
                CompetitionId = competition.CompetitionId,
                CompetitionName=competition.CompetitionName,
                AwardId=competition.AwardId,
                Descriptions = competition.Descriptions,
                StartDate = competition.StartDate,
                EndDate= competition.EndDate,

                PostingId = posting.PostingId,
                Mark = posting.Mark,
                Quote = posting.Quote,
                LastEdit = posting.LastEdit,
                ImagePath = posting.ImagePath,
                PostingUserId = posting.UserId,
                UserNameCreatePosting = user.Username
            });
        }

        // PUT: api/Postings/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutPosting([FromUri]int id, [FromBody]Posting posting)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            User user = db.Users.Where(u => u.Username == User.Identity.Name).FirstOrDefault();
            Competition competition = db.Competitions.Where(c => c.CompetitionId == posting.CompetitionId).FirstOrDefault();
            Posting postingDB = db.Postings.Where(p => p.PostingId == id).FirstOrDefault();
            Posting postingSubmit = new Posting();
            postingSubmit = postingDB;
            switch (user.RoleId)
            {
                case 3:
                    //TODO teacher
                    if (user.UserId != competition.UserId)
                    {
                        return BadRequest();
                    }
                    postingSubmit.Mark = posting.Mark;
                    break;
                case 4:
                    //TODO student
                    if(user.UserId != postingDB.UserId)
                    {
                        return BadRequest();
                    }
                    postingSubmit.ImagePath = posting.ImagePath;
                    postingSubmit.Quote = posting.Quote;
                    break;
                default:
                    return BadRequest();
            }
            db.Entry(postingSubmit).State = EntityState.Modified;
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
            posting.UserId = user.UserId;
            posting.LastEdit = DateTime.Now;
            int intIdt = db.Users.Max(u => u.UserId);
            posting.PostingId = intIdt + 1;
            posting.isActive = true;
            posting.Mark = 0;
            db.Postings.Add(posting);
            db.SaveChanges();

            return Ok(posting.PostingId);
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
            db.Postings.Remove(posting);
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