using System;

namespace OrderByExtensions
{
    internal struct OrderByProperty
    {
        static readonly char[] _splitOnSpace = new[] { ' ' };

        public OrderByProperty(string property, bool isAscendingDefault)
        {
            if (string.IsNullOrWhiteSpace(property))
            {
                throw new ArgumentException("Property name must not be blank", "property");
            }

            var propertyParse = property.Split(_splitOnSpace, StringSplitOptions.RemoveEmptyEntries);

            PropertyName = propertyParse[0];

            if (propertyParse.Length > 1)
            {
                var direction = propertyParse[1].ToLowerInvariant();
                switch (direction)
                {
                    case "asc":
                        IsAscending = true;
                        break;
                    case "desc":
                        IsAscending = false;
                        break;
                    default:
                        throw new ArgumentException("Only 'asc' and 'desc' are valid modifiers", "property");
                }
            }
            else
            {
                IsAscending = isAscendingDefault;
            }
        }
        public string PropertyName;
        public bool IsAscending;
    }
}
