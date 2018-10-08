using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Types.Sequence;

namespace Xamla.Graph.Modules
{
    public class UnaryOperation
        : ModuleBase
    {
        Func<double, double> operation;

        protected GenericInputPin operandPin;
        protected GenericOutputPin resultPin;

        public UnaryOperation(IGraphRuntime runtime, string moduleType, Func<double, double> operation, string resultPinName = "Result", string operandPinName = "Operand")
            : base(runtime, ModuleKind.Module, DisplayMode.Expanded, moduleType)
        {
            this.operation = operation;
            this.operandPin = this.AddInputPin(operandPinName, PinDataTypeFactory.CreateFloat64(), PropertyMode.Never);
            this.resultPin = this.AddOutputPin(resultPinName, PinDataTypeFactory.CreateFloat64());
        }

        public IInputPin OperandPin
        {
            get { return operandPin; }
        }

        public IOutputPin ResultPin
        {
            get { return resultPin; }
        }

        protected override Task<object[]> EvaluateInternal(object[] inputs, CancellationToken cancel)
        {
            var operand = (double)inputs[0];
            var result = operation(operand);

            return Task.FromResult(new object[] { result });
        }
    }

    public class BinaryOperation
        : ModuleBase
    {
        Func<double, double, double> operation;

        protected GenericInputPin operand1Pin;
        protected GenericInputPin operand2Pin;
        protected GenericOutputPin resultPin;

        public BinaryOperation(IGraphRuntime runtime, string moduleType, Func<double, double, double> operation, string resultPinName = "Result", string operand1PinName = "Operand1", string operand2PinName = "Operand2")
            : base(runtime, ModuleKind.Module, DisplayMode.Expanded, moduleType)
        {
            this.operation = operation;
            this.operand1Pin = this.AddInputPin(operand1PinName, PinDataTypeFactory.CreateFloat64(), PropertyMode.Allow);
            this.operand2Pin = this.AddInputPin(operand2PinName, PinDataTypeFactory.CreateFloat64(), PropertyMode.Allow);
            this.resultPin = this.AddOutputPin(resultPinName, PinDataTypeFactory.CreateFloat64());
        }

        public IInputPin Operand1Pin
        {
            get { return operand1Pin; }
        }

        public IInputPin Operand2Pin
        {
            get { return operand2Pin; }
        }

        public IOutputPin ResultPin
        {
            get { return resultPin; }
        }

        protected override Task<object[]> EvaluateInternal(object[] inputs, CancellationToken cancel)
        {
            var operand1 = (double)inputs[0];
            var operand2 = (double)inputs[1];
            var result = operation(operand1, operand2);

            return Task.FromResult(new object[] { result });
        }
    }

    public static class MathOperators
    {
        abstract class FactoryInfo
        {
            protected string moduleType;
            protected string description;
            protected string category;
            protected string resultPinId;

            public FactoryInfo(string moduleType, string description, string caterogy, string resultPinName = "Result")
            {
                this.moduleType = moduleType;
                this.description = description;
                this.category = caterogy;
                this.resultPinId = resultPinName;
            }

            public abstract void Register(IModuleFactory factory);
        }

        class UnaryInfo
            : FactoryInfo
        {
            Func<double, double> operation;
            string operandPinId;


            public UnaryInfo(string moduleType, string description, Func<double, double> operation, string resultPinName = "Result", string operandPinName = "Operand")
                : base(moduleType, description, resultPinName)
            {
                this.operation = operation;
                this.operandPinId = operandPinName;
            }

            public override void Register(IModuleFactory factory)
            {
                factory.Register(moduleType, new ModuleDescription { ModuleType = moduleType, HelpText = description }, (runtime, s) => (IModule)new UnaryOperation(runtime, moduleType, operation, resultPinId, operandPinId), null);
            }
        }

