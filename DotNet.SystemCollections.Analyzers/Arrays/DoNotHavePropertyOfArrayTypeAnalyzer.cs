namespace DotNet.SystemCollections.Analyzers.Arrays
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using DotNet.SystemCollections.Analyzers.Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    ///     This is used to analyze and detect for situations where properties are arrays instead of <see cref="IReadOnlyList{T}"/>.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DoNotHavePropertyOfArrayTypeAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        ///     This is the complete diagnostic ID for this analyzer.
        /// </summary>
        public static readonly string DiagnosticId = AnalyzerHelper.GetCompleteAnalyzerId(IdNumber);

        /// <summary>
        ///     The rule (i.e. <see cref="DiagnosticDescriptor"/>) handled by this analyzer.
        /// </summary>
#pragma warning disable RS1017 // DiagnosticId for analyzers must be a non-null constant.
        internal static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, AnalyzerHelper.AnalyzerTitle, MessageFormat, AnalyzerCategory.Arrays, DiagnosticSeverity.Warning, true, Description);
#pragma warning restore RS1017 // DiagnosticId for analyzers must be a non-null constant.

        /// <summary>
        ///     The message format to use for diagnostics generated by this analyzer.
        /// </summary>
        private const string MessageFormat = "The {0} property has a type of an Array instead of an IReadOnlyList.";

        /// <summary>
        ///     The description to use for diagnostics generated by this analyzer.
        /// </summary>
        private const string Description = "This is when a property has a type of an Array instead of an IReadOnlyList.";

        /// <summary>
        ///     The number portion of the analyzer's <see cref="DiagnosticId"/>.
        /// </summary>
        private const int IdNumber = 1002;

        /// <summary>
        ///     Gets the set of rules handled by this analyzer.
        /// </summary>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        /// <summary>
        ///     This is used to initialize the analyzer.
        /// </summary>
        /// <param name="context">
        ///     The context in which the analysis takes place.
        /// </param>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Property);
        }

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            // Cast down to a IPropertySymbol.
            IPropertySymbol propertySymbol = (IPropertySymbol)context.Symbol;

            // If the type of the property symbol is an array.
            if (propertySymbol.Type is IArrayTypeSymbol)
            {
                // For every location where the property is defined.
                foreach (var location in propertySymbol.Locations)
                {
                    // Report a diagnostic that IReadOnlyList should be the type instead.
                    context.ReportDiagnostic(Diagnostic.Create(Rule, location, propertySymbol.Name));
                }
            }
        }
    }
}
