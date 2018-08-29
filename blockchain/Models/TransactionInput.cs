using System;
namespace DisBlockchain.Models
{
    public class TransactionInput
    {
        private string transactionOutputId;
        public TransactionOutput UTXO;

        public TransactionInput(TransactionOutput UTXO)
        {
            this.transactionOutputId = UTXO.Id;
            this.UTXO = UTXO;
        }
    }
}
