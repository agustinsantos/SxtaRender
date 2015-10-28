using Sxta.Math;
using System;

namespace Sxta.Render.OpenGLExt
{
    #region Support Classes

    public struct Vertex_V3N3f
    {
        public Vector3f Position;
        public Vector3f Normal;
        public static int SizeInBytes
        {
            get { return Vector3f.SizeInBytes + Vector3f.SizeInBytes; }
        }
    }

    public struct Vertex_V3N3T2f
    {
        public Vector3f Position;
        public Vector3f Normal;
        public Vector2f TexCoord;
        public static int SizeInBytes
        {
            get { return Vector3f.SizeInBytes + Vector3f.SizeInBytes + Vector2f.SizeInBytes; }
        }
    }
    #endregion

    public static class MeshUtils
    {

        #region Public Members
        /// <summary>
        /// Generates a solid sphere.
        /// </summary>
        /// <returns>The solid sphere.</returns>
        /// <param name="radius">Radius.</param>
        /// <param name="slices">Slices.</param>
        /// <param name="stacks">Stacks.</param>
        public static Mesh<Vertex_V3N3T2f, ushort> GenerateSolidSphere(double radius, int slices, int stacks)
        {
            return SphereSolidMode((float)radius, slices, stacks);
        }

        /// <summary>
        /// Generates a wire sphere.
        /// </summary>
        /// <returns>The wire sphere.</returns>
        /// <param name="radius">Radius.</param>
        /// <param name="slices">Slices.</param>
        /// <param name="stacks">Stacks.</param>
        public static Mesh<Vertex_V3N3f, ushort> GenerateWireSphere(double radius, int slices, int stacks)
        {
            return SphereWireMode((float)radius, slices, stacks);

        }

        /// <summary>
        /// Generates a solid cone.
        /// </summary>
        /// <returns>The solid cone.</returns>
        /// <param name="base_">Base_.</param>
        /// <param name="height">Height.</param>
        /// <param name="slices">Slices.</param>
        /// <param name="stacks">Stacks.</param>
        public static Mesh<Vertex_V3N3T2f, ushort> GenerateSolidCone(double base_, double height, int slices, int stacks)
        {
            return ConeSolidMode((float)base_, (float)height, slices, stacks);
        }

        /// <summary>
        /// Generates a wire cone.
        /// </summary>
        /// <returns>The wire cone.</returns>
        /// <param name="base_">Base_.</param>
        /// <param name="height">Height.</param>
        /// <param name="slices">Slices.</param>
        /// <param name="stacks">Stacks.</param>
        public static Mesh<Vertex_V3N3f, ushort> GenerateWireCone(double base_, double height, int slices, int stacks)
        {
            return ConeWireMode((float)base_, (float)height, slices, stacks);
        }

        /// <summary>
        /// Generates a solid cylinder.
        /// </summary>
        /// <returns>The solid cylinder.</returns>
        /// <param name="radius">Radius.</param>
        /// <param name="height">Height.</param>
        /// <param name="slices">Slices.</param>
        /// <param name="stacks">Stacks.</param>
        public static Mesh<Vertex_V3N3T2f, ushort> GenerateSolidCylinder(double radius, double height, int slices, int stacks)
        {
            return CylinderSolidMode((float)radius, (float)height, slices, stacks);
        }

        /// <summary>
        /// Generates a wire cylinder.
        /// </summary>
        /// <returns>The wire cylinder.</returns>
        /// <param name="radius">Radius.</param>
        /// <param name="height">Height.</param>
        /// <param name="slices">Slices.</param>
        /// <param name="stacks">Stacks.</param>
        public static Mesh<Vertex_V3N3f, ushort> GenerateWireCylinder(double radius, double height, int slices, int stacks)
        {
            return CylinderWireMode((float)radius, (float)height, slices, stacks);
        }

        /// <summary>
        /// Generates a wire torus.
        /// </summary>
        /// <returns>The wire torus.</returns>
        /// <param name="dInnerRadius">D inner radius.</param>
        /// <param name="dOuterRadius">D outer radius.</param>
        /// <param name="nSides">N sides.</param>
        /// <param name="nRings">N rings.</param>
        public static Mesh<Vertex_V3N3f, ushort> GenerateWireTorus(double dInnerRadius, double dOuterRadius, int nSides, int nRings)
        {
            return TorusWire((float)dInnerRadius, (float)dOuterRadius, nSides, nRings);
        }

        /// <summary>
        /// Generates a solid torus.
        /// </summary>
        /// <returns>The solid torus.</returns>
        /// <param name="dInnerRadius">D inner radius.</param>
        /// <param name="dOuterRadius">D outer radius.</param>
        /// <param name="nSides">N sides.</param>
        /// <param name="nRings">N rings.</param>
        public static Mesh<Vertex_V3N3T2f, ushort> GenerateSolidTorus(double dInnerRadius, double dOuterRadius, int nSides, int nRings)
        {
            return TorusSolid((float)dInnerRadius, (float)dOuterRadius, nSides, nRings);
        }

        /// <summary>
        /// Generates a wire cube.
        /// </summary>
        /// <returns>The wire cube.</returns>
        /// <param name="dSize">D size.</param>
        public static Mesh<Vertex_V3N3f, ushort> GenerateWireCube(double dSize)
        {
            float[] vertices;

            if (!cubeCached)
            {
                fghCubeGenerate();
                cubeCached = true;
            }

            if (dSize != 1.0f)
            {
                /* Need to build new vertex list containing vertices for cube of different size */
                int i;

                vertices = new float[CUBE_VERT_ELEM_PER_OBJ];

                for (i = 0; i < CUBE_VERT_ELEM_PER_OBJ; i++)
                    vertices[i] = (float)(dSize * cube_verts[i]);
            }
            else
                vertices = cube_verts;

            return MeshGeometryWire(vertices, cube_norms, CUBE_VERT_PER_OBJ,
                                        null, CUBE_NUM_FACES, CUBE_NUM_EDGE_PER_FACE, MeshMode.LINE_LOOP,
                                        null, 0, 0);
        }

        /// <summary>
        /// Generates a solid cube.
        /// </summary>
        /// <returns>The solid cube.</returns>
        /// <param name="dSize">D size.</param>
        public static Mesh<Vertex_V3N3T2f, ushort> GenerateSolidCube(double dSize)
        {
            float[] vertices;

            if (!cubeCached)
            {
                fghCubeGenerate();
                cubeCached = true;
            }

            if (dSize != 1.0f)
            {
                /* Need to build new vertex list containing vertices for cube of different size */
                int i;

                vertices = new float[CUBE_VERT_ELEM_PER_OBJ];

                for (i = 0; i < CUBE_VERT_ELEM_PER_OBJ; i++)
                    vertices[i] = (float)(dSize * cube_verts[i]);
            }
            else
                vertices = cube_verts;

            return MeshGeometrySolid(vertices, cube_norms, null, CUBE_VERT_PER_OBJ,
                                             cube_vertIdxs, 1, CUBE_VERT_PER_OBJ_TRI);
        }

        /// <summary>
        /// Generates a wire dodecahedron.
        /// </summary>
        /// <returns>The wire dodecahedron.</returns>
        public static Mesh<Vertex_V3N3f, ushort> GenerateWireDodecahedron()
        {
            if (!dodecahedronCached)
            {
                fghDodecahedronGenerate();
                dodecahedronCached = true;
            }

            return MeshGeometryWire(dodecahedron_verts, dodecahedron_norms, DODECAHEDRON_VERT_PER_OBJ,
                                        null, DODECAHEDRON_NUM_FACES, DODECAHEDRON_NUM_EDGE_PER_FACE, MeshMode.LINE_LOOP,
                                        null, 0, 0);
        }

        /// <summary>
        /// Generates a solid dodecahedron.
        /// </summary>
        /// <returns>The solid dodecahedron.</returns>
        public static Mesh<Vertex_V3N3T2f, ushort> GenerateSolidDodecahedron()
        {
            if (!dodecahedronCached)
            {
                fghDodecahedronGenerate();
                dodecahedronCached = true;
            }

            return MeshGeometrySolid(dodecahedron_verts, dodecahedron_norms, null, DODECAHEDRON_VERT_PER_OBJ,
                                         dodecahedron_vertIdxs, 1, DODECAHEDRON_VERT_PER_OBJ_TRI);
        }

        /// <summary>
        /// Generates a wire icosahedron.
        /// </summary>
        /// <returns>The wire icosahedron.</returns>
        public static Mesh<Vertex_V3N3f, ushort> GenerateWireIcosahedron()
        {
            if (!icosahedronCached)
            {
                fghIcosahedronGenerate();
                icosahedronCached = true;
            }
            return MeshGeometryWire(icosahedron_verts, icosahedron_norms, ICOSAHEDRON_VERT_PER_OBJ,
                                        null, ICOSAHEDRON_NUM_FACES, ICOSAHEDRON_NUM_EDGE_PER_FACE, MeshMode.LINE_LOOP,
                                        null, 0, 0);
        }

