precision highp float;

uniform sampler2D texGrass;
uniform sampler2D texRock;
uniform sampler2D texSand;
uniform sampler2D texGrassRock;
uniform sampler2D texDirt;
uniform vec3 ambient;
uniform vec3 directionalLight;
uniform vec3 directionalLightColor;  
uniform vec3 gamma;
            
in vec3 normal;
in vec2 TexCoord0;
in float height;

out vec4 out_frag_color;

void main(void)
{
	vec4 texture = vec4(1,1,1,1);
	if(height >= 0.00f && height <= 1.0f){ texture = texture2D(texSand, TexCoord0.xy);}
	if(height >= 1.0f && height <= 1.25f){
		float lHeight = (height - 1.0f) * 4.0f;
		texture = ( ( texture2D(texGrass, TexCoord0.xy) * lHeight ) + ( texture2D(texSand, TexCoord0.xy) * (1.0f-lHeight) ) );
	}
	if(height >= 1.25f  && height <= 2.0f){ texture = texture2D(texGrass, TexCoord0.xy);}
	if(height >= 2.0f && height <= 2.25f){
		float lHeight = (height - 2.0f) * 4.0f;
		texture = ( ( texture2D(texGrassRock, TexCoord0.xy) * lHeight ) + ( texture2D(texGrass, TexCoord0.xy) * (1.0f-lHeight) ) );
	}
	if(height >= 2.25f && height <= 3.0f){ texture = texture2D(texGrassRock, TexCoord0.xy);}
	if(height >= 3.0f && height <= 3.25f){
		float lHeight = (height - 3.0f) * 4.0f;
		texture = ( ( texture2D(texRock, TexCoord0.xy) * lHeight ) + ( texture2D(texGrassRock, TexCoord0.xy) * (1.0f-lHeight) ) );
	}
	if(height >= 3.25f){ texture = texture2D(texRock, TexCoord0.xy);}
	if(TexCoord0.x < 0.0f || TexCoord0.y < 0.0f){ texture = vec4(0,0,0,1);}   
              
	float diffuse = clamp( dot( directionalLight, normalize( normal ) ), 0.0, 1.0 );
              
	vec4 outColor = texture * vec4( ambient + diffuse * directionalLightColor, 1.0 );

	out_frag_color = vec4( pow(outColor.xyz, gamma), outColor.a );
}