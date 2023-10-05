using Nukleus.Application.Common.Persistence;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Nukleus.Infrastructure.Common.Services
{
    public static class PredicateBuilder
    {
        public static Expression<Func<T, bool>> MapPredicate<T, TDto>(TDto dto)
        {
            var entityType = typeof(T);
            var dtoType = typeof(TDto);
            var entityParameter = Expression.Parameter(entityType, "entity");

            Expression predicate = null;

            foreach (var dtoProperty in dtoType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var entityProperty = entityType.GetProperty(dtoProperty.Name);
                if (entityProperty == null)
                    continue;

                var fieldFilters = (List<FieldFilter>?)dtoProperty.GetValue(dto);
                if (fieldFilters == null)
                    continue;

                foreach (var fieldFilter in fieldFilters)
                {
                    if (fieldFilter.Value == null)
                        continue;

                    var combineMethod = fieldFilter.Operator ?? "and";
                    var comparisonMethod = fieldFilter.Comparator ?? "=";

                    Expression entityPropertyAccess = Expression.Property(entityParameter, entityProperty);

                    var value = fieldFilter.Value;
                    if (value is JsonElement jsonElement)
                    {
                        value = ConvertJsonElement(jsonElement, entityProperty.PropertyType);
                    }

                    var constant = Expression.Constant(value);

                    Expression comparisonExpression;

                    bool isCaseSensitive = fieldFilter.IsCaseSensitive ?? false;

                    if (entityProperty.PropertyType == typeof(string) && !isCaseSensitive)
                    {
                        entityPropertyAccess = Expression.Call(entityPropertyAccess, "ToLower", null);
                        if (value is string stringValue)
                        {
                            value = stringValue.ToLower();
                            constant = Expression.Constant(value);
                        }
                    }

                    if (entityProperty.PropertyType == typeof(string))
                    {
                        switch (comparisonMethod.ToLower())
                        {
                            case "contains":
                                var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                                comparisonExpression = Expression.Call(entityPropertyAccess, containsMethod, constant);
                                break;
                            case "regex":
                                var regexIsMatchMethod = typeof(Regex).GetMethod("IsMatch", new[] { typeof(string), typeof(string) });
                                comparisonExpression = Expression.Call(regexIsMatchMethod, entityPropertyAccess, constant);
                                break;
                            case "=":
                            default:
                                comparisonExpression = Expression.Equal(entityPropertyAccess, constant);
                                break;
                        }
                    }
                    else if (IsNumericType(entityProperty.PropertyType))
                    {
                        switch (comparisonMethod)
                        {
                            case "<":
                                comparisonExpression = Expression.LessThan(entityPropertyAccess, constant);
                                break;
                            case ">":
                                comparisonExpression = Expression.GreaterThan(entityPropertyAccess, constant);
                                break;
                            case "<=":
                                comparisonExpression = Expression.LessThanOrEqual(entityPropertyAccess, constant);
                                break;
                            case ">=":
                                comparisonExpression = Expression.GreaterThanOrEqual(entityPropertyAccess, constant);
                                break;
                            case "=":
                            default:
                                comparisonExpression = Expression.Equal(entityPropertyAccess, constant);
                                break;
                        }
                    }
                    else
                    {
                        comparisonExpression = Expression.Equal(entityPropertyAccess, constant);
                    }

                    if (predicate == null)
                    {
                        predicate = comparisonExpression;
                    }
                    else
                    {
                        switch (combineMethod)
                        {
                            case "or":
                            case "||":
                                predicate = Expression.OrElse(predicate, comparisonExpression);
                                break;
                            case "and":
                            case "&&":
                            default:
                                predicate = Expression.AndAlso(predicate, comparisonExpression);
                                break;
                        }
                    }
                }
            }

            if (predicate == null)
            {
                return e => true;
            }

            return Expression.Lambda<Func<T, bool>>(predicate, entityParameter);
        }

        private static bool IsNumericType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                default:
                    return false;
            }
        }

        private static object ConvertJsonElement(JsonElement jsonElement, Type targetType)
        {
            return JsonSerializer.Deserialize(jsonElement.GetRawText(), targetType);
        }
    }
}