        /// <summary>
        /// Generates a solid icosahedron.
        /// </summary>
        /// <returns>The solid icosahedron.</returns>
        public static Mesh<Vertex_V3N3T2f, ushort> GenerateSolidIcosahedron()
        {
            if (!icosahedronCached)
            {
                fghIcosahedronGenerate();
                icosahedronCached = true;
            }
            return MeshGeometrySolid(icosahedron_verts, icosahedron_norms, null, ICOSAHEDRON_VERT_PER_OBJ,
                                         null, 1, ICOSAHEDRON_VERT_PER_OBJ_TRI);
        }

        /// <summary>
        /// Generates a wire octahedron.
        /// </summary>
        /// <returns>The wire octahedron.</returns>
        public static Mesh<Vertex_V3N3f, ushort> GenerateWireOctahedron()
        {
            if (!octahedronCached)
            {
                fghOctahedronGenerate();
                octahedronCached = true;
            }
            return MeshGeometryWire(octahedron_verts, octahedron_norms, OCTAHEDRON_VERT_PER_OBJ,
                                        null, OCTAHEDRON_NUM_FACES, OCTAHEDRON_NUM_EDGE_PER_FACE, MeshMode.LINE_LOOP,
                                        null, 0, 0);
        }

        /// <summary>
        /// Generates a solid octahedron.
        /// </summary>
        /// <returns>The solid octahedron.</returns>
        public static Mesh<Vertex_V3N3T2f, ushort> GenerateSolidOctahedron()
        {
            if (!octahedronCached)
            {
                fghOctahedronGenerate();
                octahedronCached = true;
            }

            return MeshGeometrySolid(octahedron_verts, octahedron_norms, null, OCTAHEDRON_VERT_PER_OBJ,
                                         null, 1, OCTAHEDRON_VERT_PER_OBJ_TRI);
        }
        /// <summary>
        /// Generates a wire rhombic dodecahedron.
        /// </summary>
        /// <returns>The wire rhombic dodecahedron.</returns>
        public static Mesh<Vertex_V3N3f, ushort> GenerateWireRhombicDodecahedron()
        {
            if (!rhombicdodecahedronCached)
            {
                fghRhombicDodecahedronGenerate();
                rhombicdodecahedronCached = true;
            }

            return MeshGeometryWire(rhombicdodecahedron_verts, rhombicdodecahedron_norms, RHOMBICDODECAHEDRON_VERT_PER_OBJ,
                                        null, RHOMBICDODECAHEDRON_NUM_FACES, RHOMBICDODECAHEDRON_NUM_EDGE_PER_FACE, MeshMode.LINE_LOOP,
                                        null, 0, 0);
        }

        /// <summary>
        /// Generates a solid rhombic dodecahedron.
        /// </summary>
        /// <returns>The solid rhombic dodecahedron.</returns>
        public static Mesh<Vertex_V3N3T2f, ushort> GenerateSolidRhombicDodecahedron()
        {
            if (!rhombicdodecahedronCached)
            {
                fghRhombicDodecahedronGenerate();
                rhombicdodecahedronCached = true;
            }

            return MeshGeometrySolid(rhombicdodecahedron_verts, rhombicdodecahedron_norms, null, RHOMBICDODECAHEDRON_VERT_PER_OBJ,
                                         rhombicdodecahedron_vertIdxs, 1, RHOMBICDODECAHEDRON_VERT_PER_OBJ_TRI);

        }

        /// <summary>
        /// Generates a wire tetrahedron.
        /// </summary>
        /// <returns>The wire tetrahedron.</returns>
        public static Mesh<Vertex_V3N3f, ushort> GenerateWireTetrahedron()
        {
            if (!tetrahedronCached)
            {
                fghTetrahedronGenerate();
                tetrahedronCached = true;
            }

            return MeshGeometryWire(tetrahedron_verts, tetrahedron_norms, TETRAHEDRON_VERT_PER_OBJ,
                                        null, TETRAHEDRON_NUM_FACES, TETRAHEDRON_NUM_EDGE_PER_FACE, MeshMode.LINE_LOOP,
                                        null, 0, 0);
        }

        /// <summary>
        /// Generates a solid tetrahedron.
        /// </summary>
        /// <returns>The solid tetrahedron.</returns>
        public static Mesh<Vertex_V3N3T2f, ushort> GenerateSolidTetrahedron()
        {
            if (!tetrahedronCached)
            {
                fghTetrahedronGenerate();
                tetrahedronCached = true;
            }
            return MeshGeometrySolid(tetrahedron_verts, tetrahedron_norms, null, TETRAHEDRON_VERT_PER_OBJ,
                                             null, 1, TETRAHEDRON_VERT_PER_OBJ_TRI);
        }

        #endregion

        #region Sphere Generation

        /// <summary>
        /// Compute lookup table of cos and sin values forming a circle
        ///  (or half circle if halfCircle==TRUE)
        /// Notes:
        ///    It is the responsibility of the caller to free these tables
        ///   The size of the table is (n+1) to form a connected loop
        ///   The last entry is exactly the same as the first
        ///   The sign of n can be flipped to get the reverse loop
        /// </summary>
        /// <param name="sint">Sint.</param>
        /// <param name="cost">Cost.</param>
        /// <param name="n">N.</param>
        /// <param name="halfCircle">If set to <c>true</c> half circle.</param>
        private static void CircleTable(out double[] sint, out double[] cost, int n, bool halfCircle)
        {
            int i;

            /* Table size, the sign of n flips the circle direction */
            int size = System.Math.Abs(n);

            /* Determine the angle between samples */
            double angle = (halfCircle ? 1 : 2) * System.Math.PI / ((n == 0) ? 1 : n);

            /* Allocate memory for n samples, plus duplicate of first entry at the end */
            sint = new double[size + 1];
            cost = new double[size + 1];


            /* Compute cos and sin around the circle */
            sint[0] = 0.0f;
            cost[0] = 1.0f;

            for (i = 1; i < size; i++)
            {
                sint[i] = System.Math.Sin(angle * i);
                cost[i] = System.Math.Cos(angle * i);
            }


            if (halfCircle)
            {
                sint[size] = 0.0;  /* sin PI */
                cost[size] = -1.0;  /* cos PI */
            }
            else
            {
                /* Last sample is duplicate of the first (sin or cos of 2 PI) */
                sint[size] = sint[0];
                cost[size] = cost[0];
            }
        }

        private static void GenerateSphere(float radius, int slices, int stacks, out float[] vertices, out float[] normals, out int nVert)
        {
            int i, j;
            int idx = 0;    /* idx into vertex/normal buffer */
            float x, y, z;

            /* Pre-computed circle */
            double[] sint1, cost1;
            double[] sint2, cost2;

            /* number of unique vertices */
            if (slices == 0 || stacks < 2)
            {
                /* nothing to generate */
                nVert = 0;
                vertices = null;
                normals = null;
                return;
            }
            nVert = slices * (stacks - 1) + 2;
            if (nVert > 65535)
                /*
                 * limit of glushort, thats 256*256 subdivisions, should be enough in practice. See note above
                 */
                Console.WriteLine("GenerateSphere: too many slices or stacks requested, indices will wrap");

            /* precompute values on unit circle */
            CircleTable(out sint1, out cost1, -slices, false);
            CircleTable(out sint2, out cost2, stacks, true);

            /* Allocate vertex and normal buffers, bail out if memory allocation fails */
            vertices = new float[3 * nVert];
            normals = new float[3 * nVert];


            /* top */
            vertices[0] = 0.0f;
            vertices[1] = 0.0f;
            vertices[2] = radius;
            normals[0] = 0.0f;
            normals[1] = 0.0f;
            normals[2] = 1.0f;
            idx = 3;

            /* each stack */
            for (i = 1; i < stacks; i++)
            {
                for (j = 0; j < slices; j++, idx += 3)
                {
                    x = (float)(cost1[j] * sint2[i]);
                    y = (float)(sint1[j] * sint2[i]);
                    z = (float)(cost2[i]);

                    vertices[idx] = x * radius;
                    vertices[idx + 1] = y * radius;
                    vertices[idx + 2] = z * radius;
                    normals[idx] = x;
                    normals[idx + 1] = y;
                    normals[idx + 2] = z;
                }
            }

            /* bottom */
            vertices[idx] = 0.0f;
            vertices[idx + 1] = 0.0f;
            vertices[idx + 2] = -radius;
            normals[idx] = 0.0f;
            normals[idx + 1] = 0.0f;
            normals[idx + 2] = -1.0f;

        }

