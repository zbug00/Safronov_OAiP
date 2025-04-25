using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Supabase;
using Supabase.Gotrue;
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

        [HttpGet("GetAllCity", Name = "GetAllCity")]
        public async Task<string> GetAllCity()
        {
            var result = await _supabaseContext.GetCity(_supabaseClient);
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
                Name = userData.Name
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

        [HttpPost("InsertCity", Name = "InsertCity")]
        public async Task<ActionResult> InsertCity([FromBody] CityData CityData)
        {
            if (string.IsNullOrEmpty(CityData.Name))
            {
                return BadRequest("Имя пустое");
            }

            City newCity = new City
            {
                Id = CityData.Id,
                Name = CityData.Name,
            };

            bool result = await _supabaseContext.InsertCity(_supabaseClient, newCity);
            if (result)
            {
                return Ok("Регистрация прошла успешно");
            }
            else
            {
                return BadRequest("Не удалось добавить город в БД");
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
                    Name = userData.Name
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

        [HttpPut("UpdateCity", Name = "UpdateCity")]
        public async Task<ActionResult> UpdateCity([FromBody] CityData CityData)
        {
            try
            {
                if (CityData.Id <= 0 || string.IsNullOrEmpty(CityData.Name))
                {
                    return BadRequest("Некорректные данные города.");
                }

                City updatedCity = new City
                {
                    Id = CityData.Id,
                    Name = CityData.Name
                };

                bool result = await _supabaseContext.UpdateCity(_supabaseClient, updatedCity);

                if (result)
                {
                    return Ok("Город успешно обновлен.");
                }
                else
                {
                    return BadRequest("Не удалось обновить город в БД.");
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ошибка: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Произошла ошибка при обновлении города.");
            }
        }


        [HttpDelete("DeleteCity", Name = "DeleteCity")]
        public async Task<ActionResult> DeleteCity(int cityId)
        {
            try
            {
                if (cityId <= 0)
                {
                    return BadRequest("Некорректный ID города.");
                }

                bool result = await _supabaseContext.DeleteUser(_supabaseClient, cityId);

                if (result)
                {
                    return Ok("Город успешно удален.");
                }
                else
                {
                    return NotFound("Город с указанным ID не найден.");
                }
            }
            catch (ApplicationException ex)
            {
                Console.Error.WriteLine($"Ошибка: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Произошла ошибка при удалении города.");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Неожиданная ошибка: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Произошла неожиданная ошибка.");
            }
        }

        [HttpDelete("DeleteUser", Name = "DeleteUser")]
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
        public string Name { get; set; }
    }


    public class CityData
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

    }
}