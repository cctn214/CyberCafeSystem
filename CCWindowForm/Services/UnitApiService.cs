using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CCDomain.Model.UnitModels;

namespace CCWindowForm.Services
{
    public class UnitApiService : BaseApiService
    {
        public async Task<UnitListResponseModel?> GetAllUnitsAsync()
        {
            try
            {
                return await Http.GetFromJsonAsync<UnitListResponseModel>("Unit/getall");
            }
            catch
            {
                return new UnitListResponseModel { IsSuccess = false, Message = "Unable to connect to the server." };
            }
        }

        public async Task<UnitDetailResponseModel?> GetUnitByIdAsync(int id)
        {
            try
            {
                return await Http.GetFromJsonAsync<UnitDetailResponseModel>($"Unit/getbyid?UnitId={id}");
            }
            catch
            {
                return new UnitDetailResponseModel { IsSuccess = false, Message = "Unable to connect to the server." };
            }
        }

        public async Task<UnitCreateResponseModel?> CreateUnitAsync(UnitCreateRequestModel request)
        {
            try
            {
                var response = await Http.PostAsJsonAsync("Unit/create", request);
                return await response.Content.ReadFromJsonAsync<UnitCreateResponseModel>();
            }
            catch
            {
                return new UnitCreateResponseModel { IsSuccess = false, Message = "Unable to connect to the server." };
            }
        }

        public async Task<UnitPatchResponseModel?> UpdateUnitAsync(int id, UnitPatchRequestModel request)
        {
            try
            {
                var response = await Http.PatchAsJsonAsync($"Unit/update/{id}", request);
                return await response.Content.ReadFromJsonAsync<UnitPatchResponseModel>();
            }
            catch
            {
                return new UnitPatchResponseModel { IsSuccess = false, Message = "Unable to connect to the server." };
            }
        }

        public async Task<UnitDeleteResponseModel?> DeleteUnitAsync(int id)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Delete, $"Unit/delete?UnitId={id}");
                var response = await Http.SendAsync(request);
                return await response.Content.ReadFromJsonAsync<UnitDeleteResponseModel>();
            }
            catch
            {
                return new UnitDeleteResponseModel { IsSuccess = false, Message = "Unable to connect to the server." };
            }
        }
    }
}
