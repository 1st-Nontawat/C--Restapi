using JsonFlatFileDataStore;
using Microsoft.AspNetCore.Mvc;

namespace RestSample.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly IDocumentCollection<User> _users;

    public UsersController(ILogger<UsersController> logger)
    {
        _logger = logger;

        var store = new DataStore("db.json");
        _users = store.GetCollection<User>();
    }

    [HttpPost]
    public void Post([FromBody] User user)
    {
        _users.InsertOne(user);
    }

    [HttpGet]
    public IEnumerable<User> Get()
    {
        return _users.AsQueryable().ToList();
    }

    [HttpGet("{id:int}")]
    public ActionResult<User> GetById(int id)
    {
        var user = _users.AsQueryable().FirstOrDefault(user => user.Id == id);
        if (user == null)
        {
            return NotFound();
        }
        return user;
    }

    [HttpPut("{id:int}")]
    public IActionResult Put(int id, [FromBody] User updatedUser)
    {
        var existingUser = _users.AsQueryable().FirstOrDefault(user => user.Id == id);
        if (existingUser == null)
        {
            return NotFound();
        }

        existingUser.Name = updatedUser.Name;
        _users.UpdateOne(user => user.Id == id, existingUser);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var deleted = _users.DeleteOne(user => user.Id == id);
        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
