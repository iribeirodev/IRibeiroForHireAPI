using IRibeiroForHire.Models;

namespace IRibeiroForHire.ViewModels;

public class AnswerViewModel
{
    public IEnumerable<QuestionAnswerItem> QaList { get; set; }
    public int RemainingQuestions { get; set; }
}