        private static Mesh<Vertex_V3N3f, ushort> SphereWireMode(float radius, int slices, int stacks)
        {
            int i, j, idx, nVert;
            float[] vertices, normals;

            /* Generate vertices and normals */
            GenerateSphere(radius, slices, stacks, out vertices, out normals, out nVert);

            if (nVert == 0)
                /* nothing to draw */
                return null;
            ushort[] sliceIdx, stackIdx;
            /* First, generate vertex index arrays for drawing with glDrawElements
             * We have a bunch of line_loops to draw for each stack, and a
             * bunch for each slice.
             */

            sliceIdx = new ushort[slices * (stacks + 1)];
            stackIdx = new ushort[slices * (stacks - 1)];

            /* generate for each stack */
            for (i = 0, idx = 0; i < stacks - 1; i++)
            {
                ushort offset = (ushort)(1 + i * slices);           /* start at 1 (0 is top vertex), and we advance one stack down as we go along */
                for (j = 0; j < slices; j++, idx++)
                {
                    stackIdx[idx] = (ushort)(offset + j);
                }
            }

            /* generate for each slice */
            for (i = 0, idx = 0; i < slices; i++)
            {
                ushort offset = (ushort)(1 + i);        /* start at 1 (0 is top vertex), and we advance one slice as we go along */
                sliceIdx[idx++] = 0;                    /* vertex on top */
                for (j = 0; j < stacks - 1; j++, idx++)
                {
                    sliceIdx[idx] = (ushort)(offset + j * slices);
                }
                sliceIdx[idx++] = (ushort)(nVert - 1);              /* zero based index, last element in array... */
            }

            /* draw */
            return MeshGeometryWire(vertices, normals, nVert,
                                 sliceIdx, slices, stacks + 1, MeshMode.LINE_STRIP,
                                 stackIdx, stacks - 1, slices);
        }

        private static Mesh<Vertex_V3N3T2f, ushort> SphereSolidMode(float radius, int slices, int stacks)
        {
            int i, j, idx, nVert;
            float[] vertices, normals;

            /* Generate vertices and normals */
            GenerateSphere(radius, slices, stacks, out vertices, out normals, out nVert);

            if (nVert == 0)
                /* nothing to draw */
                return null;

            /* First, generate vertex index arrays for drawing with glDrawElements
             * All stacks, including top and bottom are covered with a triangle
             * strip.
             */
            ushort[] stripIdx;
            /* Create index vector */
            ushort offset;

            /* Allocate buffers for indices, bail out if memory allocation fails */
            stripIdx = new ushort[(slices + 1) * 2 * (stacks)];


            /* top stack */
            for (j = 0, idx = 0; j < slices; j++, idx += 2)
            {
                stripIdx[idx] = (ushort)(j + 1);              /* 0 is top vertex, 1 is first for first stack */
                stripIdx[idx + 1] = 0;
            }
            stripIdx[idx] = 1;                    /* repeat first slice's idx for closing off shape */
            stripIdx[idx + 1] = 0;
            idx += 2;

            /* middle stacks: */
            /* Strip indices are relative to first index belonging to strip, NOT relative to first vertex/normal pair in array */
            for (i = 0; i < stacks - 2; i++, idx += 2)
            {
                offset = (ushort)(1 + i * slices);                    /* triangle_strip indices start at 1 (0 is top vertex), and we advance one stack down as we go along */
                for (j = 0; j < slices; j++, idx += 2)
                {
                    stripIdx[idx] = (ushort)(offset + j + slices);
                    stripIdx[idx + 1] = (ushort)(offset + j);
                }
                stripIdx[idx] = (ushort)(offset + slices);        /* repeat first slice's idx for closing off shape */
                stripIdx[idx + 1] = offset;
            }

            /* bottom stack */
            offset = (ushort)(1 + (stacks - 2) * slices);               /* triangle_strip indices start at 1 (0 is top vertex), and we advance one stack down as we go along */
            for (j = 0; j < slices; j++, idx += 2)
            {
                stripIdx[idx] = (ushort)(nVert - 1);              /* zero based index, last element in array (bottom vertex)... */
                stripIdx[idx + 1] = (ushort)(offset + j);
            }
            stripIdx[idx] = (ushort)(nVert - 1);                  /* repeat first slice's idx for closing off shape */
            stripIdx[idx + 1] = offset;


            return MeshGeometrySolid(vertices, normals, null, nVert, stripIdx, stacks, (slices + 1) * 2, MeshMode.TRIANGLE_STRIP);
        }
        #endregion

        #region MeshGenereration

        public static Mesh<Vertex_V3N3f, ushort> MeshGeometryWire(float[] vertices, float[] normals, int numVertices,
                                                          ushort[] vertIdxs, int numParts, int numVertPerPart, MeshMode vertexMode,
                                                          ushort[] vertIdxs2, int numParts2, int numVertPerPart2)
        {
            Mesh<Vertex_V3N3f, ushort> mesh = new Mesh<Vertex_V3N3f, ushort>(Vertex_V3N3f.SizeInBytes, sizeof(ushort), vertexMode, MeshUsage.GPU_STATIC);
            mesh.addAttributeType(0, 3, AttributeType.A32F, false);
            mesh.addAttributeType(1, 3, AttributeType.A32F, false);

            for (int i = 0; i < numVertices; i += 1)
            {
                mesh.addVertex(new Vertex_V3N3f()
                {
                    Position = new Vector3f(vertices[i * 3], vertices[i * 3 + 1], vertices[i * 3 + 2]),
                    Normal = new Vector3f(normals[i * 3], normals[i * 3 + 1], normals[i * 3 + 2])
                });
            }
            if (vertIdxs != null && vertIdxs.Length > 0)
                mesh.addIndices(vertIdxs);
            return mesh;
        }

        public static Mesh<Vertex_V3N3T2f, ushort> MeshGeometrySolid(float[] vertices, float[] normals, float[] textcs, int numVertices,
                                                                      ushort[] vertIdxs, int numParts, int numVertIdxsPerPart, MeshMode meshMode = MeshMode.TRIANGLES)
        {
            Mesh<Vertex_V3N3T2f, ushort> mesh = new Mesh<Vertex_V3N3T2f, ushort>(Vertex_V3N3T2f.SizeInBytes, sizeof(ushort), meshMode, MeshUsage.GPU_STATIC, numVertices, vertIdxs.Length);
            mesh.addAttributeType(0, 3, AttributeType.A32F, false);
            mesh.addAttributeType(1, 3, AttributeType.A32F, false);
            mesh.addAttributeType(2, 2, AttributeType.A32F, false);

            if (textcs != null)
            {
                mesh.addAttributeType(2, 2, AttributeType.A32F, false);

                for (int i = 0; i < numVertices; i += 1)
                {
                    mesh.addVertex(new Vertex_V3N3T2f()
                    {
                        Position = new Vector3f(vertices[i * 3], vertices[i * 3 + 1], vertices[i * 3 + 2]),
                        Normal = new Vector3f(normals[i * 3], normals[i * 3 + 1], normals[i * 3 + 2]),
                        TexCoord = new Vector2f(textcs[i * 2], textcs[i * 2 + 1])
                    });
                }
            }
            else
            {
                for (int i = 0; i < numVertices; i += 1)
                {
                    mesh.addVertex(new Vertex_V3N3T2f()
                    {
                        Position = new Vector3f(vertices[i * 3], vertices[i * 3 + 1], vertices[i * 3 + 2]),
                        Normal = new Vector3f(normals[i * 3], normals[i * 3 + 1], normals[i * 3 + 2]),
                        TexCoord = new Vector2f(0, 0)
                    });
                }
            }
            if (vertIdxs != null && vertIdxs.Length > 0)
                mesh.addIndices(vertIdxs);
            return mesh;
        }
        #endregion

        #region Curved 3D Shapes
        static Mesh<Vertex_V3N3f, ushort> ConeWireMode(float base_, float height, int slices, int stacks)
        {
            int i, j, idx, nVert;
            float[] vertices, normals;

            /* Generate vertices and normals */
            /* Note, (stacks+1)*slices vertices for side of object, slices+1 for top and bottom closures */
            fghGenerateCone(base_, height, slices, stacks, out vertices, out normals, out nVert);

            if (nVert == 0)
                /* nothing to draw */
                return null;

            ushort[] sliceIdx, stackIdx;
            /* First, generate vertex index arrays for drawing with glDrawElements
             * We have a bunch of line_loops to draw for each stack, and a
             * bunch for each slice.
             */

            stackIdx = new ushort[slices * stacks];
            sliceIdx = new ushort[slices * 2];


            /* generate for each stack */
            for (i = 0, idx = 0; i < stacks; i++)
            {
                ushort offset = (ushort)(1 + (i + 1) * slices);       /* start at 1 (0 is top vertex), and we advance one stack down as we go along */
                for (j = 0; j < slices; j++, idx++)
                {
                    stackIdx[idx] = (ushort)(offset + j);
                }
            }

            /* generate for each slice */
            for (i = 0, idx = 0; i < slices; i++)
            {
                ushort offset = (ushort)(1 + i);                  /* start at 1 (0 is top vertex), and we advance one slice as we go along */
                sliceIdx[idx++] = (ushort)(offset + slices);
                sliceIdx[idx++] = (ushort)(offset + (stacks + 1) * slices);
            }

            /* draw */
            return MeshGeometryWire(vertices, normals, nVert,
                                        sliceIdx, 1, slices * 2, MeshMode.LINES,
                                        stackIdx, stacks, slices);
        }

