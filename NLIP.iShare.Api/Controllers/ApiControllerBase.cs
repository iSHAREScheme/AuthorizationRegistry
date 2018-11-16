using System;
using Microsoft.AspNetCore.Mvc;
using NLIP.iShare.Models;

namespace NLIP.iShare.Api.Controllers
{
    [ApiController]
    public class ApiControllerBase : ControllerBase
    {
        protected ApiControllerBase()
        {

        }

        protected ActionResult OkOrBadRequest(Response response)
        {
            if (response.Success)
            {
                return Ok();
            }

            return BadRequest(response.Errors);
        }

        protected ActionResult OkOrNotFound<TModel>(TModel model)
        {
            if (object.Equals(model, default(TModel)))
            {
                return NotFound();
            }

            return Ok(model);
        }

        protected ActionResult OkOrBadRequest<TModel>(Response<TModel> response)
        {
            if (response.Success)
            {
                return Ok(response.Model);
            }

            return BadRequest(response.Errors);
        }

        protected ActionResult OkOrBadRequest<TModel>(Response<TModel> response, Func<TModel, object> map)
        {
            if (response.Success)
            {
                return Ok(map(response.Model));
            }

            return BadRequest(response.Errors);
        }
    }
}