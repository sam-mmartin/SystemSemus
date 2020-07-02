using Microsoft.AspNet.Identity.EntityFramework;
using SistemaSemus.Models;
using SistemaSemus.Models.Application;
using SistemaSemus.Models.Client;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace SistemaSemus.DAL
{
    public class SemusContext : IdentityDbContext<ApplicationUser>
    {
        public SemusContext() : base("SemusContexto") { }

        public DbSet<Claims> Claims { get; set; }
        public DbSet<EstoqueSemus> EstoqueSemus { get; set; }
        public DbSet<Funcao> Funcaos { get; set; }
        public DbSet<Medico> Medicos { get; set; }
        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<PedidoCompra> PedidoCompras { get; set; }
        public DbSet<PedidoCompraNaoCadastrado> PedidoCompraNaoCadastrados { get; set; }
        public DbSet<PedidoCompraProduto> PedidoCompraProdutos { get; set; }
        public DbSet<PedidoEstoque> PedidoEstoques { get; set; }
        public DbSet<PedidoProduto> PedidoProdutos { get; set; }
        public DbSet<Prescricao> Prescricaos { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<ProdutoEstoqueSemus> ProdutoEstoqueSemus { get; set; }
        public DbSet<ProdutoNaoCadastrado> ProdutoNaoCadatrados { get; set; }
        public DbSet<Receita> Receitas { get; set; }
        public DbSet<ReceitaMedica> ReceitaMedicas { get; set; }
        public DbSet<Receitado> Receitados { get; set; }
        public DbSet<Setor> Setors { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            // Define chave composta na tabela ProdutoEstoqueSemus
            _ = modelBuilder.Entity<ProdutoEstoqueSemus>()
                            .HasKey(p => new { p.Produto_ID, p.EstoqueSemus_ID });
            // Define chave composta na tabela PedidoCompraProduto
            _ = modelBuilder.Entity<PedidoCompraProduto>()
                            .HasKey(p => new { p.Produto_ID, p.PedidoCompra_ID });
            // Define chave composta na tabela PedidoCompraNaoCadastrado
            _ = modelBuilder.Entity<PedidoCompraNaoCadastrado>()
                            .HasKey(p => new { p.Produto_ID, p.PedidoCompra_ID });

            _ = modelBuilder.Entity<Medico>().Property(m => m.RowVersion).IsConcurrencyToken();
        }

        public static SemusContext Create()
        {
            return new SemusContext();
        }
    }
}