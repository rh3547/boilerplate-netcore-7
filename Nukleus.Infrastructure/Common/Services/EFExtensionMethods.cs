using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Nukleus.Infrastructure.Common.Services
{
    internal static class EFExtensionMethods
    {
        public static IQueryable<T> IncludeMultiple<T>(this DbSet<T> dbSet, params Expression<Func<T, object>>[] includeProperties) where T : class
        {
            IQueryable<T> query = dbSet;

            if (includeProperties == null) return query;

            // Loop through each include property
            foreach (var includeProperty in includeProperties)
            {
                // Include the property in the query
                query = query.Include(includeProperty);
            }

            return query;
        }

        public static IQueryable<T> IncludeMultiple<T>(this DbSet<T> dbSet, string[] includeProperties) where T : class
        {
            IQueryable<T> query = dbSet;

            if (includeProperties == null) return query;

            // Loop through each include property
            foreach (var includeProperty in includeProperties)
            {
                // Include the property in the query
                query = IncludePropertyPath(query, includeProperty);
            }

            return query;
        }

        public static IQueryable<T> IncludeMultiple<T>(this IQueryable<T> query, string[] includeProperties) where T : class
        {
            if (includeProperties == null) return query;

            // Loop through each include property
            foreach (var includeProperty in includeProperties)
            {
                // Include the property in the query
                query = IncludePropertyPath(query, includeProperty);
            }

            return query;
        }

        public static IQueryable<T> AddPaging<T>(this IQueryable<T> query, int pageSize, int page) where T : class
        {
            int skip = (int)((page - 1) * pageSize);
            return query.Skip(skip).Take((int)pageSize);
        }

        private static IQueryable<T> IncludePropertyPath<T>(IQueryable<T> query, string propertyPath) where T : class
        {
            var pathParts = propertyPath.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            var path = string.Empty;

            for (var i = 0; i < pathParts.Length; i++)
            {
                if (i > 0)
                {
                    path += ".";
                }
                path += pathParts[i].Substring(0, 1).ToUpper() + pathParts[i].Substring(1);
                query = query.Include(path);
            }

            return query;
        }
    }
}
