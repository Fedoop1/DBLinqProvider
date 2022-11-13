﻿using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using DBLinqProvider.Attributes;

namespace DBLinqProvider.QueryProvider;

public class ExpressionToSqlQueryConverter : ExpressionVisitor
{
    private readonly string tableName;

    private SqlQueryBuilder queryBuilder;
    private StringBuilder stringBuilder = new ();

    public ExpressionToSqlQueryConverter(string tableName)
    {
        this.tableName = tableName;
    }

    public string Convert(Expression expression)
    {
        this.queryBuilder = SqlQueryBuilder.Create(this.tableName);
        this.stringBuilder.Clear();

        this.Visit(expression);

        return queryBuilder.Build();
    }

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        if (node.Method.DeclaringType != typeof(Queryable)) return base.VisitMethodCall(node);

        switch (node.Method.Name)
        {
            case "Select":
                stringBuilder.Clear();
                base.Visit(node);
                queryBuilder.AddSelector(stringBuilder.ToString());
                break;
            case "Where":
                stringBuilder.Clear();
                base.Visit(node.Arguments[1]);
                queryBuilder.AddCondition(stringBuilder.ToString());
                break;
            default: throw new NotSupportedException(node.Method.Name + "doesn't supported");
        }

        return node;
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        stringBuilder.Append(this.tableName + ".");
        stringBuilder.Append(node.Member.Name);

        return base.VisitMember(node);
    }

    protected override Expression VisitConstant(ConstantExpression node)
    {
        stringBuilder.Append(node.Value);

        return base.VisitConstant(node);
    }

    protected override Expression VisitBinary(BinaryExpression node)
    {
        switch (node.NodeType)
        {
            case ExpressionType.Equal:
                Visit(node.Left);
                stringBuilder.Append(" = ");
                Visit(node.Right);
                break;
            case ExpressionType.GreaterThan:
                Visit(node.Left);
                stringBuilder.Append(" > ");
                Visit(node.Right);
                break;
            case ExpressionType.LessThan:
                Visit(node.Left);
                stringBuilder.Append(" < ");
                Visit(node.Right);
                break;
            case ExpressionType.AndAlso:
                Visit(node.Left);
                stringBuilder.Append(" AND ");
                Visit(node.Right);
                break;
            default: throw new NotSupportedException($"Operation '{node.NodeType}' isn't supported");
        }

        return node;
    }
}
