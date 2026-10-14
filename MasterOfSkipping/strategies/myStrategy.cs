namespace MasterOfSkipping;

public class MyStrategy : ISkipStrategy
{
    string Name  => "My Strategy";

    private const int SubjectCount = 6;
    private const int HypothesisCount = 21;
    private const int  MinimumObservationDays = 14;
    private  static readonly byte []  SubjectsUsedByHypothesis;

    static MyStrategy()
    {
        byte[] SubjectsUsedByHypothesis = new byte[HypothesisCount];
        int hypothesisIndex = 0;

        for(int i = 0; i < SubjectCount; ++i)
        {
            SubjectsUsedByHypothesis[hypothesisIndex++] = (byte)(1 << i);

        }

        for( int i = 0; i < SubjectCount; ++i)
        {
            for(int j = i + 1; j < SubjectCount; ++j)
            {
                SubjectsUsedByHypothesis[hypothesisIndex++] =
                    (byte)((1 << i) | (1 << j));
            }
        }
    }

// Взять результаты вчерашнего дня и одну гипотезу, 
// после чего вычислить, предсказывает ли эта гипотеза опрос 
// сегодня.
    private static bool Predict(byte whoAskedYeasterday, byte hypothes)
    {
        return (whoAskedYesterday & hypothesis) != 0;
    }
    public bool[] DecideDay(int currentday, IReadOnlyStudentHistory history)
    {
        bool[] decisions = new bool[SubjectCount];
        if (currentDay == 0)
        {
            Array.Fill(decisions, true);
            return decisions;
        }

        byte[] PreviousDayMask = new byte[SubjectCount];

        for(int observedDay = 0; observedDay < currentDay; ++observedDay)
        {
            byte[] todaySubjectMask = new byte[SubjectCount];

            for (int subjectIndex = 0; subjectIndex < SubjectCount; ++subjectIndex)
            {   
                Subject observedSubject = (Subject)subjectIndex;
                if (studentHistory.Attended(observedDay, observedSubject) &&
                    studentHistory.WasAsked(observedDay, observedSubject) == true)
                {
                    todaySubjectMask[subjectIndex] |= (byte)(1 << subjectIndex);
                }
            }
            PreviousDayMask[observedDay] = todaySubjectMask[observedDay];
        }
        byte lastAskedSubjects = PreviousDayMask[currentDay - 1];

        for(int currentSubject = 0; currentSubject < SubjectCount; ++currentSubject)
        {
            int possibleHypotheses = (1 << HypothesisCount) - 1;

            for(int observedDay = 0; observedDay< currentDay; ++observedDay)
            {
                byte previousDayAskedSubjects;
                //  кто спрашивал за день до него;
                if(observedDay == 0)
                {
                    previousDayAskedSubjects = (byte)0;
                }
                else
                {
                    previousDayAskedSubjects = PreviousDayMask[observedDay - 1];
                }


                // спрашивал ли текущий преподаватель в сам рассматриваемый день.
            }
        }
        return decisions;
    }
}
