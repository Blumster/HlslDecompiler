vs_4_0
dcl_constantbuffer cb0[3], immediateIndexed
dcl_input v0.xyz
dcl_output o0.xyzw
dcl_output o1.xyz
dcl_output o2.xyz
dcl_output o3.xyz
dcl_temps 1
dp3 o0.x, v0.xyzx, cb0[0].xyzx
dp3 o0.y, v0.xyzx, cb0[1].xyzx
dp3 o0.z, v0.xyzx, cb0[2].xyzx
mov o0.w, l(1.000000)
dp3 o1.x, v0.yxzy, cb0[0].xyzx
dp3 o1.y, v0.yxzy, cb0[1].xyzx
dp3 o1.z, v0.yxzy, cb0[2].xyzx
dp3 o2.x, |v0.yxzy|, cb0[0].xyzx
dp3 o2.y, |v0.yxzy|, cb0[1].xyzx
dp3 o2.z, |v0.yxzy|, cb0[2].xyzx
mul r0.xyz, v0.yxzy, l(1.000000, 2.000000, 3.000000, 0.000000)
dp3 o3.x, r0.xyzx, cb0[0].xyzx
dp3 o3.y, r0.xyzx, cb0[1].xyzx
dp3 o3.z, r0.xyzx, cb0[2].xyzx
ret
