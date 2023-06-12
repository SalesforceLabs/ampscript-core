// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.Engine.Runtime
{
    public partial class RuntimeContext
    {
        private readonly Random Random = new Random();

        /// <summary>
        /// Returns a random number between the values specified, inclusive.
        /// </summary>
        /// <param name="first">Least value to return as the returned random integer</param>
        /// <param name="second">Greatest value to return as the random integer</param>
        /// <returns>A random value between first and second, inclusive</returns>
        public long RANDOM(object first, object second)
        {
            int firstInt = SageValue.ToInt(first);
            int secondInt = SageValue.ToInt(second);

            return Random.Next(firstInt, secondInt + 1);
        }

        /// <summary>
        /// Returns the sum of the provided values.
        /// </summary>
        /// <param name="first">First value to add</param>
        /// <param name="second">Second value to add</param>
        /// <returns>The sum of the two values</returns>
        public double ADD(object first, object second)
        {
            double firstDouble = SageValue.ToDouble(first);
            double secondDouble = SageValue.ToDouble(second);
            return firstDouble + secondDouble;
        }

        /// <summary>
        /// Returns the difference of two integers.
        /// </summary>
        /// <param name="first">The initial value</param>
        /// <param name="second">Number to subtract from initial value</param>
        /// <returns>The difference of the two values</returns>
        public double SUBTRACT(object first, object second)
        {
            double firstDouble = SageValue.ToDouble(first);
            double secondDouble = SageValue.ToDouble(second);
            return firstDouble - secondDouble;
        }

        /// <summary>
        /// Returns the product of multiplying two numbers.
        /// </summary>
        /// <param name="first">The first number</param>
        /// <param name="second">The second number</param>
        /// <returns>The product of the two values</returns>
        public double MULTIPLY(object first, object second)
        {
            double firstDouble = SageValue.ToDouble(first);
            double secondDouble = SageValue.ToDouble(second);
            return firstDouble * secondDouble;
        }

        /// <summary>
        /// Returns the result of dividing the first argument by the second argument.
        /// </summary>
        /// <param name="first">Dividend value</param>
        /// <param name="second">Divisor value, used to divide the dividend value</param>
        /// <returns>The result of dividing the first argument by the second argument</returns>
        public double DIVIDE(object first, object second)
        {
            double firstDouble = SageValue.ToDouble(first);
            double secondDouble = SageValue.ToDouble(second);
            return firstDouble / secondDouble;
        }

        /// <summary>
        /// Returns the remainder after dividing the first number by the second number.
        /// </summary>
        /// <param name="first">Dividend value</param>
        /// <param name="second">Divisor value</param>
        /// <returns>The remainder after dividing the first number by the second number</returns>
        public double MOD(object first, object second)
        {
            double firstDouble = SageValue.ToDouble(first);
            double secondDouble = SageValue.ToDouble(second);
            return firstDouble % secondDouble;
        }
    }
}
