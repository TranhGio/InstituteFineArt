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
    public class CompetitionsController : ApiController
    {
        private FineArtEntities db = new FineArtEntities();

        // GET: api/Competitions
        public IQueryable<Competition> GetCompetitions()
        {
            return db.Competitions;
        }

        // GET: api/Competitions/5
        [ResponseType(typeof(Competition))]
        public IHttpActionResult GetCompetition(int id)
        {
            Competition competition = db.Competitions.Find(id);
            Award award = db.Awards.Where(a => a.AwardId == competition.AwardId).FirstOrDefault();
            User user = db.Users.Where(u => u.Username == User.Identity.Name).FirstOrDefault();
            var userHasPosting = (from c in db.Competitions
                           join p in db.Postings
                             on c.CompetitionId equals p.CompetitionId
                           join u in db.Users
                             on p.UserId equals u.UserId
                           where u.UserId == user.UserId && c.CompetitionId == competition.CompetitionId
                           select u).FirstOrDefault();
            Posting posting = db.Postings.Where(p => p.CompetitionId == competition.CompetitionId).FirstOrDefault();
            if (userHasPosting != null)
            {
                return Ok(new
                {
                    AwardName = award.AwardName,
                    AwardDetail = award.AwardDetail,
                    CompetitionId = competition.CompetitionId,
                    CompetitionName = competition.CompetitionName,
                    Descriptions = competition.Descriptions,
                    StartDate = competition.StartDate,
                    EndDate = competition.EndDate,
                    UserId = competition.UserId,
                    isUserHavePosting = true,
                    PostingId = posting.PostingId
                });
            }
            if (competition == null)
            {
                return NotFound();
            }

            return Ok(new {
                CompetitionId = competition.CompetitionId,
                CompetitionName = competition.CompetitionName,
                Descriptions = competition.Descriptions,
                StartDate = competition.StartDate,
                EndDate = competition.EndDate,
                UserId = competition.UserId,
                isUserHavePosting = false,
                AwardName = award.AwardName,
                AwardDetail = award.AwardDetail,

            });
        }

        // PUT: api/Competitions/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCompetition(int id, Competition competition)
        {
            User user = db.Users.Where(u => u.Username == User.Identity.Name).FirstOrDefault();
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            if (id != competition.CompetitionId || user.UserId != competition.UserId || user.RoleId != 3)
            {
                return BadRequest();
            }
            db.Entry(competition).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompetitionExists(id))
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

        // POST: api/Competitions
        [ResponseType(typeof(Competition))]
        public IHttpActionResult PostCompetition(Competition competition)
        {
            User user = db.Users.Where(u => u.Username == User.Identity.Name).FirstOrDefault();
            if (!ModelState.IsValid || user.RoleId != 3)
            {
                return BadRequest(ModelState);
            }
            competition.UserId = user.UserId;
            db.Competitions.Add(competition);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = competition.CompetitionId }, competition);
        }

        // DELETE: api/Competitions/5
        [ResponseType(typeof(Competition))]
        public IHttpActionResult DeleteCompetition(int id)
        {
            Competition competition = db.Competitions.Find(id);
            if (competition == null)
            {
                return NotFound();
            }

            db.Competitions.Remove(competition);
            db.SaveChanges();

            return Ok(competition);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CompetitionExists(int id)
        {
            return db.Competitions.Count(e => e.CompetitionId == id) > 0;
        }
    }
}