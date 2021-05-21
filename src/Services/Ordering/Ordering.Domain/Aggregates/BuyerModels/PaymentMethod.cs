using Ordering.Domain.Common;
using Ordering.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.Aggregates
{
    [Table("PaymentMethods")]
    public class PaymentMethod : Entity
    {
        protected PaymentMethod() { }

        public PaymentMethod(int cardTypeId, string alias, string cardNumber, string securityNumber, string cardHolderName, DateTime expiration)
        {

            CardNumber = !string.IsNullOrWhiteSpace(cardNumber) ? cardNumber : throw new OrderingDomainException(nameof(cardNumber));
            SecurityNumber = !string.IsNullOrWhiteSpace(securityNumber) ? securityNumber : throw new OrderingDomainException(nameof(securityNumber));
            CardHolderName = !string.IsNullOrWhiteSpace(cardHolderName) ? cardHolderName : throw new OrderingDomainException(nameof(cardHolderName));

            if (expiration < DateTime.UtcNow)
            {
                throw new OrderingDomainException(nameof(expiration));
            }

            Alias = alias;
            Expiration = expiration;
            CardTypeId = cardTypeId;
        }
        [Required]
        [MaxLength(200)]
        public string Alias { get; set; }

        [Required]
        [MaxLength(25)]
        public string CardNumber { get; set; }
        public string SecurityNumber { get; set; }
        public string CardHolderName { get; set; }

        [Required]
        [MaxLength(25)]
        [DataType(DataType.Date)]
        public DateTime Expiration { get; set; }

        [Required]
        [ForeignKey(nameof(CardType))]
        public int CardTypeId { get; set; }
        public CardType CardType { get; set; }


        public bool IsEqualTo(int cardTypeId, string cardNumber, DateTime expiration)
        {
            return CardTypeId == cardTypeId
                && CardNumber == cardNumber
                && Expiration == expiration;
        }

    }
}
