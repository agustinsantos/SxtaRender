#ifdef _VERTEX_
// The ShadedIdentity Shader
// Vertex Shader
// Richard S. Wright Jr.
// OpenGL SuperBible

in vec4 vVertex; // Vertex position attribute
in vec4 vColor; // Vertex color attribute
out vec4 vVaryingColor; // Color value passed to fragment shader

void main(void)
{
	vVaryingColor = vColor;// Simply copy the color value
	gl_Position = vVertex; // Simply pass along the vertex position
}

#endif

#ifdef _FRAGMENT_
// The ShadedIdentity Shader
// Fragment Shader
// Richard S. Wright Jr.
// OpenGL SuperBible

out vec4 vFragColor; // Fragment color to rasterize
in vec4 vVaryingColor; // Incoming color from vertex stage

void main(void)
{
	vFragColor = vVaryingColor; // Interpolated color to fragment
}

#endif
