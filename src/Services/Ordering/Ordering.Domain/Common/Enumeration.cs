using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace Ordering.Domain.Common
{
    public abstract class Enumeration : IComparable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; } = 1;

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        protected Enumeration(int id, string name) => (Id, Name) = (id, name);

        public override string ToString() => Name;

        public int CompareTo(object obj)
        {
            return (obj as Enumeration).Id.CompareTo(Id);
        }

        public override bool Equals(object obj)
        {
            if (obj is not Enumeration other)
                return false;

            return GetType().Equals(obj.GetType())
                && other.Id == Id;
        }

        public static IEnumerable<T> GetAll<T>() where T : Enumeration
        {
            return typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Select(t => t.GetValue(default))
                .Cast<T>();
        }

        public static T FromValue<T>(int value) where T : Enumeration
        {
            var matchingItem = Parse<T, int>(value, "value", item => item.Id == value);
            return matchingItem;
        }

        public static T FromDisplayName<T>(string displayName) where T : Enumeration
        {
            var matchingItem = Parse<T, string>(displayName, "display name", item => item.Name == displayName);
            return matchingItem;
        }

        private static T Parse<T, K>(K value, string description, Func<T, bool> predicate) where T : Enumeration
        {
            var matchingItem = GetAll<T>().FirstOrDefault(predicate);

            if (matchingItem == null)
                throw new InvalidOperationException($"'{value}' is not a valid {description} in {typeof(T)}");

            return matchingItem;
        }

        public override int GetHashCode() => Id.GetHashCode();

    }
}
