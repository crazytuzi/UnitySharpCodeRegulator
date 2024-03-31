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
    internal class ForceFunctionNameConventions : DiagnosticAnalyzer
    {
        /// <summary>
        /// 错误描述
        /// </summary>
        private static readonly DiagnosticDescriptor PublicFunDescriptor =
            new DiagnosticDescriptor(
                DianogsticIDs.FORCE_NAMING_CONVENTIONS_ID,          // ID
                "public 方法名不符合规范",    // Title
                "public 方法名不符合规范", // Message format
                DiagnosticCategories.Criterion,                // Category
                DiagnosticSeverity.Error, // Severity
                isEnabledByDefault: true    // Enabled by default
            );

        private static readonly DiagnosticDescriptor PrivateFunDescriptor =
            new DiagnosticDescriptor(
                DianogsticIDs.FORCE_NAMING_CONVENTIONS_ID,          // ID
                "private 方法名不符合规范",    // Title
                "private 方法名不符合规范", // Message format
                DiagnosticCategories.Criterion,                // Category
                DiagnosticSeverity.Error, // Severity
                isEnabledByDefault: true    // Enabled by default
            );
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(PublicFunDescriptor, PrivateFunDescriptor);


        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxTreeAction(AnalyzeSymbol);
        }

        private static void AnalyzeSymbol(SyntaxTreeAnalysisContext context)
        {
            //找到文档的语法根树
            var root = context.Tree.GetRoot(context.CancellationToken);
            if (ConstraintDefinition.ExcludeAnalize(context.Tree.FilePath))
            {//排除特殊目录
                return;
            }
            var methodNodeList = root.DescendantNodes()?.OfType<MethodDeclarationSyntax>();
            foreach (var method in methodNodeList)
            {
                var clsName = method.Identifier.ToString();
                var firstChar = clsName.First().ToString();
                var tokens = method.ChildTokens();
                foreach ( var token in tokens)
                {
                    //public 方法：首字母大写，大驼峰命名法（pascal）
                    if (token.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.PublicKeyword))
                    {
                        //全是大写或首字母非大写，则不符合大驼峰命名法（粗略检查),复杂的规矩可以自行定义
                        if (clsName == clsName.ToLower()
                            || clsName == clsName.ToUpper()
                            || firstChar != firstChar.ToUpper()
                            )
                        {
                            //报错
                            var diagnostic = Diagnostic.Create(PublicFunDescriptor, method.GetFirstToken().GetLocation());
                            context.ReportDiagnostic(diagnostic);
                        }
                        break;//只检查一次
                    }
                    else if (token.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.PrivateKeyword)
                        || token.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.ProtectedKeyword)
                        || token.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.InternalKeyword)
                        || token.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.IdentifierToken))
                    {
                        //其他:private protected 等等，使用小驼峰命名法
                        //首字母小写
                        if (firstChar != firstChar.ToLower()
                            )
                        {
                            //报错
                            var diagnostic = Diagnostic.Create(PrivateFunDescriptor, method.GetFirstToken().GetLocation());
                            context.ReportDiagnostic(diagnostic);
                        }
                        break;//只检查一次
                    }
                }
                
            }
        }
    }
}
