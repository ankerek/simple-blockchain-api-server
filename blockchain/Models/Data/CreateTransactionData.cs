using System;
using System.ComponentModel.DataAnnotations;

namespace DisBlockchain.Models.Data
{
    public class CreateTransactionData
    {
        [Required]
        public string PrivateKey { get; set; }

        [Required]
        public string Recipient { get; set; }

        [Required]
        public float? Value { get; set; }
    }
}
