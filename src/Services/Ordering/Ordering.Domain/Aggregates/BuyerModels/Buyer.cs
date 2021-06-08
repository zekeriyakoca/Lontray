using Ordering.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ordering.Domain.Aggregates
{
    public class Buyer : Entity, IAggregateRoot
    {

        protected Buyer()
        { }

        public Buyer(string identity, string name) : this()
        {
            IdentityGuid = !string.IsNullOrWhiteSpace(identity) ? identity : throw new ArgumentNullException(nameof(identity));
            Name = !string.IsNullOrWhiteSpace(name) ? name : throw new ArgumentNullException(nameof(name));
        }

        [Required]
        [MaxLength(200)]
        public string IdentityGuid { get; private set; }

        [Required]
        public string Name { get; private set; }


        [ForeignKey(nameof(PaymentMethod))]
        public int PaymentMethodId { get; private set; }
        public PaymentMethod PaymentMethod { get; private set; }

    }
}
