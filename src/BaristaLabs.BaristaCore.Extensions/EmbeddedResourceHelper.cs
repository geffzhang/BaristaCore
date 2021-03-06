﻿namespace BaristaLabs.BaristaCore
{
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    internal static class EmbeddedResourceHelper
    {
        public static async Task<string> LoadResourceAsync<T>(T type, string name)
        {
            var assembly = typeof(T).Assembly;
            var resourceStream = assembly.GetManifestResourceStream(name);
            using (var reader = new StreamReader(resourceStream, Encoding.UTF8))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public static string LoadResource(string name, Assembly assembly = null)
        {
            if (assembly == null)
                assembly = Assembly.GetAssembly(typeof(EmbeddedResourceHelper));

            var resourceStream = assembly.GetManifestResourceStream(name);
            using (var reader = new StreamReader(resourceStream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
