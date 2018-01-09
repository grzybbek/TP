using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace CustomLinqProvider
{
    public class QueryProvider : IQueryProvider
    {

        #region IQueryProvider

        public IQueryable CreateQuery(Expression expression)
        {
            Type elementType = TypeSystem.GetElementType(expression.Type);

            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(Query<>).MakeGenericType(elementType), new object[] { this, expression });
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new Query<TElement>(this, expression);
        }

        public object Execute(Expression expression)
        {
            return this.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return (TResult)this.Execute(expression);
        }

        #endregion
    }
}
