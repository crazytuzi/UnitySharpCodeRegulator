using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Immutable;
using Analyzer;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using System.Linq;

namespace CdeAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class FroceFunctionComments : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(ForceCommentsDescriptor);
        /// <summary>
        /// 错误描述
        /// </summary>
        private static readonly DiagnosticDescriptor ForceCommentsDescriptor =
            new DiagnosticDescriptor(
                DianogsticIDs.FORCE_Comments_ID,          // ID
                "类必须添加XML文档注释",    // Title
                "类必须添加XML文档注释", // Message format
                DiagnosticCategories.Criterion,                // Category
                DiagnosticSeverity.Error, // Severity
                isEnabledByDefault: true    // Enabled by default
            );

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
            //找到所有类定义
            var methodNodeList = root.DescendantNodes()?.OfType<MethodDeclarationSyntax>();
            foreach (var meth in methodNodeList)
            {
                var classDeclaration = (MethodDeclarationSyntax)meth;
                var tokens = meth.ChildTokens();
                foreach (var token in tokens)
                {
                    //关键字：public,其他关键字可以自行处理：internal, protected, private, no keyword
                    if (token.IsKind(SyntaxKind.PublicKeyword))
                    {
                        //获取注释文本
                        var comments = token.GetAllTrivia().ToSyntaxTriviaList().ToString();

                        //获取有效注释行
                        List<SyntaxTrivia> liables = new List<SyntaxTrivia>();
                        var triviaList = token.GetAllTrivia();
                        foreach (var trivia in triviaList)
                        {
                            if (trivia.IsKind(SyntaxKind.SingleLineCommentTrivia))
                            {
                                if (!trivia.ToString().Contains("<summary>") && !trivia.ToString().Contains("</summary>"))
                                {
                                    liables.Add(trivia);
                                }
                            }
                        }

                        var liablesStr = string.Join("", liables);

                        liablesStr = liablesStr.Replace("///", "").Trim();
                        if (liablesStr.Length < 4)/*注释长度过短，可以自己添加其他规则*/
                        {
                            //报错
                            var diagnostic = Diagnostic.Create(ForceCommentsDescriptor, meth.GetFirstToken().GetLocation());
                            context.ReportDiagnostic(diagnostic);
                        }
                    }
                    //只判断第一个符合条件的
                    break;
                }
            }
        }
    }
}
