
using System.Collections.Generic;

namespace SSB.Service.Core.Model
{
    public class SerializeBulk
    {

        #region Fields
        private string _LineNumber;
        private string _BulkText;
        private string _SendDate;
        private byte _SendingType;
        private bool _IsPersian;
        private byte _Status;
        private string _AdminNote;
        private int _TotalPrice;
        private string _UserId;
        private string _RequestDate;
        private int _LineNumberCode;
        private string _Notation;
        private List<SerializeBulk_Request> _SerializeBulk_Request;


        #endregion

        #region Properties

        public int? BulkId { get; set; }

        public string LineNumber
        {
            get { return _LineNumber; }
            set { _LineNumber = value; }
        }
        public string Notation
        {
            get { return _Notation; }
            set { _Notation = value; }
        }
        public string BulkText
        {
            get
            { return _BulkText; }
            set { _BulkText = value; }
        }

        public string SendDate
        {
            get
            {

                return _SendDate;
            }
            set { _SendDate = value; }
        }

        public byte SendingType
        {
            get
            {

                return _SendingType;
            }
            set { _SendingType = value; }
        }

        public bool Ispersian
        {
            get
            {

                return _IsPersian;
            }
            set { _IsPersian = value; }
        }

        public byte Status
        {
            get
            {

                return _Status;
            }
            set { _Status = value; }
        }



        public string AdminNote
        {
            get { return _AdminNote; }
            set { _AdminNote = value; }
        }

        public int TotalPrice
        {
            get
            {

                return _TotalPrice;
            }
            set { _TotalPrice = value; }
        }

        public string RequestDate
        {
            get
            {

                return _RequestDate;
            }
            set { _RequestDate = value; }
        }


        public string UserId
        {
            get { return _UserId; }
            set { _UserId = value; }
        }

        public int LineNumberCode
        {
            get { return _LineNumberCode; }
            set { _LineNumberCode = value; }
        }

        public List<SerializeBulk_Request> SerializeBulk_Request
        {
            get { return _SerializeBulk_Request; }
            set { _SerializeBulk_Request = value; }
        }
        #endregion

    }
}
