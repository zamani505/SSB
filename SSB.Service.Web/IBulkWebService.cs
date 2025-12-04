using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SSB.Service.Web
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IBulkWebService" in both code and config file together.
    [ServiceContract]
    public interface IBulkWebService
    {
        [OperationContract]
        int SaveBulk(SerializeBulk bulk, List<SerializeBulk_Request> BulkRequests);

        [OperationContract]
        List<SerializeBulkStatusInfo> GetBulksState(List<int> lstBulkId);
    }
    [DataContract]
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


        #endregion

        #region Properties

        public int? BulkId { get; set; }

        [DataMember(IsRequired = true)]
        public string LineNumber
        {
            get { return _LineNumber; }
            set { _LineNumber = value; }
        }

        [DataMember(IsRequired = true)]
        public string BulkText
        {
            get
            { return _BulkText; }
            set { _BulkText = value; }
        }

        [DataMember(IsRequired = true)]
        public string SendDate
        {
            get
            {

                return _SendDate;
            }
            set { _SendDate = value; }
        }

        [DataMember(IsRequired = true)]
        public byte SendingType
        {
            get
            {

                return _SendingType;
            }
            set { _SendingType = value; }
        }

        [DataMember(IsRequired = true)]
        public bool Ispersian
        {
            get
            {

                return _IsPersian;
            }
            set { _IsPersian = value; }
        }

        [DataMember(IsRequired = true)]
        public byte Status
        {
            get
            {

                return _Status;
            }
            set { _Status = value; }
        }



        [DataMember(IsRequired = true)]
        public string AdminNote
        {
            get { return _AdminNote; }
            set { _AdminNote = value; }
        }

        [DataMember(IsRequired = true)]
        public int TotalPrice
        {
            get
            {

                return _TotalPrice;
            }
            set { _TotalPrice = value; }
        }

        [DataMember(IsRequired = true)]
        public string RequestDate
        {
            get
            {

                return _RequestDate;
            }
            set { _RequestDate = value; }
        }


        [DataMember(IsRequired = true)]
        public string UserId
        {
            get { return _UserId; }
            set { _UserId = value; }
        }


        #endregion

    }

    [DataContract]
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
        [DataMember(IsRequired = true)]
        public byte PhoneType
        {
            get { return _PhoneType; }
            set { _PhoneType = value; }
        }

        [DataMember(IsRequired = true)]
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

        [DataMember(IsRequired = true)]
        public int ValueOperation
        {
            get { return _ValueOperation; }
            set { _ValueOperation = value; }
        }


        [DataMember(IsRequired = true)]
        public string BulkValue
        {
            get { return _BulkValue; }
            set { _BulkValue = value; }
        }



        [DataMember(IsRequired = true)]
        public int RequestCount
        {
            get { return _RequestCount; }
            set { _RequestCount = value; }
        }


        [DataMember(IsRequired = true)]
        public int StartIndex
        {
            get { return _StartIndex; }
            set { _StartIndex = value; }
        }


        [DataMember(IsRequired = true)]
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
    [DataContract]
    public class SerializeBulkStatusInfo
    {
        [DataMember]
        public int BulkServerNumber { get; set; }

        [DataMember]
        public byte? BulkStatus { get; set; }

        [DataMember]
        public string AdminNote { get; set; }
    }
}
