using LanguageCourses.Application.Commands.Turmas;
using LanguageCourses.Application.Queries.Turmas;
using LanguageCourses.Application.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LanguageCourses.API.Controllers;

[ApiController]
[Route("api/turmas")]
public class TurmasController : ControllerBase
{
    private readonly IMediator _mediator;
    public TurmasController(IMediator mediator) => _mediator = mediator;

    /// <summary>Listar todas as turmas</summary>
    [HttpGet]
    public async Task<ActionResult<List<TurmaDto>>> GetAll(CancellationToken ct)
        => await _mediator.Send(new GetTurmasQuery(), ct);

    /// <summary>Obter turma por Id</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TurmaDto>> GetById(int id, CancellationToken ct)
    {
        var dto = await _mediator.Send(new GetTurmaByIdQuery(id), ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    /// <summary>Listar turmas por idioma</summary>
    [HttpGet("idioma/{idioma}")]
    public async Task<ActionResult<List<TurmaDto>>> GetByIdioma(string idioma, CancellationToken ct)
        => await _mediator.Send(new GetTurmasByIdiomaQuery(idioma), ct);

    /// <summary>Criar turma</summary>
    [HttpPost]
    public async Task<ActionResult<TurmaDto>> Create([FromBody] CreateTurmaDto body, CancellationToken ct)
    {
        var dto = await _mediator.Send(new CreateTurmaCommand(body), ct);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    /// <summary>Atualizar turma</summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<TurmaDto>> Update(int id, [FromBody] UpdateTurmaDto body, CancellationToken ct)
    {
        var dto = await _mediator.Send(new UpdateTurmaCommand(id, body), ct);
        return Ok(dto);
    }

    /// <summary>Excluir turma (bloqueia se possuir alunos)</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteTurmaCommand(id), ct);
        return NoContent();
    }

    /// <summary>Listar alunos de uma turma por Idioma e Número</summary>
    [HttpGet("{idioma}/{numero:int}/alunos")]
    public async Task<ActionResult<List<AlunoTurmaListItem>>> GetAlunosDaTurma(string idioma, int numero, CancellationToken ct)
        => await _mediator.Send(new GetAlunosDaTurmaQuery(idioma, numero), ct);
}
