using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IRibeiroForHireAPI.Domain.Entities;

[Table("qa_interactions")]
public class QaInteraction
{
    [Key] 
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Column("visitor_id")]
    public string VisitorId { get; set; }

    [Required]
    [Column("question")]
    public string QuestionText { get; set; } = string.Empty;

    [Required]
    [Column("answer")]
    public string AnswerText { get; set; } = string.Empty;

    [Column("interaction_time")]
    public DateTime InteractionTime { get; set; }

    [Column("user_ip")] 
    public string UserIp { get; set; }
}
