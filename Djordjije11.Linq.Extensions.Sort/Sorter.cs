namespace Djordjije11.Linq.Extensions.Sort
{
    /// <summary>
    /// Build object which constructs the sorting query on top of original input query or enumerable.
    /// </summary>
    /// <typeparam name="TEntity">Domain model type.</typeparam>
    public class Sorter<TEntity> where TEntity : class
    {
        private readonly ICollection<SortPropertyExpression<TEntity>> _sortPropertyExpressions = new LinkedList<SortPropertyExpression<TEntity>>();
        private IQueryable<TEntity>? _query;

        /// <summary>
        /// Sets up the query on which the sorting query will be added when the <b>Build</b> method is called.
        /// When calling the <b>BuildOnQuery</b> or <b>BuildOnEnumerable</b> method, this query value gets overwritten.
        /// </summary>
        /// <param name="query">The query on which the sorting query will be added when the <b>Build</b> method is called.</param>
        public Sorter(IQueryable<TEntity> query)
        {
            _query = query;
        }

        /// <summary>
        /// Sets up the enumerable on which the sorting query will be added when the <b>Build</b> method is called.
        /// When calling the <b>BuildOnEnumerable</b> or <b>BuildOnQuery</b> method, this enumerable value gets overwritten.
        /// </summary>
        /// <param name="enumerable">The query on which the sorting query will be added when the <b>Build</b> method is called.</param>
        public Sorter(IEnumerable<TEntity> enumerable)
        {
            _query = enumerable.AsQueryable();
        }

        /// <summary>
        /// When using this constructor, enumerable or query need to be set up by calling the <b>BuildOnQuery</b> or <b>BuildOnEnumerable</b> method. Otherwise, <b>Build</b> method will throw an ArgumentNullException.
        /// </summary>
        public Sorter() { }

        /// <summary>
        /// Adds SortPropertyExpression object to collection that represents the properties used for sorting.
        /// Properties are added for sorting in the same order as the SortPropertyExpression objects are added using this method.
        /// </summary>
        /// <param name="sortPropertyExpression">Stores information about which property of model is used for sorting and whether the sorting order is ascending or descending.</param>
        /// <returns>The same instance of Sorter for continuing the use of Builder pattern.</returns>
        public Sorter<TEntity> Add(SortPropertyExpression<TEntity> sortPropertyExpression)
        {
            if (sortPropertyExpression is null || sortPropertyExpression.IsAscending is null)
            {
                return this;
            }

            _sortPropertyExpressions.Add(sortPropertyExpression);
            return this;
        }

        /// <summary>
        /// Adds range of SortPropertyExpression objects to collection that represents the properties used for sorting.
        /// Properties are added for sorting in the same order as the SortPropertyExpression objects are added using this method.
        /// </summary>
        /// <param name="sortPropertyExpressions">Range of objects that store informations about which properties of model are used for sorting and whether they are ascending or descending.</param>
        /// <returns>The same instance of Sorter for continuing the use of Builder pattern.</returns>
        public Sorter<TEntity> AddRange(IEnumerable<SortPropertyExpression<TEntity>> sortPropertyExpressions)
        {
            foreach (var sortPropertyExpression in sortPropertyExpressions)
            {
                Add(sortPropertyExpression);
            }
            return this;
        }

        /// <summary>
        /// Builds the query from the original query or enumerable and added SortPropertyExpression objects.
        /// Use this method only when the object is instanced by parameterized constructor.
        /// </summary>
        /// <returns>The built query from the original query and added SortPropertyExpression objects.</returns>
        public IQueryable<TEntity> Build() 
            => sortQuery(_query, _sortPropertyExpressions);

        /// <summary>
        /// Builds the query from the original query and added SortPropertyExpression objects.
        /// The parameter query overwrites the original query if it is already set by parameterized constructor.
        /// </summary>
        /// <param name="query">The query on which the sorting query will be added.</param>
        /// <returns>The built query from the original query and added SortPropertyExpression objects.</returns>
        public IQueryable<TEntity> BuildOnQuery(IQueryable<TEntity> query)
        {
            _query = query;
            return Build();
        }

        /// <summary>
        /// Builds the query from the original enumerable and added SortPropertyExpression objects.
        /// The parameter enumerable overwrites the original enumerable if it is already set by parameterized constructor.
        /// </summary>
        /// <param name="enumerable">The enumerable on which the sorting query will be added.</param>
        /// <returns>The built query from the original enumerable and added SortPropertyExpression objects.</returns>
        public IQueryable<TEntity> BuildOnEnumerable(IEnumerable<TEntity> enumerable)
        {
            _query = enumerable.AsQueryable();
            return Build();
        }

        private IQueryable<TEntity> sortQuery(IQueryable<TEntity>? query, IEnumerable<SortPropertyExpression<TEntity>> sortPropertyExpressions)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (sortPropertyExpressions is null)
            {
                return query;
            }

            IOrderedQueryable<TEntity>? orderedQuery = null;
            bool firstOrderAdded = false;

            foreach (var sortPropertyExpression in sortPropertyExpressions)
            {
                if (firstOrderAdded is false)
                {
                    firstOrderAdded = true;
                    orderedQuery = sortPropertyExpression.IsAscending is true
                        ? query.OrderBy(sortPropertyExpression.PropertyExpression)
                        : query.OrderByDescending(sortPropertyExpression.PropertyExpression);
                    continue;
                }
                orderedQuery = sortPropertyExpression.IsAscending is true
                    ? orderedQuery?.ThenBy(sortPropertyExpression.PropertyExpression)
                    : orderedQuery?.ThenByDescending(sortPropertyExpression.PropertyExpression);
            }

            return orderedQuery ?? query;
        }
    }
}
