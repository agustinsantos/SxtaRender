layout(location = 0)  vec4 aPos;
layout(location = 0)  vec2 aTex;

out vec2 sampleCoord;

uniform mat4 u_matrix;
uniform vec2 textureScale;

void main()
{
     sampleCoord = tCoord * textureScale;
     gl_Position = matrix * vCoord;
}