using FluentValidation;
using LanguageCourses.Application.Commands.Turmas;
using System.Text.RegularExpressions;

namespace LanguageCourses.Application.Validators.Turmas;

/// <summary>
/// Validador para o comando <see cref="CreateTurmaCommand"/>.
/// </summary>
/// <remarks>
/// Aplica validações de formato e regras de negócio aos campos da turma:
/// <list type="bullet">
/// <item><description><c>Numero</c> deve ser maior que zero.</description></item>
/// <item><description><c>Idioma</c> é obrigatório e limitado a 50 caracteres.</description></item>
/// <item><description><c>AnoLetivo</c> é obrigatório, limitado a 16 caracteres e deve seguir o padrão <c>YYYY/1</c> ou <c>YYYY/2</c>.</description></item>
/// </list>
/// </remarks>
public class CreateTurmaCommandValidator : AbstractValidator<CreateTurmaCommand>
{
    /// <summary>
    /// Constrói as regras de validação para criação de turma.
    /// </summary>
    public CreateTurmaCommandValidator()
    {
        // Número da turma
        RuleFor(x => x.Turma.Numero)
            .GreaterThan(0).WithMessage("Número deve ser maior que zero.");

        // Idioma
        RuleFor(x => x.Turma.Idioma)
            .NotEmpty().WithMessage("Idioma é obrigatório.")
            .MaximumLength(50);

        // Ano letivo
        RuleFor(x => x.Turma.AnoLetivo)
            .NotEmpty().WithMessage("Ano letivo é obrigatório.")
            .MaximumLength(16)
            .Must(IsValidAnoLetivo).WithMessage("Ano letivo inválido. Use formato 'YYYY/1' ou 'YYYY/2'.");
    }

    /// <summary>
    /// Valida o formato do ano letivo.
    /// </summary>
    /// <param name="s">Texto do ano letivo a validar.</param>
    /// <returns>
    /// <see langword="true"/> se corresponder ao padrão <c>YYYY/1</c> ou <c>YYYY/2</c>; caso contrário, <see langword="false"/>.
    /// </returns>
    private static bool IsValidAnoLetivo(string s)
        => Regex.IsMatch(s ?? "", @"^\d{4}\/[12]$");
}