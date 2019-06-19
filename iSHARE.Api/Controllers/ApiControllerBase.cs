using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using iSHARE.Abstractions;
using iSHARE.Models;
using Microsoft.AspNetCore.Mvc;

namespace iSHARE.Api.Controllers
{
    [ApiController]
    public class ApiControllerBase : ControllerBase
    {
        protected ApiControllerBase()
        {

        }

        protected ActionResult FromResponse(Response response)
        {
            if (response.Success)
            {
                return Ok();
            }

            return Fail(response);
        }

        protected ActionResult FromResponse<TModel>(Response<TModel> response)
        {
            if (response.Success)
            {
                return Ok(response.Model);
            }

            return Fail(response);
        }

        protected ActionResult OkOrNotFound(object model)
        {
            if (model == null)
            {
                return NotFound();
            }

            return Ok(model);
        }

        protected ActionResult FromResponse<TModel>(Response<TModel> response, Func<TModel, object> map)
        {
            if (!response.Success)
            {
                return Fail(response);
            }

            if (Equals(response.Model, default(TModel)))
            {
                return Ok();
            }

            return Ok(map(response.Model));
        }

        protected ActionResult FromResponse<TModel>(Response<IReadOnlyCollection<TModel>> response, Func<TModel, object> map)
        {
            if (!response.Success)
            {
                return Fail(response);
            }

            if (response.Model == null)
            {
                return Ok();
            }

            return Ok(response.Model.Select(map));

        }

        protected ActionResult FromResponse<TModel, TProject>(Response<PagedResult<TModel>> response, Func<TModel, TProject> map)
        {
            if (response.Success)
            {
                return Ok(new PagedResult<TProject>
                {
                    Data = response.Model.Data.Select(map),
                    Count = response.Model.Count
                });
            }

            return Fail(response);
        }


        private ActionResult Fail<TItem>(Response<TItem> response)
        {
            return BuildFailAction(response.Status, response.Errors);
        }

        private ActionResult Fail(Response result)
        {
            return BuildFailAction(result.Status, result.Errors);
        }

        private ActionResult BuildFailAction(ResponseStatus status, IReadOnlyCollection<string> errors)
        {
            switch (status)
            {
                case ResponseStatus.NotFound when errors.Any():
                    return NotFound(errors);
                case ResponseStatus.NotFound:
                    return NotFound();
                case ResponseStatus.NotAuthorized:
                    return Unauthorized();
                case ResponseStatus.InvalidOperation:
                    return BadRequest(errors);
                default:
                    return null;
            }
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
