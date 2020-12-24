#ifdef GL_ES
precision highp float;
#endif

uniform vec3 lightDirection;
uniform float ambientLight;
uniform sampler2D diffuse;
varying vec3 vNormal;
varying vec2 vUv;

void main() {
    float lightness = -clamp(dot(normalize(vNormal), normalize(lightDirection)), -1., 0.);
    lightness = ambientLight + (1. - ambientLight) * lightness;
    gl_FragColor = vec4(texture2D(diffuse, vUv).rgb * lightness, 1.);
}