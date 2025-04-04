using Microsoft.AspNetCore.Mvc;
using Supabase.Gotrue;
using Supabase.Postgrest.Exceptions;
using System.Reflection;
using static Supabase.Postgrest.Constants;

namespace WebApplication7
{
    public class SupaBaseContext
    {

        public async Task<List<User>> GetUsers(Supabase.Client _supabaseClient)
        {
            var result = await _supabaseClient.From<User>().Order("id", Ordering.Ascending) .Get();
            return result.Models ?? new List<User>();
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

        public async Task<bool> UpdateUser(Supabase.Client _supabaseClient, User updatedUser)
        {
            try
            {
                // Выполняем запрос на обновление всех полей пользователя
                await _supabaseClient.From<User>()
                                     .Where(user => user.Id == updatedUser.Id)
                                     .Set(user => user.Login, updatedUser.Login)
                                     .Set(user => user.Password, updatedUser.Password)
                                     .Set(user => user.Name, updatedUser.Name)
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
