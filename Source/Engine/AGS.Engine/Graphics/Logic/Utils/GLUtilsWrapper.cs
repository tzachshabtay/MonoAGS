namespace AGS.Engine
{
    public class GLUtilsWrapper : IGLUtils
    {
        public void AdjustResolution(int width, int height)
        {
            GLUtils.AdjustResolution(width, height);
        }
    }
}
