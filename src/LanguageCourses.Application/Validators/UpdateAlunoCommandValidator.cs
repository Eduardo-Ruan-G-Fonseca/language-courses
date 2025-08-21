using System.Text.RegularExpressions;
using FluentValidation;
using LanguageCourses.Application.Commands.Alunos;
using LanguageCourses.Application.ViewModels;

namespace LanguageCourses.Application.Validators.Alunos;

/// <summary>
/// Validador para o comando <see cref="UpdateAlunoCommand"/>.
/// </summary>
/// <remarks>
/// Aplica validações de formato e regras de negócio para atualização de aluno:
/// <list type="bullet">
/// <item><description><c>Id</c> do aluno deve ser maior que zero.</description></item>
/// <item><description><c>Nome</c> obrigatório (≤ 150 caracteres).</description></item>
/// <item><description><c>Email</c> obrigatório, válido e ≤ 200 caracteres.</description></item>
/// <item><description><c>CPF</c> obrigatório e válido (aceita entrada com ou sem máscara).</description></item>
/// <item><description><c>Idade</c> ≥ 0.</description></item>
/// <item><description>Lista de turmas obrigatória, com pelo menos uma turma e sem duplicidade por Idioma+Número.</description></item>
/// <item><description>Para cada turma: <c>Idioma</c> obrigatório (≤ 50) e <c>Numero</c> &gt; 0.</description></item>
/// </list>
/// </remarks>
public class UpdateAlunoCommandValidator : AbstractValidator<UpdateAlunoCommand>
{
    /// <summary>
    /// Constrói as regras de validação para atualização de aluno.
    /// </summary>
    public UpdateAlunoCommandValidator()
    {
        // Id do aluno
        RuleFor(x => x.Id)
            .GreaterThan(0);

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
            .Must(t => t.Count > 0).WithMessage("Aluno deve permanecer em pelo menos 1 turma.")
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
