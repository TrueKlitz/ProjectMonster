uniform mat4 camera_matrix;            
uniform mat4 projection_matrix;

in vec3 vertex_position;
in vec3 vertex_normal;
in vec2 vertex_texCoord;

out vec3 normal;
out vec2 TexCoord0;
out float height;

void main(void)
{
	normal = (vec4( vertex_normal, 0 ) ).xyz;
	height = vertex_position.y;
	TexCoord0 = vertex_texCoord;
	gl_Position = projection_matrix * camera_matrix * vec4(vertex_position.xyz, 1 );
}