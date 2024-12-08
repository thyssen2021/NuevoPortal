namespace Portal_2_0.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IdEmpleado_Usuarios : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "IdEmpleado", c => c.Int(nullable: false));
            AlterColumn("dbo.AspNetUsers", "Nombre", c => c.String(maxLength: 40));
            DropColumn("dbo.AspNetUsers", "Apellidos");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "Apellidos", c => c.String(maxLength: 45));
            AlterColumn("dbo.AspNetUsers", "Nombre", c => c.String(maxLength: 25));
            DropColumn("dbo.AspNetUsers", "IdEmpleado");
        }
    }
}
