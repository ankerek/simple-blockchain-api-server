using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DisBlockchain.Models;
using DisBlockchain.Utils;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DisBlockchain.Controllers
{
    [Route("api/[controller]")]
    public class BlockchainController : Controller
    {
        // get list of blocks
        [HttpGet]
        [HttpGet("blocks")]
        public IActionResult GetBlockchain()
        {
            return Ok(Blockchain.Instance.Blocks);
        }

        // get blockchain validity
        [HttpGet("valid")]
        public IActionResult GetIsBlockchainValid()
        {
            return Ok(Blockchain.Instance.IsValid());
        }

        // mine new block
        [HttpPost("blocks/mine")]
        public IActionResult MineBlock([FromBody]dynamic body)
        {
            string minerPublicKey = null;
            if (body != null && body.publicKey != null)
            {
                minerPublicKey = (string)body.publicKey;
            }
            return Ok(Blockchain.Instance.MineBlock(minerPublicKey));
        }

        // get current transactions
        [HttpGet("transactions")]
        public IActionResult GetCurrentTransactions()
        {
            return Ok(Blockchain.Instance.CurrentTransactions);
        }

        // get unspent transaction outputs
        [HttpGet("unspent")]
        public IActionResult GetUTXOs()
        {
            return Ok(Blockchain.Instance.UTXOs);
        }
    }
}
