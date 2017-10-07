#ifndef _PROCEDURAL_TREE_COMMON_
#define _PROCEDURAL_TREE_COMMON_

fixed _T;

void procedural_tree_clip(float2 uv) {
	clip(_T - uv.y);
}

#endif