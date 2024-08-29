namespace AspNetCoreEx.Models
{
    public class TokenBucket
    {
        private readonly int _capacity;

        private readonly int _refillRate;

        private double _tokens;

        private DateTime _lastRefill;

        private readonly object _lock = new();

        public TokenBucket(int capacity, int refillRate)
        {
            _capacity = capacity;
            _refillRate = refillRate;
            _tokens = capacity; // 初始時桶是滿的
            _lastRefill = DateTime.Now;
        }

        private void Refill()
        {
            var now = DateTime.Now;
            var timeElapsed = (now - _lastRefill).TotalSeconds;

            if (timeElapsed > 0)
            {
                lock (_lock)
                {
                    _tokens += timeElapsed * _refillRate;
                    if (_tokens > _capacity)
                    {
                        _tokens = _capacity;
                    }
                    _lastRefill = now;
                }
            }
        }

        public bool TryConsume(int tokens)
        {
            Refill();
            lock (_lock)
            {
                if (_tokens >= tokens)
                {
                    _tokens -= tokens;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public int AvailableTokens()
        {
            lock (_lock)
            {
                return (int)_tokens;
            }
        }
    }

}

