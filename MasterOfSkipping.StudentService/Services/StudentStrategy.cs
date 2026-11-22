using MasterOfSkipping.StudentService.Contracts.History;
using MasterOfSkipping.StudentService.Core;

namespace MasterOfSkipping.StudentService.Services;

public sealed class StudentStrategy
{
    private const int HypothesisCount = 21;
    private const int MinimumObservationDays = 10;
    private static readonly byte[] SubjectsUsedByHypothesis = CreateHypotheses();

    public bool[] DecideDay(
        int currentDay,
        IReadOnlyCollection<HistoryLessonResponse> history)
    {
        int completedDays = currentDay - 1;
        var shouldAttend = new bool[SubjectCatalog.Codes.Length];

        if (completedDays < MinimumObservationDays)
        {
            Array.Fill(shouldAttend, true);
            return shouldAttend;
        }

        byte[] askedSubjectsByDay = CreateAskedMasks(completedDays, history);
        byte askedYesterday = askedSubjectsByDay[completedDays - 1];

        for (int currentSubject = 0;
             currentSubject < SubjectCatalog.Codes.Length;
             ++currentSubject)
        {
            int possibleHypotheses = (1 << HypothesisCount) - 1;

            for (int observedDay = 0; observedDay < completedDays; ++observedDay)
            {
                byte askedOnPreviousDay = observedDay == 0
                    ? (byte)0
                    : askedSubjectsByDay[observedDay - 1];

                bool professorAsked =
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

                    bool predicted = Predict(
                        askedOnPreviousDay,
                        SubjectsUsedByHypothesis[hypothesisIndex]);

                    if (predicted != professorAsked)
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

            for (int hypothesisIndex = 0;
                 allPossibleRulesPredictNoQuestion && hypothesisIndex < HypothesisCount;
                 ++hypothesisIndex)
            {
                int hypothesisBit = 1 << hypothesisIndex;

                if ((possibleHypotheses & hypothesisBit) != 0 &&
                    Predict(
                        askedYesterday,
                        SubjectsUsedByHypothesis[hypothesisIndex]))
                {
                    allPossibleRulesPredictNoQuestion = false;
                }
            }

            shouldAttend[currentSubject] = !allPossibleRulesPredictNoQuestion;
        }

        return shouldAttend;
    }

    private static byte[] CreateHypotheses()
    {
        var hypotheses = new byte[HypothesisCount];
        int hypothesisIndex = 0;

        for (int subject = 0; subject < SubjectCatalog.Codes.Length; ++subject)
        {
            hypotheses[hypothesisIndex++] = (byte)(1 << subject);
        }

        for (int first = 0; first < SubjectCatalog.Codes.Length; ++first)
        {
            for (int second = first + 1;
                 second < SubjectCatalog.Codes.Length;
                 ++second)
            {
                hypotheses[hypothesisIndex++] =
                    (byte)((1 << first) | (1 << second));
            }
        }

        return hypotheses;
    }

    private static byte[] CreateAskedMasks(
        int completedDays,
        IReadOnlyCollection<HistoryLessonResponse> history)
    {
        var askedByDay = new byte[completedDays];
        var subjectIndexes = SubjectCatalog.Codes
            .Select((code, index) => new { code, index })
            .ToDictionary(item => item.code, item => item.index);

        foreach (HistoryLessonResponse lesson in history)
        {
            if (lesson.Day < 1 || lesson.Day > completedDays ||
                lesson.WasAsked != true ||
                !subjectIndexes.TryGetValue(lesson.SubjectCode, out int subjectIndex))
            {
                continue;
            }

            askedByDay[lesson.Day - 1] |= (byte)(1 << subjectIndex);
        }

        return askedByDay;
    }

    private static bool Predict(byte askedYesterday, byte hypothesis)
    {
        int matchingAskedSubjects = askedYesterday & hypothesis;

        return matchingAskedSubjects != 0 &&
               (matchingAskedSubjects & (matchingAskedSubjects - 1)) == 0;
    }
}
