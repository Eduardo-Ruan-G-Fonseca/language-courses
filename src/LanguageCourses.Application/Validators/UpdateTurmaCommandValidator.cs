using FluentValidation;
using LanguageCourses.Application.Commands.Turmas;
using System.Text.RegularExpressions;

namespace LanguageCourses.Application.Validators.Turmas;

/// <summary>
/// Validador para o comando <see cref="UpdateTurmaCommand"/>.
/// 
/// Regras aplicadas:
/// - <b>Id</b>: deve ser maior que zero.
/// - <b>Número</b>: deve ser maior que zero.
/// - <b>Idioma</b>: obrigatório, máximo de 50 caracteres.
/// - <b>AnoLetivo</b>: obrigatório, máximo de 16 caracteres,
///   e precisa seguir o formato 'YYYY/1' ou 'YYYY/2'.
/// </summary>
public class UpdateTurmaCommandValidator : AbstractValidator<UpdateTurmaCommand>
{
    /// <summary>
    /// Construtor que define as regras de validação
    /// para atualização de uma turma.
    /// </summary>
    public UpdateTurmaCommandValidator()
    {
        // O ID da turma deve ser válido (positivo)
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Id da turma deve ser maior que zero.");

        // Número da turma deve ser positivo
        RuleFor(x => x.Turma.Numero)
            .GreaterThan(0)
            .WithMessage("Número da turma deve ser maior que zero.");

        // Idioma não pode ser vazio e deve ter no máximo 50 caracteres
        RuleFor(x => x.Turma.Idioma)
            .NotEmpty().WithMessage("Idioma é obrigatório.")
            .MaximumLength(50).WithMessage("Idioma deve ter no máximo 50 caracteres.");

        // AnoLetivo deve ter formato válido 'YYYY/1' ou 'YYYY/2'
        RuleFor(x => x.Turma.AnoLetivo)
            .NotEmpty().WithMessage("Ano letivo é obrigatório.")
            .MaximumLength(16).WithMessage("Ano letivo deve ter no máximo 16 caracteres.")
            .Must(IsValidAnoLetivo).WithMessage("Ano letivo inválido. Use formato 'YYYY/1' ou 'YYYY/2'.");
    }

    /// <summary>
    /// Verifica se o ano letivo está no formato válido (YYYY/1 ou YYYY/2).
    /// Exemplo: "2025/1", "2025/2".
    /// </summary>
    private static bool IsValidAnoLetivo(string s)
        => Regex.IsMatch(s ?? "", @"^\d{4}\/[12]$");
}
