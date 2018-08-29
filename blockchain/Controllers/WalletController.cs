using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DisBlockchain.Models;
using DisBlockchain.Models.Data;
using DisBlockchain.Utils;
using Microsoft.AspNetCore.Mvc;


namespace DisBlockchain.Controllers
{
    [Route("api/[controller]")]
    public class WalletController : Controller
    {
        [HttpPost()]
        public IActionResult GenerateWallet()
        {
            Wallet wallet = new Wallet();
            return Ok(wallet);
        }

        [HttpGet("{publicKey}/balance")]
        public IActionResult GetBalance(string publicKey)
        {
            Wallet wallet = new Wallet(null, publicKey);
            return Ok(wallet.GetBalance());
        }

        // create transaction from wallet
        [HttpPost("{publicKey}/transactions")]
        public IActionResult CreateTransaction(string publicKey, [FromBody]CreateTransactionData body)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(publicKey != Cryptography.GetPublicKeyFromPrivateKey(body.PrivateKey))
            {
                return BadRequest("Public key doesn't belong to private key.");
            }

            Wallet wallet = new Wallet(body.PrivateKey);
            try
            {
                return Ok(wallet.CreateTransaction(body.Recipient, (float)body.Value));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
