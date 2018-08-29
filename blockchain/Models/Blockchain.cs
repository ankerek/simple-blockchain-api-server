using System;
using System.Linq;
using System.Collections.Generic;

// singleton class for storing the one and only blockchain
namespace DisBlockchain.Models
{
    public sealed class Blockchain
    {
        static readonly Blockchain instance = new Blockchain();
        static readonly float minerReward = 10;

        // linked blocks
        private List<Block> blocks = new List<Block>();

        // current transactions that are waiting to be mined
        private List<Transaction> currentTransactions = new List<Transaction>();

        // unspent transaction outputs
        public Dictionary<string, TransactionOutput> UTXOs = new Dictionary<string, TransactionOutput>();


        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static Blockchain()
        {
        }

        private Blockchain()
        {
        }

        public static Blockchain Instance
        {
            get { return instance; }
        }

        public float MinerReward
        {
            get { return minerReward; }
        }

        public List<Block> Blocks
        {
            get { return this.blocks; }
        }

        public List<Transaction> CurrentTransactions
        {
            get { return this.currentTransactions; }
        }

        public void AddCurrentTransaction(Transaction transaction)
        {
            this.currentTransactions.Add(transaction);
        }

        public void AddUTXO(TransactionOutput UTXO)
        {
            this.UTXOs.Add(UTXO.Id, UTXO);
        }

        public Block MineBlock(string minerPublicKey = null)
        {
            List<Transaction> transactions = new List<Transaction>();

            foreach(Transaction transaction in this.currentTransactions)
            {
                if (transaction.IsValid())
                {
                    transactions.Add(transaction);
                }
                else
                {
                    // if transaction is not valid, restore all unspent transaction outputs from inputs
                    foreach(TransactionInput input in transaction.Inputs)
                    {
                        Instance.AddUTXO(input.UTXO);
                    }
                }
            }

            // previous block's hash
            string previousHash = this.blocks.Count > 0 ? this.blocks.Last().Hash : null;

            // create a new block with all current transactinos
            Block block = new Block((uint) this.blocks.Count, previousHash, transactions);
            this.blocks.Add(block);

            // clear current transactions
            this.currentTransactions.Clear();

            if(minerPublicKey != null)
            {
				this.GenerateMiningReward(minerPublicKey);
            }

            return block;
        }

        // generate new unspent transaction output as a reward for miner
        public void GenerateMiningReward(string minerPublicKey)
        {
            TransactionOutput transactionOutput = new TransactionOutput(minerPublicKey, minerReward);
            Instance.AddUTXO(transactionOutput);
        }


        //
        public bool IsValid()
        {
            Block previousBlock;
            Block currentBlock;

            for (int i = 0; i < Instance.blocks.Count; i++)
            {
                previousBlock = i == 0 ? null : Instance.blocks.ElementAt(i - 1);
                currentBlock = Instance.blocks.ElementAt(i);

                // check hashes
                if (currentBlock.Hash != currentBlock.CalculateHash())
                {
                    return false;
                }

                if (previousBlock != null && currentBlock.PreviousHash != previousBlock.Hash)
                {
                    return false;
                }

                foreach(Transaction transaction in currentBlock.Transactions)
                {
                    if (!transaction.IsValid())
                    {
                        return false;
                    }
                }

            }

            return true;
        }

    }
}
