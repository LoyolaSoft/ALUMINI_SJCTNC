using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Utility
{
    public class DataValueBase
    {
    private string _FieldName;
        private string _FieldValue;
        private EnumCommand.DataType _DataType;

        public DataValueBase(string fieldName, string fieldValue, EnumCommand.DataType dataType)
        {
            this._FieldName = fieldName;
            this._FieldValue = fieldValue;
            this._DataType = dataType;
        }

        public string FieldName
        {
            set { _FieldName = value; }
            get { return _FieldName; }
        }

        public string FieldValue
        {
            set { _FieldValue = value; }
            get { return _FieldValue; }
        }

        public EnumCommand.DataType FieldDataType
        {
            set { _DataType = value; }
            get { return _DataType; }
        }
    }

    public class DataValue
    {
        private List<DataValueBase> DataItem = new List<DataValueBase>();
        public int Count
        {
            get { return DataItem.Count; }
        }
        public DataValueBase this[int index]
        {
            get { return DataItem[index]; }
        }
        public void Add(string fieldName, string fieldValue, EnumCommand.DataType dataType)
        {
            DataItem.Add(new DataValueBase(fieldName, fieldValue, dataType));
        }
        public DataValueBase GetItem(int index)
        {
            return DataItem[index];
        }
        public void Clear()
        {
            DataItem.Clear();
        }
        public void Remove(DataValueBase dv)
        {
            DataItem.Remove(dv);
        }
        public void RemoveAt(int index)
        {
            DataItem.RemoveAt(index);
        }
        public IEnumerator GetEnumerator()
        {
            return DataItem.GetEnumerator();
        }
    }
}


