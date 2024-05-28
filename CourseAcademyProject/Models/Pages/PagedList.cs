using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;

namespace CourseAcademyProject.Models.Pages
{
    public class PagedList<T> : List<T>
    {
        public PagedList(IQueryable<T> query, QueryOptions options = null)
        {
            CurrentPage = options.CurrentPage;
            PageSize = options.PageSize;
            Options = options;

            if (options != null)
            {
                if (options.CategoryId.HasValue)
                {
                    query = FilterByProperties(query, nameof(options.CategoryId), options.CategoryId);
                }
                if (options.DifficultyLevel.HasValue)
                {
                    query = FilterByProperties(query, nameof(options.DifficultyLevel), options.DifficultyLevel);
                }
                if (options.Price.HasValue)
                {
                    query = Order(query, nameof(options.Price), options.Price ?? true);
                }
                if (options.DateOfPublication.HasValue)
                {
                    query = Order(query, nameof(options.DateOfPublication), options.DateOfPublication ?? true);
                }

                if (!string.IsNullOrEmpty(options.OrderPropertyName))
                {
                    query = Order(query, options.OrderPropertyName, options.DescendingOrder);
                }
                if (!string.IsNullOrEmpty(options.SearchPropertyName) && !string.IsNullOrEmpty(options.SearchTerm))
                {
                    query = Search(query, options.SearchPropertyName, options.SearchTerm);
                }
            }

            int queryCount = query.Count();

            TotalPages = queryCount / PageSize;
            if (queryCount % PageSize > 0)
            {
                TotalPages += 1;
            }

            AddRange(query.Skip((CurrentPage - 1) * PageSize).Take(PageSize));
        }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public QueryOptions Options { get; set; }

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;

        private static IQueryable<T> Search(IQueryable<T> query, string propertyName, string searchTerm)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var source = propertyName.Split('.').Aggregate((Expression)parameter, Expression.Property);
            var body = Expression.Call(source, "Contains", Type.EmptyTypes, Expression.Constant(searchTerm, typeof(string)));
            var lambda = Expression.Lambda<Func<T, bool>>(body, parameter);
            return query.Where(lambda);
        }
        private static IQueryable<T> Order(IQueryable<T> query, string propertyName, bool desc)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var source = propertyName.Split('.').Aggregate((Expression)parameter, Expression.Property);
            var lambda = Expression.Lambda(typeof(Func<,>).MakeGenericType(typeof(T), source.Type), source, parameter);
            return typeof(Queryable).GetMethods().Single(e => e.Name == (desc ? "OrderByDescending" : "OrderBy") &&
            e.IsGenericMethodDefinition &&
            e.GetGenericArguments().Length == 2 &&
            e.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(T), source.Type)
            .Invoke(null, new object[] { query, lambda }) as IQueryable<T>;
        }

        private static IQueryable<T> FilterByProperties<T, TValue>(IQueryable<T> query, string propertyName, TValue value)
        {
            var parameter = Expression.Parameter(typeof(T), "x");

            var property = Expression.Property(parameter, propertyName);
            var getedValue = Expression.Constant(value);
            var convertedValue = Expression.Convert(getedValue, property.Type);
            var filter = Expression.Equal(property, convertedValue);

            var lambda = Expression.Lambda<Func<T, bool>>(filter, parameter);
            return query.Where(lambda);
        }
    }
}
