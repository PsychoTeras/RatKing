using System;
using System.IO;
using System.Reflection;
using GMechanics.Core.Classes.Interfaces;
using GMechanics.Core.Classes.Storages;

namespace GMechanics.Core.Helpers
{
    public static class Helper
    {
        public delegate bool IsAttributeNameExistsCheckHandler(string className);

        public static readonly string ApplicationPath;
        public static readonly Assembly CallingAssembly;

        static Helper()
        {
            CallingAssembly = Assembly.GetCallingAssembly();
            ApplicationPath = Path.GetDirectoryName(Assembly.GetAssembly(typeof (Helper)).Location);
        }

        public static uint HashX65599(string str)
        {
            uint hash = 0;
            int length = str.Length;
            for (int i = 0; i < length; i++)
            {
                hash = 65599 * hash + str[i];
            }
            return hash ^ (hash >> 16);
        }

        //!!! Horrible realization
        public static void RemoveDestroyedItemsFromListsOwner(object owner)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | 
                                       BindingFlags.ExactBinding;
            PropertyInfo[] properties = owner.GetType().GetProperties(flags);

            foreach (PropertyInfo propertyInfo in properties)
            {
                if (propertyInfo.ReflectedType != null &&
                    propertyInfo.ReflectedType.IsClass &&
                    propertyInfo.PropertyType.Assembly.Equals(CallingAssembly) &&
                    propertyInfo.Name != "ClassAsAtom" &&
                    propertyInfo.Name != "Parent")
                {
                    object property = propertyInfo.GetValue(owner, null);
                    if (property != null)
                    {
                        if (property is IBaseContainer || property is IBaseList)
                        {
                            RemoveDestroyedItemsFromList(property);
                        }
                        else
                        {
                            RemoveDestroyedItemsFromListsOwner(property);
                        }
                    }
                }
            }
        }

        public static void RemoveDestroyedItemsFromList(object container)
        {
            if (container != null && (container is IBaseContainer || container is IBaseList))
            {
                Type cType = container.GetType();
                MethodInfo mRemoveDestroyedItems = cType.GetMethod("RemoveDestroyedItems");
                if (mRemoveDestroyedItems != null)
                {
                    mRemoveDestroyedItems.Invoke(container, null);
                }
            }
        }

        public static bool IsGameObjectAttributeNameExists(string attributeName)
        {
            return GlobalStorage.Instance.GameObjectAttributes.ContainsKey(attributeName);
        }
    }
}