using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uracle.Domain.ValueObjects
{
    public record @string
    {
        public Guid Value { get; }
        private @string(Guid value) => Value = value;
        public static @string Of(Guid value)
        {
            ArgumentNullException.ThrowIfNull(value);
            if (value == Guid.Empty)
            {
                throw new ("UserId cannot be empty.");
            }

            return new @string(value);
        }
    }
}
