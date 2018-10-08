using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamla.Graph.Models;

namespace Xamla.Graph
{
    public interface ICompilerDiagnosticsNode
        : INode
    {
        void SetCompileResult(DateTime date, TimeSpan duration, bool success, IEnumerable<CompilerMessageModel> messages);
        void Clear();

        DateTime? CompileDate { get; }
        TimeSpan CompileDuration { get; }
        bool CompileSuccess { get; }
        IList<CompilerMessageModel> CompileMessages { get; }
    }
}
