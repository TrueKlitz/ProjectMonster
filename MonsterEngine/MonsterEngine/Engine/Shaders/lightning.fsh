precision highp float;
 
uniform sampler2D texGround;
uniform sampler2D texNormal;
uniform vec3 ambient;
uniform vec3 directionalLight;
uniform vec3 directionalLightColor;  
uniform bool normalMapping;
uniform bool parallaxMapping;
uniform float specluar;
uniform vec3 c_eyePos;
uniform vec3 gamma;
  
in mat3 tbnMatrix;
in vec2 texCoord0;
in vec3 worldPos0;            
            
out vec4 out_frag_color;

void main(void)
{
	vec3 directionToEye = normalize( c_eyePos - worldPos0  );
	vec2 texCoords = texCoord0.xy;
	vec3 normal = vec3(0.0, 0.0, 0.0);
	vec3 spec = vec4(0.0,0.0,0.0,0.0);
             
	if( normalMapping ){normal = normalize(tbnMatrix * (2 * texture2D(texNormal, texCoords).xyz - 1));}else{ normal = normalize(tbnMatrix * (2 * texture2D(texNormal, vec2(0,0)).xyz - 1));}            
	float diffuse = clamp( dot( directionalLight, normal ), 0.0, 1.0 );

	if(specluar > 0.01){
		float specularCoefficient = 0.0;
			specularCoefficient = pow( max( 0.0, dot( directionToEye, reflect( normalize(worldPos0 - directionalLight ) , normal ) )), specluar);
		spec = specularCoefficient;
	}

	vec4 outColor = texture2D( texGround , texCoords ) * (vec4( ambient + (diffuse * (directionalLightColor + spec))  , 1.0 ) );

	out_frag_color = vec4( pow(outColor.xyz, gamma), outColor.a );
}