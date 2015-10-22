using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace SharedKernel
{

    public static class Extensions
    {
        public static string ToReadableHourString(this TimeSpan span)
        {
            string format = string.Empty;


            if (span.Hours != 0)
            {
                format = string.Format("{0:D2} h, {1:D2} m, {2:D2}.{3:D2} s",
                    span.Hours, span.Minutes, span.Seconds, span.Milliseconds);
            }
            else
            {
                format = string.Format("{0:D2} m, {1:D2}.{2:D2} s",
                     span.Minutes, span.Seconds, span.Milliseconds);
            }


            return format;

        }

        public static long ToLong(this object value, long defaultValue = 0)
        {
            if (value == null)
                return defaultValue;

            try
            {
                return Convert.ToInt64(value);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static int ToInt(this string value, int defaultValue)
        {
            if (string.IsNullOrEmpty(value))
                return defaultValue;

            try
            {
                return Convert.ToInt32(value);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static string GetReadRepositoryName(this CoreObject item)
        {

            try
            {
                return item.GetType().Namespace.Split('.')[0] + "RepositoryRead";
            }
            catch
            {
                return string.Empty;
            }

        }

        public static string GetRepositoryName(this IQueryMessage item)
        {

            try
            {
                return item.GetType().Namespace.Split('.')[0] + "RepositoryRead";
            }
            catch
            {
                return string.Empty;
            }

        }

        public static string GetWriteRepositoryName(this CoreObject item) {

            try
            {
                return item.GetType().Namespace.Split('.')[0] + "RepositoryWrite";
            }
            catch
            {
                return string.Empty;
            }

        }

        public static MethodInfo InjectTypeToGenericMethod(object instance, string methodName, Type type)
        {

            // get the type and the method

            Type t = instance.GetType();

            MethodInfo mi = t.GetMethod(methodName);

            if (mi == null)
                mi = t.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);

            Type[] argTypes = { type };

            MethodInfo result = mi.MakeGenericMethod(argTypes);

            return result;

        }

        public static Result ToResult<TData>(this Result<TData> item)
        {
            Result r = new Result();

            r.Data = item.Data;
            r.Message = item.Message;
            r.ObjectId = item.ObjectId;
            r.Success = item.Success;
            r.ValidationResults = item.ValidationResults;

            return r;
        }

     

        public static Result StopwatchWraper(this ICommandMessage input, Func<ICommandMessage, Result> action)
        {
            Stopwatch sw = new Stopwatch();

            sw.Start();

            Result output = action.Invoke(input);

            sw.Stop();

            if (output != null)
            {
                output.ExecuteTime = sw.Elapsed.ToReadableHourString();
            }

            return output;
        }
    }
}

namespace System
{

    public static class TypeExtension
    {

        public static bool Implements<TItf>(this Type t)
        {
            return t.Implements(typeof(TItf));
        }

        public static bool Implements(this Type t, Type interfaceType)
        {
            return t.GetInterfaces().Any(itf => itf == interfaceType || (itf.IsGenericType && itf.GetGenericTypeDefinition() == interfaceType));
        }

        public static bool DerivesFrom<T>(this Type t)
        {
            return t.DerivesFrom(typeof(T));
        }

        public static bool DerivesFrom(this Type t, Type baseType)
        {
            return baseType.IsAssignableFrom(t);
        }

        public static bool InheritsFrom<T>(this Type t)
        {
            return t.InheritsFrom(typeof(T));
        }

        /// <summary>
        /// Extension method to check the entire inheritance hierarchy of a
        /// type to see whether the given base type is inherited.
        /// </summary>
        /// <param name="t">The Type object this method was called on</param>
        /// <param name="baseType">The base type to look for in the 
        /// inheritance hierarchy</param>
        /// <returns>True if baseType is found somewhere in the inheritance 
        /// hierarchy, false if not</returns>
        public static bool InheritsFrom(this Type t, Type baseType)
        {
            if (t == baseType)
                return true;

            if (baseType.IsInterface)
            {
                var items = t.GetInterfaces();

                if (items != null && items.Contains(baseType))
                    return true;
            }

            Type cur = t.BaseType;

            while (cur != null)
            {
                if (baseType.IsGenericType && cur.IsGenericType)
                {
                    if (cur.GetGenericTypeDefinition() == baseType)
                        return true;
                }

                if (baseType.IsInterface)
                {
                    var items = cur.GetInterfaces();

                    if (items != null && items.Contains(baseType))
                        return true;
                }

                if (cur.Equals(baseType))
                {
                    return true;
                }

                cur = cur.BaseType;
            }

            return false;
        }
    }
}

namespace System.Collections.Generic
{
    public static class ListExtension
    {
        public static void SortByPriority<T>(this List<T> value)
        {
            value.Sort(new SharedKernel.ByPriorityComparison<T>());
        }
    }
}

namespace System.Reflection
{
    public static class AssemblyExtension
    {
        public static T GetAttribute<T>(this Assembly assembly) where T : Attribute
        {
            try
            {
                object[] items = assembly.GetCustomAttributes(typeof(T), true);


                if (items != null && items.Count() > 0)
                    return (T)items[0];
                else
                    return null;
            }
            catch (Exception)
            {
                

                return null;
            }
        }
    }
}

namespace Microsoft.Practices.ServiceLocation
{
    public static class ServiceLocationExtension
    {
        public static object TryGet(this IServiceLocator locator, Type type, string key = null)
        {
            try
            {
                if (locator == null)
                    return null;

                if (string.IsNullOrWhiteSpace(key))
                    return locator.GetInstance(type);
                else
                    return locator.GetInstance(type, key);
            }
            catch (Exception)
            {
                 
                return null;
            }
        }

        public static T TryGet<T>(this IServiceLocator locator, string key = null)
        {
            try
            {
               
                if (locator == null)
                    return default(T);

                if (string.IsNullOrWhiteSpace(key))
                    return locator.GetInstance<T>();
                else
                    return locator.GetInstance<T>(key);
            }
            catch (Exception)
            {
                 
                return default(T);
            }
        }

        public static IEnumerable<object> TryGetAll(this IServiceLocator locator, Type type)
        {
            try
            {
                if (locator == null)
                    return Enumerable.Empty<object>();

                return locator.GetAllInstances(type);
            }
            catch (Exception)
            {
                 
                return Enumerable.Empty<object>();
            }
        }


        public static IEnumerable<T> TryGetAll<T>(this IServiceLocator locator)
        {
            try
            {
                if (locator == null)
                    return Enumerable.Empty<T>();

                return locator.GetAllInstances<T>();
            }
            catch (Exception)
            {
                 
                return Enumerable.Empty<T>();
            }
        }
    }
}
