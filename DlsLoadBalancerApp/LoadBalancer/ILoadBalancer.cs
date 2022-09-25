namespace LoadBalancer
{
    public interface ILoadBalancer
    {
        public IList<string> GetAllServices();
        public void AddService(string url);
        public void RemoveService(string url);
        public ILoadBalancerStrategy GetBalancerStrategy();
        public void SetActiveStrategy(ILoadBalancerStrategy strategy);
        public string NextService();
    }

    public interface ILoadBalancerStrategy
    {
        public string NextService(List<string> services);
    }
}
