ps_4_0
dcl_input_ps linear v0.xyzw
dcl_output o0.xyzw
dcl_output o1.xyzw
dcl_output o2.xyzw
dcl_output o3.xyzw
mov o0.xyzw, v0.xyzw
mov o1.xyz, v0.xyzx
mov o1.w, l(0)
mov o2.xy, v0.xyxx
mov o2.zw, l(0,0,0,1.000000)
mov o3.x, v0.x
mov o3.yzw, l(0,0,1.000000,2.000000)
ret 
