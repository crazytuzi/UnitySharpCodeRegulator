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
    internal class ForceMemberVariableConventions : DiagnosticAnalyzer
    {
        /// <summary>
        /// 错误描述
        /// </summary>
        private static readonly DiagnosticDescriptor PublicVarDescriptor =
            new DiagnosticDescriptor(
                DianogsticIDs.FORCE_NAMING_CONVENTIONS_ID,          // ID
                "类中不不能定义共有变量，请使用Getter Setter 或方法",    // Title
                "类中不不能定义共有变量，请使用Getter Setter 或方法", // Message format
                DiagnosticCategories.Criterion,                // Category
                DiagnosticSeverity.Error, // Severity
                isEnabledByDefault: true    // Enabled by default
            );

        private static readonly DiagnosticDescriptor PrivateVarDescriptor =
            new DiagnosticDescriptor(
                DianogsticIDs.FORCE_NAMING_CONVENTIONS_ID,          // ID
                "private 变量名名不符合规范,必须以下划线（_）开头的小驼峰命名",    // Title
                "private 变量名名不符合规范,必须以下划线（_）开头的小驼峰命名", // Message format
                DiagnosticCategories.Criterion,                // Category
                DiagnosticSeverity.Error, // Severity
                isEnabledByDefault: true    // Enabled by default
            );
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(PublicVarDescriptor, PrivateVarDescriptor);

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
            var fieldNodeList = root.DescendantNodes()?.OfType<FieldDeclarationSyntax>();
            foreach (var field in fieldNodeList)
            {
                var filedName = field.Declaration.Variables.ToString();
                var firstChar = filedName.First().ToString();
                var tokens = field.ChildTokens();
                foreach (var token in tokens)
                {
                    //不能包含Public 变量
                    if (token.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.PublicKeyword))
                    {
                        //报错
                        var diagnostic = Diagnostic.Create(PublicVarDescriptor, field.GetFirstToken().GetLocation());
                        context.ReportDiagnostic(diagnostic);
                        break;//只检查一次
                    }
                    else if (token.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.PrivateKeyword)
                        || token.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.ProtectedKeyword)
                        || token.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.InternalKeyword)
                        || token.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.IdentifierToken))
                    {
                        //其他:private protected 等等，使用_开头的小驼峰命名法
                        //首字母小写
                        if (firstChar != "_" || filedName == filedName.ToUpper())
                        {
                            //报错
                            var diagnostic = Diagnostic.Create(PrivateVarDescriptor, field.GetFirstToken().GetLocation());
                            context.ReportDiagnostic(diagnostic);
                        }
                        break;//只检查一次
                    }
                }

            }
        }
    }
}
