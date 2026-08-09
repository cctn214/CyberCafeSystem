using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CCDomain.Model.UserModels;
using CCDomain.Model.UnitModels;
using CCDomain.Model.RentModels;

namespace CCConsoleApp
{
    internal class Program
    {
        private static readonly HttpClient client = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5243/api/")
        };

        static async Task Main(string[] args)
        {
            Console.Clear();
            while (true)
            {
                Console.WriteLine("==================================================");
                Console.WriteLine("       CYPHER CAFE SYSTEM - MAIN MENU             ");
                Console.WriteLine("==================================================");
                Console.WriteLine("1. User Management");
                Console.WriteLine("2. Unit Management");
                Console.WriteLine("3. Rent Management");
                Console.WriteLine("4. Exit");
                Console.WriteLine("==================================================");
                Console.Write("Please select an option (1-4): ");
                
                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        await RunUserMenuAsync();
                        break;
                    case "2":
                        await RunUnitMenuAsync();
                        break;
                    case "3":
                        await RunRentMenuAsync();
                        break;
                    case "4":
                        Console.WriteLine("Exiting application. Goodbye!");
                        return;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid option. Please choose 1-4.");
                        Console.ResetColor();
                        Console.WriteLine("\nPress Enter to continue...");
                        Console.ReadLine();
                        Console.Clear();
                        break;
                }
            }
        }

        // ================== USER MANAGEMENT ==================
        private static async Task RunUserMenuAsync()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("==================================================");
                Console.WriteLine("                 USER MANAGEMENT                  ");
                Console.WriteLine("==================================================");
                Console.WriteLine("1. List All Users");
                Console.WriteLine("2. Search User by ID");
                Console.WriteLine("3. Create User");
                Console.WriteLine("4. Update User");
                Console.WriteLine("5. Delete User");
                Console.WriteLine("6. Back to Main Menu");
                Console.WriteLine("==================================================");
                Console.Write("Please select an option (1-6): ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        await ListAllUsersAsync();
                        break;
                    case "2":
                        await SearchUserByIdAsync();
                        break;
                    case "3":
                        await CreateUserAsync();
                        break;
                    case "4":
                        await UpdateUserAsync();
                        break;
                    case "5":
                        await DeleteUserAsync();
                        break;
                    case "6":
                        Console.Clear();
                        return;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid option. Press Enter to try again.");
                        Console.ResetColor();
                        Console.ReadLine();
                        break;
                }
            }
        }

        private static async Task ListAllUsersAsync()
        {
            Console.Clear();
            Console.WriteLine("Loading Users...");
            try
            {
                var response = await client.GetAsync("User/getall");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var res = JsonConvert.DeserializeObject<UserListResponseModel>(content);
                    if (res != null && res.IsSuccess)
                    {
                        Console.WriteLine("--------------------------------------------------------------------------------");
                        Console.WriteLine($"{"ID",-6} | {"Username",-20} | {"Full Name",-25} | {"Balance",-15}");
                        Console.WriteLine("--------------------------------------------------------------------------------");
                        foreach (var user in res.Users)
                        {
                            Console.WriteLine($"{user.UserId,-6} | {user.Username,-20} | {user.Name,-25} | {user.Balance,14:C2}");
                        }
                        Console.WriteLine("--------------------------------------------------------------------------------");
                    }
                    else
                    {
                        Console.WriteLine($"Failed: {res?.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"Server returned error code: {response.StatusCode}");
                }
            }
            catch
            {
                Console.WriteLine("Unable to connect to the server.");
            }
            Console.WriteLine("\nPress Enter to return...");
            Console.ReadLine();
        }

        private static async Task SearchUserByIdAsync()
        {
            Console.Clear();
            Console.Write("Enter User ID to search: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID. Press Enter to return.");
                Console.ReadLine();
                return;
            }

            try
            {
                var response = await client.GetAsync($"User/getbyid?UserId={id}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var res = JsonConvert.DeserializeObject<UserDetailResponseModel>(content);
                    if (res != null && res.IsSuccess)
                    {
                        Console.WriteLine("\nUser Found:");
                        Console.WriteLine($"User ID:   {res.UserId}");
                        Console.WriteLine($"Username:  {res.Username}");
                        Console.WriteLine($"Full Name: {res.Name}");
                        Console.WriteLine($"Balance:   {res.Balance:C2}");
                    }
                    else
                    {
                        Console.WriteLine($"\nNo record found with this ID.");
                    }
                }
                else
                {
                    Console.WriteLine("\nNo record found with this ID.");
                }
            }
            catch
            {
                Console.WriteLine("Unable to connect to the server.");
            }
            Console.WriteLine("\nPress Enter to return...");
            Console.ReadLine();
        }

        private static async Task CreateUserAsync()
        {
            Console.Clear();
            Console.WriteLine("--- CREATE NEW USER ---");
            Console.Write("Username: ");
            string username = Console.ReadLine();
            Console.Write("Password: ");
            string password = Console.ReadLine();
            Console.Write("Full Name: ");
            string name = Console.ReadLine();
            Console.Write("Initial Balance ($): ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal balance))
            {
                balance = 0;
            }

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Error: Username, Password, and Full Name are required.");
                Console.ReadLine();
                return;
            }

            var request = new UserCreateRequestModel
            {
                Username = username.Trim(),
                Password = password,
                Name = name.Trim(),
                Balance = balance
            };

            try
            {
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("User/create", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                var res = JsonConvert.DeserializeObject<UserCreateResponseModel>(responseContent);

                if (res != null && res.IsSuccess)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\nUser created successfully! New User ID: {res.UserId}");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"\nFailed to create user: {res?.Message}");
                }
            }
            catch
            {
                Console.WriteLine("Unable to connect to the server.");
            }
            Console.ReadLine();
        }

        private static async Task UpdateUserAsync()
        {
            Console.Clear();
            Console.Write("Enter User ID to update: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID. Press Enter to return.");
                Console.ReadLine();
                return;
            }

            try
            {
                var getRes = await client.GetAsync($"User/getbyid?UserId={id}");
                if (!getRes.IsSuccessStatusCode)
                {
                    Console.WriteLine("User not found.");
                    Console.ReadLine();
                    return;
                }
                var getContent = await getRes.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<UserDetailResponseModel>(getContent);
                if (user == null || !user.IsSuccess)
                {
                    Console.WriteLine("User not found.");
                    Console.ReadLine();
                    return;
                }

                Console.WriteLine($"\nCurrent Details: Name: {user.Name}, Username: {user.Username}, Balance: {user.Balance:C2}");
                Console.WriteLine("\nEnter new details (Leave blank to keep current):");
                Console.Write("New Username: ");
                string username = Console.ReadLine();
                Console.Write("New Password: ");
                string password = Console.ReadLine();
                Console.Write("New Name: ");
                string name = Console.ReadLine();
                Console.Write("New Balance: ");
                string balanceStr = Console.ReadLine();

                decimal? balance = null;
                if (decimal.TryParse(balanceStr, out decimal balVal)) balance = balVal;

                var request = new UserPatchRequestModel
                {
                    Username = string.IsNullOrWhiteSpace(username) ? null : username.Trim(),
                    Password = string.IsNullOrEmpty(password) ? null : password,
                    Name = string.IsNullOrWhiteSpace(name) ? null : name.Trim(),
                    Balance = balance
                };

                var requestMessage = new HttpRequestMessage(new HttpMethod("PATCH"), $"User/update/{id}")
                {
                    Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")
                };
                var patchResponse = await client.SendAsync(requestMessage);
                var patchContent = await patchResponse.Content.ReadAsStringAsync();
                var res = JsonConvert.DeserializeObject<UserPatchResponseModel>(patchContent);

                if (res != null && res.IsSuccess)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nUser updated successfully!");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"\nFailed: {res?.Message}");
                }
            }
            catch
            {
                Console.WriteLine("Unable to connect to the server.");
            }
            Console.ReadLine();
        }

        private static async Task DeleteUserAsync()
        {
            Console.Clear();
            Console.Write("Enter User ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID. Press Enter to return.");
                Console.ReadLine();
                return;
            }

            Console.Write($"Are you sure you want to delete User {id}? (y/n): ");
            string confirm = Console.ReadLine();
            if (confirm?.ToLower() != "y")
            {
                Console.WriteLine("Deletion cancelled.");
                Console.ReadLine();
                return;
            }

            try
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Delete, $"User/delete?UserId={id}");
                var response = await client.SendAsync(requestMessage);
                var content = await response.Content.ReadAsStringAsync();
                var res = JsonConvert.DeserializeObject<UserDeleteResponseModel>(content);

                if (res != null && res.IsSuccess)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nUser deleted successfully.");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"\nFailed: {res?.Message}");
                }
            }
            catch
            {
                Console.WriteLine("Unable to connect to the server.");
            }
            Console.ReadLine();
        }

        // ================== UNIT MANAGEMENT ==================
        private static async Task RunUnitMenuAsync()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("==================================================");
                Console.WriteLine("                 UNIT MANAGEMENT                  ");
                Console.WriteLine("==================================================");
                Console.WriteLine("1. List All Units");
                Console.WriteLine("2. Search Unit by ID");
                Console.WriteLine("3. Create Unit");
                Console.WriteLine("4. Update Unit");
                Console.WriteLine("5. Delete Unit");
                Console.WriteLine("6. Back to Main Menu");
                Console.WriteLine("==================================================");
                Console.Write("Please select an option (1-6): ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        await ListAllUnitsAsync();
                        break;
                    case "2":
                        await SearchUnitByIdAsync();
                        break;
                    case "3":
                        await CreateUnitAsync();
                        break;
                    case "4":
                        await UpdateUnitAsync();
                        break;
                    case "5":
                        await DeleteUnitAsync();
                        break;
                    case "6":
                        Console.Clear();
                        return;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid option. Press Enter to try again.");
                        Console.ResetColor();
                        Console.ReadLine();
                        break;
                }
            }
        }

        private static async Task ListAllUnitsAsync()
        {
            Console.Clear();
            Console.WriteLine("Loading Units...");
            try
            {
                var response = await client.GetAsync("Unit/getall");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var res = JsonConvert.DeserializeObject<UnitListResponseModel>(content);
                    if (res != null && res.IsSuccess)
                    {
                        Console.WriteLine("--------------------------------------------------------------------------------");
                        Console.WriteLine($"{"ID",-6} | {"Type",-20} | {"Hourly Rate",-15} | {"Status",-10}");
                        Console.WriteLine("--------------------------------------------------------------------------------");
                        foreach (var unit in res.Units)
                        {
                            Console.WriteLine($"{unit.UnitId,-6} | {unit.Type,-20} | {unit.Rate,14:C2} | {(unit.IsActive ? "Active" : "Inactive")}");
                        }
                        Console.WriteLine("--------------------------------------------------------------------------------");
                    }
                    else
                    {
                        Console.WriteLine($"Failed: {res?.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"Server error: {response.StatusCode}");
                }
            }
            catch
            {
                Console.WriteLine("Unable to connect to the server.");
            }
            Console.WriteLine("\nPress Enter to return...");
            Console.ReadLine();
        }

        private static async Task SearchUnitByIdAsync()
        {
            Console.Clear();
            Console.Write("Enter Unit ID to search: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID. Press Enter to return.");
                Console.ReadLine();
                return;
            }

            try
            {
                var response = await client.GetAsync($"Unit/getbyid?UnitId={id}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var res = JsonConvert.DeserializeObject<UnitDetailResponseModel>(content);
                    if (res != null && res.IsSuccess)
                    {
                        Console.WriteLine("\nUnit Found:");
                        Console.WriteLine($"Unit ID:     {res.UnitId}");
                        Console.WriteLine($"Type:        {res.Type}");
                        Console.WriteLine($"Hourly Rate: {res.Rate:C2}");
                        Console.WriteLine($"Status:      {(res.IsActive ? "Active" : "Inactive")}");
                    }
                    else
                    {
                        Console.WriteLine($"\nNo record found with this ID.");
                    }
                }
                else
                {
                    Console.WriteLine("\nNo record found with this ID.");
                }
            }
            catch
            {
                Console.WriteLine("Unable to connect to the server.");
            }
            Console.WriteLine("\nPress Enter to return...");
            Console.ReadLine();
        }

        private static async Task CreateUnitAsync()
        {
            Console.Clear();
            Console.WriteLine("--- CREATE NEW UNIT ---");
            Console.Write("Unit Type (VIP/PC/Console): ");
            string type = Console.ReadLine();
            Console.Write("Hourly Rate ($/hr): ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal rate))
            {
                rate = 0;
            }
            Console.Write("Is Unit Active? (y/n): ");
            bool active = Console.ReadLine()?.ToLower() == "y";

            if (string.IsNullOrWhiteSpace(type))
            {
                Console.WriteLine("Error: Unit Type is required.");
                Console.ReadLine();
                return;
            }

            var request = new UnitCreateRequestModel
            {
                Type = type.Trim(),
                Rate = rate,
                IsActive = active
            };

            try
            {
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("Unit/create", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                var res = JsonConvert.DeserializeObject<UnitCreateResponseModel>(responseContent);

                if (res != null && res.IsSuccess)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\nUnit created successfully! New Unit ID: {res.UnitId}");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"\nFailed: {res?.Message}");
                }
            }
            catch
            {
                Console.WriteLine("Unable to connect to the server.");
            }
            Console.ReadLine();
        }

        private static async Task UpdateUnitAsync()
        {
            Console.Clear();
            Console.Write("Enter Unit ID to update: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID. Press Enter to return.");
                Console.ReadLine();
                return;
            }

            try
            {
                var getRes = await client.GetAsync($"Unit/getbyid?UnitId={id}");
                if (!getRes.IsSuccessStatusCode)
                {
                    Console.WriteLine("Unit not found.");
                    Console.ReadLine();
                    return;
                }
                var getContent = await getRes.Content.ReadAsStringAsync();
                var unit = JsonConvert.DeserializeObject<UnitDetailResponseModel>(getContent);
                if (unit == null || !unit.IsSuccess)
                {
                    Console.WriteLine("Unit not found.");
                    Console.ReadLine();
                    return;
                }

                Console.WriteLine($"\nCurrent Details: Type: {unit.Type}, Rate: {unit.Rate:C2}, Active: {unit.IsActive}");
                Console.WriteLine("\nEnter new details (Leave blank to keep current):");
                Console.Write("New Type: ");
                string type = Console.ReadLine();
                Console.Write("New Rate: ");
                string rateStr = Console.ReadLine();
                Console.Write("Is Active? (y/n, blank to skip): ");
                string activeStr = Console.ReadLine();

                decimal? rate = null;
                if (decimal.TryParse(rateStr, out decimal rateVal)) rate = rateVal;

                bool? isActive = null;
                if (!string.IsNullOrWhiteSpace(activeStr)) isActive = activeStr.ToLower() == "y";

                var request = new UnitPatchRequestModel
                {
                    Type = string.IsNullOrWhiteSpace(type) ? null : type.Trim(),
                    Rate = rate,
                    IsActive = isActive
                };

                var requestMessage = new HttpRequestMessage(new HttpMethod("PATCH"), $"Unit/update/{id}")
                {
                    Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")
                };
                var patchResponse = await client.SendAsync(requestMessage);
                var patchContent = await patchResponse.Content.ReadAsStringAsync();
                var res = JsonConvert.DeserializeObject<UnitPatchResponseModel>(patchContent);

                if (res != null && res.IsSuccess)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nUnit updated successfully!");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"\nFailed: {res?.Message}");
                }
            }
            catch
            {
                Console.WriteLine("Unable to connect to the server.");
            }
            Console.ReadLine();
        }

        private static async Task DeleteUnitAsync()
        {
            Console.Clear();
            Console.Write("Enter Unit ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID. Press Enter to return.");
                Console.ReadLine();
                return;
            }

            Console.Write($"Are you sure you want to delete Unit {id}? (y/n): ");
            string confirm = Console.ReadLine();
            if (confirm?.ToLower() != "y")
            {
                Console.WriteLine("Deletion cancelled.");
                Console.ReadLine();
                return;
            }

            try
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Delete, $"Unit/delete?UnitId={id}");
                var response = await client.SendAsync(requestMessage);
                var content = await response.Content.ReadAsStringAsync();
                var res = JsonConvert.DeserializeObject<UnitDeleteResponseModel>(content);

                if (res != null && res.IsSuccess)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nUnit deleted successfully.");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"\nFailed: {res?.Message}");
                }
            }
            catch
            {
                Console.WriteLine("Unable to connect to the server.");
            }
            Console.ReadLine();
        }

        // ================== RENT MANAGEMENT ==================
        private static async Task RunRentMenuAsync()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("==================================================");
                Console.WriteLine("                 RENT MANAGEMENT                  ");
                Console.WriteLine("==================================================");
                Console.WriteLine("1. List All Rents");
                Console.WriteLine("2. Search Rent by ID");
                Console.WriteLine("3. Create Rent");
                Console.WriteLine("4. Update Rent");
                Console.WriteLine("5. Delete Rent");
                Console.WriteLine("6. Back to Main Menu");
                Console.WriteLine("==================================================");
                Console.Write("Please select an option (1-6): ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        await ListAllRentsAsync();
                        break;
                    case "2":
                        await SearchRentByIdAsync();
                        break;
                    case "3":
                        await CreateRentAsync();
                        break;
                    case "4":
                        await UpdateRentAsync();
                        break;
                    case "5":
                        await DeleteRentAsync();
                        break;
                    case "6":
                        Console.Clear();
                        return;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid option. Press Enter to try again.");
                        Console.ResetColor();
                        Console.ReadLine();
                        break;
                }
            }
        }

        private static async Task ListAllRentsAsync()
        {
            Console.Clear();
            Console.WriteLine("Loading Rents...");
            try
            {
                var response = await client.GetAsync("Rent/getall");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var res = JsonConvert.DeserializeObject<RentListResponseModel>(content);
                    if (res != null && res.IsSuccess)
                    {
                        Console.WriteLine("----------------------------------------------------------------------------------------------------");
                        Console.WriteLine($"{"ID",-6} | {"User ID",-8} | {"Unit ID",-8} | {"Start Time",-20} | {"End Time",-20} | {"Cost",-10}");
                        Console.WriteLine("----------------------------------------------------------------------------------------------------");
                        foreach (var rent in res.Rents)
                        {
                            string start = rent.StartTime.ToString("yyyy-MM-dd HH:mm");
                            string end = rent.EndTime.HasValue ? rent.EndTime.Value.ToString("yyyy-MM-dd HH:mm") : "Ongoing";
                            string cost = rent.TotalCost.HasValue ? rent.TotalCost.Value.ToString("C2") : "Pending";
                            Console.WriteLine($"{rent.RentId,-6} | {rent.UserId,-8} | {rent.UnitId,-8} | {start,-20} | {end,-20} | {cost,10}");
                        }
                        Console.WriteLine("----------------------------------------------------------------------------------------------------");
                    }
                    else
                    {
                        Console.WriteLine($"Failed: {res?.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"Server error: {response.StatusCode}");
                }
            }
            catch
            {
                Console.WriteLine("Unable to connect to the server.");
            }
            Console.WriteLine("\nPress Enter to return...");
            Console.ReadLine();
        }

        private static async Task SearchRentByIdAsync()
        {
            Console.Clear();
            Console.Write("Enter Rent ID to search: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID. Press Enter to return.");
                Console.ReadLine();
                return;
            }

            try
            {
                var response = await client.GetAsync($"Rent/getbyid?RentId={id}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var res = JsonConvert.DeserializeObject<RentDetailResponseModel>(content);
                    if (res != null && res.IsSuccess)
                    {
                        Console.WriteLine("\nRent Found:");
                        Console.WriteLine($"Rent ID:    {res.RentId}");
                        Console.WriteLine($"User ID:    {res.UserId}");
                        Console.WriteLine($"Unit ID:    {res.UnitId}");
                        Console.WriteLine($"Start Time: {res.StartTime:yyyy-MM-dd HH:mm}");
                        Console.WriteLine($"End Time:   {(res.EndTime.HasValue ? res.EndTime.Value.ToString("yyyy-MM-dd HH:mm") : "Ongoing")}");
                        Console.WriteLine($"Duration:   {(res.Duration.HasValue ? res.Duration.Value + " hrs" : "N/A")}");
                        Console.WriteLine($"Total Cost: {(res.TotalCost.HasValue ? res.TotalCost.Value.ToString("C2") : "Pending")}");
                    }
                    else
                    {
                        Console.WriteLine($"\nNo record found with this ID.");
                    }
                }
                else
                {
                    Console.WriteLine("\nNo record found with this ID.");
                }
            }
            catch
            {
                Console.WriteLine("Unable to connect to the server.");
            }
            Console.WriteLine("\nPress Enter to return...");
            Console.ReadLine();
        }

        private static async Task CreateRentAsync()
        {
            Console.Clear();
            Console.WriteLine("--- CREATE NEW RENT ---");
            Console.Write("User ID: ");
            if (!int.TryParse(Console.ReadLine(), out int userId))
            {
                Console.WriteLine("Invalid User ID.");
                Console.ReadLine();
                return;
            }
            Console.Write("Unit ID: ");
            if (!int.TryParse(Console.ReadLine(), out int unitId))
            {
                Console.WriteLine("Invalid Unit ID.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("\nEnter Start Time (Hours & Minutes relative to Today):");
            Console.Write("Start Hour (0-23): ");
            if (!int.TryParse(Console.ReadLine(), out int startHour) || startHour < 0 || startHour > 23)
            {
                Console.WriteLine("Invalid Start Hour.");
                Console.ReadLine();
                return;
            }
            Console.Write("Start Minute (0-59): ");
            if (!int.TryParse(Console.ReadLine(), out int startMin) || startMin < 0 || startMin > 59)
            {
                Console.WriteLine("Invalid Start Minute.");
                Console.ReadLine();
                return;
            }
            DateTime startTime = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, startHour, startMin, 0);

            Console.WriteLine("\nEnter End Time (Optional, leave blank to skip):");
            Console.Write("End Hour (0-23): ");
            string endHourStr = Console.ReadLine();
            Console.Write("End Minute (0-59): ");
            string endMinStr = Console.ReadLine();

            DateTime? endTime = null;
            if (!string.IsNullOrWhiteSpace(endHourStr) || !string.IsNullOrWhiteSpace(endMinStr))
            {
                if (!int.TryParse(endHourStr, out int endHour) || endHour < 0 || endHour > 23 ||
                    !int.TryParse(endMinStr, out int endMin) || endMin < 0 || endMin > 59)
                {
                    Console.WriteLine("Invalid End Time values.");
                    Console.ReadLine();
                    return;
                }
                endTime = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, endHour, endMin, 0);
            }

            Console.Write("Duration in hours (optional): ");
            string durStr = Console.ReadLine();
            int? duration = null;
            if (int.TryParse(durStr, out int durVal)) duration = durVal;

            Console.Write("Total Cost (optional): ");
            string costStr = Console.ReadLine();
            decimal? cost = null;
            if (decimal.TryParse(costStr, out decimal costVal)) cost = costVal;

            var request = new RentCreateRequestModel
            {
                UserId = userId,
                UnitId = unitId,
                StartTime = startTime,
                EndTime = endTime,
                Duration = duration,
                TotalCost = cost
            };

            try
            {
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("Rent/create", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                var res = JsonConvert.DeserializeObject<RentCreateResponseModel>(responseContent);

                if (res != null && res.IsSuccess)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\nRent created successfully! New Rent ID: {res.RentId}");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"\nFailed: {res?.Message}");
                }
            }
            catch
            {
                Console.WriteLine("Unable to connect to the server.");
            }
            Console.ReadLine();
        }

        private static async Task UpdateRentAsync()
        {
            Console.Clear();
            Console.Write("Enter Rent ID to update: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID. Press Enter to return.");
                Console.ReadLine();
                return;
            }

            try
            {
                var getRes = await client.GetAsync($"Rent/getbyid?RentId={id}");
                if (!getRes.IsSuccessStatusCode)
                {
                    Console.WriteLine("Rent not found.");
                    Console.ReadLine();
                    return;
                }
                var getContent = await getRes.Content.ReadAsStringAsync();
                var rent = JsonConvert.DeserializeObject<RentDetailResponseModel>(getContent);
                if (rent == null || !rent.IsSuccess)
                {
                    Console.WriteLine("Rent not found.");
                    Console.ReadLine();
                    return;
                }

                Console.WriteLine($"\nCurrent Details: User: {rent.UserId}, Unit: {rent.UnitId}, Start: {rent.StartTime:HH:mm}, End: {(rent.EndTime.HasValue ? rent.EndTime.Value.ToString("HH:mm") : "Ongoing")}");
                Console.WriteLine("\nEnter new details (Leave blank to keep current):");
                Console.Write("New User ID: ");
                string userIdStr = Console.ReadLine();
                Console.Write("New Unit ID: ");
                string unitIdStr = Console.ReadLine();

                int? userId = null;
                if (int.TryParse(userIdStr, out int userVal)) userId = userVal;

                int? unitId = null;
                if (int.TryParse(unitIdStr, out int unitVal)) unitId = unitVal;

                // Start Time update
                Console.Write("New Start Hour (0-23): ");
                string startHourStr = Console.ReadLine();
                Console.Write("New Start Minute (0-59): ");
                string startMinStr = Console.ReadLine();

                DateTime startTime = rent.StartTime;
                if (!string.IsNullOrWhiteSpace(startHourStr) || !string.IsNullOrWhiteSpace(startMinStr))
                {
                    int h = string.IsNullOrWhiteSpace(startHourStr) ? rent.StartTime.Hour : int.Parse(startHourStr);
                    int m = string.IsNullOrWhiteSpace(startMinStr) ? rent.StartTime.Minute : int.Parse(startMinStr);
                    startTime = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, h, m, 0);
                }

                // End Time update
                Console.Write("New End Hour (0-23, blank to skip): ");
                string endHourStr = Console.ReadLine();
                Console.Write("New End Minute (0-59, blank to skip): ");
                string endMinStr = Console.ReadLine();

                DateTime? endTime = rent.EndTime;
                if (!string.IsNullOrWhiteSpace(endHourStr) || !string.IsNullOrWhiteSpace(endMinStr))
                {
                    int h = string.IsNullOrWhiteSpace(endHourStr) ? (rent.EndTime?.Hour ?? 0) : int.Parse(endHourStr);
                    int m = string.IsNullOrWhiteSpace(endMinStr) ? (rent.EndTime?.Minute ?? 0) : int.Parse(endMinStr);
                    endTime = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, h, m, 0);
                }

                Console.Write("New Duration (hours): ");
                string durStr = Console.ReadLine();
                int? duration = null;
                if (int.TryParse(durStr, out int durVal)) duration = durVal;
                else if (rent.Duration.HasValue) duration = rent.Duration;

                Console.Write("New Total Cost: ");
                string costStr = Console.ReadLine();
                decimal? cost = null;
                if (decimal.TryParse(costStr, out decimal costVal)) cost = costVal;
                else if (rent.TotalCost.HasValue) cost = rent.TotalCost;

                var request = new RentPatchRequestModel
                {
                    UserId = userId,
                    UnitId = unitId,
                    StartTime = startTime,
                    EndTime = endTime,
                    Duration = duration,
                    TotalCost = cost
                };

                var requestMessage = new HttpRequestMessage(new HttpMethod("PATCH"), $"Rent/update/{id}")
                {
                    Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")
                };
                var patchResponse = await client.SendAsync(requestMessage);
                var patchContent = await patchResponse.Content.ReadAsStringAsync();
                var res = JsonConvert.DeserializeObject<RentPatchResponseModel>(patchContent);

                if (res != null && res.IsSuccess)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nRent updated successfully!");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"\nFailed: {res?.Message}");
                }
            }
            catch
            {
                Console.WriteLine("Unable to connect to the server.");
            }
            Console.ReadLine();
        }

        private static async Task DeleteRentAsync()
        {
            Console.Clear();
            Console.Write("Enter Rent ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID. Press Enter to return.");
                Console.ReadLine();
                return;
            }

            Console.Write($"Are you sure you want to delete Rent {id}? (y/n): ");
            string confirm = Console.ReadLine();
            if (confirm?.ToLower() != "y")
            {
                Console.WriteLine("Deletion cancelled.");
                Console.ReadLine();
                return;
            }

            try
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Delete, $"Rent/delete?RentId={id}");
                var response = await client.SendAsync(requestMessage);
                var content = await response.Content.ReadAsStringAsync();
                var res = JsonConvert.DeserializeObject<RentDeleteResponseModel>(content);

                if (res != null && res.IsSuccess)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nRent deleted successfully.");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"\nFailed: {res?.Message}");
                }
            }
            catch
            {
                Console.WriteLine("Unable to connect to the server.");
            }
            Console.ReadLine();
        }
    }
}