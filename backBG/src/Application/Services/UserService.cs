using Application.Common.Interfaces;
using Application.DTOs;
using Application.DTOs.Auth;
using Application.DTOs.Customer;
using Application.DTOs.Product;
using AutoMapper;
using Domain.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;

        public UserService(
            IUserRepository userRepository,
            IMapper mapper,
            IRoleRepository roleRepository,
            ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _roleRepository = roleRepository;
            _logger = logger;
        }

        public async Task<User> GetByIdAsync(int id)
        {
            _logger.LogInformation("Buscando usuario por Id: {UserId}", id);

            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                _logger.LogWarning("Usuario no encontrado. Id: {UserId}", id);
                throw new KeyNotFoundException($"Usuario con Id {id} no encontrado.");
            }

            return user;
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("El username no puede estar vacío.", nameof(username));

            _logger.LogInformation("Buscando usuario por Username: {Username}", username);

            var user = await _userRepository.GetByUsernameAsync(username);

            if (user == null)
            {
                _logger.LogWarning("Usuario no encontrado. Username: {Username}", username);
                throw new KeyNotFoundException($"Usuario '{username}' no encontrado.");
            }

            return user;
        }

        public async Task<List<User>> GetAllAsync()
        {
            _logger.LogInformation("Obteniendo lista de usuarios");

            return await _userRepository.GetAllAsync();
        }

        public async Task<UserDto> CreateAsync(RegisterRequestDto request)
        {
            if (await _userRepository.ExistsByUsernameAsync(request.Username))
                throw new InvalidOperationException("El nombre de usuario ya está en uso");

            if (await _userRepository.ExistsByEmailAsync(request.Email))
                throw new InvalidOperationException("El email ya está registrado");

            _logger.LogInformation("Creando usuario con Username: {Username}", request.Username);

            var exists = await _userRepository.ExistsByUsernameAsync(request.Username);
            if (exists)
            {
                _logger.LogWarning("Intento de crear usuario duplicado. Username: {Username}", request.Username);

                throw new InvalidOperationException(
                    $"Ya existe un usuario con el username '{request.Username}'.");
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = passwordHash,
                FirstName = request.FirstName,
                LastName = request.LastName,
                IsActive = true,
            };

            var createdUser = await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            _logger.LogInformation(
                "Usuario creado correctamente. Id: {UserId}, Username: {Username}",
                createdUser.Id, createdUser.Username);

            // Asignar roles (ADMIN, VENDEDOR, CLIENTE)
            var rolesToAssign = request.Roles.ToList();

            foreach (var roleName in rolesToAssign)
            {
                var role = await _roleRepository.GetByNameAsync(roleName);
                if (role != null) 
                    await _roleRepository.AddUserRoleAsync(user.Id, role.Id);
            }
            await _roleRepository.SaveChangesAsync();

            var resp = await _userRepository.GetByUsernameAsync(request.Username);
            return _mapper.Map<UserDto>(resp);
        }

        public async Task UpdateAsync(int id, UpdateUserDto user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            _logger.LogInformation("Actualizando usuario Id: {UserId}", id);

            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser == null)
            {
                _logger.LogWarning("Usuario no encontrado para actualizar. Id: {UserId}", id);
                throw new KeyNotFoundException($"Usuario con Id {id} no encontrado.");
            }

            existingUser.Username = user.Username;
            existingUser.Email = user.Email;
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.IsActive = user.IsActive;

            await _userRepository.UpdateAsync(existingUser);
            await _userRepository.SaveChangesAsync();

            _logger.LogInformation("Usuario actualizado correctamente. Id: {UserId}", id);
        }

        public async Task<PagedResult<UserDto>> GetPagedAsync(PageRequestDto request, bool isActive)
        {
            var pagedData = await _userRepository.GetPagedAsync(
                           request.Page,
                           request.PageSize,
                           isActive,
                           request.SearchTerm
                       );

            return new PagedResult<UserDto>
            {
                Items = _mapper.Map<List<UserDto>>(pagedData.Items),
                TotalItems = pagedData.TotalItems,
                TotalPages = pagedData.TotalPages,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }
    }

}