        private static Mesh<Vertex_V3N3T2f, ushort> ConeSolidMode(float base_, float height, int slices, int stacks)
        {
            int i, j, idx, nVert;
            float[] vertices, normals;

            /* Generate vertices and normals */
            /* Note, (stacks+1)*slices vertices for side of object, slices+1 for top and bottom closures */
            fghGenerateCone(base_, height, slices, stacks, out vertices, out normals, out nVert);

            if (nVert == 0)
                /* nothing to draw */
                return null;
            /* First, generate vertex index arrays for drawing with glDrawElements
            * All stacks, including top and bottom are covered with a triangle
            * strip.
            */
            ushort[] stripIdx;
            /* Create index vector */
            ushort offset;

            /* Allocate buffers for indices, bail out if memory allocation fails */
            stripIdx = new ushort[(slices + 1) * 2 * (stacks + 1)];    /*stacks +1 because of closing off bottom */


            /* top stack */
            for (j = 0, idx = 0; j < slices; j++, idx += 2)
            {
                stripIdx[idx] = 0;
                stripIdx[idx + 1] = (ushort)(j + 1);              /* 0 is top vertex, 1 is first for first stack */
            }
            stripIdx[idx] = 0;                    /* repeat first slice's idx for closing off shape */
            stripIdx[idx + 1] = 1;
            idx += 2;

            /* middle stacks: */
            /* Strip indices are relative to first index belonging to strip, NOT relative to first vertex/normal pair in array */
            for (i = 0; i < stacks; i++, idx += 2)
            {
                offset = (ushort)(1 + (i + 1) * slices);                /* triangle_strip indices start at 1 (0 is top vertex), and we advance one stack down as we go along */
                for (j = 0; j < slices; j++, idx += 2)
                {
                    stripIdx[idx] = (ushort)(offset + j);
                    stripIdx[idx + 1] = (ushort)(offset + j + slices);
                }
                stripIdx[idx] = offset;               /* repeat first slice's idx for closing off shape */
                stripIdx[idx + 1] = (ushort)(offset + slices);
            }

            /* draw */
            return MeshGeometrySolid(vertices, normals, null, nVert, stripIdx, stacks + 1, (slices + 1) * 2, MeshMode.TRIANGLE_STRIP);
        }

        private static void fghGenerateCone(float base_, float height, int slices, int stacks,   /*  input */
                                    out float[] vertices, out float[] normals, out int nVert)           /* output */
        {
            int i, j;
            int idx = 0;    /* idx into vertex/normal buffer */

            /* Pre-computed circle */
            double[] sint, cost;

            /* Step in z and radius as stacks are drawn. */
            float z = 0;
            float r = (float)base_;

            float zStep = (float)height / ((stacks > 0) ? stacks : 1);
            float rStep = (float)base_ / ((stacks > 0) ? stacks : 1);

            /* Scaling factors for vertex normals */
            float cosn = (float)(height / System.Math.Sqrt(height * height + base_ * base_));
            float sinn = (float)(base_ / System.Math.Sqrt(height * height + base_ * base_));



            /* number of unique vertices */
            if (slices == 0 || stacks < 1)
            {
                /* nothing to generate */
                nVert = 0;
                vertices = null;
                normals = null;
                return;
            }
            nVert = slices * (stacks + 2) + 1;   /* need an extra stack for closing off bottom with correct normals */

            if (nVert > 65535)
                /*
                 * limit of glushort, thats 256*256 subdivisions, should be enough in practice. See note above
                 */
                Console.WriteLine("fghGenerateCone: too many slices or stacks requested, indices will wrap");

            /* Pre-computed circle */
            CircleTable(out sint, out cost, -slices, false);

            /* Allocate vertex and normal buffers, bail out if memory allocation fails */
            vertices = new float[nVert * 3];
            normals = new float[nVert * 3];

            /* bottom */
            vertices[0] = 0.0f;
            vertices[1] = 0.0f;
            vertices[2] = z;
            normals[0] = 0.0f;
            normals[1] = 0.0f;
            normals[2] = -1.0f;
            idx = 3;
            /* other on bottom (get normals right) */
            for (j = 0; j < slices; j++, idx += 3)
            {
                vertices[idx] = (float)(cost[j] * r);
                vertices[idx + 1] = (float)(sint[j] * r);
                vertices[idx + 2] = z;
                normals[idx] = 0.0f;
                normals[idx + 1] = 0.0f;
                normals[idx + 2] = -1.0f;
            }

            /* each stack */
            for (i = 0; i < stacks + 1; i++)
            {
                for (j = 0; j < slices; j++, idx += 3)
                {
                    vertices[idx] = (float)(cost[j] * r);
                    vertices[idx + 1] = (float)(sint[j] * r);
                    vertices[idx + 2] = z;
                    normals[idx] = (float)(cost[j] * cosn);
                    normals[idx + 1] = (float)(sint[j] * cosn);
                    normals[idx + 2] = sinn;
                }

                z += zStep;
                r -= rStep;
            }
        }

        private static Mesh<Vertex_V3N3f, ushort> CylinderWireMode(float radius, float height, int slices, int stacks)
        {
            int i, j, idx, nVert;
            float[] vertices, normals;

            /* Generate vertices and normals */
            /* Note, (stacks+1)*slices vertices for side of object, 2*slices+2 for top and bottom closures */
            fghGenerateCylinder(radius, height, slices, stacks, out vertices, out normals, out nVert);

            if (nVert == 0)
                /* nothing to draw */
                return null;

            ushort[] sliceIdx, stackIdx;
            /* First, generate vertex index arrays for drawing with glDrawElements
            * We have a bunch of line_loops to draw for each stack, and a
            * bunch for each slice.
            */

            stackIdx = new ushort[slices * (stacks + 1)];
            sliceIdx = new ushort[slices * 2];

            /* generate for each stack */
            for (i = 0, idx = 0; i < stacks + 1; i++)
            {
                ushort offset = (ushort)(1 + (i + 1) * slices);       /* start at 1 (0 is top vertex), and we advance one stack down as we go along */
                for (j = 0; j < slices; j++, idx++)
                {
                    stackIdx[idx] = (ushort)(offset + j);
                }
            }

            /* generate for each slice */
            for (i = 0, idx = 0; i < slices; i++)
            {
                ushort offset = (ushort)(1 + i);                  /* start at 1 (0 is top vertex), and we advance one slice as we go along */
                sliceIdx[idx++] = (ushort)(offset + slices);
                sliceIdx[idx++] = (ushort)(offset + (stacks + 1) * slices);
            }

            /* draw */
            return MeshGeometryWire(vertices, normals, nVert,
                                        sliceIdx, 1, slices * 2, MeshMode.LINES,
                                        stackIdx, stacks + 1, slices);
        }

        private static Mesh<Vertex_V3N3T2f, ushort> CylinderSolidMode(float radius, float height, int slices, int stacks)
        {
            int i, j, idx, nVert;
            float[] vertices, normals;

            /* Generate vertices and normals */
            /* Note, (stacks+1)*slices vertices for side of object, 2*slices+2 for top and bottom closures */
            fghGenerateCylinder(radius, height, slices, stacks, out vertices, out normals, out nVert);

            if (nVert == 0)
                /* nothing to draw */
                return null;

            /* First, generate vertex index arrays for drawing with glDrawElements
             * All stacks, including top and bottom are covered with a triangle
             * strip.
             */
            ushort[] stripIdx;
            /* Create index vector */
            ushort offset;

            /* Allocate buffers for indices, bail out if memory allocation fails */
            stripIdx = new ushort[(slices + 1) * 2 * (stacks + 2)];    /*stacks +2 because of closing off bottom and top */

            /* top stack */
            for (j = 0, idx = 0; j < slices; j++, idx += 2)
            {
                stripIdx[idx] = 0;
                stripIdx[idx + 1] = (ushort)(j + 1);              /* 0 is top vertex, 1 is first for first stack */
            }
            stripIdx[idx] = 0;                    /* repeat first slice's idx for closing off shape */
            stripIdx[idx + 1] = 1;
            idx += 2;

            /* middle stacks: */
            /* Strip indices are relative to first index belonging to strip, NOT relative to first vertex/normal pair in array */
            for (i = 0; i < stacks; i++, idx += 2)
            {
                offset = (ushort)(1 + (i + 1) * slices);                /* triangle_strip indices start at 1 (0 is top vertex), and we advance one stack down as we go along */
                for (j = 0; j < slices; j++, idx += 2)
                {
                    stripIdx[idx] = (ushort)(offset + j);
                    stripIdx[idx + 1] = (ushort)(offset + j + slices);
                }
                stripIdx[idx] = offset;               /* repeat first slice's idx for closing off shape */
                stripIdx[idx + 1] = (ushort)(offset + slices);
            }

            /* top stack */
            offset = (ushort)(1 + (stacks + 2) * slices);
            for (j = 0; j < slices; j++, idx += 2)
            {
                stripIdx[idx] = (ushort)(offset + j);
                stripIdx[idx + 1] = (ushort)(nVert - 1);              /* zero based index, last element in array (bottom vertex)... */
            }
            stripIdx[idx] = offset;
            stripIdx[idx + 1] = (ushort)(nVert - 1);                  /* repeat first slice's idx for closing off shape */

            /* draw */
            return MeshGeometrySolid(vertices, normals, null, nVert, stripIdx, stacks + 2, (slices + 1) * 2, MeshMode.TRIANGLE_STRIP);

        }

