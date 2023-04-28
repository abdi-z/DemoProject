using backendAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace backendAPI.Controllers
{
    public class BaseController : ControllerBase
    {
        protected OkObjectResult Ok(object value, string message)
        {
            return base.Ok(new Response()
            {
                Status = backendAPI.Models.Response.RequestStatus.Success,
                Message = message,
                Payload = value.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .ToDictionary(prop => prop.Name, prop => prop.GetValue(value, null))
            });
        }
        public override BadRequestObjectResult BadRequest(object value)
        {

            if (value.GetType() != typeof(string))
            {
                Dictionary<string, object> errorDictionary = value.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .ToDictionary(prop => prop.Name, prop => prop.GetValue(value, null));

                return base.BadRequest(new Response()
                {
                    Status = backendAPI.Models.Response.RequestStatus.Error,
                    Message = errorDictionary["Message"].ToString(),
                    Errors = errorDictionary["Errors"]
                });
            }
            return base.BadRequest(new Response()
            {
                Status = backendAPI.Models.Response.RequestStatus.Error,
                Message = value.ToString(),
                Errors = null
            });
        }

    }
}
