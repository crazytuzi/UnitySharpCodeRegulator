﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
/**
 * Author: Laugh（笑微）
 * https://developer.unity.cn/projects/65937455edbc2a001cbd8102
 */
namespace Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class ForceStatementBrace : DiagnosticAnalyzer
    {
        private static readonly DiagnosticDescriptor FroceBraceDescriptor =
            new DiagnosticDescriptor(
                DianogsticIDs.FORCE_BRACE_ID,          // ID
                "逻辑分支必须使用花括号包括",    // Title
                "逻辑分支必须使用花括号包括", // Message format
                DiagnosticCategories.Criterion,                // Category
                DiagnosticSeverity.Error, // Severity
                isEnabledByDefault: true    // Enabled by default
            );
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(FroceBraceDescriptor);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxTreeAction(AnalyzeSymbol);
        }
        private static void AnalyzeSymbol(SyntaxTreeAnalysisContext context)
        {
            var root = context.Tree.GetRoot(context.CancellationToken);
            List<ExpressionStatementSyntax> reportedNode = new List<ExpressionStatementSyntax>();
            foreach (var ifstatement in root.DescendantNodes()?.OfType<IfStatementSyntax>())
            {
                var expressStatements = ifstatement.DescendantNodes()?.OfType<ExpressionStatementSyntax>().ToList() ;
                foreach(var estate in expressStatements)
                {
                    if (ConstraintDefinition.ExcludeAnalize(context.Tree.FilePath))
                    {//排除特殊目录
                        return;
                    }
                    if (reportedNode.Contains(estate))
                    {
                        continue;
                    }
                    if(!(estate.Parent is BlockSyntax))
                    {
                        reportedNode.Add(estate);
                        var diagnostic = Diagnostic.Create(FroceBraceDescriptor, estate.GetFirstToken().GetLocation());
                        context.ReportDiagnostic(diagnostic);
                    }
                }
                
            }
        }
    }
}
