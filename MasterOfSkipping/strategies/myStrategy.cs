namespace MasterOfSkipping;

public class MyStrategy : ISkipStrategy
{
    public string Name => "My Strategy";

    private const int SubjectCount = 6;
    private const int HypothesisCount = 21;
    private const int MinimumObservationDays = 10;
    private static readonly byte[] SubjectsUsedByHypothesis;

    static MyStrategy()
    {
        SubjectsUsedByHypothesis = new byte[HypothesisCount];
        int hypothesisIndex = 0;

        for (int subjectIndex = 0; subjectIndex < SubjectCount; ++subjectIndex)
        {
            SubjectsUsedByHypothesis[hypothesisIndex++] =
                (byte)(1 << subjectIndex);
        }

        for (int firstSubject = 0; firstSubject < SubjectCount; ++firstSubject)
        {
            for (int secondSubject = firstSubject + 1;
                 secondSubject < SubjectCount;
                 ++secondSubject)
            {
                SubjectsUsedByHypothesis[hypothesisIndex++] =
                    (byte)((1 << firstSubject) | (1 << secondSubject));
            }
        }
    }

    private static bool Predict(byte askedYesterday, byte hypothesis)
    {
        int matchingAskedSubjects = askedYesterday & hypothesis;

        return matchingAskedSubjects != 0 &&
               (matchingAskedSubjects & (matchingAskedSubjects - 1)) == 0;
    }

    public bool[] DecideDay(int currentDay, IReadOnlyStudentHistory history)
    {
        ArgumentNullException.ThrowIfNull(history);

        if (currentDay < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(currentDay),
                "Day cannot be negative.");
        }

        bool[] shouldAttend = new bool[SubjectCount];

        if (currentDay == 0)
        {
            Array.Fill(shouldAttend, true);
            return shouldAttend;
        }

        
        byte[] askedSubjectsByDay = new byte[currentDay];

        for (int observedDay = 0; observedDay < currentDay; ++observedDay)
        {
            byte askedSubjects = 0;

            for (int subjectIndex = 0; subjectIndex < SubjectCount; ++subjectIndex)
            {
                Subject subject = (Subject)subjectIndex;

                if (history.Attended(observedDay, subject) &&
                    history.WasAsked(observedDay, subject) == true)
                {
                    askedSubjects |= (byte)(1 << subjectIndex);
                }
            }

            askedSubjectsByDay[observedDay] = askedSubjects;
        }

        byte askedYesterday = askedSubjectsByDay[currentDay - 1];

        for (int currentSubject = 0;
             currentSubject < SubjectCount;
             ++currentSubject)
        {
        
            int possibleHypotheses = (1 << HypothesisCount) - 1;

            for (int observedDay = 0; observedDay < currentDay; ++observedDay)
            {
                byte askedOnPreviousDay = observedDay == 0
                    ? (byte)0
                    : askedSubjectsByDay[observedDay - 1];

                bool currentProfessorAsked =
                    (askedSubjectsByDay[observedDay] & (1 << currentSubject)) != 0;

                for (int hypothesisIndex = 0;
                     hypothesisIndex < HypothesisCount;
                     ++hypothesisIndex)
                {
                    int hypothesisBit = 1 << hypothesisIndex;

                    if ((possibleHypotheses & hypothesisBit) == 0)
                    {
                        continue;
                    }

                    bool predictedQuestion = Predict(
                        askedOnPreviousDay,
                        SubjectsUsedByHypothesis[hypothesisIndex]);

                    if (predictedQuestion != currentProfessorAsked)
                    {
                        possibleHypotheses &= ~hypothesisBit;
                    }
                }
                if (possibleHypotheses == 0)
                {
                    break;
                }
            }

            bool allPossibleRulesPredictNoQuestion = possibleHypotheses != 0;

            if (allPossibleRulesPredictNoQuestion)
            {
                for (int hypothesisIndex = 0;
                     hypothesisIndex < HypothesisCount;
                     ++hypothesisIndex)
                {
                    int hypothesisBit = 1 << hypothesisIndex;

                    if ((possibleHypotheses & hypothesisBit) != 0 &&
                        Predict(
                            askedYesterday,
                            SubjectsUsedByHypothesis[hypothesisIndex]))
                    {
                        allPossibleRulesPredictNoQuestion = false;
                        break;
                    }
                }
            }

            bool enoughObservations = currentDay >= MinimumObservationDays;

            shouldAttend[currentSubject] =
                !enoughObservations || !allPossibleRulesPredictNoQuestion;
        }

        return shouldAttend;
    }
}
