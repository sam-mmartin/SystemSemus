namespace SistemaSemus.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deleteforeignKey : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.AspNetUsers", "Funcao_ID");
            DropIndex("dbo.AspNetUsers", "Setor_ID");
        }
        
        public override void Down()
        {
        }
    }
}
