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

namespace Sxta.OSG
{
    [Flags]
    public enum CopyOp
    {
        SHALLOW_COPY = 0,
        DEEP_COPY_OBJECTS = 1 << 0,
        DEEP_COPY_NODES = 1 << 1,
        DEEP_COPY_DRAWABLES = 1 << 2,
        DEEP_COPY_STATESETS = 1 << 3,
        DEEP_COPY_STATEATTRIBUTES = 1 << 4,
        DEEP_COPY_TEXTURES = 1 << 5,
        DEEP_COPY_IMAGES = 1 << 6,
        DEEP_COPY_ARRAYS = 1 << 7,
        DEEP_COPY_PRIMITIVES = 1 << 8,
        DEEP_COPY_SHAPES = 1 << 9,
        DEEP_COPY_UNIFORMS = 1 << 10,
        DEEP_COPY_CALLBACKS = 1 << 11,
        DEEP_COPY_USERDATA = 1 << 12,
        DEEP_COPY_ALL = 0x7FFFFFFF
    }

    public static class CopyOpUtil
    {
        public static object Copy(this CopyOp copyop, object obj)
        {
            return obj;
        }

        public static Node Copy(this CopyOp copyop, Node node)
        {
            if (node == null) return null;
            Drawable drawable = node as Drawable;
            if (drawable == null) return copyop.Copy(drawable);
            else if (copyop.HasFlag(CopyOp.DEEP_COPY_NODES)) return CloneUtil<Node>.Clone(node, copyop);
            else return node;
        }
    }
}