using System.Collections.Generic;

namespace SRUL.Types
{
    public enum WorldUNOpinion
    {
        Unknown,
        Outraged,
        Dissaproving,
        Concerned,
        Indifferent,
        Satisfied,
        Pleased,
        Delighted
    }

    public class Opinion
    {
        private string _sentiment;

        public string sentiment
        {
            get => _sentiment;
            set => _sentiment = value;
        }
        private string _value;
        public string value
        {
            get => _value;
            set => _value = value;
        }

        public Opinion(string sentiment, string value)
        {
            this.sentiment = sentiment;
            this.value = value;
        }
    }

    public class WorldMarketOpinion
    {
        public IList<Opinion> List;
        public static WorldMarketOpinion Instance { get; } = new WorldMarketOpinion();

        private WorldMarketOpinion()
        {
            this.List = new List<Opinion>()
            {
                new Opinion("Outraged", "0"),
                new Opinion("Dissaproving", "0.29"),
                new Opinion("Concerned", "0.39"),
                new Opinion("Indifferent", "0.49"),
                new Opinion("Satisfied", "0.59"),
                new Opinion("Pleased", "0.69"),
                new Opinion("Delighted", "1"),
                new Opinion("+Delighted", "2"),
                new Opinion("++Delighted", "3"),
                new Opinion("+++Delighted", "4"),
                new Opinion("++++Delighted", "5"),
            };
        }
    }
    
}