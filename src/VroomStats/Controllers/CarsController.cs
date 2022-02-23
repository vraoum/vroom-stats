using Microsoft.AspNetCore.Mvc;
using VroomStats.Models;
using VroomStats.Payloads;
using VroomStats.Services;

namespace VroomStats.Controllers;

/// <summary>
/// Represents the API controller to interact with the Database. 
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class CarsController : Controller
{
    private readonly IDatabaseService _database;
    private readonly ILogger<CarsController> _logger;

    /// <summary>
    /// Creates a new instance of the <see cref="CarsController"/>.
    /// </summary>
    /// <param name="database">Instance of <see cref="IDatabaseService"/>.</param>
    /// <param name="logger">Instance of the <see cref="ILogger{CarsController}"/></param>
    public CarsController(IDatabaseService database, ILogger<CarsController> logger)
    {
        _database = database;
        _logger = logger;
    }
    
    /// <summary>
    /// Gets the latest data for a specific car.
    /// </summary>
    /// <param name="carId">Id of the car.</param>
    [HttpGet("{carId}/data")]
    public async Task<IActionResult> GetLatestDataAsync(string carId)
    {
        var data = await _database.GetLatestDataAsync(carId);
        if (data is null)
        {
            return BadRequest();
        }
        
        return Json(data);
    }
    
    /// <summary>
    /// Gets the entire known things about a car.
    /// </summary>
    /// <param name="carId">Id of the car.</param>
    [HttpGet("{carId}")]
    public async Task<IActionResult> GetCarAsync(string carId)
    {
        var data = await _database.GetCarAsync(carId);
        if (data is null)
        {
            return BadRequest();
        }
        
        return Json(data);
    }

    /// <summary>
    /// Registers a new car into the database.
    /// </summary>
    /// <param name="carId">Id of the car.</param>
    /// <param name="carSettingsModel">Instance of a body model.</param>
    [HttpPost("{carId}")]
    public async Task<IActionResult> RegisterCarAsync(string carId, CarSettingsModel carSettingsModel)
    {
        if (await _database.RegisterCarAsync(carId, carSettingsModel))
        {
            return NoContent();
        }

        return BadRequest();
    }
    
    /// <summary>
    /// Registers a new car into the database.
    /// </summary>
    /// <param name="carId">Id of the car.</param>
    /// <param name="carSettingsModel">Instance of a body model.</param>
    [HttpPut("{carId}")]
    public async Task<IActionResult> EditSettingsAsync(string carId, CarSettingsModel carSettingsModel)
    {
        var carData = await _database.UpdateSettingsAsync(carId, carSettingsModel);
        if (carData is null)
        {
            return BadRequest();
        }
        
        return Json(carData);
    }

    /// <summary>
    /// Adds data to a specific car.
    /// </summary>
    /// <param name="carId">Id of the car.</param>
    /// <param name="carDataModel">Instance of a body model.</param>
    /// <remarks>
    /// It is an alternative of the WebSocket connection.
    /// </remarks>
    [HttpPost("{carId}/data")]
    public async Task<IActionResult> AppendDataAsync(string carId, CarDataAppendModel carDataModel)
    {
        await _database.AppendDataAsync(carId, new BasePayload(OpCode.Data, carDataModel.Data));
        return NoContent();
    }
    
    /// <summary>
    /// Gets the available and known cars.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetCarsAsync()
    {
        return Json(await _database.GetCarsAsync());
    }
}