using Application.DTOs.Auth;
using Domain.Entities;

namespace Application.Common.Mappings
{
    public static class UserMapper
    {
        public static UserDto ToDto(this User user, List<string>? roles = null)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName,
                IsActive = user.IsActive,
                Roles = roles ?? user.UserRoles?.Select(ur => ur.Role.Name).ToList() ?? new List<string>()
            };
        }
    }
}
