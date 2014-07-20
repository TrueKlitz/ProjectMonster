uniform mat4 camera_matrix;            
uniform mat4 projection_matrix;
uniform mat4 modelview_matrix;

in vec3 vertex_position;
in vec3 vertex_normal;
in vec2 vertex_texCoord0;
in vec3 vertex_tangent;

out mat3 tbnMatrix;
out vec2 texCoord0;
out vec3 worldPos0;

void main(void)
{ 
	worldPos0 = (modelview_matrix * vec4(vertex_position.xyz , 1.0 )).xyz;
	vec3 n = normalize((modelview_matrix * vec4(vertex_normal , 0.0 )).xyz);
	vec3 t = normalize((modelview_matrix * vec4(vertex_tangent , 0.0 )).xyz);
	t = normalize(t - dot(t, n) * n);

	vec3 biTangent = cross(t, n);
              
	tbnMatrix = mat3(t, biTangent, n);
	texCoord0 = vertex_texCoord0;

	gl_Position = projection_matrix * camera_matrix * modelview_matrix * vec4(vertex_position.xyz , 1.0 );           
}