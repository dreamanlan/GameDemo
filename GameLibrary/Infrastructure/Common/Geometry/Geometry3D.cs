using System;
using System.Collections.Generic;
//using System.Diagnostics;
using UnityEngine;

namespace GameLibrary
{
  public class Geometry3D
  {
    public static UnityEngine.Vector3 GetCenter(UnityEngine.Vector3 fvPos1, UnityEngine.Vector3 fvPos2)
    {
      UnityEngine.Vector3 fvRet = new UnityEngine.Vector3();

      fvRet.x = (fvPos1.x + fvPos2.x) / 2.0f;
      fvRet.y = (fvPos1.y + fvPos2.y) / 2.0f;
      fvRet.z = (fvPos1.z + fvPos2.z) / 2.0f;

      return fvRet;
    }
  }
}

