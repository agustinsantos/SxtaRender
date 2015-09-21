using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SxtaUI2.Core
{
    /// <summary>
    /// An element instancer provides a method for allocating and deallocating decorators.
    /// It is important at the same instancer that allocated a decorator releases it. This ensures there are no issues with
    /// memory from different DLLs getting mixed up.
    /// </summary>
    public abstract class DecoratorInstancer
    {
#if TODO
        public DecoratorInstancer();

        /// Instances a decorator given the property tag and attributes from the RCSS file.
        /// @param[in] name The type of decorator desired. For example, "background-decorator: simple;" is declared as type "simple".
        /// @param[in] properties All RCSS properties associated with the decorator.
        /// @return The decorator if it was instanced successfully, NULL if an error occured.
        public abstract Decorator InstanceDecorator(string name, PropertyDictionary properties);

        /// Releases the given decorator.
        /// @param[in] decorator Decorator to release. This is guaranteed to have been constructed by this instancer.
        public abstract void ReleaseDecorator(Decorator decorator);

        /// Releases the instancer.
        public abstract void Release();

        /// Returns the property specification associated with the instancer.
        public PropertySpecification GetPropertySpecification();


        /// Registers a property for the decorator.
        /// @param[in] property_name The name of the new property (how it is specified through RCSS).
        /// @param[in] default_value The default value to be used.
        /// @return The new property definition, ready to have parsers attached.
        protected PropertyDefinition RegisterProperty(string property_name, string default_value);
 
        /// Registers a shorthand property definition.
        /// @param[in] shorthand_name The name to register the new shorthand property under.
        /// @param[in] properties A comma-separated list of the properties this definition is shorthand for. The order in which they are specified here is the order in which the values will be processed.
        /// @param[in] type The type of shorthand to declare.
        /// @param True if all the property names exist, false otherwise.
        protected bool RegisterShorthand(string shorthand_name, string property_names, PropertySpecification.ShorthandType type = PropertySpecification.AUTO);
 
        // Releases the instancer.
        protected virtual void OnReferenceDeactivate();


        private PropertySpecification properties;
#endif
    }
}
