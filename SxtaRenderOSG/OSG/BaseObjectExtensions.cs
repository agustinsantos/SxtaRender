/* OpenSceneGraph - Copyright (C) 1998-2006 Robert Osfield
 *
 * This library is open source and may be redistributed and/or modified under
 * the terms of the OpenSceneGraph Public License (OSGPL) version 0.0 or
 * (at your option) any later version.  The full license is in LICENSE file
 * included with this distribution, and on the openscenegraph.org website.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * OpenSceneGraph Public License for more details.
 *
 * Ported to C#  by Agustin Santos
*/


namespace Sxta.OSG
{
    public static class BaseObjectExtensions
    {
        /// <summary>
        /// Convenience function for getting the User Object associated with specificed name from an Object's UserDataContainer.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static BaseObject GetUserObject(this BaseObject obj, string name)
        {
            UserDataContainer udc = obj.UserDataContainer;
            return udc != null ? udc.GetUserObject(name) : null;
        }
    }
}