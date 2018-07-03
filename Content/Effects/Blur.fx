float4x4 World; 
float4x4 View; 
float4x4 Projection; 
 
// TODO: add effect parameters here. 
sampler TextureSampler; 
 
//Blur amount
float blur = 0.009; 
//Size of the screen, should be 1f
float height;
float width;
//How big the edges are, should be the tile width (as a float)
float edgeX;
float edgeY;
 
struct PixelInput 
{ 
    float2 TexCoord : TEXCOORD0; 
}; 
float4 PixelShaderFunction(PixelInput input) : COLOR0 
{ 

    float4 Color =  tex2D(TextureSampler, input.TexCoord.xy);
	//Make sure we are not on the very edge, where blurring will glitch it out
    if (input.TexCoord.x > edgeX && input.TexCoord.y > edgeY && input.TexCoord.y < height  - edgeY && input.TexCoord.x < width - edgeX)
	{
	   Color += tex2D(TextureSampler, input.TexCoord.xy - blur);
       Color += tex2D(TextureSampler, input.TexCoord.xy + blur);
       Color = Color / 3;
	}
    return (Color); 
} 
 
technique Default 
{ 
    pass P0 
    { 
        PixelShader = compile ps_2_0 PixelShaderFunction(); 
    } 
} 