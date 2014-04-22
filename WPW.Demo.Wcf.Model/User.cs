using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace WPW.Demo.Wcf.Model
{
    [DataContract]
    [Serializable]
    public class User
    {
        private int userID;
        [DataMember]
        public int UserID
        {
            get { return userID; }
            set { userID = value; }
        }

        private string userName;
        [DataMember]
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }
    }
}
