using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Expressions.Internal;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Internal;

using System.Linq.Expressions;

namespace StreamMaster.Infrastructure.EF.PGSQL;

#pragma warning disable EF1001
internal class MyQueryTranslationPostprocessorFactory : IQueryTranslationPostprocessorFactory
{
    public MyQueryTranslationPostprocessorFactory(
        QueryTranslationPostprocessorDependencies dependencies,
        RelationalQueryTranslationPostprocessorDependencies relationalDependencies)
    {
        Dependencies = dependencies;
        RelationalDependencies = relationalDependencies;
    }

    protected virtual QueryTranslationPostprocessorDependencies Dependencies { get; }
    protected virtual RelationalQueryTranslationPostprocessorDependencies RelationalDependencies { get; }

    public virtual QueryTranslationPostprocessor Create(QueryCompilationContext queryCompilationContext)
    {
        return new MyQueryTranslationPostprocessor(Dependencies, RelationalDependencies, queryCompilationContext);
    }
}

internal class MyQueryTranslationPostprocessor : NpgsqlQueryTranslationPostprocessor
{
    private readonly CaseInsensitiveLikeReplacer _caseInsensitiveLikeReplacer = new();

    public MyQueryTranslationPostprocessor(
        QueryTranslationPostprocessorDependencies dependencies,
        RelationalQueryTranslationPostprocessorDependencies relationalDependencies,
        QueryCompilationContext queryCompilationContext)
        : base(dependencies, relationalDependencies, queryCompilationContext)
    {
    }

    public override Expression Process(Expression query)
    {
        query = _caseInsensitiveLikeReplacer.Visit(query);

        return base.Process(query);
    }

    private class CaseInsensitiveLikeReplacer : ExpressionVisitor
    {
        protected override Expression VisitExtension(Expression node)
        {
            return node switch
            {
                ShapedQueryExpression shapedQueryExpression
                    => shapedQueryExpression.Update(
                        Visit(shapedQueryExpression.QueryExpression),
                        Visit(shapedQueryExpression.ShaperExpression)),
                LikeExpression likeExpression => new PgILikeExpression(likeExpression.Match, likeExpression.Pattern, likeExpression.EscapeChar, likeExpression.TypeMapping),
                _ => base.VisitExtension(node)
            };
        }
    }
}
#pragma warning restore EF1001