        private static void fghGenerateCylinder(float radius, float height, int slices, int stacks, /*  input */
                                                out float[] vertices, out float[] normals, out int nVert)           /* output */

        {
            int i, j;
            int idx = 0;    /* idx into vertex/normal buffer */

            /* Step in z as stacks are drawn. */
            float radf = (float)radius;
            float z;
            float zStep = (float)height / ((stacks > 0) ? stacks : 1);

            /* Pre-computed circle */
            double[] sint, cost;

            /* number of unique vertices */
            if (slices == 0 || stacks < 1)
            {
                /* nothing to generate */
                nVert = 0;
                vertices = null;
                normals = null;
                return;
            }
            nVert = slices * (stacks + 3) + 2;   /* need two extra stacks for closing off top and bottom with correct normals */

            if (nVert > 65535)
                /*
                * limit of glushort, thats 256*256 subdivisions, should be enough in practice. See note above
                */
                Console.WriteLine("fghGenerateCylinder: too many slices or stacks requested, indices will wrap");

            /* Pre-computed circle */
            CircleTable(out sint, out cost, -slices, false);

            /* Allocate vertex and normal buffers, bail out if memory allocation fails */
            vertices = new float[nVert * 3];
            normals = new float[nVert * 3];


            z = 0;
            /* top on Z-axis */
            vertices[0] = 0.0f;
            vertices[1] = 0.0f;
            vertices[2] = 0.0f;
            normals[0] = 0.0f;
            normals[1] = 0.0f;
            normals[2] = -1.0f;
            idx = 3;
            /* other on top (get normals right) */
            for (j = 0; j < slices; j++, idx += 3)
            {
                vertices[idx] = (float)(cost[j] * radf);
                vertices[idx + 1] = (float)(sint[j] * radf);
                vertices[idx + 2] = z;
                normals[idx] = 0.0f;
                normals[idx + 1] = 0.0f;
                normals[idx + 2] = -1.0f;
            }

            /* each stack */
            for (i = 0; i < stacks + 1; i++)
            {
                for (j = 0; j < slices; j++, idx += 3)
                {
                    vertices[idx] = (float)(cost[j] * radf);
                    vertices[idx + 1] = (float)(sint[j] * radf);
                    vertices[idx + 2] = z;
                    normals[idx] = (float)cost[j];
                    normals[idx + 1] = (float)sint[j];
                    normals[idx + 2] = 0.0f;
                }

                z += zStep;
            }

            /* other on bottom (get normals right) */
            z -= zStep;
            for (j = 0; j < slices; j++, idx += 3)
            {
                vertices[idx] = (float)(cost[j] * radf);
                vertices[idx + 1] = (float)(sint[j] * radf);
                vertices[idx + 2] = z;
                normals[idx] = 0.0f;
                normals[idx + 1] = 0.0f;
                normals[idx + 2] = 1.0f;
            }

            /* bottom */
            vertices[idx] = 0.0f;
            vertices[idx + 1] = 0.0f;
            vertices[idx + 2] = height;
            normals[idx] = 0.0f;
            normals[idx + 1] = 0.0f;
            normals[idx + 2] = 1.0f;
        }


        private static Mesh<Vertex_V3N3f, ushort> TorusWire(float dInnerRadius, float dOuterRadius, int nSides, int nRings)
        {
            int i, j, idx, nVert;
            float[] vertices, normals;

            /* Generate vertices and normals */
            fghGenerateTorus(dInnerRadius, dOuterRadius, nSides, nRings, out vertices, out normals, out nVert);

            if (nVert == 0)
                /* nothing to draw */
                return null;

            ushort[] sideIdx, ringIdx;
            /* First, generate vertex index arrays for drawing with glDrawElements
             * We have a bunch of line_loops to draw each side, and a
             * bunch for each ring.
             */

            ringIdx = new ushort[nRings * nSides];
            sideIdx = new ushort[nSides * nRings];

            /* generate for each ring */
            for (j = 0, idx = 0; j < nRings; j++)
                for (i = 0; i < nSides; i++, idx++)
                    ringIdx[idx] = (ushort)(j * nSides + i);

            /* generate for each side */
            for (i = 0, idx = 0; i < nSides; i++)
                for (j = 0; j < nRings; j++, idx++)
                    sideIdx[idx] = (ushort)(j * nSides + i);

            /* draw */
            return MeshGeometryWire(vertices, normals, nVert,
                                        ringIdx, nRings, nSides, MeshMode.LINE_LOOP,
                                        sideIdx, nSides, nRings);
        }

        static Mesh<Vertex_V3N3T2f, ushort> TorusSolid(float dInnerRadius, float dOuterRadius, int nSides, int nRings)
        {
            int i, j, idx, nVert;
            float[] vertices, normals;

            /* Generate vertices and normals */
            fghGenerateTorus(dInnerRadius, dOuterRadius, nSides, nRings, out vertices, out normals, out nVert);

            if (nVert == 0)
                /* nothing to draw */
                return null;
            /* First, generate vertex index arrays for drawing with glDrawElements
            * All stacks, including top and bottom are covered with a triangle
            * strip.
            */
            ushort[] stripIdx;

            /* Allocate buffers for indices, bail out if memory allocation fails */
            stripIdx = new ushort[(nRings + 1) * 2 * nSides];


            for (i = 0, idx = 0; i < nSides; i++)
            {
                int ioff = 1;
                if (i == nSides - 1)
                    ioff = -i;

                for (j = 0; j < nRings; j++, idx += 2)
                {
                    int offset = j * nSides + i;
                    stripIdx[idx] = (ushort)(offset);
                    stripIdx[idx + 1] = (ushort)(offset + ioff);
                }
                /* repeat first to close off shape */
                stripIdx[idx] = (ushort)(i);
                stripIdx[idx + 1] = (ushort)(i + ioff);
                idx += 2;
            }

            /* draw */
            return MeshGeometrySolid(vertices, normals, null, nVert, stripIdx, nSides, (nRings + 1) * 2, MeshMode.TRIANGLE_STRIP);
        }
        static void fghGenerateTorus(double dInnerRadius, double dOuterRadius, int nSides, int nRings, /*  input */
            out float[] vertices, out float[] normals, out int nVert                     /* output */
        )
        {
            float iradius = (float)dInnerRadius;
            float oradius = (float)dOuterRadius;
            int i, j;

            /* Pre-computed circle */
            double[] spsi, cpsi;
            double[] sphi, cphi;

            /* number of unique vertices */
            if (nSides < 2 || nRings < 2)
            {
                /* nothing to generate */
                nVert = 0;
                vertices = null;
                normals = null;
                return;
            }
            nVert = nSides * nRings;

            if (nVert > 65535)
                /*
                * limit of glushort, thats 256*256 subdivisions, should be enough in practice. See note above
                */
                Console.WriteLine("fghGenerateTorus: too many slices or stacks requested, indices will wrap");

            /* precompute values on unit circle */
            CircleTable(out spsi, out cpsi, nRings, false);
            CircleTable(out sphi, out cphi, -nSides, false);

            /* Allocate vertex and normal buffers, bail out if memory allocation fails */
            vertices = new float[nVert * 3];
            normals = new float[nVert * 3];


            for (j = 0; j < nRings; j++)
            {
                for (i = 0; i < nSides; i++)
                {
                    int offset = 3 * (j * nSides + i);

                    vertices[offset] = (float)(cpsi[j] * (oradius + cphi[i] * iradius));
                    vertices[offset + 1] = (float)(spsi[j] * (oradius + cphi[i] * iradius));
                    vertices[offset + 2] = (float)(sphi[i] * iradius);
                    normals[offset] = (float)(cpsi[j] * cphi[i]);
                    normals[offset + 1] = (float)(spsi[j] * cphi[i]);
                    normals[offset + 2] = (float)(sphi[i]);
                }
            }
        }
        #endregion

        #region Platonic solids

