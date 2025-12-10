using Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequestDto>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("El usuario es requerido")
                .MinimumLength(3).WithMessage("El usuario debe tener al menos 3 caracteres")
                .MaximumLength(50).WithMessage("El usuario no puede exceder 50 caracteres")
                .Matches("^[a-zA-Z0-9_-]+$").WithMessage("El usuario solo puede contener letras, números, guiones y guiones bajos");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El email es requerido")
                .EmailAddress().WithMessage("El email no es válido")
                .MaximumLength(100).WithMessage("El email no puede exceder 100 caracteres");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es requerida")
                .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres")
                .Matches("[A-Z]").WithMessage("La contraseña debe contener al menos una mayúscula")
                .Matches("[a-z]").WithMessage("La contraseña debe contener al menos una minúscula")
                .Matches("[0-9]").WithMessage("La contraseña debe contener al menos un número");
                //.Matches("[^a-zA-Z0-9]").WithMessage("La contraseña debe contener al menos un carácter especial");

            RuleFor(x => x.FirstName)
                .MaximumLength(50).WithMessage("El nombre no puede exceder 50 caracteres")
                .When(x => !string.IsNullOrEmpty(x.FirstName));

            RuleFor(x => x.LastName)
                .MaximumLength(50).WithMessage("El apellido no puede exceder 50 caracteres")
                .When(x => !string.IsNullOrEmpty(x.LastName));
        }
    }
}
