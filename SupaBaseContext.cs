using Supabase.Postgrest.Exceptions;
using static Supabase.Postgrest.Constants;

namespace WebApplication7
{
    public class SupaBaseContext
    {

        public async Task<List<User>> GetUsers(Supabase.Client _supabaseClient)
        {
            var result = await _supabaseClient.From<User>().Order("id", Ordering.Ascending).Get();
            return result.Models ?? new List<User>();
        }

        public async Task<List<City>> GetCity(Supabase.Client _supabaseClient)
        {
            var result = await _supabaseClient.From<City>().Order("id", Ordering.Ascending).Get();
            return result.Models ?? new List<City>();
        }

        public async Task<bool> InsertUsers(Supabase.Client _supabaseClient, User user)
        {
            try
            {
                await _supabaseClient.From<User>().Insert(user);
                return true;
            }
            catch (PostgrestException ex)
            {
                Console.WriteLine($"Ошибка при вставке пользователя: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> InsertCity(Supabase.Client _supabaseClient, City city)
        {
            try
            {
                await _supabaseClient.From<City>().Insert(city);
                return true;
            }
            catch (PostgrestException ex)
            {
                Console.WriteLine($"Ошибка при вставке города: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateUser(Supabase.Client _supabaseClient, User updatedUser)
        {
            try
            {
                await _supabaseClient.From<User>()
                                     .Where(user => user.Id == updatedUser.Id)
                                     .Set(user => user.Login, updatedUser.Login)
                                     .Set(user => user.Password, updatedUser.Password)
                                     .Set(user => user.Name, updatedUser.Name)
                                     .Set(user => user.Age, updatedUser.Age)
                                     .Set(user => user.Email, updatedUser.Email)
                                     .Update();

                return true;
            }
            catch (HttpRequestException ex)
            {
                Console.Error.WriteLine($"Ошибка HTTP-запроса: {ex.Message}");

                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return false;
                }

                throw new ApplicationException("Не удалось подключиться к базе данных.", ex);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Произошла ошибка: {ex.Message}");
                throw new ApplicationException("Произошла ошибка при обновлении пользователя.", ex);
            }
        }
        public async Task<bool> UpdateUserName(Supabase.Client _supabaseClient, User updatedUserName)
        {
            try
            {
                await _supabaseClient.From<User>()
                                     .Where(user => user.Id == updatedUserName.Id)
                                     .Set(user => user.Name, updatedUserName.Name)
                                     .Update();

                return true;
            }
            catch (HttpRequestException ex)
            {
                Console.Error.WriteLine($"Ошибка HTTP-запроса: {ex.Message}");

                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return false;
                }

                throw new ApplicationException("Не удалось подключиться к базе данных.", ex);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Произошла ошибка: {ex.Message}");
                throw new ApplicationException("Произошла ошибка при обновлении пользователя.", ex);
            }
        }
        public async Task<bool> UpdateCity(Supabase.Client _supabaseClient, City updatedCity)
        {
            try
            {
                await _supabaseClient.From<City>()
                                     .Where(city => city.Id == updatedCity.Id)
                                     .Set(city => city.Name, updatedCity.Name)
                                     .Update();
                return true;
            }
            catch (HttpRequestException ex)
            {
                Console.Error.WriteLine($"Ошибка HTTP-запроса: {ex.Message}");

                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return false;
                }

                throw new ApplicationException("Не удалось подключиться к базе данных.", ex);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Произошла ошибка: {ex.Message}");
                throw new ApplicationException("Произошла ошибка при обновлении города.", ex);
            }
        }

        public async Task<bool> DeleteUser(Supabase.Client _supabaseClient, int userId)
        {
            try
            {
                await _supabaseClient.From<User>()
                                     .Where(user => user.Id == userId)
                                     .Delete();

                return true;
            }
            catch (HttpRequestException ex)
            {
                Console.Error.WriteLine($"Ошибка HTTP-запроса: {ex.Message}");

                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new ApplicationException("Пользователь с указанным ID не найден.");
                }

                throw new ApplicationException("Не удалось подключиться к базе данных.", ex);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Произошла ошибка: {ex.Message}");
                throw new ApplicationException("Произошла ошибка при удалении пользователя.", ex);
            }
        }
    }


}
