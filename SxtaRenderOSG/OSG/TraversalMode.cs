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
    public enum TraversalMode
    {
        /// <summary>
        /// Visit the current node, no traversal.
        /// </summary>
        TRAVERSE_NONE,
        /// <summary>
        /// Backtracks from current node until arriving up to the root node
        /// </summary>
        TRAVERSE_PARENTS,
        /// <summary>
        /// Traverse all children of the node
        /// </summary>
        TRAVERSE_ALL_CHILDREN,
        /// <summary>
        /// Traverse all active children of the node
        /// </summary>
        TRAVERSE_ACTIVE_CHILDREN
    }
}