        class BinaryInfo
            : FactoryInfo
        {
            Func<double, double, double> operation;
            string operand1PinId;
            string operand2PinId;

             public BinaryInfo(string moduleType, string description, Func<double, double, double> operation, string resultPinName = "Result", string operand1PinName = "Operand1", string operand2PinName = "Operand2")
                : base(moduleType, description, resultPinName)
            {
                this.operation = operation;
                this.operand1PinId = operand1PinName;
                this.operand2PinId = operand2PinName;
            }

             public override void Register(IModuleFactory factory)
             {
                 factory.Register(moduleType, new ModuleDescription { ModuleType = moduleType, HelpText = description }, (runtime, s) => (IModule)new BinaryOperation(runtime, moduleType, operation, resultPinId, operand1PinId, operand2PinId), null);
             }
        }

        static FactoryInfo[] operations =
        {
            // unary operators
            new UnaryInfo("System.Math.Abs","Returns the absolute value of a number.", Math.Abs),
            new UnaryInfo("System.Math.Acos", "Returns the angle whose cosine is the specified number.", Math.Acos),
            new UnaryInfo("System.Math.Asin", "Returns the angle whose sine is the specified number.", Math.Asin),
            new UnaryInfo("System.Math.Atan", "Returns the angle whose tangent is the specified number.", Math.Atan),
            new UnaryInfo("System.Math.Ceiling", "Returns the smallest integral value that is greater than or equal to the specified number.", Math.Ceiling),
            new UnaryInfo("System.Math.Cos", "Returns the cosine of the specified angle.", Math.Cos),
            new UnaryInfo("System.Math.Cosh", "Returns the hyperbolic cosine of the specified angle.", Math.Cosh),
            new UnaryInfo("System.Math.Floor", "Returns the largest integer less than or equal to the specified number.", Math.Floor),
            new UnaryInfo("System.Math.Exp", "Returns e raised to the specified power.", Math.Exp, "Result", "Exponent"),
            new UnaryInfo("System.Math.Log_e", "Returns the natural (base e) logarithm of a specified number.", Math.Log),
            new UnaryInfo("System.Math.Log_10", "Returns the base 10 logarithm of a specified number.", Math.Log10),
            new UnaryInfo("System.Math.Round", "Rounds a value to the nearest integral value.", Math.Round),
            new UnaryInfo("System.Math.Sign", "Returns a value indicating the sign of a number.", x => (double)Math.Sign(x)),
            new UnaryInfo("System.Math.Sinh", "Returns the hyperbolic sine of the specified angle.", Math.Sinh),
            new UnaryInfo("System.Math.Sqrt", "Returns the square root of a specified number.", Math.Sqrt),
            new UnaryInfo("System.Math.Tan", "Returns the tangent of the specified angle.", Math.Tan),
            new UnaryInfo("System.Math.Tanh", "Returns the hyperbolic tangent of the specified angle.", Math.Tanh),
            new UnaryInfo("System.Math.Square", "Returns the square of a value.", x => Math.Pow(x, 2.0)),
            new UnaryInfo("System.Math.Reciprocal", "Returns the reciprocal of a value.", x => 1.0 / x),
            new UnaryInfo("System.Math.Negate", "Returns the negated value.", x => -x),

            // binary operators
            new BinaryInfo("System.Math.Add", "Returns the sum of two numbers.", (x, y) => x + y, "Sum"),
            new BinaryInfo("System.Math.Subtract", "Returns the difference of two numbers.", (x, y) => x - y, "Difference"),
            new BinaryInfo("System.Math.Multiply", "Returns the product of two numbers.", (x, y) => x * y, "Product"),
            new BinaryInfo("System.Math.Divide", "Returns the quotient of two numbers.", (x, y) => x / y, "Quotient", "Dividend", "Devisor"),
            new BinaryInfo("System.Math.Min", "Returns the smaller of two numbers.", Math.Min),
            new BinaryInfo("System.Math.Max", "Returns the greater of two numbers.", Math.Max),
            new BinaryInfo("System.Math.Pow", "Returns a specified number raised to the specified power.", Math.Pow, "Result", "Base", "Exponent"),
            new BinaryInfo("System.Math.Log", "Returns the logarithm of a specified number in a specified base.", Math.Log, "Result", "Power", "Base"),
            new BinaryInfo("System.Math.Atan2", "Returns the angle whose tangent is the quotient of two specified numbers.", Math.Atan2, "Angle", "y", "x"),
        };

        public static void RegisterAll(IModuleFactory factory)
        {
            foreach (var op in operations)
                op.Register(factory);
        }
    }
}
