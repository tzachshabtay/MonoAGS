using System;
using AGS.API;

namespace AGS.Engine
{
    public interface IAGSInput : IInput
    {
        void Init(API.Size virtualResolution);
    }
}
