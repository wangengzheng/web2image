using Nancy;
using Nancy.ModelBinding;
using NReco.PhantomJS;
using System;
using System.Security.Cryptography;
using System.Text;

namespace web2image
{
    public class Image :NancyModule
    {
        private static PhantomJS phantomJS = new PhantomJS();

        private static string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        private static string ImagePath = BaseDirectory + "\\content\\phantomjs";
        //<
        private static string PhantomJSPath = ImagePath + "\\exe";

        public Image()
        {
            Get["/"] = _ => {

                Parameter parameter = this.Bind<Parameter>();

                if (string.IsNullOrEmpty(parameter.Url))
                    return "url 参数不能为空！";

                //cache
                var fileName = parameter.Url.MD5Hash() + ".png";

                var filePath = string.Concat(ImagePath, "\\", fileName);

                if (System.IO.File.Exists(filePath))
                {
                    return Response.AsFile(filePath, "image/png");                    
                }

                phantomJS.TempFilesPath = ImagePath;
                phantomJS.ToolPath = PhantomJSPath;
                phantomJS.ExecutionTimeout = TimeSpan.FromSeconds(20);

                phantomJS.OutputReceived += (sender, e) =>
                {                   
                    System.Diagnostics.Debug.WriteLine("phantomJS.OutputReceived += (sender, e) ");
                };

                phantomJS.Run(ImagePath + "\\Page2Image.js", new string[] { parameter.Url, fileName });

                return Response.AsFile(filePath, "image/png");                
            };


        }
    }

    public class Parameter
    {
        public string Url { get; set; }

        public int Width { get; set; }

        public string FileName { get; set; }

        public int Heigth { get; set; }

        public string Opacity { get; set; }

    }

    public static class Extend
    {
        public static string MD5Hash(this string input)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }
    }
}