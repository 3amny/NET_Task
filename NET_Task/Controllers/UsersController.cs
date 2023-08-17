using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NET_Task.Context;
using NET_Task.Models;

namespace NET_Task.Controllers
{
    public class UsersController : Controller
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }
        [HttpPost("upload")]
        public async Task<IActionResult> UploadCsvFile(IFormFile file)
        {
            // checking the file
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is empty or does not exist.");
            }
            
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                bool isFirstLine = true; // check if header
                Dictionary<string, int> columnIndices = new Dictionary<string, int>(); // Creating a dictionary to set the header column to its index value

                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    var values = line.Split(','); // to avoid warnings line?.Split(',')

                    if (isFirstLine)
                    {
                        for (int i = 0; i < values.Length; i++)
                        {
                            columnIndices[values[i].ToLower()] = i;  // Mapping each header to its column index
                        }

                        isFirstLine = false;
                        continue;
                    }

                    var idString = values[columnIndices["useridentifier"]]; // parse useridentifier as an int
                    if (int.TryParse(idString, out int parsedId))
                    {
                        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == parsedId); 
                        if (user == null) // check if the user with the same id exists if not create new user, otherwise update user's data
                        {
                            user = new User
                            {
                                UserId = parsedId,
                                Username = values[columnIndices["username"]],
                                Age = int.Parse(values[columnIndices["age"]]),
                                City = values[columnIndices["city"]],
                                PhoneNumber = values[columnIndices["phonenumber"]],
                                Email = values[columnIndices["email"]]
                            };

                            _context.Users.Add(user);
                        }
                        else
                        {
                            user.Username = values[columnIndices["username"]];
                            user.Age = int.Parse(values[columnIndices["age"]]);
                            user.City = values[columnIndices["city"]];
                            user.PhoneNumber = values[columnIndices["phonenumber"]];
                            user.Email = values[columnIndices["email"]];
                        }

                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        return BadRequest("Invalid id type");
                    }
                }
            }

            return Ok("File processed successfully.");
        }


        [HttpGet("getusers")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers(string sortDirection = "asc", int? limit = null)
        {
            IQueryable<User> userQuery = _context.Users; 

            if (sortDirection == "asc") // ascending order
            {
                userQuery = userQuery.OrderBy(u => u.Username);
            }
            else if (sortDirection == "desc") // descending order
            {
                userQuery = userQuery.OrderByDescending(u => u.Username);
            }

            if (limit.HasValue) // size of the output
            {
                userQuery = userQuery.Take(limit.Value);
            }

            return await userQuery.ToListAsync();
        }
    }

    
}

