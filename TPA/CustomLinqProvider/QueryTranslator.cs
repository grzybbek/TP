using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CustomLinqProvider
{
    internal class QueryTranslator : ExpressionVisitor
    {
        private StringBuilder sb;

        private static Expression StripQuotes(Expression e)
        {
            while (e.NodeType == ExpressionType.Quote)
            {
                e = ((UnaryExpression)e).Operand;
            }
            return e;
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "Where")
            {
                sb.Append("SELECT * FROM (");

                this.Visit(m.Arguments[0]);

                sb.Append(") AS T WHERE ");

                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);

                this.Visit(lambda.Body);

                return m;
            }

            throw new InvalidQueryException(string.Format("The method '{0}' is not supported", m.Method.Name));
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            sb.Append("(");
            this.Visit(b.Left);
            switch (b.NodeType)
            {
                case ExpressionType.And:
                    sb.Append(" AND ");
                    break;
                case ExpressionType.Or:
                    sb.Append(" OR");
                    break;
                case ExpressionType.Equal:
                    sb.Append(" = ");
                    break;
                case ExpressionType.NotEqual:
                    sb.Append(" <> ");
                    break;
                case ExpressionType.LessThan:
                    sb.Append(" < ");
                    break;
                case ExpressionType.LessThanOrEqual:
                    sb.Append(" <= ");
                    break;
                case ExpressionType.GreaterThan:
                    sb.Append(" > ");
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    sb.Append(" >= ");
                    break;
                default:
                    throw new InvalidQueryException(string.Format("The binary operator '{0}' is not supported", b.NodeType));
            }
            this.Visit(b.Right);
            sb.Append(")");
            return b;
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            if (c.Value is IQueryable q)
            {
                sb.Append("SELECT * FROM ");
                sb.Append(q.ElementType.Name);
            }
            else if (c.Value == null)
            {
                sb.Append("NULL");
            }
            else
            {
                switch (Type.GetTypeCode(c.Value.GetType()))
                {
                    case TypeCode.Boolean:
                        sb.Append(((bool)c.Value) ? 1 : 0);
                        break;
                    case TypeCode.String:
                        sb.Append("'");
                        sb.Append(c.Value);
                        sb.Append("'");
                        break;
                    case TypeCode.Object:
                        throw new InvalidQueryException(string.Format("The constant for '{0}' is not supported", c.Value));
                    default:
                        sb.Append(c.Value);
                        break;
                }
            }
            return c;
        }
    }
}
