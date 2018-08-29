using System;
using System.Collections.Generic;
using DisBlockchain.Utils;
using Newtonsoft.Json;

namespace DisBlockchain.Models
{
    public class Block
    {

        private uint index;
        private long timestamp;
        private string hash;
        private string previousHash;
        private List<Transaction> transactions;

        public uint Index
        {
            get { return this.index; }
        }

        public string Hash
        {
            get { return this.hash; }
        }

        public string PreviousHash
        {
            get { return this.previousHash; }
        }

        public List<Transaction> Transactions
        {
            get { return this.transactions; }
        }

        public long Timestamp
        {
            get { return this.timestamp; }
        }

        // Block constructor
        public Block(uint index, string previousHash, List<Transaction> transactions)
        {
            this.index = index;
            this.previousHash = previousHash;
            this.transactions = transactions;
            this.timestamp = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();

            // block hash generated is from block's information
            this.hash = this.CalculateHash();
        }

        // calculate hash based on block's properties
        public string CalculateHash()
        {
            return Cryptography.GenerateSHA256String(this.index.ToString() + this.previousHash + this.timestamp + JsonConvert.SerializeObject(this.transactions));
        }
    }
}
