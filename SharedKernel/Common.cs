using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel
{
    public class AvailableTypeFinder : TypeFinder
    {
        //private static readonly Lazy<bool> assembliesLoaded = new Lazy<bool>(ForceLoadAssemblies);

        private readonly Func<Assembly, bool> assemblySelector;

        public AvailableTypeFinder()
            : this(a => true)
        {
        }

        public AvailableTypeFinder(Func<Assembly, bool> assemblySelector)
        {
            this.assemblySelector = assemblySelector;
        }

        public override IEnumerable<Type> GetAllTypes()
        {
            //if (assembliesLoaded.Value)
            //{
            return from assembly in AppDomain.CurrentDomain.GetAssemblies()
                   where !assembly.IsDynamic && assemblySelector(assembly)
                   from type in assembly.GetExportedTypes()

                   select type;

            //}
            // Note: only done to satisfy compiler, will never get here
            //return Enumerable.Empty<Type>();
        }

        public virtual IEnumerable<Type> GetConcreteTypes(Func<Type, bool> typeSelector)
        {
            return from type in GetAllTypes()
                   where !type.IsAbstract && !type.IsInterface && typeSelector(type)
                   select type;
        }

        private static bool ForceLoadAssemblies()
        {
            //foreach (var fileName in Directory.GetFiles(AppDomain.CurrentDomain.RelativeSearchPath, "*.dll"))
            //{
            //    string assemblyName = Path.GetFileNameWithoutExtension(fileName);
            //    Assembly.Load(assemblyName);
            //}

            return true;
        }

    }

    public abstract class TypeFinder
    {
        public abstract IEnumerable<Type> GetAllTypes();

        public virtual IEnumerable<Type> GetConcreteTypes()
        {
            return from type in GetAllTypes()
                   where !type.IsAbstract && !type.IsInterface
                   select type;
        }
    }

    /// <summary>
    /// Indicates that the assembly should be considered a named module using the
    /// provided name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public sealed class ModuleAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ModuleAttribute"/> class using the
        /// provided module type.
        /// </summary>
        /// <param name="name">The name of the module.</param>
        public ModuleAttribute(Type moduleInitType)
        {
            this.ModuleInitType = moduleInitType;
        }

        /// <summary>
        /// The name of the module.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Type of Module Init Class
        /// </summary>
        public Type ModuleInitType { get; set; }

        /// <summary>
        ///     Module GUID Identifier
        /// </summary>
        public Guid Identifier { get; set; }
    }

    /// <summary>
    ///     Set priority
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    public sealed class PriorityAttribute : Attribute
    {
        public PriorityAttribute()
            : this(-1)
        {
        }

        public PriorityAttribute(int priority)
        {
            Priority = priority;
        }

        /// <summary>
        ///     Get Or Set Priority
        /// </summary>
        public int Priority { get; set; }
    }

    internal class ByPriorityComparison<T> : IComparer<T>
    {
        #region IComparer<Type> Members

        public int Compare(T a, T b)
        {

            Type x = (typeof(T) == typeof(Type)) ? a as Type : a.GetType();
            Type y = (typeof(T) == typeof(Type)) ? b as Type : b.GetType();

            int xPriority = Priority(x);
            int yPriority = Priority(y);

            if (xPriority == yPriority)
            {
                return 0;
            }

            if (xPriority == -1)
            {
                return 1;
            }

            if (yPriority == -1)
            {
                return -1;
            }

            return xPriority - yPriority;
        }

        #endregion

        private static int Priority(Type t)
        {
            var attr = t.GetCustomAttributes(typeof(PriorityAttribute), false)
                .Cast<PriorityAttribute>()
                .FirstOrDefault();

            if (attr == null)
            {
                return -1;
            }

            return attr.Priority;
        }
    }
}
