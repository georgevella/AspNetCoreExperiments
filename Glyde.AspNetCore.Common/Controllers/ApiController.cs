using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Glyde.AspNetCore.Versioning;
using Glyde.Web.Api.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Glyde.AspNetCore.Controllers
{
    [IgnoreVersioningConvention]
    public abstract class ApiController<TResource, TResourceId> : ControllerBase
        where TResource : class
    {
        protected Func<TResourceId, TResource, Task<bool>> UpdateAction { get; set; } = null;
        protected Func<TResource, Task<TResourceId>> CreateAction { get; set; } = null;
        protected Func<TResourceId, Task<bool>> DeleteAction { get; set; } = null;
        protected Func<Task<IEnumerable<TResource>>> GetAllAction { get; set; } = null;
        protected Func<TResourceId, Task<TResource>> GetAction { get; set; } = null;


        [HttpGet]
        [Route("")]
        public virtual async Task<IActionResult> GetAll()
        {            
            if (GetAllAction == null)
                return BadRequest();

            return new JsonResult(await GetAllAction());
        }

        [HttpGet]
        [Route("{id}")]
        public virtual async Task<IActionResult> Get(TResourceId id)
        {
            if (GetAction == null)
                return BadRequest();

            try
            {
                var item = await GetAction(id);

                if (item != null)
                {
                    return new JsonResult(item);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }            

            return NotFound();
        }

        [HttpPut]
        [Route("{id}")]
        public virtual async Task<IActionResult> Update(TResourceId id, [FromBody]TResource resource)
        {
            if (UpdateAction == null)
                return BadRequest();

            try
            {
                var result = await UpdateAction(id, resource);

                return result ? NoContent() : new StatusCodeResult(StatusCodes.Status304NotModified);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost]
        [Route("")]
        public virtual async Task<IActionResult> Create([FromBody]TResource resource)
        {
            if (CreateAction == null)
                return BadRequest();

            try
            {
                var id = await CreateAction(resource);

                // TODO: build get uri from id returned above
                return Created(string.Empty, resource);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual async Task<IActionResult> Delete(TResourceId id)
        {
            if (DeleteAction == null)
                return BadRequest();

            try
            {
                return await DeleteAction(id) ? (IActionResult)NoContent() : NotFound();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}