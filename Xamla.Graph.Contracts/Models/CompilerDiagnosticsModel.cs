﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamla.Graph.Models
{
    public class SourceLocationModel
    {
        public string FilePath { get; set; }
        public int StartLine { get; set; }
        public int StartCharacter { get; set; }
        public int EndLine { get; set; }
        public int EndCharacter { get; set; }
    }

    public class CompilerMessageModel
    {
        public string ModuleObjectId { get; set; }
        public string ModuleTrace { get; set; }

        /// <summary>
        /// Gets the category of diagnostic. For diagnostics generated by the compiler, the category will be "Compiler".
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets an array of custom tags for the diagnostic.
        /// </summary>
        public List<string> CustomTags { get; set; }

        /// <summary>
        /// The compiler message text.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets a longer description for the diagnostic.
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Gets a hyperlink that provides more detailed information regarding the diagnostic.
        /// </summary>
        public string HelpLink { get; set; }

        /// <summary>
        /// Gets the diagnostic identifier. For diagnostics generated by the compiler,
        /// this will be a numeric code with a prefix such as "CS1001".
        /// </summary>
        public string Id { get; set; }

        public SourceLocationModel Location { get; set; }

        /// <summary>
        /// Gets the Microsoft.CodeAnalysis.DiagnosticSeverity.
        /// </summary>
        public string Severity { get; set; }

        /// <summary>
        /// Gets the warning level. This is an integer between 1 and 4 if severity is
        /// Microsoft.CodeAnalysis.DiagnosticSeverity.Warning; otherwise 0.
        /// </summary>
        public int WarningLevel { get; set; }
    }

    public class CompilerDiagnosticsModel
        : NodeModel
    {
        public CompilerDiagnosticsModel()
            : base("CompilerDiagnostics")
        {
        }

        public DateTime? Date { get; set; }
        public TimeSpan? Duration { get; set; }
        public bool Success { get; set; }
        public List<CompilerMessageModel> Messages { get; set; }
    }
}