        static void fghGenerateGeometry(int numFaces, int numEdgePerFace, float[] vertices, byte[] vertIndices, float[] normals, float[] vertOut, float[] normOut)
        {
            /* This function does the same as fghGenerateGeometryWithIndexArray, just skipping the index array generation... */
            fghGenerateGeometryWithIndexArray(numFaces, numEdgePerFace, vertices, vertIndices, normals, vertOut, normOut, null);
        }
        /* -- Cube -- */
        private const int CUBE_NUM_VERT = 8;
        private const int CUBE_NUM_FACES = 6;
        private const int CUBE_NUM_EDGE_PER_FACE = 4;
        private const int CUBE_VERT_PER_OBJ = (CUBE_NUM_FACES * CUBE_NUM_EDGE_PER_FACE);
        private const int CUBE_VERT_ELEM_PER_OBJ = (CUBE_VERT_PER_OBJ * 3);
        private const int CUBE_VERT_PER_OBJ_TRI = (CUBE_VERT_PER_OBJ + CUBE_NUM_FACES * 2);
        /* 2 extra edges per face when drawing quads as triangles */
        /* Vertex Coordinates */
        static float[] cube_v = new float[] {
            .5f, .5f, .5f,
            -.5f, .5f, .5f,
            -.5f, -.5f, .5f,
            .5f, -.5f, .5f,
            .5f, -.5f, -.5f,
            .5f, .5f, -.5f,
            -.5f, .5f, -.5f,
            -.5f, -.5f, -.5f
        };
        /* Normal Vectors */
        static float[] cube_n = new float[] {
            0.0f, 0.0f, 1.0f,
            1.0f, 0.0f, 0.0f,
            0.0f, 1.0f, 0.0f,
            -1.0f, 0.0f, 0.0f,
            0.0f, -1.0f, 0.0f,
            0.0f, 0.0f, -1.0f
        };
        /* Vertex indices, as quads, before triangulation */
        static byte[] cube_vi = new byte[] {
            0, 1, 2, 3,
            0, 3, 4, 5,
            0, 5, 6, 1,
            1, 6, 7, 2,
            7, 4, 3, 2,
            4, 7, 6, 5
        };
        static bool cubeCached = false;
        static float[] cube_verts = new float[CUBE_VERT_ELEM_PER_OBJ];
        static float[] cube_norms = new float[CUBE_VERT_ELEM_PER_OBJ];
        static ushort[] cube_vertIdxs = new ushort[CUBE_VERT_PER_OBJ_TRI];

        private static void fghCubeGenerate()
        {
            fghGenerateGeometryWithIndexArray(CUBE_NUM_FACES, CUBE_NUM_EDGE_PER_FACE,
                                               cube_v, cube_vi, cube_n,
                                               cube_verts, cube_norms, cube_vertIdxs);
        }
        /* -- Dodecahedron -- */
        /* Magic Numbers:  It is possible to create a dodecahedron by attaching two
         * pentagons to each face of of a cube. The coordinates of the points are:
         *   (+-x,0, z); (+-1, 1, 1); (0, z, x )
         * where x = (-1 + sqrt(5))/2, z = (1 + sqrt(5))/2 or
         *       x = 0.61803398875 and z = 1.61803398875.
         */
        private const int DODECAHEDRON_NUM_VERT = 20;
        private const int DODECAHEDRON_NUM_FACES = 12;
        private const int DODECAHEDRON_NUM_EDGE_PER_FACE = 5;
        private const int DODECAHEDRON_VERT_PER_OBJ = (DODECAHEDRON_NUM_FACES * DODECAHEDRON_NUM_EDGE_PER_FACE);
        private const int DODECAHEDRON_VERT_ELEM_PER_OBJ = (DODECAHEDRON_VERT_PER_OBJ * 3);
        private const int DODECAHEDRON_VERT_PER_OBJ_TRI = (DODECAHEDRON_VERT_PER_OBJ + DODECAHEDRON_NUM_FACES * 4);
        /* 4 extra edges per face when drawing pentagons as triangles */
        /* Vertex Coordinates */
        static float[] dodecahedron_v = new float[] {
            0.0f, 1.61803398875f, 0.61803398875f,
            -          1.0f, 1.0f, 1.0f,
            -0.61803398875f, 0.0f, 1.61803398875f,
            0.61803398875f, 0.0f, 1.61803398875f,
            1.0f, 1.0f, 1.0f,
            0.0f, 1.61803398875f, -0.61803398875f,
            1.0f, 1.0f, -          1.0f,
            0.61803398875f, 0.0f, -1.61803398875f,
            -0.61803398875f, 0.0f, -1.61803398875f,
            -          1.0f, 1.0f, -          1.0f,
            0.0f, -1.61803398875f, 0.61803398875f,
            1.0f, -          1.0f, 1.0f,
            -          1.0f, -          1.0f, 1.0f,
            0.0f, -1.61803398875f, -0.61803398875f,
            -          1.0f, -          1.0f, -          1.0f,
            1.0f, -          1.0f, -          1.0f,
            1.61803398875f, -0.61803398875f, 0.0f,
            1.61803398875f, 0.61803398875f, 0.0f,
            -1.61803398875f, 0.61803398875f, 0.0f,
            -1.61803398875f, -0.61803398875f, 0.0f
        };
        /* Normal Vectors */
        static float[] dodecahedron_n = new float[] {
            0.0f, 0.525731112119f, 0.850650808354f,
            0.0f, 0.525731112119f, -0.850650808354f,
            0.0f, -0.525731112119f, 0.850650808354f,
            0.0f, -0.525731112119f, -0.850650808354f,

            0.850650808354f, 0.0f, 0.525731112119f,
            -0.850650808354f, 0.0f, 0.525731112119f,
            0.850650808354f, 0.0f, -0.525731112119f,
            -0.850650808354f, 0.0f, -0.525731112119f,

            0.525731112119f, 0.850650808354f, 0.0f,
            0.525731112119f, -0.850650808354f, 0.0f,
            -0.525731112119f, 0.850650808354f, 0.0f,
            -0.525731112119f, -0.850650808354f, 0.0f,
        };
        /* Vertex indices */
        static byte[] dodecahedron_vi = new byte[] {
            0, 1, 2, 3, 4,
            5, 6, 7, 8, 9,
            10, 11, 3, 2, 12,
            13, 14, 8, 7, 15,

            3, 11, 16, 17, 4,
            2, 1, 18, 19, 12,
            7, 6, 17, 16, 15,
            8, 14, 19, 18, 9,

            17, 6, 5, 0, 4,
            16, 11, 10, 13, 15,
            18, 1, 0, 5, 9,
            19, 14, 13, 10, 12
        };
        static bool dodecahedronCached = false;
        static float[] dodecahedron_verts = new float[DODECAHEDRON_VERT_ELEM_PER_OBJ];
        static float[] dodecahedron_norms = new float[DODECAHEDRON_VERT_ELEM_PER_OBJ];
        static ushort[] dodecahedron_vertIdxs = new ushort[DODECAHEDRON_VERT_PER_OBJ_TRI];

