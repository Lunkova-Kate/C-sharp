using System.Collections.Specialized;
using MasterOfSkipping;
using MasterOfSkipping.Tests.TestedClass;


public class HistoryTest
{

[Fact]
public void RecordVisit_AttendedAndAsked_SavesBothValues()
{
    var history = new History();
    var day = 0;
    Subject subject = Subject.Calculus;
    bool wasAsked = true;
    history.RecordVisit(day,subject,wasAsked);

    Assert.True( history.Attended(day, subject));
    Assert.True(history.WasAsked(day, subject));
}

[Fact]
public void RecordSkip_StudentAbsent_QuestionUnknown()
{
    var history = new History();
    var day = 0;
    Subject subject = Subject.Calculus;
    history.RecordSkip(day,subject);

    Assert.False( history.Attended(day, subject));
    Assert.Null(history.WasAsked(day, subject));
}

[Fact]
public void CheckHistory()
    {
        var history = new History();
        Subject subject = Subject.Programming;
        bool wasAsked = true;
        history.RecordVisit(0,subject,wasAsked);
        history.RecordSkip(1, subject);

        Assert.True(history.Attended(0, subject));
        Assert.True(history.WasAsked(0,subject));
        Assert.False(history.Attended(1,subject));
        Assert.Null(history.WasAsked(1,subject));

    }
}