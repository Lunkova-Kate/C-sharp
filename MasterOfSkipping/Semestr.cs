public class Semestr
{
    int enjoyment;
    bool[] yesterdayAsked = new bool[6];
    History history;
    Professor[] professors;
    ISkipStrategy strategy;
    private const int smallPie = 1;

    public Semestr(Professor[] professors, ISkipStrategy strategies)
    {
        this.professors = professors;
        this.strategy = strategy;
        
        enjoyment = 0;
        for(int i = 0; i < yesterdayAsked.Length; ++i)
        {
            yesterdayAsked[i] = false;
        }
        history = new History();
    }

    public int Run()
    {
        for( int day = 0; day< 100; ++day)
        {
           if (!ProcessDay(day))
            return 0; 
        
    }
        return enjoyment;
    }
 bool ProcessDay(int dayNumber)
    {
        bool[] attend = strategy.DecideDay(dayNumber, history);
        bool[] todayAsked = new bool[6];
        
        for(int i = 0 ; i<todayAsked.Length; ++i)
        {
            
             bool willAsk = professors[i].WillAsk(yesterdayAsked);
             todayAsked[i] = willAsk;
            if (attend[i])
            {
                if (willAsk)
                {
                    history.RecordVisit(dayNumber, (Subject)i, willAsk);
                }
                else
                {
                    history.RecordVisit(dayNumber, (Subject)i, willAsk);
                }
            }
            else
            {
                if (willAsk)
                {
                    enjoyment = 0;
                    // history.RecordVisit(dayNumber, (Subject)i, willAsk);
                    return false;
                }
                else
                {
                    enjoyment += 1;
                    history.RecordSkip(dayNumber, (Subject)i);
                }
            }
           
        }
            enjoyment += smallPie; 
            yesterdayAsked = todayAsked;
            return true;
    }
 
}