using Ordering.Domain.Common;

namespace Ordering.Domain.Aggregates
{
    public class CardType : Enumeration
    {
        private CardType(int id, string name) : base(id, name) { }

        public static CardType Amex = new CardType(1, nameof(Amex));
        public static CardType Visa => new CardType(2, nameof(Visa));
        public static CardType Master => new CardType(3, nameof(Master));


    }
}
