namespace LoadBalancer
{
    public class LoadBalancer : ILoadBalancer
    {
        private List<string> services = new();
        private ILoadBalancerStrategy strategy;

        public LoadBalancer()
        {
            this.strategy = new RoundRobinStrategy();
        }

        public void AddService(string url)
        {
            this.services.Add(url);
        }

        public void RemoveService(string url)
        {
            this.services.Remove(url);
        }

        public IList<string> GetAllServices()
        {
            return new List<string>(this.services);
        }

        public string NextService()
        {
            return this.strategy.NextService(this.services);
        }

        public void SetActiveStrategy(ILoadBalancerStrategy strategy)
        {
            this.strategy = strategy;
        }

        public ILoadBalancerStrategy GetBalancerStrategy()
        {
            return this.strategy;
        }
    }
}
