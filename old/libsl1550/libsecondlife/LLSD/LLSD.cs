using System;
using System.Collections;
using System.Collections.Generic;

namespace libsecondlife.StructuredData
{
    public enum LLSDType
    {
        Unknown,
        Boolean,
        Integer,
        Real,
        String,
        UUID,
        Date,
        URI,
        Binary,
        Map,
        Array
    }

    public class LLSDException : Exception
    {
        public LLSDException(string message) : base(message) { }
    }

    public partial class LLSD
    {
        public virtual LLSDType Type { get { return LLSDType.Unknown; } }

        public virtual bool AsBoolean() { return false; }
        public virtual int AsInteger() { return 0; }
        public virtual double AsReal() { return 0d; }
        public virtual string AsString() { return String.Empty; }
        public virtual LLUUID AsUUID() { return LLUUID.Zero; }
        public virtual DateTime AsDate() { return Helpers.Epoch; }
        public virtual Uri AsUri() { return new Uri(String.Empty); }
        public virtual byte[] AsBinary() { return new byte[0]; }

        public static LLSD FromBoolean(bool value) { return new LLSDBoolean(value); }
        public static LLSD FromInteger(int value) { return new LLSDInteger(value); }
        public static LLSD FromInteger(uint value) { return new LLSDInteger((int)value); }
        public static LLSD FromInteger(short value) { return new LLSDInteger((int)value); }
        public static LLSD FromInteger(ushort value) { return new LLSDInteger((int)value); }
        public static LLSD FromInteger(sbyte value) { return new LLSDInteger((int)value); }
        public static LLSD FromInteger(byte value) { return new LLSDInteger((int)value); }
        public static LLSD FromReal(double value) { return new LLSDReal(value); }
        public static LLSD FromReal(float value) { return new LLSDReal((double)value); }
        public static LLSD FromString(string value) { return new LLSDString(value); }
        public static LLSD FromUUID(LLUUID value) { return new LLSDUUID(value); }
        public static LLSD FromDate(DateTime value) { return new LLSDDate(value); }
        public static LLSD FromUri(Uri value) { return new LLSDURI(value); }
        public static LLSD FromBinary(byte[] value) { return new LLSDBinary(value); }
        public static LLSD FromBinary(long value) { return new LLSDBinary(value); }
        public static LLSD FromBinary(ulong value) { return new LLSDBinary(value); }
        public static LLSD FromObject(object value)
        {
            if (value == null) { return new LLSD(); }
            else if (value is bool) { return new LLSDBoolean((bool)value); }
            else if (value is int) { return new LLSDInteger((int)value); }
            else if (value is uint) { return new LLSDInteger((int)(uint)value); }
            else if (value is short) { return new LLSDInteger((int)(short)value); }
            else if (value is ushort) { return new LLSDInteger((int)(ushort)value); }
            else if (value is sbyte) { return new LLSDInteger((int)(sbyte)value); }
            else if (value is byte) { return new LLSDInteger((int)(byte)value); }
            else if (value is double) { return new LLSDReal((double)value); }
            else if (value is float) { return new LLSDReal((double)(float)value); }
            else if (value is string) { return new LLSDString((string)value); }
            else if (value is LLUUID) { return new LLSDUUID((LLUUID)value); }
            else if (value is DateTime) { return new LLSDDate((DateTime)value); }
            else if (value is Uri) { return new LLSDURI((Uri)value); }
            else if (value is byte[]) { return new LLSDBinary((byte[])value); }
            else if (value is long) { return new LLSDBinary((long)value); }
            else if (value is ulong) { return new LLSDBinary((ulong)value); }
            else return new LLSD();
        }
    }

    public class LLSDBoolean : LLSD
    {
        private bool value;

        public override LLSDType Type { get { return LLSDType.Boolean; } }

        public LLSDBoolean(bool value)
        {
            this.value = value;
        }

        public override bool AsBoolean() { return value; }
        public override int AsInteger() { return value ? 1 : 0; }
        public override double AsReal() { return value ? 1d : 0d; }
        public override string AsString() { return value ? "1" : "0"; }

        public override string ToString() { return AsString(); }
    }

    public class LLSDInteger : LLSD
    {
        private int value;

        public override LLSDType Type { get { return LLSDType.Integer; } }

        public LLSDInteger(int value)
        {
            this.value = value;
        }

