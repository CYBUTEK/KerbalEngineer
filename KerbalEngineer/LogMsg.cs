using System.Text;

namespace KerbalEngineer
{
    public class LogMsg
    {
        public StringBuilder buf;

        public LogMsg()
        {
            buf = new StringBuilder(64 * 1024);
        }

        public void Flush()
        {
            if (buf.Length > 0)
            {
                MyLogger.Log(buf);
            }
            buf.Length = 0;
        }

        public LogMsg AppendLine(string val)
        {
            buf.AppendLine(val);
            return this;
        }

        public LogMsg Append<T>(T val)
        {
            buf.Append(val);
            return this;
        }

        public LogMsg Append<T, U>(T val, U val2)
        {
            buf.Append(val);
            buf.Append(val2);
            return this;
        }

        public LogMsg Append<T, U, V>(T val, U val2, V val3)
        {
            buf.Append(val);
            buf.Append(val2);
            buf.Append(val3);
            return this;
        }

        public LogMsg Append<T, U, V, W>(T val, U val2, V val3, W val4)
        {
            buf.Append(val);
            buf.Append(val2);
            buf.Append(val3);
            buf.Append(val4);
            return this;
        }

        public LogMsg Append<T, U, V, W, X>(T val, U val2, V val3, W val4, X val5)
        {
            buf.Append(val);
            buf.Append(val2);
            buf.Append(val3);
            buf.Append(val4);
            buf.Append(val5);
            return this;
        }

        public LogMsg AppendLine<T>(T val)
        {
            buf.Append(val);
            buf.AppendLine();
            return this;
        }

        public LogMsg AppendLine<T, U>(T val, U val2)
        {
            buf.Append(val);
            buf.Append(val2);
            buf.AppendLine();
            return this;
        }

        public LogMsg AppendLine<T, U, V>(T val, U val2, V val3)
        {
            buf.Append(val);
            buf.Append(val2);
            buf.Append(val3);
            buf.AppendLine();
            return this;
        }

        public LogMsg AppendLine<T, U, V, W>(T val, U val2, V val3, W val4)
        {
            buf.Append(val);
            buf.Append(val2);
            buf.Append(val3);
            buf.Append(val4);
            buf.AppendLine();
            return this;
        }

        public LogMsg AppendLine<T, U, V, W, X>(T val, U val2, V val3, W val4, X val5)
        {
            buf.Append(val);
            buf.Append(val2);
            buf.Append(val3);
            buf.Append(val4);
            buf.Append(val5);
            buf.AppendLine();
            return this;
        }

        public LogMsg EOL()
        {
            buf.AppendLine();
            return this;
        }
    }
}
