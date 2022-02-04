using Microsoft.AspNetCore.Mvc;
using VroomStats.Models;
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

    /// <summary>
    /// Creates a new instance of the <see cref="CarsController"/>.
    /// </summary>
    /// <param name="database">Instance of <see cref="IDatabaseService"/>.</param>
    public CarsController(IDatabaseService database)
    {
        _database = database;
    }
    
    /// <summary>
    /// Gets the latest data for a specific car.
    /// </summary>
    /// <param name="carId">Id of the car.</param>
    [HttpGet("{carId}")]
    public async Task<IActionResult> GetLatestDataAsync(string carId)
    {
        var data = await _database.GetLatestDataAsync(carId);
        return Json(data);
    }

    /// <summary>
    /// Registers a new car into the database.
    /// </summary>
    /// <param name="carId">Id of the car.</param>
    /// <param name="carCreateModel">Instance of a body model.</param>
    [HttpPost("{carId}")]
    public async Task<IActionResult> RegisterCarAsync(string carId, CarCreateModel carCreateModel)
    {
        if (await _database.RegisterCarAsync(carId, carCreateModel.DisplayName))
        {
            return NoContent();
        }

        return BadRequest();
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