namespace LoadBalancer
{
    public class RoundRobinStrategy : ILoadBalancerStrategy
    {
        private int currentServiceIndex = 0;
        public string NextService(List<string> services)
        {
            if(this.currentServiceIndex >= services.Count)
            {
                this.currentServiceIndex = 0;
            }
            return services[this.currentServiceIndex++];
        }
    }
}