        static void fghDodecahedronGenerate()
        {
            fghGenerateGeometryWithIndexArray(DODECAHEDRON_NUM_FACES, DODECAHEDRON_NUM_EDGE_PER_FACE,
                                               dodecahedron_v, dodecahedron_vi, dodecahedron_n,
                                               dodecahedron_verts, dodecahedron_norms, dodecahedron_vertIdxs);
        }
        /* -- Icosahedron -- */
        private const int ICOSAHEDRON_NUM_VERT = 12;
        private const int ICOSAHEDRON_NUM_FACES = 20;
        private const int ICOSAHEDRON_NUM_EDGE_PER_FACE = 3;
        private const int ICOSAHEDRON_VERT_PER_OBJ = (ICOSAHEDRON_NUM_FACES * ICOSAHEDRON_NUM_EDGE_PER_FACE);
        private const int ICOSAHEDRON_VERT_ELEM_PER_OBJ = (ICOSAHEDRON_VERT_PER_OBJ * 3);
        private const int ICOSAHEDRON_VERT_PER_OBJ_TRI = ICOSAHEDRON_VERT_PER_OBJ;
        /* Vertex Coordinates */
        static float[] icosahedron_v = new float[] {
            1.0f, 0.0f, 0.0f,
            0.447213595500f, 0.894427191000f, 0.0f,
            0.447213595500f, 0.276393202252f, 0.850650808354f,
            0.447213595500f, -0.723606797748f, 0.525731112119f,
            0.447213595500f, -0.723606797748f, -0.525731112119f,
            0.447213595500f, 0.276393202252f, -0.850650808354f,
            -0.447213595500f, -0.894427191000f, 0.0f,
            -0.447213595500f, -0.276393202252f, 0.850650808354f,
            -0.447213595500f, 0.723606797748f, 0.525731112119f,
            -0.447213595500f, 0.723606797748f, -0.525731112119f,
            -0.447213595500f, -0.276393202252f, -0.850650808354f,
            -           1.0f, 0.0f, 0.0f
        };
        /* Normal Vectors:
         * icosahedron_n[i][0] = ( icosahedron_v[icosahedron_vi[i][1]][1] - icosahedron_v[icosahedron_vi[i][0]][1] ) * ( icosahedron_v[icosahedron_vi[i][2]][2] - icosahedron_v[icosahedron_vi[i][0]][2] ) - ( icosahedron_v[icosahedron_vi[i][1]][2] - icosahedron_v[icosahedron_vi[i][0]][2] ) * ( icosahedron_v[icosahedron_vi[i][2]][1] - icosahedron_v[icosahedron_vi[i][0]][1] ) ;
         * icosahedron_n[i][1] = ( icosahedron_v[icosahedron_vi[i][1]][2] - icosahedron_v[icosahedron_vi[i][0]][2] ) * ( icosahedron_v[icosahedron_vi[i][2]][0] - icosahedron_v[icosahedron_vi[i][0]][0] ) - ( icosahedron_v[icosahedron_vi[i][1]][0] - icosahedron_v[icosahedron_vi[i][0]][0] ) * ( icosahedron_v[icosahedron_vi[i][2]][2] - icosahedron_v[icosahedron_vi[i][0]][2] ) ;
         * icosahedron_n[i][2] = ( icosahedron_v[icosahedron_vi[i][1]][0] - icosahedron_v[icosahedron_vi[i][0]][0] ) * ( icosahedron_v[icosahedron_vi[i][2]][1] - icosahedron_v[icosahedron_vi[i][0]][1] ) - ( icosahedron_v[icosahedron_vi[i][1]][1] - icosahedron_v[icosahedron_vi[i][0]][1] ) * ( icosahedron_v[icosahedron_vi[i][2]][0] - icosahedron_v[icosahedron_vi[i][0]][0] ) ;
        */
        static float[] icosahedron_n = new float[] {
            0.760845213037948f, 0.470228201835026f, 0.341640786498800f,
            0.760845213036861f, -0.179611190632978f, 0.552786404500000f,
            0.760845213033849f, -0.581234022404097f, 0.0f,
            0.760845213036861f, -0.179611190632978f, -0.552786404500000f,
            0.760845213037948f, 0.470228201835026f, -0.341640786498800f,
            0.179611190628666f, 0.760845213037948f, 0.552786404498399f,
            0.179611190634277f, -0.290617011204044f, 0.894427191000000f,
            0.179611190633958f, -0.940456403667806f, 0.0f,
            0.179611190634278f, -0.290617011204044f, -0.894427191000000f,
            0.179611190628666f, 0.760845213037948f, -0.552786404498399f,
            -0.179611190633958f, 0.940456403667806f, 0.0f,
            -0.179611190634277f, 0.290617011204044f, 0.894427191000000f,
            -0.179611190628666f, -0.760845213037948f, 0.552786404498399f,
            -0.179611190628666f, -0.760845213037948f, -0.552786404498399f,
            -0.179611190634277f, 0.290617011204044f, -0.894427191000000f,
            -0.760845213036861f, 0.179611190632978f, -0.552786404500000f,
            -0.760845213033849f, 0.581234022404097f, 0.0f,
            -0.760845213036861f, 0.179611190632978f, 0.552786404500000f,
            -0.760845213037948f, -0.470228201835026f, 0.341640786498800f,
            -0.760845213037948f, -0.470228201835026f, -0.341640786498800f,
        };
        /* Vertex indices */
        static byte[] icosahedron_vi = new byte[] {
            0, 1, 2,
            0, 2, 3,
            0, 3, 4,
            0, 4, 5,
            0, 5, 1,
            1, 8, 2,
            2, 7, 3,
            3, 6, 4,
            4, 10, 5,
            5, 9, 1,
            1, 9, 8,
            2, 8, 7,
            3, 7, 6,
            4, 6, 10,
            5, 10, 9,
            11, 9, 10,
            11, 8, 9,
            11, 7, 8,
            11, 6, 7,
            11, 10, 6
        };
        static bool icosahedronCached = false;
        static float[] icosahedron_verts = new float[ICOSAHEDRON_VERT_ELEM_PER_OBJ];
        static float[] icosahedron_norms = new float[ICOSAHEDRON_VERT_ELEM_PER_OBJ];
        static ushort[] icosahedron_vertIdxs = new ushort[ICOSAHEDRON_VERT_PER_OBJ_TRI];

        static void fghIcosahedronGenerate()
        {
            fghGenerateGeometryWithIndexArray(ICOSAHEDRON_NUM_FACES, ICOSAHEDRON_NUM_EDGE_PER_FACE,
                                               icosahedron_v, icosahedron_vi, icosahedron_n,
                                               icosahedron_verts, icosahedron_norms, icosahedron_vertIdxs);
        }
        /* -- Octahedron -- */
        private const int OCTAHEDRON_NUM_VERT = 6;
        private const int OCTAHEDRON_NUM_FACES = 8;
        private const int OCTAHEDRON_NUM_EDGE_PER_FACE = 3;
        private const int OCTAHEDRON_VERT_PER_OBJ = (OCTAHEDRON_NUM_FACES * OCTAHEDRON_NUM_EDGE_PER_FACE);
        private const int OCTAHEDRON_VERT_ELEM_PER_OBJ = (OCTAHEDRON_VERT_PER_OBJ * 3);
        private const int OCTAHEDRON_VERT_PER_OBJ_TRI = OCTAHEDRON_VERT_PER_OBJ;
        /* Vertex Coordinates */
        static float[] octahedron_v = new float[] {
            1.0f, 0.0f, 0.0f,
            0.0f, 1.0f, 0.0f,
            0.0f, 0.0f, 1.0f,
            -1.0f, 0.0f, 0.0f,
            0.0f, -1.0f, 0.0f,
            0.0f, 0.0f, -1.0f,

        };
        /* Normal Vectors */
        static float[] octahedron_n = new float[] {
            0.577350269189f, 0.577350269189f, 0.577350269189f,    /* sqrt(1/3) */
			0.577350269189f, 0.577350269189f, -0.577350269189f,
            0.577350269189f, -0.577350269189f, 0.577350269189f,
            0.577350269189f, -0.577350269189f, -0.577350269189f,
            -0.577350269189f, 0.577350269189f, 0.577350269189f,
            -0.577350269189f, 0.577350269189f, -0.577350269189f,
            -0.577350269189f, -0.577350269189f, 0.577350269189f,
            -0.577350269189f, -0.577350269189f, -0.577350269189f

        };
        /* Vertex indices */
        static byte[] octahedron_vi = new byte[] {
            0, 1, 2,
            0, 5, 1,
            0, 2, 4,
            0, 4, 5,
            3, 2, 1,
            3, 1, 5,
            3, 4, 2,
            3, 5, 4
        };
        static bool octahedronCached = false;
        static float[] octahedron_verts = new float[OCTAHEDRON_VERT_ELEM_PER_OBJ];
        static float[] octahedron_norms = new float[OCTAHEDRON_VERT_ELEM_PER_OBJ];
        static ushort[] octahedron_vertIdxs = new ushort[OCTAHEDRON_VERT_PER_OBJ_TRI];

        static void fghOctahedronGenerate()
        {
            fghGenerateGeometryWithIndexArray(OCTAHEDRON_NUM_FACES, OCTAHEDRON_NUM_EDGE_PER_FACE,
                                               octahedron_v, octahedron_vi, octahedron_n,
                                               octahedron_verts, octahedron_norms, octahedron_vertIdxs);
        }
        /* -- RhombicDodecahedron -- */
        private const int RHOMBICDODECAHEDRON_NUM_VERT = 14;
        private const int RHOMBICDODECAHEDRON_NUM_FACES = 12;
        private const int RHOMBICDODECAHEDRON_NUM_EDGE_PER_FACE = 4;
        private const int RHOMBICDODECAHEDRON_VERT_PER_OBJ = (RHOMBICDODECAHEDRON_NUM_FACES * RHOMBICDODECAHEDRON_NUM_EDGE_PER_FACE);
        private const int RHOMBICDODECAHEDRON_VERT_ELEM_PER_OBJ = (RHOMBICDODECAHEDRON_VERT_PER_OBJ * 3);
        private const int RHOMBICDODECAHEDRON_VERT_PER_OBJ_TRI = (RHOMBICDODECAHEDRON_VERT_PER_OBJ + RHOMBICDODECAHEDRON_NUM_FACES * 2);
        /* 2 extra edges per face when drawing quads as triangles */
        /* Vertex Coordinates */
        static float[] rhombicdodecahedron_v = new float[] {
            0.0f, 0.0f, 1.0f,
            0.707106781187f, 0.0f, 0.5f,
            0.0f, 0.707106781187f, 0.5f,
            -0.707106781187f, 0.0f, 0.5f,
            0.0f, -0.707106781187f, 0.5f,
            0.707106781187f, 0.707106781187f, 0.0f,
            -0.707106781187f, 0.707106781187f, 0.0f,
            -0.707106781187f, -0.707106781187f, 0.0f,
            0.707106781187f, -0.707106781187f, 0.0f,
            0.707106781187f, 0.0f, -0.5f,
            0.0f, 0.707106781187f, -0.5f,
            -0.707106781187f, 0.0f, -0.5f,
            0.0f, -0.707106781187f, -0.5f,
            0.0f, 0.0f, -1.0f
        };
        /* Normal Vectors */
        static float[] rhombicdodecahedron_n = new float[] {
            0.353553390594f, 0.353553390594f, 0.5f,
            -0.353553390594f, 0.353553390594f, 0.5f,
            -0.353553390594f, -0.353553390594f, 0.5f,
            0.353553390594f, -0.353553390594f, 0.5f,
            0.0f, 1.0f, 0.0f,
            -1.0f, 0.0f, 0.0f,
            0.0f, -           1.0f, 0.0f,
            1.0f, 0.0f, 0.0f,
            0.353553390594f, 0.353553390594f, -0.5f,
            -0.353553390594f, 0.353553390594f, -0.5f,
            -0.353553390594f, -0.353553390594f, -0.5f,
            0.353553390594f, -0.353553390594f, -0.5f
        };
        /* Vertex indices */
        static byte[] rhombicdodecahedron_vi = new byte[] {
            0, 1, 5, 2,
            0, 2, 6, 3,
            0, 3, 7, 4,
            0, 4, 8, 1,
            5, 10, 6, 2,
            6, 11, 7, 3,
            7, 12, 8, 4,
            8, 9, 5, 1,
            5, 9, 13, 10,
            6, 10, 13, 11,
            7, 11, 13, 12,
            8, 12, 13, 9
        };
        static bool rhombicdodecahedronCached = false;
        static float[] rhombicdodecahedron_verts = new float[RHOMBICDODECAHEDRON_VERT_ELEM_PER_OBJ];
        static float[] rhombicdodecahedron_norms = new float[RHOMBICDODECAHEDRON_VERT_ELEM_PER_OBJ];
        static ushort[] rhombicdodecahedron_vertIdxs = new ushort[RHOMBICDODECAHEDRON_VERT_PER_OBJ_TRI];