        public override bool AsBoolean() { return value != 0; }
        public override int AsInteger() { return value; }
        public override double AsReal() { return (double)value; }
        public override string AsString() { return value.ToString(); }

        public override string ToString() { return AsString(); }
    }

    public class LLSDReal : LLSD
    {
        private double value;

        public override LLSDType Type { get { return LLSDType.Real; } }

        public LLSDReal(double value)
        {
            this.value = value;
        }

        public override bool AsBoolean() { return (!Double.IsNaN(value) && value != 0d); }
        public override int AsInteger() { return !Double.IsNaN(value) ? (int)value : 0; }
        public override double AsReal() { return value; }
        public override string AsString() { return value.ToString(Helpers.EnUsCulture); }

        public override string ToString() { return AsString(); }
    }

    public class LLSDString : LLSD
    {
        private string value;

        public override LLSDType Type { get { return LLSDType.String; } }

        public LLSDString(string value)
        {
            // Refuse to hold null pointers
            if (value != null)
                this.value = value;
            else
                this.value = String.Empty;
        }

        public override bool AsBoolean() { return !String.IsNullOrEmpty(value); }
        public override int AsInteger()
        {
            double dbl;
            if (Double.TryParse(value, out dbl))
                return (int)dbl;
            else
                return 0;
        }
        public override double AsReal()
        {
            double dbl;
            if (Double.TryParse(value, out dbl))
                return dbl;
            else
                return 0d;
        }
        public override string AsString() { return value; }
        public override LLUUID AsUUID()
        {
            LLUUID uuid;
            if (LLUUID.TryParse(value, out uuid))
                return uuid;
            else
                return LLUUID.Zero;
        }
        public override DateTime AsDate()
        {
            DateTime dt;
            if (DateTime.TryParse(value, out dt))
                return dt;
            else
                return Helpers.Epoch;
        }
        public override Uri AsUri() { return new Uri(value); }

        public override string ToString() { return AsString(); }
    }

    public class LLSDUUID : LLSD
    {
        private LLUUID value;

        public override LLSDType Type { get { return LLSDType.UUID; } }

        public LLSDUUID(LLUUID value)
        {
            this.value = value;
        }

        public override string AsString() { return value.ToString(); }
        public override LLUUID AsUUID() { return value; }

        public override string ToString() { return AsString(); }
    }

    public class LLSDDate : LLSD
    {
        private DateTime value;

        public override LLSDType Type { get { return LLSDType.Date; } }

        public LLSDDate(DateTime value)
        {
            this.value = value;
        }

        public override string AsString() { return value.ToString(); }
        public override DateTime AsDate() { return value; }

        public override string ToString() { return AsString(); }
    }

    public class LLSDURI : LLSD
    {
        private Uri value;

        public override LLSDType Type { get { return LLSDType.URI; } }

        public LLSDURI(Uri value)
        {
            this.value = value;
        }

        public override string AsString() { return value.ToString(); }
        public override Uri AsUri() { return value; }

        public override string ToString() { return AsString(); }
    }

    public class LLSDBinary : LLSD
    {
        private byte[] value;

        public override LLSDType Type { get { return LLSDType.Binary; } }

        public LLSDBinary(byte[] value)
        {
            if (value != null)
                this.value = value;
            else
                this.value = new byte[0];
        }

        public LLSDBinary(long value)
        {
            this.value = BitConverter.GetBytes(value);
        }

        public LLSDBinary(ulong value)
        {
            this.value = BitConverter.GetBytes(value);
        }

        public override string AsString() { return Convert.ToBase64String(value); }
        public override byte[] AsBinary() { return value; }

        public override string ToString()
        {
            // TODO: ToString() is only meant for friendly display, a hex string would
            // be more friendly then a base64 string
            return AsString();
        }
    }

    public class LLSDMap : LLSD, IDictionary<string, LLSD>
    {
        private Dictionary<string, LLSD> value;

        public override LLSDType Type { get { return LLSDType.Map; } }

        public LLSDMap()
        {
            value = new Dictionary<string, LLSD>();
        }

        public LLSDMap(int capacity)
        {
            value = new Dictionary<string, LLSD>(capacity);
        }

        public LLSDMap(Dictionary<string, LLSD> value)
        {
            if (value != null)
                this.value = value;
            else
                this.value = new Dictionary<string, LLSD>();
        }

        public override bool AsBoolean() { return value.Count > 0; }

