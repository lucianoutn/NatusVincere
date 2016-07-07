//Matrices de transformacion
float4x4 matWorld; //Matriz de transformacion World
float4x4 matWorldView; //Matriz World * View
float4x4 matWorldViewProj; //Matriz World * View * Projection
float4x4 matInverseTransposeWorld; //Matriz Transpose(Invert(World))


float time;

//Textura para DiffuseMap
texture texDiffuseMap;
sampler2D diffuseMap = sampler_state
{
	Texture = (texDiffuseMap);
	ADDRESSU = WRAP;
	ADDRESSV = WRAP;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};

struct VS_OUTPUT
{
	float4 Position : POSITION;
	float4 Color : COLOR0;
	float2 TexCoord : TEXCOORD0;
};

VS_OUTPUT VS_onlyTexture(
	float4 Position : POSITION,
	float3 Normal : NORMAL,
	float4 Color : COLOR,
	float2 TexCoord : TEXCOORD0
	)
{
	VS_OUTPUT Out;

	Out.Position = mul(Position, matWorldViewProj);
	Out.Color = Color;
	Out.TexCoord = TexCoord;

	return Out;
}

VS_OUTPUT VS_viento (
	float4 Position : POSITION,
	float3 Normal : NORMAL,
	float4 Color : COLOR,
	float2 TexCoord : TEXCOORD0
)
{
	VS_OUTPUT Out = (VS_OUTPUT)0;

	float timeFactor = sin(time);
	if(Position.y > 140){
		Position.z += Position.y *  timeFactor  * 0.2;
        	Position.x += Position.y * timeFactor  * 0.1;
	}
	Out.Position = mul(Position, matWorldViewProj);
	Out.Color = Color; 
	Out.TexCoord = TexCoord;
	
	return Out;
}

VS_OUTPUT VS_viento2(
	float4 Position : POSITION,
	float3 Normal : NORMAL,
	float4 Color : COLOR,
	float2 TexCoord : TEXCOORD0
	)
{
	VS_OUTPUT Out = (VS_OUTPUT)0;

	float timeFactor = sin(time);
	if (Position.y > 7) {
		Position.z += Position.y *  timeFactor  * 0.2;
		Position.x += Position.y * timeFactor  * 0.1;
	}
	Out.Position = mul(Position, matWorldViewProj);
	Out.Color = Color;
	Out.TexCoord = TexCoord;

	return Out;
}


float4 PS_onlyTexture(VS_OUTPUT In): COLOR
{
	return tex2D(diffuseMap, In.TexCoord);
}


/*********************************************** Techniques ***************************************************/

technique Viento {
	pass p0 {
		VertexShader = compile vs_2_0 VS_viento();
		PixelShader = compile ps_2_0 PS_onlyTexture();
	}
}

technique renderNormal {
	pass p0 {
		VertexShader = compile vs_2_0 VS_onlyTexture();
		PixelShader = compile ps_2_0 PS_onlyTexture();
	}
}

technique Viento2 {
	pass p0 {
		VertexShader = compile vs_2_0 VS_viento2();
		PixelShader = compile ps_2_0 PS_onlyTexture();
	}
}
