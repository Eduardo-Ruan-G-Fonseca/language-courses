using System.Text.RegularExpressions;
using FluentValidation;
using LanguageCourses.Application.Commands.Alunos;
using LanguageCourses.Application.ViewModels;

namespace LanguageCourses.Application.Validators.Alunos;

public class UpdateAlunoCommandValidator : AbstractValidator<UpdateAlunoCommand>
{
    public UpdateAlunoCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);

        RuleFor(x => x.Aluno.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório.")
            .MaximumLength(150);

        RuleFor(x => x.Aluno.Email)
            .NotEmpty().WithMessage("Email é obrigatório.")
            .MaximumLength(200)
            .EmailAddress().WithMessage("Email inválido.");

        RuleFor(x => x.Aluno.Cpf)
            .NotEmpty().WithMessage("CPF é obrigatório.")
            .Must(IsValidCpfWithMask).WithMessage("CPF inválido."); // aceita com/sem máscara

        RuleFor(x => x.Aluno.Idade)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Aluno.Turmas)
            .NotNull().WithMessage("Informe ao menos uma turma.")
            .Must(t => t.Count > 0).WithMessage("Aluno deve permanecer em pelo menos 1 turma.")
            .Must(NoDuplicates).WithMessage("Não repita a mesma turma (Idioma+Número).");

        RuleForEach(x => x.Aluno.Turmas).ChildRules(t =>
        {
            t.RuleFor(r => r.Idioma).NotEmpty().MaximumLength(50);
            t.RuleFor(r => r.Numero).GreaterThan(0);
        });
    }

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

    private static bool IsValidCpfWithMask(string? cpfRaw)
    {
        if (string.IsNullOrWhiteSpace(cpfRaw)) return false;
        var cpf = Regex.Replace(cpfRaw, "[^0-9]", "");
        if (cpf.Length != 11) return false;
        if (new string(cpf[0], 11) == cpf) return false;

        int[] m1 = {10,9,8,7,6,5,4,3,2};
        int[] m2 = {11,10,9,8,7,6,5,4,3,2};

        string temp = cpf[..9];
        int soma = 0;
        for (int i = 0; i < 9; i++) soma += (temp[i] - '0') * m1[i];
        int r = soma % 11;
        int d1 = r < 2 ? 0 : 11 - r;

        temp += d1.ToString();
        soma = 0;
        for (int i = 0; i < 10; i++) soma += (temp[i] - '0') * m2[i];
        r = soma % 11;
        int d2 = r < 2 ? 0 : 11 - r;

        return cpf[9] - '0' == d1 && cpf[10] - '0' == d2;
    }
}
