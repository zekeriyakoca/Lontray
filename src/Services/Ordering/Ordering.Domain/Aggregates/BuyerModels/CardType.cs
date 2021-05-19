using Ordering.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.Aggregates.BuyerModels
{
    public class CardType : Enumeration
    {
        public CardType(int id, string name) : base(id, name) { }

        public static CardType Amex = new CardType(1, nameof(Amex));
        public static CardType Visa => new CardType(2, nameof(Visa));
        public static CardType Master => new CardType(3, nameof(Master));


    }
}
