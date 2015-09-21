using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SxtaUI2.Core
{
    public class PropertyDefinition
    {
        public PropertyDefinition(string default_value, bool _inherited, bool _forces_layout)
        {
            inherited = _inherited;
            forces_layout = _forces_layout;
        }

        /// Registers a parser to parse values for this definition.
        /// @param[in] parser_name The name of the parser (default parsers are 'string', 'keyword', 'number' and 'colour').
        /// @param[in] parser_parameters A comma-separated list of validation parameters for the parser.
        /// @return This property definition.
        public PropertyDefinition AddParser(string parser_name, string parser_parameters = "")
        {
            throw new NotImplementedException();
        }


        /// Called when parsing a RCSS declaration.
        /// @param property[out] The property to set the parsed value onto.
        /// @param value[in] The raw value defined for this property.
        /// @return True if all values were parsed successfully, false otherwise.
        public bool ParseValue(Property property, string value)
        {
#if TODO
	        for (size_t i = 0; i < parsers.size(); i++)
	        {
		        if (parsers[i].parser.ParseValue(property, value, parsers[i].parameters))
		        {
			        property.definition = this;
			        property.parser_index = (int) i;
			        return true;
		        }
	        }

	        property.unit = Property::UNKNOWN;
	        return false;
#endif
            throw new NotImplementedException();
        }

        /// Called to convert a parsed property back into a value.
        /// @param value[out] The string to return the value in.
        /// @param property[in] The processed property to parse.
        /// @return True if the property was reverse-engineered successfully, false otherwise.
        public bool GetValue(out string value, Property property)
        {
#if TODO
	        value = property.value.Get< String >();

	        switch (property.unit)
	        {
		        case Property::KEYWORD:
		        {
			        if (property.parser_index < 0 || property.parser_index >= (int) parsers.size())
				        return false;

			        int keyword = property.value.Get< int >();
			        for (ParameterMap::const_iterator i = parsers[property.parser_index].parameters.begin(); i != parsers[property.parser_index].parameters.end(); ++i)
			        {
				        if ((*i).second == keyword)
				        {
					        value = (*i).first;
					        break;
				        }
			        }

			        return false;
		        }
		        break;

		        case Property::COLOUR:
		        {
			        Colourb colour = property.value.Get< Colourb >();
			        value.FormatString(32, "rgb(%d,%d,%d,%d)", colour.red, colour.green, colour.blue, colour.alpha);
		        }
		        break;

		        case Property::PX:		value.Append("px"); break;
		        case Property::EM:		value.Append("em"); break;
		        case Property::REM:		value.Append("rem"); break;
		        case Property::PERCENT:	value.Append("%"); break;
		        case Property::INCH:	value.Append("in"); break;
		        case Property::CM:		value.Append("cm"); break;
		        case Property::MM:		value.Append("mm"); break;
		        case Property::PT:		value.Append("pt"); break;
		        case Property::PC:		value.Append("pc"); break;
		        default:					break;
	        }

	        return true;
#endif
            throw new NotImplementedException();
        }

        /// Returns true if this property is inherited from parent to child elements.
        public bool IsInherited()
        {
            return inherited;
        }

        /// Returns true if this property forces a re-layout when changed.
        public bool IsLayoutForced()
        {
            return forces_layout;
        }

        /// Returns the default defined for this property.
        public Property GetDefaultValue()
        {
            return default_value;
        }


        private Property default_value;
        private bool inherited;
        private bool forces_layout;

        /*
            private struct ParserState
	        {
                public PropertyParser* parser;
                publicParameterMap parameters;
	        } 

	        std::vector< ParserState > parsers;
        */
    }
}
