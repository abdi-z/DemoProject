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
using Microsoft.EntityFrameworkCore;
using backendAPI.UnitOfWork;
using backendAPI.IRepository;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace backendAPI.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class DirectController : ControllerBase
    {
        private readonly IUnitOfWork<DatabaseContextCla> _unitOfWork;
        // private UnitOfWork<DatabaseContextCla> unitOfWork = new UnitOfWork<DatabaseContextCla>();
        private GenericRepository<LocationModel> genericRepository;
        private ILocationRepository locationRepository;
     

        JsonSerializerOptions options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true
            
        };
        
        private TimeSpan currentTime;

        // GET: api/<ValuesController>
        private readonly IGenericRepository<Models.LocationModel> _location;

        public DirectController(IGenericRepository<Models.LocationModel> location,IUnitOfWork<DatabaseContextCla> unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _location = location;
            genericRepository = new GenericRepository<LocationModel>(unitOfWork);
        }
        //UoW-below
        [HttpPost]
        [Route("/uow-get")]
        public ActionResult<List<string[]>> UowGet()
        {
            try
            {
                var locations = _location.GetAll().ToList();
                var uowLocations = genericRepository.GetAll().ToList();

                TimeSpan startTime = TimeSpan.Parse(LocationConstant.StartingTime);
                TimeSpan endTime = TimeSpan.Parse(LocationConstant.EndingTime);
                List<LocationModel> filteredLocations = new List<LocationModel>();

                foreach (var location in uowLocations)
                {
                    TimeSpan currentTime = TimeSpan.Parse(location.Hour.ToString("H:mm"));

                    if (currentTime <= endTime && currentTime >= startTime)
                    {
                        filteredLocations.Add(location);
                    }
                }


                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Locations received successfully",
                    Data = filteredLocations
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = 500,
                    Message = ex.Message
                });
            }
        }


        //UoW-Above




        [HttpPost]
        [Route("/dbaddlocation")]
        public async Task<ActionResult<List<Models.LocationModel>>> Dbaddlocation([FromBody] JsonObject data)
        {
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

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Success",
                    Data = locationHourRange
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = 500,
                    Message = ex.Message
                });

            }

        }


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
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "CSV file parsed successfully",
                    Data = locations
            });
            }
            catch (Exception ex)
            {
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


                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Locations received successfully",
                    Data = filteredLocations
                });
            }
            catch (Exception ex)
            {
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

