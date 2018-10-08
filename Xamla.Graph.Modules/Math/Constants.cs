using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamla.Graph.Modules
{
    public static class MathConstants
    {
        /// <summary>
        /// This module only outputs $\infty$.
        /// </summary>
        /// <returns>$\infty$</returns>
        [StaticModule(ModuleType = "Xamla.Math.Constants.Infinity")]
        public static double Infininty()
        {
            return double.PositiveInfinity;
        }

        /// <summary>
        /// This module only outputs $-\infty$.
        /// </summary>
        /// <returns>$-\infty$</returns>
        [StaticModule(ModuleType = "Xamla.Math.Constants.NegativeInfinity")]
        public static double NegativeInfinity()
        {
            return double.NegativeInfinity;
        }

        /// <summary>
        /// Represents a value that is not a number (NaN).
        /// </summary>
        /// <returns>NaN</returns>
        [StaticModule(ModuleType = "Xamla.Math.Constants.NaN")]
        public static double NaN()
        {
            return double.NaN;
        }

        /// <summary>
        /// Represents the natural logarithmic base, specified by the constant, e.
        /// </summary>
        /// <returns>2.718281828459...</returns>
        [StaticModule(ModuleType = "Xamla.Math.Constants.E")]
        public static double E()
        {
            return 2.7182818284590452353602874713527;
        }

        /// <summary>
        /// Represents the ratio of the circumference of a circle to its diameter, specified
        /// by the constant, π.
        /// </summary>
        /// <returns>3.14159265359...</returns>
        [StaticModule(ModuleType = "Xamla.Math.Constants.PI")]
        public static double PI()
        {
            return 3.1415926535897932384626433832795;
        }

        /// <summary>
        /// The square root of 2.
        /// </summary>
        /// <returns>1.41421356237309...</returns>
        [StaticModule(ModuleType = "Xamla.Math.Constants.Sqrt2")]
        public static double Sqrt2()
        {
            return 1.4142135623730950488016887242097;
        }
    }
}
