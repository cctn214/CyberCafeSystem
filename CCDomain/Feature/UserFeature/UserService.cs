using System;
using System.Collections.Generic;
using System.Text;
using CCDatabase.Models;
using CCDomain.Model.UserModels; // <- fixed

namespace CCDomain.Feature.UserFeature
{
    public class UserService
    {
        private readonly AppDbContext _context;
        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public UserListResponseModel GetAllUsers()
        {
            try
            {
                var list = _context.Users.ToList();
                return new UserListResponseModel
                {
                    Users = list.Select(u => new UserModel
                    {
                        UserId = u.UserId,
                        Username = u.Username,
                        Name = u.Name,
                        Balance = u.Balance
                    }).ToList(),
                    IsSuccess = true,
                    Message = "Users retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                return new UserListResponseModel
                {
                    IsSuccess = false,
                    Message = ex.ToString(),
                };
            }
        }

        public UserDetailResponseModel GetUserById(UserDetailRequestModel request)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.UserId == request.UserId);
                if (user == null)
                {
                    return new UserDetailResponseModel
                    {
                        IsSuccess = false,
                        Message = "User not found"
                    };
                }

                return new UserDetailResponseModel
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Name = user.Name,
                    Balance = user.Balance,
                    IsSuccess = true,
                    Message = "User found"
                };
            }
            catch (Exception ex)
            {

                return new UserDetailResponseModel
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public UserCreateResponseModel CreateUser(UserCreateRequestModel request)
        {
            try
            {
                var user = new User
                {
                    Username = request.Username,
                    Password = request.Password,
                    Name = request.Name,
                    Balance = request.Balance
                };
                _context.Users.Add(user);
                _context.SaveChanges();
                return new UserCreateResponseModel
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Name = user.Name,
                    Balance = user.Balance,
                    IsSuccess = true,
                    Message = "User created successfully"
                };
            }
            catch (Exception ex)
            {
                return new UserCreateResponseModel
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
        public UserPatchResponseModel UpdateUser(UserPatchRequestModel request, int userId)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
                if (user == null)
                {
                    return new UserPatchResponseModel
                    {
                        IsSuccess = false,
                        Message = "User not found"
                    };
                }
                if (!string.IsNullOrEmpty(request.Username))
                {
                    user.Username = request.Username;
                }
                if (!string.IsNullOrEmpty(request.Password))
                {
                    user.Password = request.Password;
                }
                if (!string.IsNullOrEmpty(request.Name))
                {
                    user.Name = request.Name;
                }
                if (request.Balance.HasValue)
                {
                    user.Balance = request.Balance.Value;
                }
                _context.SaveChanges();
                return new UserPatchResponseModel
                {
                    Username = user.Username,
                    Name = user.Name,
                    Balance = user.Balance,
                    IsSuccess = true,
                    Message = "User updated successfully"
                };
            }
            catch (Exception ex)
            {
                return new UserPatchResponseModel
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
        public UserDeleteResponseModel DeleteUser(UserDeleteRequestModel request)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(x => x.UserId == request.UserId);
                if(user == null)
                {
                    return new UserDeleteResponseModel
                    {
                        IsSuccess = false,
                        Message = "User not found"
                    };
                }
                _context.Users.Remove(user);
                _context.SaveChanges();
                return new UserDeleteResponseModel
                {
                    IsSuccess = true,
                    Message = "User deleted successfully"
                };
            }
            catch (Exception)
            {

                return new UserDeleteResponseModel
                {
                    IsSuccess = false,
                    Message = "An error occurred while deleting the user"
                };
            }
        }
    }
}
