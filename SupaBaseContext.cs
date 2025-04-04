using Microsoft.AspNetCore.Mvc;
using Supabase.Gotrue;
using Supabase.Postgrest.Exceptions;
using System.Reflection;

namespace WebApplication7
{
    public class SupaBaseContext
    {
       
        public async Task<List<User>> GetUsers(Supabase.Client _supabaseClient)
        {
            var result = await _supabaseClient.From<User>().Get();
            return result.Models;
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

        public async Task<bool> UpdateUserName(Supabase.Client _supabaseClient, int id, string newName)
        {
            try
            {
          
                var response = await _supabaseClient.From<User>().Where(x => x.Id == id).Set(x => x.Name, newName).Update();

                return true;
            }

            catch (Exception ex)
            {

                Console.WriteLine($"Error updating user: {ex.Message}");
                return false;
            }
        }

    }


}
