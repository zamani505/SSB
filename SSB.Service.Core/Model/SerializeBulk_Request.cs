
namespace SSB.Service.Core.Model
{
    public class SerializeBulk_Request
    {
        #region Fields

        private int _CategoryId;

        private int _StartIndex;

        private int _RequestCount;



        private string _BulkValue;

        private int _ValueType;

        private int _ValueOperation;

        private byte _PhoneType;

        #endregion

        #region Properties
        public byte PhoneType
        {
            get { return _PhoneType; }
            set { _PhoneType = value; }
        }

        public int ValueType
        {
            get
            {
                return _ValueType;
            }
            set
            {

                _ValueType = value;
            }
        }

        public int ValueOperation
        {
            get { return _ValueOperation; }
            set { _ValueOperation = value; }
        }


        public string BulkValue
        {
            get { return _BulkValue; }
            set { _BulkValue = value; }
        }



        public int RequestCount
        {
            get { return _RequestCount; }
            set { _RequestCount = value; }
        }


        public int StartIndex
        {
            get { return _StartIndex; }
            set { _StartIndex = value; }
        }


        public int CategoryId
        {
            get { return _CategoryId; }
            set
            {

                _CategoryId = value;
            }
        }

        #endregion
    }
}
