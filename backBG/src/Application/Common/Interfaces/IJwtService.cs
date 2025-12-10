using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IJwtService
    {
        string GenerateAccessToken(User user, List<string> roles);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
        int? GetUserIdFromToken(string token);
    }
}
