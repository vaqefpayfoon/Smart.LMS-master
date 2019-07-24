namespace SmartLMS.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class vaqefChanges : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Assessment",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        MinimumScore = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        Course_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Course", t => t.Course_Id)
                .Index(t => t.Course_Id);
            
            CreateTable(
                "dbo.Question",
                c => new
                    {
                        QuestionId = c.Guid(nullable: false),
                        Type = c.Int(nullable: false),
                        Description = c.String(),
                        Score = c.Int(nullable: false),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                        RightAnswer_Id = c.Guid(),
                        Assessment_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.QuestionId)
                .ForeignKey("dbo.QuestionAnswer", t => t.RightAnswer_Id)
                .ForeignKey("dbo.Assessment", t => t.Assessment_Id)
                .Index(t => t.RightAnswer_Id)
                .Index(t => t.Assessment_Id);
            
            CreateTable(
                "dbo.QuestionAnswer",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Order = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        Answer_Id = c.Guid(),
                        Question_QuestionId = c.Guid(),
                        MultipleChoiceQuestion_QuestionId = c.Guid(),
                        ReorderingQuestion_QuestionId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Answer", t => t.Answer_Id)
                .ForeignKey("dbo.Question", t => t.Question_QuestionId)
                .ForeignKey("dbo.Question", t => t.MultipleChoiceQuestion_QuestionId)
                .ForeignKey("dbo.Question", t => t.ReorderingQuestion_QuestionId)
                .Index(t => t.Answer_Id)
                .Index(t => t.Question_QuestionId)
                .Index(t => t.MultipleChoiceQuestion_QuestionId)
                .Index(t => t.ReorderingQuestion_QuestionId);
            
            CreateTable(
                "dbo.Answer",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Text = c.String(),
                        Active = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Question", "Assessment_Id", "dbo.Assessment");
            DropForeignKey("dbo.Question", "RightAnswer_Id", "dbo.QuestionAnswer");
            DropForeignKey("dbo.QuestionAnswer", "ReorderingQuestion_QuestionId", "dbo.Question");
            DropForeignKey("dbo.QuestionAnswer", "MultipleChoiceQuestion_QuestionId", "dbo.Question");
            DropForeignKey("dbo.QuestionAnswer", "Question_QuestionId", "dbo.Question");
            DropForeignKey("dbo.QuestionAnswer", "Answer_Id", "dbo.Answer");
            DropForeignKey("dbo.Assessment", "Course_Id", "dbo.Course");
            DropIndex("dbo.QuestionAnswer", new[] { "ReorderingQuestion_QuestionId" });
            DropIndex("dbo.QuestionAnswer", new[] { "MultipleChoiceQuestion_QuestionId" });
            DropIndex("dbo.QuestionAnswer", new[] { "Question_QuestionId" });
            DropIndex("dbo.QuestionAnswer", new[] { "Answer_Id" });
            DropIndex("dbo.Question", new[] { "Assessment_Id" });
            DropIndex("dbo.Question", new[] { "RightAnswer_Id" });
            DropIndex("dbo.Assessment", new[] { "Course_Id" });
            DropTable("dbo.Answer");
            DropTable("dbo.QuestionAnswer");
            DropTable("dbo.Question");
            DropTable("dbo.Assessment");
        }
    }
}
