using backendAPI.Constant;
using backendAPI.Models;
using backendAPI.Repository.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Nodes;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Configuration;
using System.Text.Encodings.Web;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace backendAPI.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class DirectController : BaseController
    {
        JsonSerializerOptions options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true
        };
        private TimeSpan currentTime;

        // GET: api/<ValuesController>
        private readonly IGenericRepository<Models.LocationModel> _location;
        public DirectController(IGenericRepository<Models.LocationModel> location)
        {
            _location = location;
        }

        [HttpPost]
        [Route("/dbaddlocation")]
        public async Task<ActionResult<List<Models.LocationModel>>> Dbaddlocation([FromBody] JsonObject data)
        {
            try
            {
                dynamic location = data["location"];
                dynamic hour = IsValidTimeFormat((string)data["hour"]);

                if (string.IsNullOrEmpty((string)location))
                    return BadRequest(new
                    {
                        Message = "Location was empty"
                    });
                ;

                if (hour == null)
                    hour = DateTime.Now.TimeOfDay;

                var locationHourRange = new Models.LocationModel
                {
                    Location = (string)location,
                    Hour = hour
                };

                await _location.InsertAsync(locationHourRange);

                return Ok(locationHourRange, "Success");

            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Errors = ex,
                    ex.Message
                });
            }

        }


        [HttpGet]
        [Route("/dbgetlocations")]
        public async Task<ActionResult<List<Models.LocationModel>>> Dbgetlocations()
        {
            try
            {
                List<Models.LocationModel> locations = _location.GetAll().ToList();
                if (locations.Count == 0)
                {
                    return BadRequest(new
                    {
                        StatusCode = 404,
                        Message = "No objects found",
                        Data = LocationDTO.serializeLocList(locations)
                    });

                }

                return Ok(locations, "Success");
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Errors = ex,
                    ex.Message
                });
            }
        }


        //HERE TEST
        [HttpPost("/getLocationsFromCsv")]
        public ActionResult<List<string[]>> GetLocationsFromCsv(IFormFile file)
        {
            try
            {
                using var reader = new StreamReader(file.OpenReadStream());
                using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ",",
                    HasHeaderRecord = true,
                    IgnoreBlankLines = true,
                });
                List<LocationDTO> locations = new List<LocationDTO>();
                TimeSpan startTime = TimeSpan.Parse(LocationConstant.StartingTime);
                TimeSpan endTime = TimeSpan.Parse(LocationConstant.EndingTime);

                while (csv.Read())
                {
                    string location = csv.GetField<string>(0);
                    dynamic hours = IsValidTimeFormat(csv.GetField<string>(1));

                    if (hours == null)
                        continue;

                    LocationDTO loc = new LocationDTO
                    {
                        Location = location,
                        Hour = hours
                    };

                    if (hours <= endTime && hours >= startTime)
                    {
                        locations.Add(loc);
                    }
                }
                return Ok(locations, "CSV file parsed successfully");

            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Errors = ex,
                    ex.Message
                });
            }
        }


        [HttpPost("/getLocationsFromDB")]
        public ActionResult<List<string[]>> GetLocationsFromDB()
        {
            try
            {
                var locations = _location.GetAll().ToList();

                TimeSpan startTime = TimeSpan.Parse(LocationConstant.StartingTime);
                TimeSpan endTime = TimeSpan.Parse(LocationConstant.EndingTime);
                List<LocationModel> filteredLocations = new List<LocationModel>();

                foreach (var location in locations)
                {
                    TimeSpan currentTime = TimeSpan.Parse(location.Hour.ToString("H:mm"));

                    if (currentTime <= endTime && currentTime >= startTime)
                    {
                        filteredLocations.Add(location);
                    }
                }

                return Ok(filteredLocations, "Locations received successfully");

            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Errors = ex,
                    ex.Message
                });
            }
        }

        public static dynamic IsValidTimeFormat(string input)
        {
            TimeSpan dummyOutput;

            if (TimeSpan.TryParse(input, out dummyOutput))
                return TimeSpan.Parse(input);

            return null;
        }



    }
}

