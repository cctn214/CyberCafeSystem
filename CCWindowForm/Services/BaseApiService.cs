using System;
using System.Net.Http;

namespace CCWindowForm.Services
{
    public class BaseApiService
    {
        private static readonly HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5243/api/")
        };

        protected HttpClient Http => _httpClient;
    }
}
