using System.IO;
using System.Reflection;

public static class EmbeddedResource
{
    public static string Read(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var fullResourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(n => n.EndsWith(resourceName));
        
        if (fullResourceName == null)
        {
            // If not found as embedded resource, try to read from file system
            var currentDirectory = Directory.GetCurrentDirectory();
            var filePath = Path.Combine(currentDirectory, resourceName);
            
            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath);
            }
            
            throw new FileNotFoundException($"Resource '{resourceName}' not found as embedded resource or file.");
        }

        using (var stream = assembly.GetManifestResourceStream(fullResourceName))
        using (var reader = new StreamReader(stream))
        {
            return reader.ReadToEnd();
        }
    }
}