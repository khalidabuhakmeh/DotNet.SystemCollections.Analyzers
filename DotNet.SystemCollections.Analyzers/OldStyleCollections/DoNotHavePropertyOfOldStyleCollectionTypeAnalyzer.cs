namespace DotNet.SystemCollections.Analyzers.OldStyleCollections
{
    using System.Collections.Immutable;
    using DotNet.SystemCollections.Analyzers.Helpers;
    using DotNet.SystemCollections.Analyzers.Helpers.Collections;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    ///     This is used to analyze and detect for situations where fields are using "Old-Style" collections (i.e. collections from System.Collections).
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DoNotHavePropertyOfOldStyleCollectionTypeAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        ///     This is the complete diagnostic ID for this analyzer.
        /// </summary>
        public static readonly string DiagnosticId = AnalyzerHelper.GetCompleteAnalyzerId(IdNumber);

        /// <summary>
        ///     This is the rule (i.e. <see cref="DiagnosticDescriptor"/>) that is handled by this analyzer.
        /// </summary>
#pragma warning disable RS1017 // DiagnosticId for analyzers must be a non-null constant.
        internal static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, AnalyzerHelper.AnalyzerTitle, MessageFormat, AnalyzerCategory.OldStyleCollections, DiagnosticSeverity.Warning, true, Description);
#pragma warning restore RS1017 // DiagnosticId for analyzers must be a non-null constant.

        /// <summary>
        ///     The message format to use for diagnostics generated by this analyzer.
        /// </summary>
        private const string MessageFormat = "The {0} property has a type ({1}) of an \"Old-Style\" collection (i.e. collection class from System.Collections).";

        /// <summary>
        ///     The description to use for diagnostics generated by this analyzer.
        /// </summary>
        private const string Description = "This is when a field is an \"Old-Style\" collection (i.e. a collection class from System.Collections).";

        /// <summary>
        ///     The number portion of the analyzer's <see cref="DiagnosticId"/>.
        /// </summary>
        private const int IdNumber = 1007;

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

            // Attempt to get the named type symbol associated with that property.
            INamedTypeSymbol propertyTypeSymbol = AnalyzerHelper.GetNamedTypeSymbol(propertySymbol.Type);

            // If it was not possible to get the named type symbol.
            if (propertyTypeSymbol == null)
            {
                // Nothing more needs to be done here; just return.
                return;
            }

            // If the property's type is an old-style Collection class (i.e. class from System.Collections).
            if (CollectionHelper.IsOldStyleCollectionClass(propertyTypeSymbol))
            {
                // For every location where the property is defined.
                foreach (var location in propertySymbol.Locations)
                {
                    // Report a diagnostic that "Old-Style" collections should not be used anymore.
                    context.ReportDiagnostic(Diagnostic.Create(Rule, location, propertySymbol.Name, propertyTypeSymbol.GetFullNameWithoutPrefix()));
                }
            }
        }
    }
}
