using Spyder.Client.Common;

namespace Spyder.Client.Scripting
{
    public class StillElement : ScriptElement
    {
        public Content Content
        {
            get
            {
                if (Contents == null || Contents.Count < 1)
                    return null;
                else
                    return Contents[0];
            }
        }

        public string FileName
        {
            get { return Content?.Name; }
        }
    }
}
