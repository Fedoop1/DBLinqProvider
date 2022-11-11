using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using DBLinqProvider.Attributes;

namespace DBLinqProvider.QueryProvider;
public class ExpressionToSqlQueryConverter : ExpressionVisitor
{
    private readonly StringBuilder sb = new();

    public string Convert<TEntity>(Expression expression)
    {
        var tableAttribute = (TableAttribute)typeof(TEntity).GetCustomAttribute(typeof(TableAttribute)) ?? 
                        throw new Exception($"{typeof(TEntity).Name} doesn't contain {nameof(TableAttribute)} attribute");

        this.sb.Clear();
        this.sb.Append($"SELECT * FROM {tableAttribute.Name}");

        this.Visit(expression);

        return sb.ToString();
    }

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        if (node.Method.DeclaringType != typeof(Queryable)) return base.VisitMethodCall(node);

        switch (node.Method.Name)
        {
            case "Select":
                sb.Replace("*", string.Empty);
                break;
            case "Where":
                sb.Append(" WHERE ");
                break;
            default: throw new NotSupportedException(node.Method.Name + "doesn't supported");
        }

        return base.VisitMethodCall(node);
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        sb.Append(node.Expression!.Type.Name + ".");
        sb.Append(node.Member.Name);

        return base.VisitMember(node);
    }

    protected override Expression VisitConstant(ConstantExpression node)
    {
        sb.Append(node.Value);

        return base.VisitConstant(node);
    }

    protected override Expression VisitBinary(BinaryExpression node)
    {
        if (!sb.ToString().Contains("WHERE"))
        {
            sb.Append(" WHERE ");
        }

        switch (node.NodeType)
        {
            case ExpressionType.Equal:
                Visit(node.Left);
                sb.Append(" = ");
                Visit(node.Right);
                break;
            case ExpressionType.GreaterThan:
                Visit(node.Left);
                sb.Append(" > ");
                Visit(node.Right);
                break;
            case ExpressionType.LessThan:
                Visit(node.Left);
                sb.Append(" < ");
                Visit(node.Right);
                break;
            case ExpressionType.AndAlso:
                Visit(node.Left);
                sb.Append(" AND ");
                Visit(node.Right);
                break;
            default: throw new NotSupportedException($"Operation '{node.NodeType}' isn't supported");
        }

        return node;
    }
}
