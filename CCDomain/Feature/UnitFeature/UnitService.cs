using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CCDatabase.Models;
using CCDomain.Model.UnitModels;

namespace CCDomain.Feature.UnitFeature
{
    public class UnitService
    {
        private readonly AppDbContext _context;
        public UnitService(AppDbContext context)
        {
            _context = context;
        }

        public UnitListResponseModel GetAllUnits()
        {
            try
            {
                var list = _context.Units.ToList();
                return new UnitListResponseModel
                {
                    Units = list.Select(u => new UnitModel
                    {
                        UnitId = u.UnitId,
                        Type = u.Type,
                        Rate = u.Rate,
                        IsActive = u.IsActive
                    }).ToList(),
                    IsSuccess = true,
                    Message = "Units retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                return new UnitListResponseModel
                {
                    IsSuccess = false,
                    Message = ex.ToString(),
                };
            }
        }

        public UnitDetailResponseModel GetUnitById(UnitDetailRequestModel request)
        {
            try
            {
                var unit = _context.Units.FirstOrDefault(u => u.UnitId == request.UnitId);
                if (unit == null)
                {
                    return new UnitDetailResponseModel
                    {
                        IsSuccess = false,
                        Message = "Unit not found"
                    };
                }

                return new UnitDetailResponseModel
                {
                    UnitId = unit.UnitId,
                    Type = unit.Type,
                    Rate = unit.Rate,
                    IsActive = unit.IsActive,
                    IsSuccess = true,
                    Message = "Unit found"
                };
            }
            catch (Exception ex)
            {
                return new UnitDetailResponseModel
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public UnitCreateResponseModel CreateUnit(UnitCreateRequestModel request)
        {
            try
            {
                var unit = new Unit
                {
                    Type = request.Type,
                    Rate = request.Rate,
                    IsActive = request.IsActive
                };
                _context.Units.Add(unit);
                _context.SaveChanges();
                return new UnitCreateResponseModel
                {
                    UnitId = unit.UnitId,
                    Type = unit.Type,
                    Rate = unit.Rate,
                    IsActive = unit.IsActive,
                    IsSuccess = true,
                    Message = "Unit created successfully"
                };
            }
            catch (Exception ex)
            {
                return new UnitCreateResponseModel
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public UnitPatchResponseModel UpdateUnit(UnitPatchRequestModel request, int unitId)
        {
            try
            {
                var unit = _context.Units.FirstOrDefault(u => u.UnitId == unitId);
                if (unit == null)
                {
                    return new UnitPatchResponseModel
                    {
                        IsSuccess = false,
                        Message = "Unit not found"
                    };
                }
                if (!string.IsNullOrEmpty(request.Type))
                {
                    unit.Type = request.Type;
                }
                if (request.Rate.HasValue)
                {
                    unit.Rate = request.Rate.Value;
                }
                if (request.IsActive.HasValue)
                {
                    unit.IsActive = request.IsActive.Value;
                }
                _context.SaveChanges();
                return new UnitPatchResponseModel
                {
                    Type = unit.Type,
                    Rate = unit.Rate,
                    IsActive = unit.IsActive,
                    IsSuccess = true,
                    Message = "Unit updated successfully"
                };
            }
            catch (Exception ex)
            {
                return new UnitPatchResponseModel
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public UnitDeleteResponseModel DeleteUnit(UnitDeleteRequestModel request)
        {
            try
            {
                var unit = _context.Units.FirstOrDefault(x => x.UnitId == request.UnitId);
                if (unit == null)
                {
                    return new UnitDeleteResponseModel
                    {
                        IsSuccess = false,
                        Message = "Unit not found"
                    };
                }
                _context.Units.Remove(unit);
                _context.SaveChanges();
                return new UnitDeleteResponseModel
                {
                    IsSuccess = true,
                    Message = "Unit deleted successfully"
                };
            }
            catch (Exception)
            {
                return new UnitDeleteResponseModel
                {
                    IsSuccess = false,
                    Message = "An error occurred while deleting the unit"
                };
            }
        }
    }
}
