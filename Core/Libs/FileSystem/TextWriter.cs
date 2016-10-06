using System;
using System.IO;
using System.Text;

namespace Core.Libs.FileSystem
{
    public class TextWriter
    {
        /// <summary>
        /// Append text at the end of a text file
        /// </summary>
        /// <param name="content"></param>
        /// <param name="dir"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool AppendToTextFile(string content, string dir, string fileName)
        {
            try
            {
                var filePath = Path.Combine(dir, fileName);
                if (dir != null && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                if (!File.Exists(filePath))
                {
                    var fs = File.Create(filePath);
                    fs.Close();
                }

                var sw = File.AppendText(filePath);
                sw.WriteLine(content);
                sw.Close();
            }
            catch (Exception)
            {
                return false;
            }
            
            return true;
        }

        public static bool ReplaceContent(string content, string path)
        {
            File.WriteAllText(path, content);
            return true;
        }

        public static bool ReplaceContent(string content, string dir, string fileName)
        {
            File.WriteAllText(Path.Combine(dir, fileName), content);
            return true;
        }

        /// <summary>
        /// Append text at the end of a text file
        /// </summary>
        /// <param name="content"></param>
        /// <param name="dir"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool AppendToNextTextFile(string content, string dir, string fileName)
        {
            try
            {
                var byteArray = Encoding.UTF8.GetBytes(content);
                new MemoryStream(byteArray);
                var filePath = Path.Combine(dir, fileName);
                if (dir != null && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                if (!File.Exists(filePath))
                {
                    var fs = File.Create(filePath);
                    fs.Close();
                }

                var sw = File.AppendText(filePath);
                sw.Write(content);
                // Writing a string directly to the file
                sw.Close();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Write file
        /// </summary>
        /// <param name="content"></param>
        /// <param name="dir"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool WriteTextFile(string content, string dir, string fileName)
        {
            try
            {
                var byteArray = Encoding.UTF8.GetBytes(content);
                new MemoryStream(byteArray);
                var filePath = Path.Combine(dir, fileName);

                if (dir != null && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                if (!File.Exists(filePath))
                {
                    var fs = File.Create(filePath);
                    fs.Close();
                }

                var sw = File.CreateText(filePath);
                sw.Write(content);
                // Writing a string directly to the file
                sw.Close();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}