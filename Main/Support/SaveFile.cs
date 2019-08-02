using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Support
{
    public static class SaveFile
    {
        public static async void ProcessWrite(string text, string name)
        {
            string filePath =  $"{Directory.GetCurrentDirectory()}{name}.txt";

            await WriteTextAsync(filePath, text);
        }

        public static  async Task WriteTextAsync(string filePath, string text)
        {
            byte[] encodedText = Encoding.Unicode.GetBytes(text);

            if (File.Exists(filePath))
                File.Delete(filePath);

            using (FileStream sourceStream = new FileStream(filePath,
                FileMode.Append, FileAccess.Write, FileShare.None,
                bufferSize: 4096, useAsync: true))
            {
                await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
            };
        }
    }
}
