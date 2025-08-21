/* ============================================================================
   Seed de dados para LanguageCoursesDb
   - Limpa dados existentes
   - Insere Turmas, Alunos e Matrículas
   - Respeita unicidades e até 5 alunos por turma
   ---------------------------------------------------------------------------
   Execução: SQL Server Management Studio (SSMS)
   ==========================================================================*/

USE [LanguageCoursesDb];
GO

SET NOCOUNT ON;

BEGIN TRY
    BEGIN TRAN;

    /* ---------- Limpeza segura (mantém o schema) ---------- */
    -- Respeitar FKs: primeiro matrículas, depois alunos e turmas
    IF OBJECT_ID('dbo.Matriculas', 'U') IS NOT NULL
        DELETE FROM dbo.Matriculas;

    IF OBJECT_ID('dbo.Alunos', 'U') IS NOT NULL
        DELETE FROM dbo.Alunos;

    IF OBJECT_ID('dbo.Turmas', 'U') IS NOT NULL
        DELETE FROM dbo.Turmas;

    /* ---------- Turmas ---------- */
    DECLARE @Turmas TABLE (
        Idioma NVARCHAR(50),
        Numero INT,
        AnoLetivo NVARCHAR(16)
    );

    INSERT INTO @Turmas (Idioma, Numero, AnoLetivo)
    VALUES
        (N'Ingles',   101, N'2025/1'),
        (N'Ingles',   102, N'2025/2'),
        (N'Ingles',   103, N'2025/2'),
        (N'Espanhol', 201, N'2025/1'),
        (N'Espanhol', 202, N'2025/2'),
        (N'Frances',  301, N'2025/2');

    DECLARE @TurmasInserted TABLE (
        Idioma NVARCHAR(50),
        Numero INT,
        AnoLetivo NVARCHAR(16),
        TurmaId INT
    );

    INSERT INTO dbo.Turmas (Numero, Idioma, AnoLetivo)
        OUTPUT inserted.Idioma, inserted.Numero, inserted.AnoLetivo, inserted.Id
        SELECT Numero, Idioma, AnoLetivo
        FROM @Turmas;

    INSERT INTO @TurmasInserted (Idioma, Numero, AnoLetivo, TurmaId)
        SELECT Idioma, Numero, AnoLetivo, Id
        FROM dbo.Turmas;

    /* ---------- Alunos ---------- */
    DECLARE @Alunos TABLE (
        Nome NVARCHAR(150),
        Email NVARCHAR(200),
        Cpf NVARCHAR(11),
        Idade INT
    );

    -- CPFs fictícios (11 dígitos). A API valida, mas via SQL não há checagem; mantenha únicos.
    INSERT INTO @Alunos (Nome, Email, Cpf, Idade)
    VALUES
        (N'Maria Silva',         N'maria.silva@example.com',      N'39053344705', 22),
        (N'João Pereira',        N'joao.pereira@example.com',     N'52998224725', 28),
        (N'Ana Souza',           N'ana.souza@example.com',        N'15350946056', 19),
        (N'Carlos Lima',         N'carlos.lima@example.com',      N'11122233396', 31),
        (N'Fernanda Oliveira',   N'fernanda.oli@example.com',     N'22233344405', 26),
        (N'Paulo Santos',        N'paulo.santos@example.com',     N'33344455506', 35),
        (N'Juliana Alves',       N'juliana.alves@example.com',    N'44455566607', 27),
        (N'Bruno Costa',         N'bruno.costa@example.com',      N'55566677708', 24),
        (N'Camila Rodrigues',    N'camila.rod@example.com',       N'66677788809', 21),
        (N'Rafael Gomes',        N'rafael.gomes@example.com',     N'77788899900', 29);

    DECLARE @AlunosInserted TABLE (
        Nome NVARCHAR(150),
        Email NVARCHAR(200),
        AlunoId INT
    );

    INSERT INTO dbo.Alunos (Nome, Email, Cpf, Idade)
        OUTPUT inserted.Nome, inserted.Email, inserted.Id
        SELECT Nome, Email, Cpf, Idade
        FROM @Alunos;

    INSERT INTO @AlunosInserted (Nome, Email, AlunoId)
        SELECT Nome, Email, Id
        FROM dbo.Alunos;

    /* ---------- Matrículas ----------
       Regra: até 5 alunos por turma. Vamos distribuir 10 alunos em 6 turmas. */

    -- Helper: função inline para pegar TurmaId por (Idioma, Numero)
    -- (aqui usamos select direto; poderia ser função, mas mantemos simples)
    DECLARE @Ingles101 INT = (SELECT TurmaId FROM @TurmasInserted WHERE Idioma = N'Ingles' AND Numero = 101);
    DECLARE @Ingles102 INT = (SELECT TurmaId FROM @TurmasInserted WHERE Idioma = N'Ingles' AND Numero = 102);
    DECLARE @Ingles103 INT = (SELECT TurmaId FROM @TurmasInserted WHERE Idioma = N'Ingles' AND Numero = 103);
    DECLARE @Espanhol201 INT = (SELECT TurmaId FROM @TurmasInserted WHERE Idioma = N'Espanhol' AND Numero = 201);
    DECLARE @Espanhol202 INT = (SELECT TurmaId FROM @TurmasInserted WHERE Idioma = N'Espanhol' AND Numero = 202);
    DECLARE @Frances301 INT  = (SELECT TurmaId FROM @TurmasInserted WHERE Idioma = N'Frances'  AND Numero = 301);

    -- Alunos (IDs) por email
    DECLARE 
        @aMaria   INT = (SELECT AlunoId FROM @AlunosInserted WHERE Email = N'maria.silva@example.com'),
        @aJoao    INT = (SELECT AlunoId FROM @AlunosInserted WHERE Email = N'joao.pereira@example.com'),
        @aAna     INT = (SELECT AlunoId FROM @AlunosInserted WHERE Email = N'ana.souza@example.com'),
        @aCarlos  INT = (SELECT AlunoId FROM @AlunosInserted WHERE Email = N'carlos.lima@example.com'),
        @aFer     INT = (SELECT AlunoId FROM @AlunosInserted WHERE Email = N'fernanda.oli@example.com'),
        @aPaulo   INT = (SELECT AlunoId FROM @AlunosInserted WHERE Email = N'paulo.santos@example.com'),
        @aJuliana INT = (SELECT AlunoId FROM @AlunosInserted WHERE Email = N'juliana.alves@example.com'),
        @aBruno   INT = (SELECT AlunoId FROM @AlunosInserted WHERE Email = N'bruno.costa@example.com'),
        @aCamila  INT = (SELECT AlunoId FROM @AlunosInserted WHERE Email = N'camila.rod@example.com'),
        @aRafael  INT = (SELECT AlunoId FROM @AlunosInserted WHERE Email = N'rafael.gomes@example.com');

    -- Monta um staging para matrículas desejadas
    DECLARE @Desejadas TABLE (AlunoId INT, TurmaId INT);

    INSERT INTO @Desejadas(AlunoId, TurmaId) VALUES
        (@aMaria,   @Ingles103),
        (@aJoao,    @Ingles103),
        (@aAna,     @Ingles103),
        (@aCarlos,  @Ingles102),
        (@aFer,     @Ingles102),
        (@aPaulo,   @Ingles101),
        (@aJuliana, @Ingles101),
        (@aBruno,   @Espanhol201),
        (@aCamila,  @Espanhol202),
        (@aRafael,  @Frances301),

        -- alguns com duas turmas (sem duplicar na mesma)
        (@aMaria,   @Espanhol201),
        (@aJoao,    @Frances301),
        (@aAna,     @Ingles101),
        (@aCarlos,  @Espanhol202),
        (@aFer,     @Frances301);

    /* Garante no máximo 5 alunos por turma ao inserir (contagem prévia) */
    ;WITH Contagem AS (
        SELECT d.TurmaId, COUNT(*) AS Qtde
        FROM @Desejadas d
        GROUP BY d.TurmaId
    )
    INSERT INTO dbo.Matriculas (AlunoId, TurmaId, DataMatricula)
    SELECT d.AlunoId, d.TurmaId, SYSUTCDATETIME()
    FROM @Desejadas d
    WHERE (
        SELECT COUNT(*) FROM dbo.Matriculas m WHERE m.TurmaId = d.TurmaId
    ) + (
        SELECT COUNT(*) FROM @Desejadas dd WHERE dd.TurmaId = d.TurmaId AND dd.AlunoId <= d.AlunoId
    ) <= 5
    -- evita duplicidade (mesmo aluno/mesma turma)
    AND NOT EXISTS (
        SELECT 1 FROM dbo.Matriculas m WHERE m.AlunoId = d.AlunoId AND m.TurmaId = d.TurmaId
    );

    COMMIT TRAN;

    PRINT 'Seed concluído com sucesso.';
    SELECT 
        (SELECT COUNT(*) FROM dbo.Turmas)  AS Turmas,
        (SELECT COUNT(*) FROM dbo.Alunos)  AS Alunos,
        (SELECT COUNT(*) FROM dbo.Matriculas) AS Matriculas;

    -- Visualização rápida
    SELECT a.Id AS AlunoId, a.Nome, a.Email, a.Cpf, a.Idade,
           t.Id AS TurmaId, t.Idioma, t.Numero, t.AnoLetivo, m.DataMatricula
    FROM dbo.Matriculas m
    JOIN dbo.Alunos a ON a.Id = m.AlunoId
    JOIN dbo.Turmas t ON t.Id = m.TurmaId
    ORDER BY a.Nome, t.Idioma, t.Numero;

END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRAN;

    DECLARE @ErrMsg NVARCHAR(4000) = ERROR_MESSAGE(), @ErrSev INT = ERROR_SEVERITY(), @ErrState INT = ERROR_STATE();
    RAISERROR(@ErrMsg, @ErrSev, @ErrState);
END CATCH;
GO
