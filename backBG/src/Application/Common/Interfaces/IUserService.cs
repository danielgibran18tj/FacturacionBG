using Application.DTOs;
using Application.DTOs.Auth;
using Domain.DTOs;
using Domain.Entities;

namespace Application.Common.Interfaces
{
    public interface IUserService
    {
        Task<User> GetByIdAsync(int id);
        Task<User> GetByUsernameAsync(string username);
        Task<List<User>> GetAllAsync();
        Task<UserDto> CreateAsync(RegisterRequestDto request);
        Task UpdateAsync(int id, UpdateUserDto user);
        Task<PagedResult<UserDto>> GetPagedAsync(PageRequestDto request, bool isActive);
    }

}