        public override string ToString()
        {
            System.Text.StringBuilder output = new System.Text.StringBuilder("{");

            foreach (KeyValuePair<string, LLSD> kvp in value)
            {
                output.AppendFormat("(\"{0}\": \"{1}\")", kvp.Key, kvp.Value);
            }
            output.Append("}");

            return output.ToString();
        }

        #region IDictionary Implementation

        public int Count { get { return value.Count; } }
        public bool IsReadOnly { get { return false; } }
        public ICollection<string> Keys { get { return value.Keys; } }
        public ICollection<LLSD> Values { get { return value.Values; } }
        public LLSD this[string key]
        {
            get
            {
                LLSD llsd;
                if (this.value.TryGetValue(key, out llsd))
                    return llsd;
                else
                    return new LLSD();
            }
            set { this.value[key] = value; }
        }

        public bool ContainsKey(string key)
        {
            return value.ContainsKey(key);
        }

        public void Add(string key, LLSD llsd)
        {
            value.Add(key, llsd);
        }

        public void Add(KeyValuePair<string, LLSD> kvp)
        {
            value.Add(kvp.Key, kvp.Value);
        }

        public bool Remove(string key)
        {
            return value.Remove(key);
        }

        public bool TryGetValue(string key, out LLSD llsd)
        {
            return value.TryGetValue(key, out llsd);
        }

        public void Clear()
        {
            value.Clear();
        }

        public bool Contains(KeyValuePair<string, LLSD> kvp)
        {
            // This is a bizarre function... we don't really implement it
            // properly, hopefully no one wants to use it
            return value.ContainsKey(kvp.Key);
        }

        public void CopyTo(KeyValuePair<string, LLSD>[] array, int index)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<string, LLSD> kvp)
        {
            return this.value.Remove(kvp.Key);
        }

        public System.Collections.IDictionaryEnumerator GetEnumerator()
        {
            return value.GetEnumerator();
        }

        IEnumerator<KeyValuePair<string, LLSD>> IEnumerable<KeyValuePair<string, LLSD>>.GetEnumerator()
        {
            return null;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return value.GetEnumerator();
        }

        #endregion IDictionary Implementation
    }

    public class LLSDArray : LLSD, IList<LLSD>
    {
        private List<LLSD> value;

        public override LLSDType Type { get { return LLSDType.Array; } }

        public LLSDArray()
        {
            value = new List<LLSD>();
        }

        public LLSDArray(int capacity)
        {
            value = new List<LLSD>(capacity);
        }

        public LLSDArray(List<LLSD> value)
        {
            if (value != null)
                this.value = value;
            else
                this.value = new List<LLSD>();
        }

        public override bool AsBoolean() { return value.Count > 0; }

        public override string ToString()
        {
            System.Text.StringBuilder output = new System.Text.StringBuilder("[");
            for (int i = 0; i < value.Count; i++)
            {
                output.AppendFormat("\"{0}\"", value[i]);
                if (i != value.Count - 1)
                    output.Append(",");
            }
            output.Append("]");

            return output.ToString();
        }

        #region IList Implementation

        public int Count { get { return value.Count; } }
        public bool IsReadOnly { get { return false; } }
        public LLSD this[int index]
        {
            get { return value[index]; }
            set { this.value[index] = value; }
        }

        public int IndexOf(LLSD llsd)
        {
            return value.IndexOf(llsd);
        }

        public void Insert(int index, LLSD llsd)
        {
            value.Insert(index, llsd);
        }

        public void RemoveAt(int index)
        {
            value.RemoveAt(index);
        }

        public void Add(LLSD llsd)
        {
            value.Add(llsd);
        }

        public void Add(string str)
        {
            // This is so common that we throw a little helper in here
            value.Add(LLSD.FromString(str));
        }

        public void Clear()
        {
            value.Clear();
        }

        public bool Contains(LLSD llsd)
        {
            return value.Contains(llsd);
        }

        public bool Contains(string element)
        {
            for (int i = 0; i < value.Count; i++)
            {
                if (value[i].Type == LLSDType.String && value[i].AsString() == element)
                    return true;
            }

            return false;
        }

        public void CopyTo(LLSD[] array, int index)
        {
            throw new NotImplementedException();
        }

        public bool Remove(LLSD llsd)
        {
            return value.Remove(llsd);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return value.GetEnumerator();
        }

        IEnumerator<LLSD> IEnumerable<LLSD>.GetEnumerator()
        {
            return value.GetEnumerator();
        }

        #endregion IList Implementation
    }
}
