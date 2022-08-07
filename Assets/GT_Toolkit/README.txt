GAMETEXTURES SHADE - Unity 2018
===============================================
SHADE is a collection of pre-defined materials
for Metallic and Specular workflows
-----------------------------------------------


SHADE - Unity consists of basic shaders for Standard,
Light-Weight Render Pipeline and High-Definition Render Pipeline:

===============================================
GT_Metallic / GT_Specular
===============================================
Basic shader template for any type of asset.
Geared toward entry-level users.
-----------------------------------------------

[METALLIC]
Base Color (tex)
Base Color Tint (color)
Metallic (tex)
Roughness (tex)
Invert Roughness (bool)
Roughness Multiplier (scalar)

[SPECULAR]
Albedo (tex)
Albedo Color Tint (color)
Specular Color (tex)
Glossiness (tex)
Invert Glossiness (bool)
Glossiness Multiplier (scalar)

[GLOBAL]
UV Tile / Offset (v4): X/Y = UV, Z/W = OFFSET
Ambient Occlusion (tex)
Ambient Occlusion Multiplier (scalar)
Height (tex)
Normal (tex)
Invert Normal (bool)
Emissive (tex)
Emissive Multiplier (scalar)
Opacity (tex)
Mask Clip Value(scalar)

[POM]
Use Pom (bool)
POM Height (scalar)
Curvature V (scalar)
Curvature U (scalar)

===============================================
LWRP_GT_Metallic / LWRP_GT_Specular
HDRP_GT_Metallic / HDRP_GT_Specular
===============================================
Basic shader template for any type of asset for the LWRP / HDRP.
Geared toward entry-level users.
-----------------------------------------------

[METALLIC]
Base Color (tex)
Base Color Tint (color)
Metallic (tex)
Roughness (tex)
Invert Roughness (bool)
Roughness Multiplier (scalar)

[SPECULAR]
Albedo (tex)
Albedo Color Tint (color)
Specular Color (tex)
Glossiness (tex)
Invert Glossiness (bool)
Glossiness Multiplier (scalar)

[GLOBAL]
UV Tile / Offset (v4): X/Y = UV, Z/W = OFFSET
Ambient Occlusion (tex)
Ambient Occlusion Multiplier (scalar)
Height (tex)
Normal (tex)
Invert Normal (bool)
Emissive (tex)
Emissive Multiplier (scalar)
Opacity (tex)


