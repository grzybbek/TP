using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CustomLinqProvider
{
    internal class Query<T> : IQueryable<T>, IQueryable, IEnumerable<T>, IEnumerable, IOrderedQueryable<T>, IOrderedQueryable
    {
        private QueryProvider queryProvider;
        private Expression expression;

        public Query(QueryProvider queryProvider, Expression expression)
        {
            this.queryProvider = queryProvider;
            this.expression = expression;
        }

        #region IQueryable

        public Expression Expression { get { return this.expression; } }

        public Type ElementType { get { return typeof(T); } }

        public IQueryProvider Provider { get { return this.queryProvider; } }

        #endregion

        #region IEnumerable

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)this.queryProvider.Execute(this.expression)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)this.queryProvider.Execute(this.expression)).GetEnumerator();
        }

        #endregion
    }
}