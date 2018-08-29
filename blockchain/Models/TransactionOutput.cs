using System;
using DisBlockchain.Utils;

namespace DisBlockchain.Models
{
    public class TransactionOutput
    {
        private string id;
        private string recipient;
        private float value;

        public TransactionOutput(string recipient, float value)
        {
            this.id = Guid.NewGuid().ToString();
            this.recipient = recipient;
            this.value = value;
            //this.parentTransactionId = parentTransactionId;
            //this.id = Cryptography.GenerateSHA256String(this.recipient + this.value.ToString() + this.parentTransactionId);
        }

        public string Id
        {
            get { return this.id; }
        }

        public string Recipient
        {
            get { return this.recipient; }
        }

        public float Value
        {
            get { return this.value; }
        }
    }
}
