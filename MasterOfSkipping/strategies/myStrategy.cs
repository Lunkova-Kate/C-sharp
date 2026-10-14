namespace MasterOfSkipping;

public class MyStrategy : ISkipStrategy
{
    public string Name => "My Strategy";

    private const int SubjectCount = 6;
    private const int HypothesisCount = 21;
    private const int MinimumObservationDays = 14;
    private static readonly byte[] SubjectsUsedByHypothesis;

    static MyStrategy()
    {
        byte[] SubjectsUsedByHypothesis = new byte[HypothesisCount];
        int hypothesisIndex = 0;

        for (int i = 0; i < SubjectCount; ++i)
        {
            SubjectsUsedByHypothesis[hypothesisIndex++] = (byte)(1 << i);

        }

        for (int i = 0; i < SubjectCount; ++i)
        {
            for (int j = i + 1; j < SubjectCount; ++j)
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

        for (int observedDay = 0; observedDay < currentDay; ++observedDay)
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

        for (int currentSubject = 0; currentSubject < SubjectCount; ++currentSubject)
        {
            int possibleHypotheses = (1 << HypothesisCount) - 1;

            for (int observedDay = 0; observedDay < currentDay; ++observedDay)
            {
                //  кто спрашивал за день до него;
                byte previousDayAskedSubjects = observedDay == 0 ? (byte)0 : PreviousDayMask[observedDay - 1];

                // спрашивал ли текущий преподаватель в сам рассматриваемый день.
                bool wasAskedOncurrentSubject = (PreviousDayMask[observedDay] & (1 << currentSubject)) != 0;
                for (int hypothesisIndex = 0; hypothesisIndex < HypothesisCount; ++hypothesisIndex)
                {
                    bool hypothesisIsStillPossible = (possibleHypotheses & (1 << hypothesisIndex)) != 0;
                    if (hypothesisIsStillPossible)
                    {
                        byte subjectsUsedByRule = SubjectsUsedByHypothesis[hypothesisIndex];
                        bool predictsQuestion = Predict(previousDayAskedSubjects, subjectsUsedByRule);
                        if (predictsQuestion != wasAskedOncurrentSubject)
                        {
                            possibleHypotheses &= ~(1 << hypothesisIndex);
                        }
                    }
                    if (possibleHypotheses == 0)// значит преподаватель Random50 скорее всего
                    {
                        break;
                    }
                }
                allPossibleRulesPredictNoQuestion = possibleHypotheses != 0;

                if (allPossibleRulesPredictNoQuestion && currentDay < MinimumObservationDays)
                {
                    for (int hypothesisIndex = 0; hypothesisIndex < HypothesisCount; ++hypothesisIndex)
                    {
                        bool hypothesisIsStillPossible = (possibleHypotheses & (1 << hypothesisIndex)) != 0;

                        if (hypothesisIsStillPossible && Predict(lastAskedSubjects, SubjectsUsedByHypothesis[hypothesisIndex]))
                        {
                            allPossibleRulesPredictNoQuestion = false; // если хоття бы одно правило говорит что могут спросить, то прогул не безопасен
                            break;
                        }

                    }
                }
                else
                {
                    allPossibleRulesPredictNoQuestion = false;
                }

                shouldAttendSubject[currentSubject] = !allPossibleRulesPredictNoQuestion;
            }
            return decisions;
        }
    }
}
