using LanguageCourses.Application.ViewModels;
using MediatR;

namespace LanguageCourses.Application.Commands.Turmas;

/// <summary>
/// Comando responsável por solicitar a criação de uma nova <see cref="Turma"/>.
/// 
/// Este comando segue o padrão CQRS com MediatR, encapsulando os dados
/// necessários para a criação de uma turma sem expor diretamente a entidade de domínio.
/// 
/// O fluxo é o seguinte:
/// 1. O controlador recebe a requisição HTTP de criação de turma.
/// 2. O controlador instancia este comando com os dados do DTO recebido.
/// 3. O comando é enviado via <see cref="IMediator"/> para o respectivo handler.
/// 4. O handler cria a turma no banco de dados e retorna um <see cref="TurmaDto"/>.
/// </summary>
/// <param name="Turma">
/// Objeto de transferência (<see cref="CreateTurmaDto"/>) contendo os dados obrigatórios
/// para criar uma nova turma, como Idioma, Número e AnoLetivo.
/// </param>
/// <returns>
/// Um objeto <see cref="TurmaDto"/> representando a turma criada, 
/// incluindo o Id gerado pelo banco de dados.
/// </returns>
public record CreateTurmaCommand(CreateTurmaDto Turma) : IRequest<TurmaDto>;