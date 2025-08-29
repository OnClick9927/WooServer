using System.Linq.Expressions;
using System.Reflection;

namespace WS.Core.Tool;

public static class MiscTools
{
    public static T Set<T, TValue>(this T obj, Expression<Func<T, TValue>> expression, TValue value)
    {
        var memberExpression = expression.Body as MemberExpression;
        if (memberExpression == null)
            throw new ArgumentException("表达式必须指向属性或字段");

        var member = memberExpression.Member;
        if (member is PropertyInfo property)
        {
            property.SetValue(obj, value);
        }
        else if (member is FieldInfo field)
        {
            field.SetValue(obj, value);
        }
        else
        {
            throw new ArgumentException("表达式必须指向属性或字段");
        }

        return obj;
    }
    public static void Shuffle<T>(this T[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = Random.Shared.Next(i + 1);
            (array[i], array[j]) = (array[j], array[i]);
        }
    }
    public static T? As<T>(this object obj)where T : class
    {
        if (obj == null) return default;
        return obj as T;
    }

}
