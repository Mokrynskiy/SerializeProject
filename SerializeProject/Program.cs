using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

namespace SerializeProject
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var f = new F().Get();
            Stopwatch sw = new Stopwatch();
            string serializedCustom = "";
            string serializedJson = "";
            int amountIterations = 1000;

            // сериализация свой метод
            sw.Start();
            for (int i = 0; i < amountIterations; i++) 
            {
                serializedCustom = CustomSerializer.Serialize(f);
            }
            sw.Stop();
            Console.WriteLine(sw.Elapsed.TotalMilliseconds);

            // десериализация свой метод
            sw.Start();
            for (int i = 0; i < amountIterations; i++)
            {
                var deserializedObject = (F)CustomSerializer.Deserialize<F>(serializedCustom);
            }
            sw.Stop();
            Console.WriteLine(sw.Elapsed.TotalMilliseconds);

            // сериализация Newtonsoft.Json
            sw.Start();
            for (int i = 0; i < amountIterations; i++)
            {
                serializedJson = JsonConvert.SerializeObject(f);
            }            
            sw.Stop();
            Console.WriteLine(sw.Elapsed.TotalMilliseconds);

            // сериализация Newtonsoft.Json
            sw.Start();
            for (int i = 0; i < amountIterations; i++)
            {
                var deserializedObject = JsonConvert.DeserializeObject<F>(serializedJson);
            }            
            sw.Stop();
            Console.WriteLine(sw.Elapsed.TotalMilliseconds);

        }

        class F
        {
            public int i1, i2, i3, i4, i5;
            public F Get() => new (){ i1 = 1, i2 = 2, i3 = 3, i4 = 4, i5 = 5};           
        }


        static class CustomSerializer
        {
            public static string Serialize (object obj)
            {
                StringBuilder sb = new StringBuilder ();
                Type t = obj.GetType();                
                foreach (var field in t.GetFields())
                {
                    sb.Append($"[{field.Name}:{field.GetValue(obj)}]\n");
                }
                sb.Remove(sb.Length - 1, 1);
                return sb.ToString();
            }

            public static object Deserialize<T> (string serializedString) where T : class, new ()
            {                
                var obj = new T ();
                var type = obj.GetType();
                var arey = serializedString.Split("\n");
                foreach (var item in arey)
                {
                    var serializedFild = item.Trim('[').Trim(']').Split(":");
                    var fild = type.GetField(serializedFild[0]);
                    if (fild == null) continue;
                    fild.SetValue(obj, ConvertValue(fild.FieldType, serializedFild[1]));
                }
                return obj;           
            }

            public static object ConvertValue (Type type, string value)
            {                
                switch (type.Name)
                {
                    case "Int32":
                        return int.Parse(value);                        
                    default:
                        return 0;

                }
            }
        }
    }
}