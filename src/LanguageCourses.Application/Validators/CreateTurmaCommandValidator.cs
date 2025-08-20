using FluentValidation;
using LanguageCourses.Application.Commands.Turmas;
using System.Text.RegularExpressions;

namespace LanguageCourses.Application.Validators.Turmas;

public class CreateTurmaCommandValidator : AbstractValidator<CreateTurmaCommand>
{
    public CreateTurmaCommandValidator()
    {
        RuleFor(x => x.Turma.Numero)
            .GreaterThan(0).WithMessage("Número deve ser maior que zero.");

        RuleFor(x => x.Turma.Idioma)
            .NotEmpty().MaximumLength(50);

        RuleFor(x => x.Turma.AnoLetivo)
            .NotEmpty().MaximumLength(16)
            .Must(IsValidAnoLetivo).WithMessage("Ano letivo inválido. Use formato 'YYYY/1' ou 'YYYY/2'.");
    }

    private static bool IsValidAnoLetivo(string s)
        => Regex.IsMatch(s ?? "", @"^\d{4}\/[12]$");
}