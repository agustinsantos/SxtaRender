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

using System.Collections.Generic;

namespace Sxta.OSG
{
    /// <summary>
    ///  Internal structure for storing all user data.
    /// </summary>
    public abstract class UserDataContainer : BaseObject
    {
        public UserDataContainer()
        {
        }

        public UserDataContainer(UserDataContainer udc, CopyOp copyop):
                base(udc, copyop)
        {
        }

        /// <summary>
        /// Add user data object. Returns the index position of object added.
        /// </summary>
        public abstract int SetUserObject(BaseObject obj);

        /// <summary>
        /// Add element to list of user data objects.
        /// </summary>
        public abstract void SetUserObject(int i, BaseObject obj);

        /// <summary>
        /// Remove element from the list of user data objects.
        /// </summary>
        /// <param name="i"></param>
        public abstract void RemoveUserObject(int i);

        /// <summary>
        /// Get user data object as specified index position.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public abstract BaseObject GetUserObject(int i);

        /// <summary>
        /// Get number of user objects assigned to this object.
        /// </summary>
        public abstract int NumUserObjects { get; }

        /// <summary>
        /// Get the index position of specified user data object.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="startPos"></param>
        /// <returns></returns>
        public abstract int GetUserObjectIndex(BaseObject obj, int startPos = 0);

        /// <summary>
        /// Get the index position of first user data object that matches specified name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="startPos"></param>
        /// <returns></returns>
        public abstract int GetUserObjectIndex(string name, int startPos = 0);
        public abstract BaseObject FindUserObject(string name, int startPos = 0);

        /// <summary>
        /// Get first user data object with specified name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="startPos"></param>
        /// <returns></returns>
        public BaseObject GetUserObject(string name, int startPos = 0)
        {
            return FindUserObject(name, startPos);
        }


        /// <summary>
        /// Get/Set the list of string descriptions.
        /// </summary>
        public abstract List<string> Descriptions { get; set; }

        /// <summary>
        /// Get number of description strings.
        /// </summary>
        public abstract int NumDescriptions { get; }

        /// <summary>
        /// Add a description string.
        /// </summary>
        /// <param name="desc"></param>
        public abstract void AddDescription(string desc);

        /// <summary>
        /// Get/Set user data.
        /// </summary>
        public new abstract object UserData { get; set; }
     }
}