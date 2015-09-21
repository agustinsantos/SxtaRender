using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SxtaUI2.Core
{
    /// <summary>
    /// A dictionary to property names to values.
    /// </summary>
    public class PropertyDictionary
    {
#if TODO
        public PropertyDictionary();

        /// Sets a property on the dictionary. Any existing property with a similar name will be overwritten.
        /// @param[in] name The name of the property to add.
        /// @param[in] property The value of the new property.
        public void SetProperty(string name, Property property);
        /// Removes a property from the dictionary, if it exists.
        /// @param[in] name The name of the property to remove.
        public void RemoveProperty(string name);
        /// Returns the value of the property with the requested name, if one exists.
        /// @param[in] name The name of the desired property.
        public Property GetProperty(string name);

        /// Returns the number of properties in the dictionary.
        /// @return The number of properties in the dictionary.
        public int GetNumProperties();
        /// Returns the map of properties in the dictionary.
        /// @return The property map.
        public Dictionary<string, Property> GetProperties();

        /// Imports into the dictionary, and optionally defines the specificity of, potentially
        /// un-specified properties. In the case of name conflicts, the incoming properties will
        /// overwrite the existing properties if their specificity (or their forced specificity)
        /// are at least equal.
        /// @param[in] property_dictionary The properties to import.
        /// @param[in] specificity The specificity for all incoming properties. If this is not specified, the properties will keep their original specificity.
        void Import(PropertyDictionary property_dictionary, int specificity = -1);

        /// Merges the contents of another fully-specified property dictionary with this one.
        /// Properties defined in the new dictionary will overwrite those with the same name as
        /// appropriate.
        /// @param[in] property_dictionary The dictionary to merge.
        /// @param[in] specificity_offset The specificities of all incoming properties will be offset by this value.
        void Merge(PropertyDictionary property_dictionary, int specificity_offset = 0);


        // Sets a property on the dictionary and its specificity if there is no name conflict, or its
        // specificity (given by the parameter, not read from the property itself) is at least equal to
        // the specificity of the conflicting property.
        private void SetProperty(string name, Property property, int specificity);

        private Dictionary<string, Property> properties;
#endif
    }
}
