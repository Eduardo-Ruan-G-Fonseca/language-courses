using System.Text.RegularExpressions;
using FluentValidation;
using LanguageCourses.Application.Commands.Alunos;
using LanguageCourses.Application.ViewModels;

namespace LanguageCourses.Application.Validators.Alunos;

/// <summary>
/// Validador para o comando <see cref="CreateAlunoCommand"/>.
/// </summary>
/// <remarks>
/// Aplica regras de negócio e de formato aos dados do aluno durante a criação:
/// <list type="bullet">
/// <item><description>Nome obrigatório e com limite de 150 caracteres.</description></item>
/// <item><description>Email obrigatório, válido e com limite de 200 caracteres.</description></item>
/// <item><description>CPF obrigatório e válido, aceitando entrada com ou sem máscara.</description></item>
/// <item><description>Idade maior ou igual a zero.</description></item>
/// <item><description>Pelo menos uma turma informada, sem duplicidades por Idioma+Número.</description></item>
/// <item><description>Para cada turma: Idioma obrigatório (≤ 50) e Número &gt; 0.</description></item>
/// </list>
/// </remarks>
public class CreateAlunoCommandValidator : AbstractValidator<CreateAlunoCommand>
{
    /// <summary>
    /// Constrói as regras de validação para criação de aluno.
    /// </summary>
    public CreateAlunoCommandValidator()
    {
        // Nome
        RuleFor(x => x.Aluno.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório.")
            .MaximumLength(150);

        // Email
        RuleFor(x => x.Aluno.Email)
            .NotEmpty().WithMessage("Email é obrigatório.")
            .MaximumLength(200)
            .EmailAddress().WithMessage("Email inválido.");

        // CPF (aceita com/sem máscara)
        RuleFor(x => x.Aluno.Cpf)
            .NotEmpty().WithMessage("CPF é obrigatório.")
            .Must(IsValidCpfWithMask).WithMessage("CPF inválido.");

        // Idade
        RuleFor(x => x.Aluno.Idade)
            .GreaterThanOrEqualTo(0);

        // Turmas (lista)
        RuleFor(x => x.Aluno.Turmas)
            .NotNull().WithMessage("Informe ao menos uma turma.")
            .Must(t => t.Count > 0).WithMessage("Aluno deve estar matriculado em pelo menos 1 turma.")
            .Must(NoDuplicates).WithMessage("Não repita a mesma turma (Idioma+Número).");

        // Regras por item de turma
        RuleForEach(x => x.Aluno.Turmas).ChildRules(t =>
        {
            t.RuleFor(r => r.Idioma)
                .NotEmpty()
                .MaximumLength(50);

            t.RuleFor(r => r.Numero)
                .GreaterThan(0);
        });
    }

    /// <summary>
    /// Garante que não existam turmas duplicadas na lista (chave Idioma+Número, case-insensitive).
    /// </summary>
    private static bool NoDuplicates(List<TurmaRefDto> list)
    {
        var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var t in list)
        {
            var key = $"{t.Idioma?.Trim()}|{t.Numero}";
            if (!set.Add(key)) return false;
        }
        return true;
    }

    /// <summary>
    /// Valida CPF aceitando entrada com ou sem máscara.
    /// Remove caracteres não numéricos, checa tamanho, repetição e dígitos verificadores.
    /// </summary>
    private static bool IsValidCpfWithMask(string? cpfRaw)
    {
        if (string.IsNullOrWhiteSpace(cpfRaw)) return false;

        // Normaliza para apenas dígitos
        var cpf = Regex.Replace(cpfRaw, "[^0-9]", "");
        if (cpf.Length != 11) return false;

        // Rejeita sequências repetidas (ex.: 11111111111)
        if (new string(cpf[0], 11) == cpf) return false;

        int[] m1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] m2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        string temp = cpf[..9];
        int soma = 0;

        for (int i = 0; i < 9; i++)
            soma += (temp[i] - '0') * m1[i];

        int r = soma % 11;
        int d1 = r < 2 ? 0 : 11 - r;

        temp += d1.ToString();
        soma = 0;

        for (int i = 0; i < 10; i++)
            soma += (temp[i] - '0') * m2[i];

        r = soma % 11;
        int d2 = r < 2 ? 0 : 11 - r;

        return cpf[9] - '0' == d1 && cpf[10] - '0' == d2;
    }
}
