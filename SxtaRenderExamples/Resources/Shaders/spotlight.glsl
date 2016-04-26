uniform light {
    vec3 worldLightPos;
    vec3 worldLightDir;
    vec2 spotlightAngle;
};

float illuminance(vec3 worldP, vec3 worldN, out vec3 worldL) {
    worldL = abs(normalize(worldLightPos - worldP));
    //float falloff = 1.0 - smoothstep(spotlightAngle.x, spotlightAngle.y, acos(dot(worldLightDir, -worldL)));
    float falloff = 1.0 ;//- smoothstep(0.6, 0.8, acos(dot(worldLightDir, -worldL))); // TODO Review this
    return max(dot(worldN, worldL), 0.0) * falloff;
}

#ifdef _VERTEX_
#endif

#ifdef _FRAGMENT_
#endif
