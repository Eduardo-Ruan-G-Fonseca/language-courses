using LanguageCourses.Application.ViewModels;
using MediatR;

namespace LanguageCourses.Application.Commands.Alunos;

/// <summary>
/// Comando para matricular um aluno em uma turma.
/// </summary>
/// <param name="AlunoId">
/// Identificador único do aluno que será matriculado.
/// </param>
/// <param name="Matricula">
/// Objeto contendo os dados da matrícula, como turma e data da matrícula.
/// </param>
/// <returns>
/// Retorna um <see cref="AlunoDetalheDto"/> atualizado com as informações
/// do aluno já associado à nova matrícula.
/// </returns>
public record MatricularAlunoCommand(int AlunoId, MatricularAlunoDto Matricula) 
    : IRequest<AlunoDetalheDto>;