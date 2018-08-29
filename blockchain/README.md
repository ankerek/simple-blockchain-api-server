# Simple Blockchain API server

Simple Blockchain API server built with <span>ASP</span>.NET Core Web API.

## Features
 - Chained list of blocks containing information about transactions
 - Generating wallets
 - Sending transactions from wallets
 - Secured by cryptography preventing sending funds from different wallets, double spending

## API documentation

|Method|URL|Description
|----------------|-------------------------------|-----------------------------|
|GET|`/api/blockchain/blocks`|[Get all blocks](#get-apiblockchainblocks)|
|POST|`/api/blockchain/blocks/mine`|[Mine new block](#post-apiblockchainblocksmine)|
|GET|`/api/blockchain/transactions`|[Get all unconfirmed transactions](#get-apiblockchaintransactions)|
|GET|`/api/blockchain/unspent`|[Get all unspent transactions outputs](#get-apiblockchainunspent)|
|GET|`/api/blockchain/valid`|[Get blockchain's validity](#get-apiblockchainvalid)|
|POST|`/api/wallet`|[Create a new wallet](#post-apiwallet)|
|GET|`/api/wallet/{publicKey}/balance`|[Get wallet's balance](#get-apiwalletbalance)|
|POST|`/api/wallet/transactions`|[Create a new transaction](#post-apiwallettransactions)|

### Operations

#### GET /api/blockchain/blocks
Get all unconfirmed blocks.
#### POST /api/blockchain/blocks/mine
Gather all unconfirmed transactions and create a new block with them. Public key in request body gets a mining reward.

Request body:
```
{
    "publicKey": "myPublicKey"
}
```
### GET /api/blockchain/transactions
Get all unconfirmed transactions.

#### GET /api/blockchain/unspent
Get all unspent transactions outputs.

#### GET /api/blockchain/valid
Get information if blockchain is valid.

#### POST /api/wallet
Create a new wallet. This endpoint generates pair of private a public keys.

#### GET /api/wallet/{publicKey}/balance
Get balance of wallet of `publicKey`.

#### POST /api/wallet/{publicKey}/transactions
Create a new transaction.

Request body:
```
{
    "privateKey": "myPrivateKey",
    "recipient": "recipientsPublicKey",
    "value": 10
}
```

## Example usage

 1. Call `POST /api/wallet` to generate a new pair of private and public keys. Save and mark these keys as a **walletA**.
 2. Call again `POST /api/wallet` to generate a new pair of private and public keys. Save and mark these keys as a **walletB**.
 3. You don't have any coins, but if you mine a new block, you get a mining reward of **10 coins**. Mine new block by calling `POST /api/blockchain/blocks/mine` with walletA's `publicKey` in request body.
 4. Check balance of walletA `GET /api/wallet/{publicKey}/balance` with it's `publicKey`. 
 5. Send coins from walletA to walletB by calling `POST /api/wallet/transactions`. Requst body must contain walletA's `privateKey`, walletB's `publicKey` and `value` less than or equal to **10**.
 6. Balances of both wallet's are updated, but the transaction is not confirmed. Confirm the transaction by mining a new block.
 7. Call `GET /api/blockchain/blocks` and check the blockchain.
