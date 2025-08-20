using LanguageCourses.Application.Commands.Alunos;
using LanguageCourses.Application.Queries.Alunos;
using LanguageCourses.Application.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LanguageCourses.API.Controllers;

[ApiController]
[Route("api/alunos")]
public class AlunosController : ControllerBase
{
    private readonly IMediator _mediator;
    public AlunosController(IMediator mediator) => _mediator = mediator;

    /// <summary>Listar todos os alunos</summary>
    [HttpGet]
    public async Task<ActionResult<List<AlunoListItemDto>>> GetAll(CancellationToken ct)
        => await _mediator.Send(new GetAlunosQuery(), ct);

    /// <summary>Obter aluno por Id</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<AlunoDetalheDto>> GetById(int id, CancellationToken ct)
    {
        var dto = await _mediator.Send(new GetAlunoByIdQuery(id), ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    /// <summary>Criar aluno já matriculado em ≥ 1 turma (Idioma+Número)</summary>
    [HttpPost]
    public async Task<ActionResult<AlunoDetalheDto>> Create([FromBody] CreateAlunoDto body, CancellationToken ct)
    {
        var dto = await _mediator.Send(new CreateAlunoCommand(body), ct);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    /// <summary>Atualizar dados do aluno e suas turmas (mantendo ≥ 1 turma)</summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<AlunoDetalheDto>> Update(int id, [FromBody] UpdateAlunoDto body, CancellationToken ct)
        => await _mediator.Send(new UpdateAlunoCommand(id, body), ct);

    /// <summary>Excluir aluno (bloqueado se estiver matriculado)</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteAlunoCommand(id), ct);
        return NoContent();
    }

    /// <summary>Matricular aluno em uma turma existente (Idioma+Número)</summary>
    [HttpPost("{id:int}/matriculas")]
    public async Task<ActionResult<AlunoDetalheDto>> Matricular(int id, [FromBody] MatricularAlunoDto body, CancellationToken ct)
        => await _mediator.Send(new MatricularAlunoCommand(id, body), ct);

    /// <summary>Desmatricular aluno de uma turma (Idioma+Número) — aluno deve permanecer com ≥ 1 turma</summary>
    [HttpDelete("{id:int}/matriculas")]
    public async Task<ActionResult<AlunoDetalheDto>> Desmatricular(int id, [FromBody] DesmatricularAlunoDto body, CancellationToken ct)
        => await _mediator.Send(new DesmatricularAlunoCommand(id, body), ct);
}
