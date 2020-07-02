namespace SistemaSemus.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatemodelproduto : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Produto", "Categoria", c => c.String(maxLength: 50));
            AlterColumn("dbo.Produto", "Descricao", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Produto", "Descricao", c => c.String());
            DropColumn("dbo.Produto", "Categoria");
        }
    }
}
