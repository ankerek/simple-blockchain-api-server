using System;
using System.Collections.Generic;
using DisBlockchain.Models;
using Xunit;


namespace DisBlockchainTests
{

    public class BlockchainTest
    {
        Wallet walletA = new Wallet();
		readonly float INITIAL_VALUE = 100;

        // constructor is called before each test
        public BlockchainTest()
        {
            
        }

        // cleanup
        public void Dispose()
        {
        }

        [Fact]
        public void WalletBalance()
        {
            Blockchain.Instance.AddUTXO(new TransactionOutput(walletA.PublicKey, INITIAL_VALUE));
            Assert.Equal(INITIAL_VALUE, this.walletA.GetBalance());
        }

        [Fact]
        public void WalletBalance2()
        {
            Blockchain.Instance.AddUTXO(new TransactionOutput(walletA.PublicKey, INITIAL_VALUE));
            float sendValue = 10;
            Wallet walletB = new Wallet();

            walletA.CreateTransaction(walletB.PublicKey, sendValue);

            Assert.Equal(INITIAL_VALUE - sendValue, walletA.GetBalance());
            Assert.Equal(sendValue, walletB.GetBalance());
        }

        [Fact]
        public void BlockchainFlow()
        {
            Wallet walletB = new Wallet();
            Wallet walletC = new Wallet();
            Wallet walletD = new Wallet();

            Blockchain.Instance.MineBlock(walletB.PublicKey);

            Assert.Equal(Blockchain.Instance.MinerReward, walletB.GetBalance());

            walletB.CreateTransaction(walletC.PublicKey, 5);
            walletB.CreateTransaction(walletD.PublicKey, 5);

            Assert.Throws<OperationCanceledException>(() => walletB.CreateTransaction(walletC.PublicKey, 5));

            Assert.Equal(Blockchain.Instance.MinerReward - 10, walletB.GetBalance());
            Assert.Equal(Blockchain.Instance.MinerReward - 5, walletC.GetBalance());
            Assert.Equal(Blockchain.Instance.MinerReward - 5, walletD.GetBalance());
            Assert.Equal(2, Blockchain.Instance.CurrentTransactions.Count);

            Blockchain.Instance.MineBlock(walletB.PublicKey);

            Assert.Equal(Blockchain.Instance.MinerReward - 10 + Blockchain.Instance.MinerReward, walletB.GetBalance());

            Assert.True(Blockchain.Instance.IsValid());
        }

        // double spending
        [Fact]
        public void InvalidTransaction()
        {
            Wallet walletB = new Wallet();
            TransactionOutput transactionOutput = new TransactionOutput(walletA.PublicKey, INITIAL_VALUE);
            Blockchain.Instance.AddUTXO(transactionOutput);

            TransactionInput transactionInput = new TransactionInput(transactionOutput);

            Transaction transaction = new Transaction(
                walletA.PublicKey, walletB.PrivateKey,
                10,
                new List<TransactionInput> { transactionInput },
                new List<TransactionOutput> { transactionOutput }
            );

            transaction.GenerateSignature(walletA.PrivateKey);

            Assert.False(transaction.IsValid());
        }

    }
}
