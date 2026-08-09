using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CCDomain.Model.RentModels;

namespace CCWindowForm.Services
{
    public class RentApiService : BaseApiService
    {
        public async Task<RentListResponseModel?> GetAllRentsAsync()
        {
            try
            {
                return await Http.GetFromJsonAsync<RentListResponseModel>("Rent/getall");
            }
            catch
            {
                return new RentListResponseModel { IsSuccess = false, Message = "Unable to connect to the server." };
            }
        }

        public async Task<RentDetailResponseModel?> GetRentByIdAsync(int id)
        {
            try
            {
                return await Http.GetFromJsonAsync<RentDetailResponseModel>($"Rent/getbyid?RentId={id}");
            }
            catch
            {
                return new RentDetailResponseModel { IsSuccess = false, Message = "Unable to connect to the server." };
            }
        }

        public async Task<RentCreateResponseModel?> CreateRentAsync(RentCreateRequestModel request)
        {
            try
            {
                var response = await Http.PostAsJsonAsync("Rent/create", request);
                return await response.Content.ReadFromJsonAsync<RentCreateResponseModel>();
            }
            catch
            {
                return new RentCreateResponseModel { IsSuccess = false, Message = "Unable to connect to the server." };
            }
        }

        public async Task<RentPatchResponseModel?> UpdateRentAsync(int id, RentPatchRequestModel request)
        {
            try
            {
                var response = await Http.PatchAsJsonAsync($"Rent/update/{id}", request);
                return await response.Content.ReadFromJsonAsync<RentPatchResponseModel>();
            }
            catch
            {
                return new RentPatchResponseModel { IsSuccess = false, Message = "Unable to connect to the server." };
            }
        }

        public async Task<RentDeleteResponseModel?> DeleteRentAsync(int id)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Delete, $"Rent/delete?RentId={id}");
                var response = await Http.SendAsync(request);
                return await response.Content.ReadFromJsonAsync<RentDeleteResponseModel>();
            }
            catch
            {
                return new RentDeleteResponseModel { IsSuccess = false, Message = "Unable to connect to the server." };
            }
        }
    }
}
