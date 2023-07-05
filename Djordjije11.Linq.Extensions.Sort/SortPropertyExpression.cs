using System.Linq.Expressions;

namespace Djordjije11.Linq.Extensions.Sort
{
    /// <summary>
    /// <para>
    /// Stores information about which property of model is used for sorting and whether the sorting order is ascending or descending.
    /// Consists of two properties where:
    /// </para>
    /// <para>
    /// - <b>PropertyExpression</b> represents the property of model object that will be used as a variable for ordering, if IsAscending is not null.
    /// </para>
    /// <para>
    /// - <b>IsAscending</b> represents whether the property is used for sorting and whether the sorting order is ascending or descending.
    /// If it is null, the property will not be ordered at all and will be ignored.
    /// </para>
    /// </summary>
    /// <typeparam name="TEntity">Domain model type.</typeparam>
    public class SortPropertyExpression<TEntity> where TEntity : class
    {
        private Expression<Func<TEntity, object?>> _propertyExpression;
        private bool? _ascending;

        /// <summary>
        /// Represents the property of model object that will be used as a variable for ordering, if IsAscending is not null.
        /// </summary>
        public Expression<Func<TEntity, object?>> PropertyExpression => _propertyExpression;
        /// <summary>
        /// If:
        /// <b>true</b> - ascending; <b>false</b> - descending; <b>null</b> - will not be used as a variable ordering.
        /// If it is null, the property will not be used for ordering at all and will be ignored.
        /// </summary>
        public bool? IsAscending => _ascending;

        public SortPropertyExpression(Expression<Func<TEntity, object?>> propertyExpression, bool? ascending)
        {
            _propertyExpression = propertyExpression;
            _ascending = ascending;
        }
    }
}
