using System;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Hosting;

namespace Module1.Helpers
{
    public class SingleViewCodeUpdateHelper
    {
        private readonly IWebHostEnvironment _webEnv;

        public SingleViewCodeUpdateHelper(IWebHostEnvironment webEnv)
        {
            _webEnv = webEnv;
        }

        public int UpdateCount { get; set; }

        public MessageResult UpdateTheView()
        {
            var theViewPath = GetTheViewPath();
            return UpdateTheView(theViewPath);
        }

        public string GetTheViewPath()
        {
            var contentRootPath = _webEnv.ContentRootPath;
            var singleViewCodePath = Path.Combine(contentRootPath, "Areas", "Module1", "Pages", "SingleViewCode.cshtml");
            if (!File.Exists(singleViewCodePath))
            {
                //under developing will find this path
                singleViewCodePath = Path.Combine(contentRootPath, "..\\Module1", "Pages", "SingleViewCode.cshtml");
            }
            return singleViewCodePath;
        }

        public MessageResult UpdateTheView(string singleViewCodePath)
        {
            var messageResult = new MessageResult();
            if (!File.Exists(singleViewCodePath))
            {
                messageResult.Message = "文件不存在: " + singleViewCodePath;
                return messageResult;
            }

            var readAllLines = File.ReadAllLines(singleViewCodePath);
            var theLine = 0;
            for (int i = 0; i < readAllLines.Length; i++)
            {
                var line = readAllLines[i];
                if (!string.IsNullOrWhiteSpace(line) && line.Contains("last_update_at"))
                {
                    theLine = i;
                    break;
                }
            }

            if (theLine != 0)
            {
                UpdateCount++;
                var update = $"        <p>last_update_at: {UpdateCount} => {DateTimeOffset.Now.ToUnixTimeMilliseconds()}</p>";
                readAllLines[theLine] = update;
            }
            File.WriteAllLines(singleViewCodePath, readAllLines, Encoding.UTF8);
            
            messageResult.Success = true;
            messageResult.Message = "更新完毕: " + singleViewCodePath;
            return messageResult;
        }
    }
}
