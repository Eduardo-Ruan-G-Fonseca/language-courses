using LanguageCourses.Application.Commands.Turmas;
using LanguageCourses.Application.Queries.Turmas;
using LanguageCourses.Application.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LanguageCourses.API.Controllers;

/// <summary>
/// Controller responsável pelo gerenciamento de Turmas.
/// </summary>
/// <remarks>
/// <para>
/// Expõe operações de CRUD e consultas relacionadas às turmas.
/// Utiliza <see cref="IMediator"/> para delegar o processamento aos handlers (CQRS).
/// </para>
/// <para>
/// <strong>Regras de negócio relevantes:</strong>
/// <list type="bullet">
/// <item><description>Cada turma é identificada por Idioma + Número (ex.: Inglês 101).</description></item>
/// <item><description>Uma turma não pode ultrapassar o limite de 5 alunos.</description></item>
/// <item><description>Exclusão de turma é bloqueada se houver alunos matriculados.</description></item>
/// </list>
/// </para>
/// <para>
/// <strong>Padrões utilizados:</strong> .NET 8, ASP.NET Core, CQRS, MediatR, FluentValidation.
/// </para>
/// </remarks>
[ApiController]
[Route("api/turmas")]
[Produces("application/json")]
public class TurmasController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Construtor que injeta o mediador (<see cref="IMediator"/>).
    /// </summary>
    public TurmasController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Lista todas as turmas existentes.
    /// </summary>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Lista de <see cref="TurmaDto"/>.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<TurmaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TurmaDto>>> GetAll(CancellationToken ct)
        => await _mediator.Send(new GetTurmasQuery(), ct);

    /// <summary>
    /// Obtém os detalhes de uma turma pelo seu identificador.
    /// </summary>
    /// <param name="id">Identificador da turma.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns><see cref="TurmaDto"/> com dados completos.</returns>
    /// <response code="200">Turma encontrada.</response>
    /// <response code="404">Turma não encontrada.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TurmaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TurmaDto>> GetById(int id, CancellationToken ct)
    {
        var dto = await _mediator.Send(new GetTurmaByIdQuery(id), ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    /// <summary>
    /// Lista todas as turmas de um idioma específico.
    /// </summary>
    /// <param name="idioma">Idioma da turma (ex.: Inglês, Espanhol).</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Lista de <see cref="TurmaDto"/> filtradas pelo idioma.</returns>
    [HttpGet("idioma/{idioma}")]
    [ProducesResponseType(typeof(List<TurmaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TurmaDto>>> GetByIdioma(string idioma, CancellationToken ct)
        => await _mediator.Send(new GetTurmasByIdiomaQuery(idioma), ct);

    /// <summary>
    /// Cria uma nova turma.
    /// </summary>
    /// <param name="body">Dados da turma a ser criada.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Turma criada.</returns>
    /// <remarks>
    /// Exemplo de requisição:
    /// <code language="json">
    /// {
    ///   "idioma": "Inglês",
    ///   "numero": 101,
    ///   "anoLetivo": "2025/2"
    /// }
    /// </code>
    /// </remarks>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(TurmaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TurmaDto>> Create([FromBody] CreateTurmaDto body, CancellationToken ct)
    {
        var dto = await _mediator.Send(new CreateTurmaCommand(body), ct);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    /// <summary>
    /// Atualiza os dados de uma turma existente.
    /// </summary>
    /// <param name="id">Id da turma a ser atualizada.</param>
    /// <param name="body">Dados atualizados.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Turma atualizada.</returns>
    [HttpPut("{id:int}")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(TurmaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TurmaDto>> Update(int id, [FromBody] UpdateTurmaDto body, CancellationToken ct)
    {
        var dto = await _mediator.Send(new UpdateTurmaCommand(id, body), ct);
        return Ok(dto);
    }

    /// <summary>
    /// Exclui uma turma.
    /// </summary>
    /// <param name="id">Id da turma.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>NoContent em caso de sucesso.</returns>
    /// <remarks>
    /// Exclusão é bloqueada caso a turma possua alunos matriculados.
    /// </remarks>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteTurmaCommand(id), ct);
        return NoContent();
    }

    /// <summary>
    /// Lista todos os alunos de uma turma específica.
    /// </summary>
    /// <param name="idioma">Idioma da turma.</param>
    /// <param name="numero">Número da turma.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Lista de alunos vinculados à turma.</returns>
    [HttpGet("{idioma}/{numero:int}/alunos")]
    [ProducesResponseType(typeof(List<AlunoTurmaListItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<AlunoTurmaListItem>>> GetAlunosDaTurma(string idioma, int numero, CancellationToken ct)
        => await _mediator.Send(new GetAlunosDaTurmaQuery(idioma, numero), ct);
}
