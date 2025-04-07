using System.Text.Json;

namespace APDP_ASM2.Helpers
{
    public static class FileHelper
    {
        public static T? LoadFromFile<T>(string fileName)
        {
            if (!System.IO.File.Exists(fileName)) return default;
            var readText = System.IO.File.ReadAllText(fileName);
            return JsonSerializer.Deserialize<T>(readText);
        }

        public static void SaveToFile<T>(string fileName, T data)
        {
            var jsonString = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(fileName, jsonString);
        }

        public static int GetNextId<T>(List<T> items) where T : class
        {
            // Assuming all classes have an Id property of type int
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null) throw new InvalidOperationException("Class does not have an Id property");

            int maxId = items.Select(item => (int)idProperty.GetValue(item)).Max();
            return maxId + 1;
        }
    }
}
