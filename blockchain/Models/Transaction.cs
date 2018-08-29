using System;
using System.Collections.Generic;
using DisBlockchain.Utils;

namespace DisBlockchain.Models
{
    public class Transaction
    {
        readonly string sender;
        readonly string recipient;
        readonly float value;
        readonly long timestamp;
        byte[] signature;
        List<TransactionInput> inputs = new List<TransactionInput>();
        List<TransactionOutput> outputs = new List<TransactionOutput>();


        public string Sender
        {
            get { return this.sender; }
        }

        public string Recipient
        {
            get { return this.recipient; }
        }

        public long Timestamp
        {
            get { return this.timestamp; }
        }

        public byte[] Signature
        {
            get { return this.signature; }
        }

        public List<TransactionInput> Inputs
        {
            get { return this.inputs; }
        }

        public List<TransactionOutput> Outputs
        {
            get { return this.outputs; }
        }

        public Transaction(string sender, string recipient, float value, List<TransactionInput> inputs, List<TransactionOutput> outputs)
        {
            this.sender = sender;
            this.recipient = recipient;
            this.value = value;
            this.inputs = inputs;
            this.outputs = outputs;
            this.timestamp = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
        }

        public bool IsValid()
        {
            if (!this.VerifySignature())
            {
                Console.WriteLine("Bad signature.");
                return false;
            }

            if (!this.InputsValue().Equals(this.OutputsValue()))
            {
                Console.WriteLine("Inputs doesn't match outputs.");
                return false;
            }

            // check for double spending
            foreach(TransactionInput input in this.inputs)
            {
                if (Blockchain.Instance.UTXOs.ContainsKey(input.UTXO.Id))
                {
                    Console.WriteLine("Double spending.");
                    return false;
                }
            }

            return true;
        }

        public float InputsValue()
        {
            float total = 0;
            foreach(TransactionInput input in this.inputs)
            {
                total += input.UTXO.Value;
            }

            return total;
        }

        public float OutputsValue()
        {
            float total = 0;
            foreach(TransactionOutput output in this.outputs)
            {
                total += output.Value;
            }

            return total;
        }

        public string DataForSignature()
        {
            return this.sender + this.recipient + this.value.ToString() + this.timestamp.ToString();
        }

        public void GenerateSignature(string privateKey)
        {
            this.signature = Cryptography.GetSignature(privateKey, this.DataForSignature());
        }

        public bool VerifySignature()
        {
            return Cryptography.VerifySignature(this.sender, this.DataForSignature(), this.signature);
        }
    }
}
