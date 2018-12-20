using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using NLIP.iShare.Abstractions;
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

        protected ActionResult OkOrBadRequest<TModel>(Response<IReadOnlyCollection<TModel>> response, Func<TModel, object> map)
        {
            if (response.Success)
            {
                return Ok(response.Model.Select(map));
            }

            return BadRequest(response.Errors);
        }

        protected ActionResult OkOrBadRequest<TModel, TProject>(Response<PagedResult<TModel>> response, Func<TModel, TProject> map)
        {
            if (!response.Success)
            {
                return BadRequest(response.Errors);
            }

            return Ok(new PagedResult<TProject>
            {
                Data = response.Model.Data.Select(map),
                Count = response.Model.Count
            });
        }

        protected ActionResult OkOrNotFound<TModel>(Response<TModel> response, Func<TModel, object> map)
        {
            if (response.Success)
            {
                return Ok(map(response.Model));
            }

            return NotFound(response.Errors);
        }

        protected FileStreamResult BuildJsonDownloadFileResult(string fileName, string jsonData)
        {
            var cd = new ContentDisposition
            {
                FileName = fileName,
                Inline = false
            };
            Response.Headers.Add("Content-Disposition", cd.ToString());
            Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(jsonData);
            writer.Flush();
            stream.Position = 0;
            return File(stream, "text/plain");
        }
    }
}
