using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SxtaUI2.Core
{
    /// <summary>
    /// The Factory contains a registry of instancers for different types.
    /// All instantiation of these rocket types should go through the factory
    /// so that scripting API's can bind in new types.
    /// </summary>
    public static class Factory
    {
        /// Initialise the element factory
        public static bool Initialise()
        {

            // Register the core XML node handlers.
            XMLParser.RegisterNodeHandler("", new XMLNodeHandlerDefault());
            XMLParser.RegisterNodeHandler("body", new XMLNodeHandlerBody());
            XMLParser.RegisterNodeHandler("head", new XMLNodeHandlerHead());
            XMLParser.RegisterNodeHandler("template", new XMLNodeHandlerTemplate());

            return true;
        }

        /// Cleanup and shutdown the factory
        public static void Shutdown()
        { }

#if TODO

	/// Registers the instancer to use when instancing contexts.
	/// @param[in] instancer The new context instancer.
	public static ContextInstancer* RegisterContextInstancer(ContextInstancer* instancer);
	/// Instances a new context.
	/// @param[in] name The name of the new context.
	/// @return The new context, or NULL if no context could be created.
	public static Context* InstanceContext(string name);

	/// Registers an element instancer that will be used to instance an element when the specified tag is encountered.
	/// @param[in] name Name of the instancer; elements with this as their tag will use this instancer.
	/// @param[in] instancer The instancer to call when the tag is encountered.
	/// @return The added instancer if the registration was successful, NULL otherwise.
	public static ElementInstancer* RegisterElementInstancer(string name, ElementInstancer* instancer);
	/// Returns the element instancer for the specified tag.
	/// @param[in] tag Name of the tag to get the instancer for.
	/// @return The requested element instancer, or NULL if no such instancer is registered.
	public static ElementInstancer* GetElementInstancer(string tag);

#endif
        /// Instances a single element.
        /// @param[in] parent The parent of the new element, or NULL for a root tag.
        /// @param[in] instancer The name of the instancer to create the element with.
        /// @param[in] tag The tag of the element to be instanced.
        /// @param[in] attributes The attributes to instance the element with.
        /// @return The instanced element, or NULL if the instancing failed.
        public static Element InstanceElement(Element parent, string instancer, string tag, XMLAttributes attributes)
        {
#if TODO
            ElementInstancer instancer = GetElementInstancer(instancer_name);

            if (instancer != null)
            {
                Element element = instancer.InstanceElement(parent, tag, attributes);

                // Process the generic attributes and bind any events
                if (element != null)
                {
                    element.SetInstancer(instancer);
                    element.SetAttributes(&attributes);
                    ElementUtilities.BindEventAttributes(element);

                    PluginRegistry.NotifyElementCreate(element);
                }

                return element;
            }
#endif
            return null;
        }
        /// Instances a single text element containing a string. The string is assumed to contain no RML markup, but will
        /// be translated and therefore may have some introduced. In this case more than one element may be instanced.
        /// @param[in] parent The element any instanced elements will be parented to.
        /// @param[in] text The text to instance the element (or elements) from.
        /// @return True if the string was parsed without error, false otherwise.
        public static bool InstanceElementText(Element parent, string text)
        {
            return true;
        }

        /// Instances a document from a stream.
        /// @param[in] context The context that is creating the document.
        /// @param[in] stream The stream to instance from.
        /// @return The instanced document, or NULL if an error occurred.
        public static ElementDocument InstanceDocumentStream(Context context, StreamReader stream)
        {
            Element element = Factory.InstanceElement(null, "body", "body", new XMLAttributes());
            if (element == null)
            {
                //Log::Message(Log::LT_ERROR, "Failed to instance document, instancer returned NULL.");
                return null;
            }

            ElementDocument document = element as ElementDocument;
            if (document == null)
            {
                //Log::Message(Log::LT_ERROR, "Failed to instance document element. Found type '%s', was expecting derivative of ElementDocument.", typeid(element).name());
                return null;
            }

            document.lock_layout = true;
            document.context = context;

            XMLParser parser = new XMLParser(element);
            throw new NotImplementedException();
            //parser.Parse(stream);

            document.lock_layout = false;

            return document;
        }

#if TODO
	/// Registers an instancer that will be used to instance decorators.
	/// @param[in] name The name of the decorator the instancer will be called for.
	/// @param[in] instancer The instancer to call when the decorator name is encountered.
	/// @return The added instancer if the registration was successful, NULL otherwise.
	public static DecoratorInstancer* RegisterDecoratorInstancer(string name, DecoratorInstancer* instancer);
	/// Attempts to instance a decorator from an instancer registered with the factory.
	/// @param[in] name The name of the desired decorator type.
	/// @param[in] properties The properties associated with the decorator.
	/// @return The newly instanced decorator, or NULL if the decorator could not be instanced.
	public static Decorator* InstanceDecorator(string name, const PropertyDictionary& properties);

	/// Registers an instancer that will be used to instance font effects.
	/// @param[in] name The name of the font effect the instancer will be called for.
	/// @param[in] instancer The instancer to call when the font effect name is encountered.
	/// @return The added instancer if the registration was successful, NULL otherwise.
	public static FontEffectInstancer* RegisterFontEffectInstancer(string name, FontEffectInstancer* instancer);
	/// Attempts to instance a font effect from an instancer registered with the factory.
	/// @param[in] name The name of the desired font effect type.
	/// @param[in] properties The properties associated with the font effect.
	/// @return The newly instanced font effect, or NULL if the font effect could not be instanced.
	public static FontEffect* InstanceFontEffect(string name, const PropertyDictionary& properties);

	/// Creates a style sheet from a user-generated string.
	/// @param[in] string The contents of the style sheet.
	/// @return A pointer to the newly created style sheet.
	public public static StyleSheet* InstanceStyleSheetString(string string);
	/// Creates a style sheet from a file.
	/// @param[in] file_name The location of the style sheet file.
	/// @return A pointer to the newly created style sheet.
	public static StyleSheet* InstanceStyleSheetFile(string file_name);
	/// Creates a style sheet from an Stream.
	/// @param[in] stream A pointer to the stream containing the style sheet's contents.
	/// @return A pointer to the newly created style sheet.
	public static StyleSheet* InstanceStyleSheetStream(Stream* stream);
	/// Clears the style sheet cache. This will force style sheets to be reloaded.
	public static void ClearStyleSheetCache();
	/// Clears the template cache. This will force template to be reloaded.
	public static void ClearTemplateCache();

	/// Registers an instancer for all events.
	/// @param[in] instancer The instancer to be called.
	/// @return The registered instanced on success, NULL on failure.
	public static EventInstancer* RegisterEventInstancer(EventInstancer* instancer);
	/// Instance and event object
	/// @param[in] target Target element of this event.
	/// @param[in] name Name of this event.
	/// @param[in] parameters Additional parameters for this event.
	/// @param[in] interruptible If the event propagation can be stopped.
	/// @return The instanced event.
	public static Event* InstanceEvent(Element* target, string name, const Dictionary& parameters, bool interruptible);

	/// Register the instancer to be used for all event listeners.
	/// @return The registered instancer on success, NULL on failure.
	public static EventListenerInstancer* RegisterEventListenerInstancer(EventListenerInstancer* instancer);
	/// Instance an event listener with the given string. This is used for instancing listeners for the on* events from
	/// RML.
	/// @param[in] value The parameters to the event listener.
	/// @return The instanced event listener.
	public static EventListener* InstanceEventListener(string value, Element* element);
#endif
    }
}