        static void fghRhombicDodecahedronGenerate()
        {
            fghGenerateGeometryWithIndexArray(RHOMBICDODECAHEDRON_NUM_FACES, RHOMBICDODECAHEDRON_NUM_EDGE_PER_FACE,
                                               rhombicdodecahedron_v, rhombicdodecahedron_vi, rhombicdodecahedron_n,
                                               rhombicdodecahedron_verts, rhombicdodecahedron_norms, rhombicdodecahedron_vertIdxs);
        }
        /* -- Tetrahedron -- */
        /* Magic Numbers:  r0 = ( 1, 0, 0 )
             *                 r1 = ( -1/3, 2 sqrt(2) / 3, 0 )
             *                 r2 = ( -1/3, - sqrt(2) / 3,  sqrt(6) / 3 )
             *                 r3 = ( -1/3, - sqrt(2) / 3, -sqrt(6) / 3 )
             * |r0| = |r1| = |r2| = |r3| = 1
             * Distance between any two points is 2 sqrt(6) / 3
             *
             * Normals:  The unit normals are simply the negative of the coordinates of the point not on the surface.
             */
        private const int TETRAHEDRON_NUM_VERT = 4;
        private const int TETRAHEDRON_NUM_FACES = 4;
        private const int TETRAHEDRON_NUM_EDGE_PER_FACE = 3;
        private const int TETRAHEDRON_VERT_PER_OBJ = (TETRAHEDRON_NUM_FACES * TETRAHEDRON_NUM_EDGE_PER_FACE);
        private const int TETRAHEDRON_VERT_ELEM_PER_OBJ = (TETRAHEDRON_VERT_PER_OBJ * 3);
        private const int TETRAHEDRON_VERT_PER_OBJ_TRI = TETRAHEDRON_VERT_PER_OBJ;
        /* Vertex Coordinates */
        static float[] tetrahedron_v = new float[] {
            1.0f, 0.0f, 0.0f,
            -0.333333333333f, 0.942809041582f, 0.0f,
            -0.333333333333f, -0.471404520791f, 0.816496580928f,
            -0.333333333333f, -0.471404520791f, -0.816496580928f
        };
        /* Normal Vectors */
        static float[] tetrahedron_n = new float[] {
            -           1.0f, 0.0f, 0.0f,
            0.333333333333f, -0.942809041582f, 0.0f,
            0.333333333333f, 0.471404520791f, -0.816496580928f,
            0.333333333333f, 0.471404520791f, 0.816496580928f
        };
        /* Vertex indices */
        static byte[] tetrahedron_vi = new byte[] {
            1, 3, 2,
            0, 2, 3,
            0, 3, 1,
            0, 1, 2
        };
        static bool tetrahedronCached = false;
        static float[] tetrahedron_verts = new float[TETRAHEDRON_VERT_ELEM_PER_OBJ];
        static float[] tetrahedron_norms = new float[TETRAHEDRON_VERT_ELEM_PER_OBJ];
        static ushort[] tetrahedron_vertIdxs = new ushort[TETRAHEDRON_VERT_PER_OBJ_TRI];

        static void fghTetrahedronGenerate()
        {
            fghGenerateGeometryWithIndexArray(TETRAHEDRON_NUM_FACES, TETRAHEDRON_NUM_EDGE_PER_FACE,
                                               tetrahedron_v, tetrahedron_vi, tetrahedron_n,
                                               tetrahedron_verts, tetrahedron_norms, tetrahedron_vertIdxs);
        }

        /**
* Generate vertex indices for visualizing the normals.
* vertices are written into verticesForNormalVisualization.
* This must be freed by caller, we do the free at the
* end of fghDrawNormalVisualization11/fghDrawNormalVisualization20
*/
        static float[] verticesForNormalVisualization;
        static int numNormalVertices = 0;

        static void fghGenerateNormalVisualization(float[] vertices, float[] normals, int numVertices)
        {
            int i, j;
            numNormalVertices = numVertices * 2;
            verticesForNormalVisualization = new float[numNormalVertices * 3];

            for (i = 0, j = 0; i < numNormalVertices * 3 / 2; i += 3, j += 6)
            {
                verticesForNormalVisualization[j + 0] = vertices[i + 0];
                verticesForNormalVisualization[j + 1] = vertices[i + 1];
                verticesForNormalVisualization[j + 2] = vertices[i + 2];
                verticesForNormalVisualization[j + 3] = vertices[i + 0] + normals[i + 0] / 4.0f;
                verticesForNormalVisualization[j + 4] = vertices[i + 1] + normals[i + 1] / 4.0f;
                verticesForNormalVisualization[j + 5] = vertices[i + 2] + normals[i + 2] / 4.0f;
            }
        }

        /**
		 * Generate all combinations of vertices and normals needed to draw object.
		 * Optional shape decomposition to triangles:
		 * We'll use glDrawElements to draw all shapes that are not naturally
		 * composed of triangles, so generate an index vector here, using the
		 * below sampling scheme.
		 * Be careful to keep winding of all triangles counter-clockwise,
		 * assuming that input has correct winding...
		 */
        static byte[] vert4Decomp = new byte[] { 0, 1, 2, 0, 2, 3 };
        /* quad    : 4 input vertices, 6 output (2 triangles) */
        static byte[] vert5Decomp = new byte[] { 0, 1, 2, 0, 2, 4, 4, 2, 3 };
        /* pentagon: 5 input vertices, 9 output (3 triangles) */
        static void fghGenerateGeometryWithIndexArray(int numFaces, int numEdgePerFace, float[] vertices, byte[] vertIndices, float[] normals, float[] vertOut, float[] normOut, ushort[] vertIdxOut)
        {
            int i, j, numEdgeIdxPerFace = 0;
            byte[] vertSamps = null;
            switch (numEdgePerFace)
            {
                case 3:
                    /* nothing to do here, we'll draw with glDrawArrays */
                    break;
                case 4:
                    vertSamps = vert4Decomp;
                    numEdgeIdxPerFace = 6;      /* 6 output vertices for each face */
                    break;
                case 5:
                    vertSamps = vert5Decomp;
                    numEdgeIdxPerFace = 9;      /* 9 output vertices for each face */
                    break;
            }
            /*
		     * Build array with vertices using vertex coordinates and vertex indices
		     * Do same for normals.
		     * Need to do this because of different normals at shared vertices.
		     */
            for (i = 0; i < numFaces; i++)
            {
                int normIdx = i * 3;
                int faceIdxVertIdx = i * numEdgePerFace; /* index to first element of "row" in vertex indices */
                for (j = 0; j < numEdgePerFace; j++)
                {
                    int outIdx = i * numEdgePerFace * 3 + j * 3;
                    int vertIdx = vertIndices[faceIdxVertIdx + j] * 3;

                    vertOut[outIdx] = vertices[vertIdx];
                    vertOut[outIdx + 1] = vertices[vertIdx + 1];
                    vertOut[outIdx + 2] = vertices[vertIdx + 2];

                    normOut[outIdx] = normals[normIdx];
                    normOut[outIdx + 1] = normals[normIdx + 1];
                    normOut[outIdx + 2] = normals[normIdx + 2];
                }

                /* generate vertex indices for each face */
                if (vertSamps != null)
                    for (j = 0; j < numEdgeIdxPerFace; j++)
                        vertIdxOut[i * numEdgeIdxPerFace + j] = (ushort)(faceIdxVertIdx + vertSamps[j]);
            }
        }

        #endregion
    }

}
