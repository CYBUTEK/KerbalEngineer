using System.Text;

using UnityEngine;

namespace KerbalEngineer
{
    public class LogMsg
    {
        public StringBuilder buf;

        public LogMsg()
        {
            this.buf = new StringBuilder();
        }

        public void Flush()
        {
            if (this.buf.Length > 0)
                MonoBehaviour.print(this.buf);
            this.buf.Length = 0;
        }
    }
}
