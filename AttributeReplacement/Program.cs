using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace AttributeReplacement
{
    public enum TestTypeEnum
    {
        ListType,
        ListScope
    }

    public static class TestTypeEnumExtensions
    {
        public static string GetDisplayItemMetadata(this TestTypeEnum type, string propertyName) 
        {
            var metadata = new Dictionary<string, string>()
            {
                { "ListType", "XYZ01" },
                { "ListScope", "XYZ02" }
            };

            return metadata.ContainsKey(propertyName) ? metadata[propertyName] : null;
        }
    }

    public class TestTypeClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? Date { get; set; }
    }

    public static class TestTypeClassExtensions
    {
        public static string GetDisplayItemMetadata(this TestTypeClass type, string propertyName)
        {
            var metadata = new Dictionary<string, string>()
            {
                { "Name", "ABC01" },
                { "Date", "ABC02" }
            };

            return metadata.ContainsKey(propertyName) ? metadata[propertyName] : null;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            ExecuteExtensionMethod(TestTypeEnum.ListType, typeof(TestTypeEnum));
            ExecuteExtensionMethod(new TestTypeClass(), typeof(TestTypeClass));

            Console.ReadLine();
        }

        static void ExecuteExtensionMethod(object @instance, Type @type)
        {
            Console.WriteLine("Type: " + @type.ToString());

            var method = GetExtensionMethods(@type, "GetDisplayItemMetadata");

            var fieldsNames = @type.IsEnum ? @type.GetEnumNames() : @type.GetProperties().Select(x => x.Name);

            foreach (var name in fieldsNames)
            {
                var result = method.Invoke(null, new object[] { @instance, name });

                if(result != null)
                    Console.WriteLine(name + " : " + result);
            }

            Console.WriteLine("");
        }

        static MethodInfo GetExtensionMethods(Type extendedType, string methodName)
        {
            var query = from type in extendedType.Assembly.GetTypes()
                        where type.IsSealed && !type.IsGenericType && !type.IsNested
                        from method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                        where method.IsDefined(typeof(ExtensionAttribute), false)
                        where method.GetParameters()[0].ParameterType == extendedType
                        where method.Name == methodName
                        select method;

            return query.First();
        }
    }
}
