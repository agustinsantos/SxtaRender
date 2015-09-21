using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SxtaUI2.Core
{
    /// <summary>
    /// Templated class for a four-component RGBA colour.
    /// </summary>
    public class Colour
    {
        /// Lightweight, non-initialising constructor.
        public Colour() { }

        /// Initialising constructor.
        /// @param[in] red Initial red value of the colour.
        /// @param[in] green Initial green value of the colour.
        /// @param[in] blue Initial blue value of the colour.
        /// @param[in] alpha Initial alpha value of the colour.
        public Colour(float _red, float _green, float _blue, float _alpha = 1.0f)
        {
            red = _red;
            green = _green;
            blue = _blue;
            alpha = _alpha;
        }

        /// Returns the sum of this colour and another. This does not saturate the channels.
        /// @param[in] rhs The colour to add this to.
        /// @return The sum of the two colours.
        public static Colour operator +(Colour lhs, Colour rhs)
        {
            return new Colour(lhs.red + rhs.red, lhs.green + rhs.green, lhs.blue + rhs.blue, lhs.alpha + rhs.alpha);
        }

        /// Returns the result of subtracting another colour from this colour.
        /// @param[in] rhs The colour to subtract from this colour.
        /// @return The result of the subtraction.
        public static Colour operator -(Colour lhs, Colour rhs)
        {
            return new Colour(lhs.red - rhs.red, lhs.green - rhs.green, lhs.blue - rhs.blue, lhs.alpha - rhs.alpha);
        }

        /// Returns the result of multiplying this colour component-wise by a scalar.
        /// @param[in] rhs The scalar value to multiply by.
        /// @return The result of the scale.
        public static Colour operator *(Colour lhs, float rhs)
        {
            return new Colour((float)(lhs.red * rhs), (float)(lhs.green * rhs), (float)(lhs.blue * rhs), (float)(lhs.alpha * rhs));
        }

        /// Returns the result of dividing this colour component-wise by a scalar.
        /// @param[in] rhs The scalar value to divide by.
        /// @return The result of the scale.
        public static Colour operator /(Colour lhs, float rhs)
        {
            return new Colour((float)(lhs.red / rhs), (float)(lhs.green / rhs), (float)(lhs.blue / rhs), (float)(lhs.alpha / rhs));
        }

        /// Adds another colour to this in-place. This does not saturate the channels.
        /// @param[in] rhs The colour to add.
        public void Add(Colour rhs)
        {
            red += rhs.red;
            green += rhs.green;
            blue += rhs.blue;
            alpha += rhs.alpha;
        }

        /// Subtracts another colour from this in-place.
        /// @param[in] rhs The colour to subtract.
        public void Substract(Colour rhs)
        {
            red -= rhs.red;
            green -= rhs.green;
            blue -= rhs.blue;
            alpha -= rhs.alpha;
        }

        /// Multiplies this colour component-wise with another in-place.
        /// @param[in] rhs The colour to multiply by.
        /// @return This colour, post-operation.
        public void Mult(float rhs)
        {
            red = red * rhs;
            green = green * rhs;
            blue = blue * rhs;
            alpha = alpha * rhs;
        }

        /// Scales this colour component-wise in-place by the inverse of a value.
        /// @param[in] rhs The value to divide this colour's components by.
        public void Div(float rhs)
        {
            red = red / rhs;
            green = green / rhs;
            blue = blue / rhs;
            alpha = alpha / rhs;
        }

        /// Equality operator.
        /// @param[in] rhs The colour to compare this against.
        /// @return True if the two colours are equal, false otherwise.
        public static bool operator ==(Colour lhs, Colour rhs)
        { return lhs.red == rhs.red && lhs.green == rhs.green && lhs.blue == rhs.blue && lhs.alpha == rhs.alpha; }

        /// Inequality operator.
        /// @param[in] rhs The colour to compare this against.
        /// @return True if the two colours are not equal, false otherwise.
        public static bool operator !=(Colour lhs, Colour rhs)
        { return lhs.red != rhs.red || lhs.green != rhs.green || lhs.blue != rhs.blue || lhs.alpha != rhs.alpha; }



        private float red, green, blue, alpha;
    }
}
