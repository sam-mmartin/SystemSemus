namespace SistemaSemus.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addmodelsreceitaMedica : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Prescricao",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ProdutoID = c.Int(),
                        ProdutoSemEstoqueID = c.Int(),
                        Descricao = c.String(),
                        Quantidade = c.Int(nullable: false),
                        ReceitaMedica_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.ReceitaMedica", t => t.ReceitaMedica_ID)
                .Index(t => t.ReceitaMedica_ID);
            
            CreateTable(
                "dbo.ReceitaMedica",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        DataCadastro = c.DateTime(nullable: false),
                        MedicoID = c.String(maxLength: 13),
                        PacienteID = c.String(maxLength: 128),
                        UserID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AspNetUsers", t => t.UserID)
                .ForeignKey("dbo.Medico", t => t.MedicoID)
                .ForeignKey("dbo.Paciente", t => t.PacienteID)
                .Index(t => t.MedicoID)
                .Index(t => t.PacienteID)
                .Index(t => t.UserID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Prescricao", "ReceitaMedica_ID", "dbo.ReceitaMedica");
            DropForeignKey("dbo.ReceitaMedica", "PacienteID", "dbo.Paciente");
            DropForeignKey("dbo.ReceitaMedica", "MedicoID", "dbo.Medico");
            DropForeignKey("dbo.ReceitaMedica", "UserID", "dbo.AspNetUsers");
            DropIndex("dbo.ReceitaMedica", new[] { "UserID" });
            DropIndex("dbo.ReceitaMedica", new[] { "PacienteID" });
            DropIndex("dbo.ReceitaMedica", new[] { "MedicoID" });
            DropIndex("dbo.Prescricao", new[] { "ReceitaMedica_ID" });
            DropTable("dbo.ReceitaMedica");
            DropTable("dbo.Prescricao");
        }
    }
}
