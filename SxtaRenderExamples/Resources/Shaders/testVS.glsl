layout (location = 0) in vec3 Position;

out vec2 TexCoord0;

void main()
{
    gl_Position = vec4(Position*0.8, 1.0);
}
 