using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Supabase;
using Supabase.Interfaces;
using WebApplication7;

namespace WebApplication7.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly Supabase.Client _supabaseClient;
        private readonly SupaBaseContext _supabaseContext;

        public WeatherForecastController(Supabase.Client supabaseClient, SupaBaseContext supabaseContext)
        {
            _supabaseClient = supabaseClient;
            _supabaseContext = supabaseContext;
        }

        [HttpGet("GetAllUsers", Name = "GetAllUsers")]
        public async Task<string> GetAllUsers()
        {
            var result = await _supabaseContext.GetUsers(_supabaseClient);
            return JsonConvert.SerializeObject(result, Formatting.Indented);
        }

        [HttpPost("InsertUsers", Name = "InsertUsers")]
        public async Task<ActionResult> InsertUser([FromBody] UserData userData)
        {
            if (string.IsNullOrEmpty(userData.Login) || string.IsNullOrEmpty(userData.Password))
            {
                return BadRequest("Или логин или пароль пустой");
            }

            User newUser = new User
            {
                Id = userData.Id,
                Login = userData.Login,
                Password = userData.Password,
                Name = userData.NewName
            };

            bool result = await _supabaseContext.InsertUsers(_supabaseClient, newUser);
            if (result)
            {
                return Ok("Регистрация прошла успешно");
            }
            else
            {
                return BadRequest("Не удалось добавить пользователя в БД");
            }
        }

        [HttpPut("UpdateUser", Name = "UpdateUser")]
        public async Task<ActionResult> UpdateUser([FromBody] UserData userData)
        {
            try
            {
                if (userData.Id <= 0 || string.IsNullOrEmpty(userData.Login) || string.IsNullOrEmpty(userData.Password))
                {
                    return BadRequest("Некорректные данные пользователя. Убедитесь, что ID, логин и пароль указаны.");
                }

                User updatedUser = new User
                {
                    Id = userData.Id,
                    Login = userData.Login,
                    Password = userData.Password,
                    Name = userData.NewName
                };

                bool result = await _supabaseContext.UpdateUser(_supabaseClient, updatedUser);

                if (result)
                {
                    return Ok("Пользователь успешно обновлен.");
                }
                else
                {
                    return BadRequest("Не удалось обновить пользователя в БД.");
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ошибка: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Произошла ошибка при обновлении пользователя.");
            }
        }

        [HttpDelete("DeleteUser/{userId}", Name = "DeleteUser")]
        public async Task<ActionResult> DeleteUser(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return BadRequest("Некорректный ID пользователя.");
                }

                bool result = await _supabaseContext.DeleteUser(_supabaseClient, userId);

                if (result)
                {
                    return Ok("Пользователь успешно удален.");
                }
                else
                {
                    return NotFound("Пользователь с указанным ID не найден.");
                }
            }
            catch (ApplicationException ex)
            {
                Console.Error.WriteLine($"Ошибка: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Произошла ошибка при удалении пользователя.");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Неожиданная ошибка: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Произошла неожиданная ошибка.");
            }
        }


    }

    public class UserData
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("login")]
        public string Login { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("newName")]
        public string NewName { get; set; }
    }

}