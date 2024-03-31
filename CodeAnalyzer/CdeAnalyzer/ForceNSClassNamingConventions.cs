using Analyzer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace CdeAnalyzer
{
    /**
     * Author: Laugh（笑微）
     * https://developer.unity.cn/projects/65937455edbc2a001cbd8102
     */
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class ForceNSClassNamingConventions : DiagnosticAnalyzer
    {
        /// <summary>
        /// 错误描述
        /// </summary>
        private static readonly DiagnosticDescriptor ForceNamingConventionsDescriptor =
            new DiagnosticDescriptor(
                DianogsticIDs.FORCE_NAMING_CONVENTIONS_ID,          // ID
                "命名空间或类名不符合规范",    // Title
                "命名空间或类名不符合规范", // Message format
                DiagnosticCategories.Criterion,                // Category
                DiagnosticSeverity.Error, // Severity
                isEnabledByDefault: true    // Enabled by default
            );
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(ForceNamingConventionsDescriptor);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxTreeAction(AnalyzeSymbol);
        }

        private static void AnalyzeSymbol(SyntaxTreeAnalysisContext context)
        {
            //找到文档的语法根树
            var root = context.Tree.GetRoot(context.CancellationToken);

            var classNodeList = root.DescendantNodes()?.OfType<ClassDeclarationSyntax>();
            foreach (var cls in classNodeList)
            {
                var clsName = cls.Identifier.ToString();
                var firstChar = clsName.First().ToString();
                //如果全是小写或全是大写或首字母非大写，则不符合驼峰命名法（粗略检查),复杂的规矩可以自行定义
                if (clsName == clsName.ToLower()
                    || clsName == clsName.ToUpper()
                    || firstChar != firstChar.ToUpper()
                    )
                {
                    //报错
                    var diagnostic = Diagnostic.Create(ForceNamingConventionsDescriptor, cls.GetFirstToken().GetLocation());
                    context.ReportDiagnostic(diagnostic);
                }
            }

            var nsNodeList = root.DescendantNodes()?.OfType<NamespaceDeclarationSyntax>();
            foreach(var ns in nsNodeList)
            {
                var nsName = ns.Name.ToString();
                //拆分命名空间的级段
                var nlist = nsName.Split(new char[] { '.' });
                foreach(var n in nlist)
                {
                    var firstChar = n.First().ToString();
                    //如果首字母非大写，则不符合驼峰命名法（粗略检查),复杂的规矩可以自行定义
                    if (firstChar != firstChar.ToUpper()
                    )
                    {
                        //报错
                        var diagnostic = Diagnostic.Create(ForceNamingConventionsDescriptor, ns.GetFirstToken().GetLocation());
                        context.ReportDiagnostic(diagnostic);
                        break;
                    }
                }
            }
        }
    }
}
