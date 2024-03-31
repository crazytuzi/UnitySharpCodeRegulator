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
    internal class ForceLocalVariableConventions : DiagnosticAnalyzer
    {
        /// <summary>
        /// 错误描述
        /// </summary>
        private static readonly DiagnosticDescriptor LocalVarDescriptor =
            new DiagnosticDescriptor(
                DianogsticIDs.FORCE_NAMING_CONVENTIONS_ID,          // ID
                "临时变量命名不符合规范，请使用以字母开头的小驼峰命名法",    // Title
                "临时变量命名不符合规范，请使用以字母开头的小驼峰命名法", // Message format
                DiagnosticCategories.Criterion,                // Category
                DiagnosticSeverity.Error, // Severity
                isEnabledByDefault: true    // Enabled by default
            );
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(LocalVarDescriptor);

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
            var localNodeList = root.DescendantNodes()?.OfType<LocalDeclarationStatementSyntax>();
            foreach(var localNode in localNodeList)
            {
                var varList = localNode.Declaration.Variables;
                foreach(var localVar in varList)
                {
                    var localName = localVar.Identifier.Value.ToString();
                    var firstChar = localName.First().ToString();
                    if (firstChar.ToLower() != firstChar)
                    {//判断第一个字母是否是小写
                        //报错
                        var diagnostic = Diagnostic.Create(LocalVarDescriptor, localNode.GetFirstToken().GetLocation());
                        context.ReportDiagnostic(diagnostic);
                    }
                }
                
            }
        }
    }
}
