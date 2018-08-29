using System;
using System.Collections.Generic;
using DisBlockchain.Utils;
using Newtonsoft.Json;

namespace DisBlockchain.Models
{
    public class Wallet
    {
        private readonly string privateKey;
        private readonly string publicKey;

        public string PrivateKey
        {
            get { return this.privateKey; }
        }

        public string PublicKey
        {
            get { return this.publicKey; }
        }

        // privateKey is optional parameter for loading wallet from existing privateKey, wallet is fully functional
        // publicKey is optional, wallet is not fully functional
        public Wallet(string privateKey = null, string publicKey = null)
        {
            if (privateKey != null)
            {
                this.privateKey = privateKey;
                this.publicKey = Cryptography.GetPublicKeyFromPrivateKey(privateKey);
            }
            else if(publicKey != null)
            {
                this.publicKey = publicKey;
            }
            else
            {
                Tuple<string, string> keys = Cryptography.GenerateKeyPair();
                this.privateKey = keys.Item1;
                this.publicKey = keys.Item2;
            }
        }

        // loop through all unspent transaction outputs
        // and calculate this wallet's balance
        public float GetBalance()
        {
            float balance = 0;

            foreach (KeyValuePair<string, TransactionOutput> item in Blockchain.Instance.UTXOs)
            {
                TransactionOutput UTXO = item.Value;

                if (UTXO.Recipient == this.publicKey) {
                    balance += UTXO.Value;
                }
            }

            return balance;
        }

        public Transaction CreateTransaction(string recipient, float value)
        {
            if (this.privateKey == null)
            {
                throw new OperationCanceledException("Wallet is missing privateKey.");
            }

            // check the balance
            if (this.GetBalance() < value)
            {
                throw new OperationCanceledException("Not enough funds. Transaction canceled.");
            }

            // get my unspent transaction outputs
            Dictionary<string, TransactionOutput> myUTXOs = new Dictionary<string, TransactionOutput>();
            foreach (KeyValuePair<string, TransactionOutput> item in Blockchain.Instance.UTXOs)
            {
                if (item.Value.Recipient == this.publicKey)
                {
                    myUTXOs.Add(item.Key, item.Value);
                }
            }

            List<TransactionInput> inputs = new List<TransactionInput>();

            float inputsValue = 0;
            // fill transaction inputs from unspent transaction outputs
            foreach (KeyValuePair<string, TransactionOutput> item in myUTXOs)
            {
                TransactionOutput UTXO = item.Value;
                inputsValue += UTXO.Value;
                inputs.Add(new TransactionInput(UTXO));
                Blockchain.Instance.UTXOs.Remove(item.Key);

                if (inputsValue > value) break;
            }

            // generate transaction outputs
            float leftOver = inputsValue - value;
            List<TransactionOutput> outputs = new List<TransactionOutput>();
            outputs.Add(new TransactionOutput(recipient, value));
            if (leftOver > 0)
            {
                outputs.Add(new TransactionOutput(this.publicKey, leftOver));
            }
            // add new outputs to global unspent transaction outputs
            foreach (TransactionOutput output in outputs)
            {
                Blockchain.Instance.AddUTXO(output);
            }

            // create new transaction
            Transaction transaction = new Transaction(this.publicKey, recipient, value, inputs, outputs);
            transaction.GenerateSignature(this.privateKey);

            Blockchain.Instance.AddCurrentTransaction(transaction);

            return transaction;
        }

    }
}
