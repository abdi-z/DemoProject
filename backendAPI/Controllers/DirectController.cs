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
using LoggerService;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace backendAPI.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class DirectController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        JsonSerializerOptions options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true
            
        };
        private TimeSpan currentTime;

        // GET: api/<ValuesController>
        private readonly IGenericRepository<Models.LocationModel> _location;
        
        public DirectController(IGenericRepository<Models.LocationModel> location, ILoggerManager logger)
        {
            _logger = logger;
            _location = location;
        }

     
        [HttpPost]
        [Route("/dbaddlocation")]
        public async Task<ActionResult<List<Models.LocationModel>>> Dbaddlocation([FromBody] JsonObject data)
        {
            _logger.LogInfo("ActionMethod Called: Dbaddlocation");
            try
            {
                dynamic location = data["location"];
                dynamic hour = IsValidTimeFormat((string)data["hour"]);

                if (string.IsNullOrEmpty((string)location))
                    return BadRequest(
                        new
                        {
                            StatusCode = 400,
                            Message = "Location was empty"
                        });

                if(hour == null)
                    hour = DateTime.Now.TimeOfDay;

                var locationHourRange = new Models.LocationModel
                {
                    Location = (string)location,
                    Hour = hour
                };

                await _location.InsertAsync(locationHourRange);
                _logger.LogInfo("New location successfully added");
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Success",
                    Data = locationHourRange
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error creating location");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = 500,
                    Message = ex.Message
                });

            }

        }


        [HttpGet]
        [Route("/dbgetlocations")]
        public async Task<ActionResult<List<Models.LocationModel>>> Dbgetlocations()
        {
            _logger.LogInfo("ActionMethod Called: Dbgetlocations");
            try
            {
                List<Models.LocationModel> locations = _location.GetAll().ToList();
                if (locations.Count == 0)
                {
                    _logger.LogError("Location was not found");
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "No objects found",
                        Data = LocationDTO.serializeLocList(locations)
                    });
                }

                _logger.LogInfo("Multiple Locations were fetched");
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Success",
                    Data = locations
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Internal server error occured");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = 500,
                    Message = ex.Message
                });
            }
        }


        //HERE TEST
        [HttpPost("/getLocationsFromCsv")]
        public ActionResult<List<string[]>> GetLocationsFromCsv(IFormFile file)
        {

            _logger.LogInfo("ActionMethod Called: GetLocationsFromCsv");
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
                _logger.LogInfo("Locations from the csv was passed successfully");
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "CSV file parsed successfully",
                    Data = locations
            });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error was occured while parsing CSV");
                return BadRequest(new
                {
                    StatusCode = 500,
                    Message = ex.Message,
                });
            }
        }


        [HttpPost("/getLocationsFromDB")]
        public ActionResult<List<string[]>> GetLocationsFromDB()
        {

            _logger.LogInfo("ActionMethod Called: GetLocationsFromDB");
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

                _logger.LogInfo("Locations on a condition were fetched from database");
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Locations received successfully",
                    Data = filteredLocations
                });
            }
            catch (Exception ex)
            {

                _logger.LogError("Internal error was occured while fetching locations on a condition from Database");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = 500,
                    Message = ex.Message
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

