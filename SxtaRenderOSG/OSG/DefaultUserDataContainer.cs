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

using System;
using System.Collections.Generic;

namespace Sxta.OSG
{
    /// <summary>
    /// Internal structure for storing all user data.
    /// </summary>
    internal class DefaultUserDataContainer : UserDataContainer
    {
        public DefaultUserDataContainer()
        {
        }

        public DefaultUserDataContainer(UserDataContainer udc, CopyOp copyop):
                base(udc, copyop)
        {
        }

        /// <summary>
        /// clone an object of the same type as the UserDataContainer.
        /// </summary>
        /// <returns></returns>
        public override BaseObject CloneType() { return new DefaultUserDataContainer(); }

        /// <summary>
        /// return a clone of a node, with BaseObject  return type.
        /// </summary>
        /// <param name="copyop"></param>
        /// <returns></returns>
        public override BaseObject Clone(CopyOp copyop) { return new DefaultUserDataContainer(this, copyop); }

        public override List<string> Descriptions
        {
            get { return descriptionList; }
            set { descriptionList = value; }
        }

        public override void AddDescription(string desc)
        {
            descriptionList.Add(desc);
        }

        public override int NumDescriptions
        {
            get { return descriptionList.Count; }
        }

        public override int SetUserObject(BaseObject obj)
        {
            // make sure that the object isn't already in the container
            int i = GetUserObjectIndex(obj);
            if (i < objectList.Count)
            {
                // object already in container so just return.
                return i;
            }

            int pos = objectList.Count;

            // object not already on user data container so add it in.
            objectList.Add(obj);

            return pos;
        }

        public override void SetUserObject(int i, BaseObject obj)
        {
            if (i < objectList.Count)
            {
                objectList[i] = obj;
            }
        }

        public override void RemoveUserObject(int i)
        {
            if (i < objectList.Count)
            {
                objectList.RemoveAt(i);
            }
        }

        public override BaseObject GetUserObject(int i)
        {
            if (i < objectList.Count)
            {
                return objectList[i];
            }
            return null;
        }

        public override int GetUserObjectIndex(BaseObject obj, int startPos = 0)
        {
            return objectList.IndexOf(obj, startPos);
        }

        public override int GetUserObjectIndex(string name, int startPos = 0)
        {
            return objectList.FindIndex(o => o.Name == name);
        }
        public override BaseObject FindUserObject(string name, int startPos = 0)
        {
            return objectList.Find(o => o.Name == name);
        }


        public override int NumUserObjects
        {
            get { return objectList.Count; }
        }

        public override object UserData
        {
            get { return userData; }
            set { userData = value; }
        }

        protected object userData;
        protected List<string> descriptionList = new List<string>();
        protected List<BaseObject> objectList = new List<BaseObject>();
    }
}