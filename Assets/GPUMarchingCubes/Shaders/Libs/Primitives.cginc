#ifndef primitives_h
#define primitives_h

// 球体
inline float sphere(float3 pos, float radius)
{
    return length(pos) - radius;
}

// 角丸立方体
inline float roundBox(float3 pos, float3 size, float round)
{
    return length(max(abs(pos) - size * 0.5, 0.0)) - round;
}

// 立方体
inline float box(float3 pos, float3 size)
{
	return roundBox(pos, size, 0);
}

// ドーナツ
float torus(float3 pos, float2 radius)
{
    float2 r = float2(length(pos.xy) - radius.x, pos.z);
    return length(r) - radius.y;
}

// 床
inline float floorPlane(float3 pos)
{
    return dot(pos, float3(0.0, 1.0, 0.0)) + 1.0;
}

// 円筒
inline float cylinder(float3 pos, float2 r){
    float2 d = abs(float2(length(pos.xy), pos.z)) - r;
    return min(max(d.x, d.y), 0.0) + length(max(d, 0.0)) - 0.1;
}

// カプセル
// a:始点, b:終点, r:半径
inline float Capsule(float3 p, float3 a, float3 b, float r)
{
	float3 pa = p - a, ba = b - a;
	float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
	return length(pa - ba*h) - r;
}

// 楕円
inline float ellipsoid(in float3 p, in float3 r)
{
	return (length(p / r) - 1.0) * min(min(r.x, r.y), r.z);
}

// 六角形(平面)
inline float hex(float2 p, float2 h)
{
	float2 q = abs(p);
	return max(q.x - h.y, max(q.x + q.y*0.57735, q.y*1.1547) - h.x);
}

// 六角柱(高さ付き)
// h.x:半径 h.y:高さ
inline float hexagonalPrismY(float3 pos, float2 h)
{
	float3 p = abs(pos);
	return max(
		p.y - h.y,
		max(
			(p.z * 0.866025 + p.x * 0.5),
			p.x
			) - h.x
		);
}
#endif