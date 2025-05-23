﻿using System.Linq.Expressions;
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

}
public static class TypeTools
{
    public static Delegate ToDelegate(this MethodInfo method, object target)
    {
        var _params = method.GetParameters();
        Type delegateType = default;
        var void_func = method.ReturnType == typeof(void);

        Type base_func_type = void_func ? typeof(Action) : typeof(Func<>);
        if (void_func)
        {
            if (_params == null || _params.Length == 0)
                delegateType = typeof(Action);
            else
            {
                if (_params.Length == 1) base_func_type = typeof(Action<>);
                else if (_params.Length == 2) base_func_type = typeof(Action<,>);
                else if (_params.Length == 3) base_func_type = typeof(Action<,,>);
                else if (_params.Length == 4) base_func_type = typeof(Action<,,,>);
                else if (_params.Length == 5) base_func_type = typeof(Action<,,,,>);
                else if (_params.Length == 6) base_func_type = typeof(Action<,,,,,>);
                else if (_params.Length == 7) base_func_type = typeof(Action<,,,,,,>);
                else if (_params.Length == 8) base_func_type = typeof(Action<,,,,,,,>);
                else if (_params.Length == 9) base_func_type = typeof(Action<,,,,,,,,>);
                else if (_params.Length == 10) base_func_type = typeof(Action<,,,,,,,,,>);
                else if (_params.Length == 11) base_func_type = typeof(Action<,,,,,,,,,,>);
                else if (_params.Length == 12) base_func_type = typeof(Action<,,,,,,,,,,,>);
                else if (_params.Length == 13) base_func_type = typeof(Action<,,,,,,,,,,,,>);
                else if (_params.Length == 14) base_func_type = typeof(Action<,,,,,,,,,,,,,>);
                else if (_params.Length == 15) base_func_type = typeof(Action<,,,,,,,,,,,,,,>);
                else if (_params.Length == 16) base_func_type = typeof(Action<,,,,,,,,,,,,,,,>);
                delegateType = base_func_type
                                .MakeGenericType(_params
                                        .Select(x => x.ParameterType)
                                        .ToArray());

            }
        }
        else
        {

            if (_params == null || _params.Length == 0)
            {
                delegateType = base_func_type.MakeGenericType(new Type[] { method.ReturnType });
            }
            else
            {
                if (_params.Length == 1) base_func_type = typeof(Func<,>);
                else if (_params.Length == 2) base_func_type = typeof(Func<,,>);
                else if (_params.Length == 3) base_func_type = typeof(Func<,,,>);
                else if (_params.Length == 4) base_func_type = typeof(Func<,,,,>);
                else if (_params.Length == 5) base_func_type = typeof(Func<,,,,,>);
                else if (_params.Length == 6) base_func_type = typeof(Func<,,,,,,>);
                else if (_params.Length == 7) base_func_type = typeof(Func<,,,,,,,>);
                else if (_params.Length == 8) base_func_type = typeof(Func<,,,,,,,,>);
                else if (_params.Length == 9) base_func_type = typeof(Func<,,,,,,,,,>);
                else if (_params.Length == 10) base_func_type = typeof(Func<,,,,,,,,,,>);
                else if (_params.Length == 11) base_func_type = typeof(Func<,,,,,,,,,,,>);
                else if (_params.Length == 12) base_func_type = typeof(Func<,,,,,,,,,,,,>);
                else if (_params.Length == 13) base_func_type = typeof(Func<,,,,,,,,,,,,,>);
                else if (_params.Length == 14) base_func_type = typeof(Func<,,,,,,,,,,,,,,>);
                else if (_params.Length == 15) base_func_type = typeof(Func<,,,,,,,,,,,,,,,>);
                else if (_params.Length == 16) base_func_type = typeof(Func<,,,,,,,,,,,,,,,,>);
                delegateType = base_func_type
                                .MakeGenericType(_params
                                        .Select(x => x.ParameterType)
                                        .Concat(new Type[] { method.ReturnType })
                                        .ToArray());

            }

        }
        return method.CreateDelegate(delegateType, target);
    }
    public static T? CreateInstance<T>(this Type? type)
    {
        if (type == null) return default;
        return (T)Activator.CreateInstance(type);
    }
    private static IEnumerable<Type> AllTypes { get; set; }

    static List<Assembly> GetReferanceAssemblies(this AppDomain domain)
    {
        var src = domain.GetAssemblies();
        var list = new List<Assembly>(src);
        src.ToList().ForEach(i =>
        {
            GetReferanceAssemblies(i, list);
        });
        return list;
    }
    static void GetReferanceAssemblies(Assembly assembly, List<Assembly> list)
    {
        assembly.GetReferencedAssemblies().ToList().ForEach(i =>
        {
            try
            {
                var ass = Assembly.Load(i);
                if (!list.Contains(ass))
                {
                    list.Add(ass);
                    GetReferanceAssemblies(ass, list);
                }
            }
            catch (Exception)
            {

            }
     
        });
    }
    public static IEnumerable<Type> GetTypes()
    {
        if (AllTypes == null)
        {
            AllTypes = AppDomain.CurrentDomain.GetReferanceAssemblies()
               .SelectMany(x => x.GetTypes());
        }
        return AllTypes;
    }
    public static IEnumerable<Type> GetSubTypes(this Type type)
    {

        if (type.IsInterface)
            return GetTypes().Where(x => !x.IsAbstract && x.GetInterfaces().Contains(type));
        return GetTypes().Where(x => !x.IsAbstract && x.IsSubclassOf(type));

    }
    public static IEnumerable<Type> GetTypesWithAttribute(Type attributeType, bool inherit)
    {
        return GetTypes()
                  .Where(x => x.IsDefined(attributeType, inherit));

    }


}
