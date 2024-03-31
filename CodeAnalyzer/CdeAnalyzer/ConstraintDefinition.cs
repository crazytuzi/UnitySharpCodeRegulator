using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Analyzer
{
    public class ConstraintDefinition
    {
        /// <summary>
        /// 检查时排除的目录
        /// </summary>
        static List<string> AnalyzerExcludePath = new List<string>() {
            "/PackageCache/",
            "/ThirdLibs/",
            "/Plugins/"
        };
        /// <summary>
        /// 检查时排除的文件
        /// </summary>
        static List<string> AnalyzerExcludeFileName = new List<string>()
        {
            "Program.cs"
        };
        /// <summary>
        /// 是否是需要排除检查
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool ExcludeAnalize(string path)
        {
            var fileName = Path.GetFileName(path);
            if (AnalyzerExcludeFileName.Contains(fileName))
            {
                return true;
            }
            foreach(var file in AnalyzerExcludePath)
            {
                if(path.Contains(file))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
