#ifdef _VERTEX_
        layout (location = 0) in vec3 Position;

        out vec2 TexCoord0;

        void main()
        {
            gl_Position = vec4(Position*0.8, 1.0);
        }
#endif
#ifdef _FRAGMENT_
        out vec4 FragColor;
 
        void main()
        {
            FragColor =  vec4(0.2, 1, 0.1, 1.0);
        }
#endif