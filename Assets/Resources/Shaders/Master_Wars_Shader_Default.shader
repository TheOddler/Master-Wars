Shader "Master Wars/Default"
{
	Properties
	{
		_AmbientColor("Ambient Color", Color) = (0,0,0,1)
		_AmbientPower("Ambient Power", Range(0,1)) = 0.2
		_MainTex ("Base (RGB)", 2D) = "white" { }
		_SpecColor ("Spec Color", Color) = (1,1,1,1)
        _Shininess ("Shininess", Range (0.01, 1)) = 0.7
	}
	
	SubShader
	{
		Pass
		{
			Material
			{
				Diffuse (1,1,1,1)
				//Color fullAmbient = _AmbientColor*_AmbientPower
				Ambient [_AmbientColor]
                Shininess [_Shininess]
                Specular [_SpecColor]
			}
			Lighting On
            SeparateSpecular On
            SetTexture [_MainTex]
            {
                //constantColor [_Color]
                Combine texture * primary DOUBLE, texture * constant
            }
		}
	} 
}
