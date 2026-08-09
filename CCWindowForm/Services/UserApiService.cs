using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CCDomain.Model.UserModels;

namespace CCWindowForm.Services
{
    public class UserApiService : BaseApiService
    {
        public async Task<UserListResponseModel?> GetAllUsersAsync()
        {
            try
            {
                return await Http.GetFromJsonAsync<UserListResponseModel>("User/getall");
            }
            catch
            {
                return new UserListResponseModel { IsSuccess = false, Message = "Unable to connect to the server." };
            }
        }

        public async Task<UserDetailResponseModel?> GetUserByIdAsync(int id)
        {
            try
            {
                return await Http.GetFromJsonAsync<UserDetailResponseModel>($"User/getbyid?UserId={id}");
            }
            catch
            {
                return new UserDetailResponseModel { IsSuccess = false, Message = "Unable to connect to the server." };
            }
        }

        public async Task<UserCreateResponseModel?> CreateUserAsync(UserCreateRequestModel request)
        {
            try
            {
                var response = await Http.PostAsJsonAsync("User/create", request);
                return await response.Content.ReadFromJsonAsync<UserCreateResponseModel>();
            }
            catch
            {
                return new UserCreateResponseModel { IsSuccess = false, Message = "Unable to connect to the server." };
            }
        }

        public async Task<UserPatchResponseModel?> UpdateUserAsync(int id, UserPatchRequestModel request)
        {
            try
            {
                var response = await Http.PatchAsJsonAsync($"User/update/{id}", request);
                return await response.Content.ReadFromJsonAsync<UserPatchResponseModel>();
            }
            catch
            {
                return new UserPatchResponseModel { IsSuccess = false, Message = "Unable to connect to the server." };
            }
        }

        public async Task<UserDeleteResponseModel?> DeleteUserAsync(int id)
        {
            try
            {
                // In ASP.NET Core, an object in [HttpDelete] binds from the query string by default.
                var request = new HttpRequestMessage(HttpMethod.Delete, $"User/delete?UserId={id}");
                var response = await Http.SendAsync(request);
                return await response.Content.ReadFromJsonAsync<UserDeleteResponseModel>();
            }
            catch
            {
                return new UserDeleteResponseModel { IsSuccess = false, Message = "Unable to connect to the server." };
            }
        }
    }
}
