using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel
{
    public class Mapper
    {

        public static T Map<T>(string typeName, dynamic data)
        {
            Type type = Type.GetType(typeName);

            object result = null;

            if (type == null)
                result = DynamicToStatic.ToStaticFromIntance(ServiceLocator.Current.TryGet<T>(typeName), data);
            else
                result = DynamicToStatic.ToStaticFromType(type, data);

            if (result is T)
                return (T)result;
            else
                return default(T);
        }

        public static T Map<T, K>(T instance, K data)
        {
            return Map<T>(instance, data);
        }

        public static T Map<T, K>(K data)
        {
            T instance = default(T);

            return Map<T>(instance, data);
        }

        public static T Map<T>(object data)
        {
            T instance = default(T);

            return Map<T>(instance, data);
        }

        public static T Map<T>(T instance, object data)
        {
            if (instance == null)
                instance = (T)Activator.CreateInstance(typeof(T), true);

            var properties = data.GetType().GetProperties();

            if (properties == null)
                return instance;

            foreach (var prop in properties)
            {
                var propertyInfo = instance.GetType().GetProperty(prop.Name);

                if (propertyInfo != null)
                    propertyInfo.SetValue(instance, prop.GetValue(data), null);
            }

            return instance;
        }
    }

    public static class DynamicToStatic
    {
        public static object ToStaticFromType(Type type, object expando)
        {
           var instance = Activator.CreateInstance(type, true);

           return DynamicToStatic.ToStaticFromIntance(instance, expando);
        }

        public static object ToStaticFromIntance(object instance, object expando)
        {
            if (instance == null)
                return null;

            //ExpandoObject implements dictionary
            var properties = expando as IDictionary<string, object>;

            if (properties == null)
                return instance;

            foreach (var entry in properties)
            {
                var propertyInfo = instance.GetType().GetProperty(entry.Key);
                if (propertyInfo != null)
                    propertyInfo.SetValue(instance, entry.Value, null);
            }
            return instance;
        }
    }
}
