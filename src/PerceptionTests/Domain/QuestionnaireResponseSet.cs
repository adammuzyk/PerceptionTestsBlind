using System.Collections.Generic;

namespace PerceptionTests.Domain
{
    public sealed class QuestionnaireResponseSet
    {
        private readonly Dictionary<string, string> _values = new Dictionary<string, string>();

        public IReadOnlyDictionary<string, string> Values => _values;

        public void Set(string fieldId, string rawValue)
        {
            if (string.IsNullOrWhiteSpace(fieldId))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(rawValue))
            {
                _values.Remove(fieldId);
                return;
            }

            _values[fieldId] = rawValue;
        }

        public string Get(string fieldId)
        {
            return _values.TryGetValue(fieldId, out var value) ? value : null;
        }

        public bool Contains(string fieldId)
        {
            return _values.ContainsKey(fieldId);
        }

        public void Remove(string fieldId)
        {
            if (!string.IsNullOrWhiteSpace(fieldId))
            {
                _values.Remove(fieldId);
            }
        }

        public void Clear()
        {
            _values.Clear();
        }
    }
}
