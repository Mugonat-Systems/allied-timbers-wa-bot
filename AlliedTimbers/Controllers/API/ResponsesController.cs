using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Mvc;
using AlliedTimbers.Models;

namespace AlliedTimbers.Controllers.API
{
    public class ResponsesController : ApiController
    {
        private readonly ApplicationDbContext _db = new();

        public IQueryable<QuickResponse> GetChatQuickResponses()
        {
            return _db.QuickResponses;
        }

        [ResponseType(typeof(QuickResponse))]
        public async Task<IHttpActionResult> GetChatQuickResponse(int id)
        {
            QuickResponse chatQuickRespons = await _db.QuickResponses.FindAsync(id);
            if (chatQuickRespons == null)
            {
                return NotFound();
            }

            return Ok(chatQuickRespons);
        }

        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutChatQuickResponse(int id, QuickResponse quickResponse)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != quickResponse.Id)
            {
                return BadRequest();
            }

            _db.Entry(quickResponse).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuickResponseExists(id))
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

        [ResponseType(typeof(QuickResponse))]
        public async Task<IHttpActionResult> PostChatQuickResponse(QuickResponse quickResponse)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _db.QuickResponses.Add(quickResponse);
            await _db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = quickResponse.Id }, quickResponse);
        }

        // DELETE: api/Responses/5
        [ResponseType(typeof(QuickResponse))]
        public async Task<IHttpActionResult> DeleteChatQuickResponse(int id)
        {
            var quickResponse = await _db.QuickResponses.FindAsync(id);
            if (quickResponse == null)
            {
                return NotFound();
            }

            _db.QuickResponses.Remove(quickResponse);
            await _db.SaveChangesAsync();

            return Ok(quickResponse);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool QuickResponseExists(int id)
        {
            return _db.QuickResponses.Count(e => e.Id == id) > 0;
        }

    }
}