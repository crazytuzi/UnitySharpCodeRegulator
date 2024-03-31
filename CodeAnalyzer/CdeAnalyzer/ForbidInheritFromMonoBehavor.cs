using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Immutable;
using Analyzer;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace CdeAnalyzer
{
    /**
     * Author: Laugh（笑微）
     * https://developer.unity.cn/projects/65937455edbc2a001cbd8102
     */
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class ForbidInheritFromMonoBehavor : DiagnosticAnalyzer
    {
        /// <summary>
        /// 错误描述
        /// </summary>
        private static readonly DiagnosticDescriptor ForbidInheritDescriptor =
            new DiagnosticDescriptor(
                DianogsticIDs.FORCE_FORBID_INHERIT_MONOBEHAVIOR_ID,          // ID
                "实现了IFrameDrivable接口的类，不能继承自MonoBehavior",    // Title
                "实现了IFrameDrivable接口的类，不能继承自MonoBehavior", // Message format
                DiagnosticCategories.Criterion,                // Category
                DiagnosticSeverity.Error, // Severity
                isEnabledByDefault: true    // Enabled by default
            );
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(ForbidInheritDescriptor);

        /// <summary>
        /// 初始化分析方法
        /// </summary>
        /// <param name="context"></param>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxTreeAction(AnalyzeSymbol);
        }
        /// <summary>
        /// 检测方法
        /// </summary>
        /// <param name="context"></param>
        private static void AnalyzeSymbol(SyntaxTreeAnalysisContext context)
        {
            //找到文档的语法根树
            var root = context.Tree.GetRoot(context.CancellationToken);
            if (ConstraintDefinition.ExcludeAnalize(context.Tree.FilePath))
            {//排除特殊目录
                return;
            }

            var classList = root.DescendantNodes()?.OfType<ClassDeclarationSyntax>();
            foreach(var cls in classList)
            {//遍历语法树中的所有类
                var baseClsList = cls.BaseList?.ChildNodes()?.OfType<SimpleBaseTypeSyntax>();
                if (baseClsList == null)
                {
                    break;
                }
                bool isMonoBehavior = false;
                bool isFrameDrivable = false;
                foreach (var bcls in baseClsList)
                {
                    var idName = bcls.ChildNodes()?.OfType<IdentifierNameSyntax>()?.First();
                    var bname = idName.ToString();
                    if(bname == "IFrameDrivable")
                    {//检查是否实现了接口IFrameDrivable
                        isFrameDrivable = true;
                    }
                    else if (bname == "ArrayList")
                    {//检查是否继承自MonoBehavior
                        isMonoBehavior = true;
                    }
                }
                if (isFrameDrivable && isMonoBehavior)
                {
                    //报错
                    var diagnostic = Diagnostic.Create(ForbidInheritDescriptor, cls.GetFirstToken().GetLocation());
                    context.ReportDiagnostic(diagnostic);
                }
            }

        }
    }
}
