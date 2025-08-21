using LanguageCourses.Application.Commands.Alunos;
using LanguageCourses.Application.Queries.Alunos;
using LanguageCourses.Application.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LanguageCourses.API.Controllers;

/// <summary>
/// Controller responsável pelo gerenciamento de Alunos.
/// </summary>
/// <remarks>
/// <para>
/// Este controller expõe operações de leitura (queries) e escrita (commands) via <see cref="IMediator"/>,
/// seguindo o padrão CQRS com MediatR. As respostas são serializadas em JSON.
/// </para>
/// <para>
/// <strong>Regras de negócio relevantes:</strong>
/// <list type="bullet">
/// <item><description>O aluno deve estar sempre matriculado em <strong>pelo menos 1 turma</strong>.</description></item>
/// <item><description>Não é permitido <strong>duplicar</strong> a mesma turma (Idioma + Número) para um aluno.</description></item>
/// <item><description>A <strong>exclusão</strong> do aluno é <strong>bloqueada</strong> se houver matrículas ativas.</description></item>
/// <item><description>As turmas são referenciadas por <strong>Idioma</strong> + <strong>Número</strong> (não por ID interno).</description></item>
/// </list>
/// </para>
/// <para>
/// <strong>Padrões:</strong> .NET 8, ASP.NET Core, CQRS, MediatR, FluentValidation.
/// </para>
/// </remarks>
[ApiController]
[Route("api/alunos")]
[Produces("application/json")]
public class AlunosController : ControllerBase
{
    private readonly IMediator _mediator;

    public AlunosController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Lista todos os alunos cadastrados (visão resumida).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<AlunoListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<AlunoListItemDto>>> GetAll(CancellationToken ct)
        => await _mediator.Send(new GetAlunosQuery(), ct);

    /// <summary>
    /// Lista todos os alunos com suas turmas (visão detalhada).
    /// </summary>
    [HttpGet("com-turmas")]
    [ProducesResponseType(typeof(List<AlunoDetalheDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<AlunoDetalheDto>>> GetAllWithTurmas(CancellationToken ct)
        => await _mediator.Send(new GetAlunosComTurmasQuery(), ct);

    /// <summary>
    /// Obtém os detalhes de um aluno pelo seu identificador.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AlunoDetalheDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AlunoDetalheDto>> GetById(int id, CancellationToken ct)
    {
        var dto = await _mediator.Send(new GetAlunoByIdQuery(id), ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    /// <summary>
    /// Cria um novo aluno já matriculado em pelo menos uma turma.
    /// </summary>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(AlunoDetalheDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AlunoDetalheDto>> Create([FromBody] CreateAlunoDto body, CancellationToken ct)
    {
        var dto = await _mediator.Send(new CreateAlunoCommand(body), ct);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    /// <summary>
    /// Atualiza dados do aluno e suas turmas (deve permanecer com ≥ 1 turma).
    /// </summary>
    [HttpPut("{id:int}")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(AlunoDetalheDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AlunoDetalheDto>> Update(int id, [FromBody] UpdateAlunoDto body, CancellationToken ct)
        => await _mediator.Send(new UpdateAlunoCommand(id, body), ct);

    /// <summary>
    /// Exclui um aluno (bloqueado se ainda estiver matriculado).
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteAlunoCommand(id), ct);
        return NoContent();
    }

    /// <summary>
    /// Matricula um aluno em uma turma existente (Idioma + Número).
    /// </summary>
    [HttpPost("{id:int}/matriculas")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(AlunoDetalheDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AlunoDetalheDto>> Matricular(int id, [FromBody] MatricularAlunoDto body, CancellationToken ct)
        => await _mediator.Send(new MatricularAlunoCommand(id, body), ct);

    /// <summary>
    /// Remove a matrícula de um aluno em uma turma específica (deve restar ≥ 1 turma).
    /// </summary>
    [HttpDelete("{id:int}/matriculas")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(AlunoDetalheDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AlunoDetalheDto>> Desmatricular(int id, [FromBody] DesmatricularAlunoDto body, CancellationToken ct)
        => await _mediator.Send(new DesmatricularAlunoCommand(id, body), ct);
}
