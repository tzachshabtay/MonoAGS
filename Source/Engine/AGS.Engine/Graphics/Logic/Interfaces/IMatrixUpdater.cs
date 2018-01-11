using System;
using AGS.API;

namespace AGS.Engine
{
    public interface IMatrixUpdater
    {
        void ClearCache();
        void RefreshMatrix(IObject obj);
    }
}
