using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Text;

#nullable disable

namespace Database
{
    public partial class ExtrusionContext : DbContext
    {
        const string DefaultConnectStr = "Host=localhost;Port=5430;Database=Extrusion;Username=postgres;Password=1";
        const string ConnectFile = "connectData.info";

        public ExtrusionContext()
        {
        }

        public ExtrusionContext(DbContextOptions<ExtrusionContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Barrel> Barrels { get; set; }
        public virtual DbSet<BarrelParametr> BarrelParametrs { get; set; }
        public virtual DbSet<BarrelParametrValue> BarrelParametrValues { get; set; }
        public virtual DbSet<BarrelPossibleСonfiguration> BarrelPossibleСonfigurations { get; set; }
        public virtual DbSet<BarrelSection> BarrelSections { get; set; }
        public virtual DbSet<BarrelSectionInСonfiguration> BarrelSectionInСonfigurations { get; set; }
        public virtual DbSet<BarrelSectionParametr> BarrelSectionParametrs { get; set; }
        public virtual DbSet<BarrelSectionParametrValue> BarrelSectionParametrValues { get; set; }
        public virtual DbSet<BarrelСonfiguration> BarrelСonfigurations { get; set; }
        public virtual DbSet<Die> Dies { get; set; }
        public virtual DbSet<DieElement> DieElements { get; set; }
        public virtual DbSet<DieElementInСonfiguration> DieElementInСonfigurations { get; set; }
        public virtual DbSet<DieElementParametr> DieElementParametrs { get; set; }
        public virtual DbSet<DieElementParametrValue> DieElementParametrValues { get; set; }
        public virtual DbSet<Extruder> Extruders { get; set; }
        public virtual DbSet<ExtruderType> ExtruderTypes { get; set; }
        public virtual DbSet<Film> Films { get; set; }
        public virtual DbSet<MathModel> MathModels { get; set; }
        public virtual DbSet<MathModelCoefficient> MathModelCoefficients { get; set; }
        public virtual DbSet<MathModelCoefficientValue> MathModelCoefficientValues { get; set; }
        public virtual DbSet<Polymer> Polymers { get; set; }
        public virtual DbSet<PolymerParametr> PolymerParametrs { get; set; }
        public virtual DbSet<PolymerParametrValue> PolymerParametrValues { get; set; }
        public virtual DbSet<ProcessParametr> ProcessParametrs { get; set; }
        public virtual DbSet<ProcessParametrValue> ProcessParametrValues { get; set; }
        public virtual DbSet<Scenario> Scenarios { get; set; }
        public virtual DbSet<Screw> Screws { get; set; }
        public virtual DbSet<ScrewElement> ScrewElements { get; set; }
        public virtual DbSet<ScrewElementInСonfiguration> ScrewElementInСonfigurations { get; set; }
        public virtual DbSet<ScrewElementParametr> ScrewElementParametrs { get; set; }
        public virtual DbSet<ScrewElementParametrValue> ScrewElementParametrValues { get; set; }
        public virtual DbSet<ScrewParametr> ScrewParametrs { get; set; }
        public virtual DbSet<ScrewParametrValue> ScrewParametrValues { get; set; }
        public virtual DbSet<ScrewPossibleСonfiguration> ScrewPossibleСonfigurations { get; set; }
        public virtual DbSet<ScrewСonfiguration> ScrewСonfigurations { get; set; }
        public virtual DbSet<Unit> Units { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.

                System.IO.FileInfo fileInfo = new FileInfo(ConnectFile);
                if (!fileInfo.Exists)
                {
                    File.WriteAllText(ConnectFile, DefaultConnectStr, Encoding.UTF8);
                }
                
                string connectStr = File.ReadAllText(ConnectFile, Encoding.UTF8);
                optionsBuilder.UseNpgsql(connectStr);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Russian_Russia.1251");

            modelBuilder.Entity<Barrel>(entity =>
            {
                entity.ToTable("Barrel");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");
            });

            modelBuilder.Entity<BarrelParametr>(entity =>
            {
                entity.ToTable("BarrelParametr");

                entity.HasIndex(e => e.IdUnit, "IX_Relationship211");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.Designation)
                    .IsRequired()
                    .HasColumnName("designation");

                entity.Property(e => e.IdUnit).HasColumnName("id_unit");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");

                entity.HasOne(d => d.IdUnitNavigation)
                    .WithMany(p => p.BarrelParametrs)
                    .HasForeignKey(d => d.IdUnit)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Unit");
            });

            modelBuilder.Entity<BarrelParametrValue>(entity =>
            {
                entity.HasKey(e => new { e.IdBarrel, e.IdParametr });

                entity.ToTable("BarrelParametrValue");

                entity.Property(e => e.IdBarrel).HasColumnName("id_barrel");

                entity.Property(e => e.IdParametr).HasColumnName("id_parametr");

                entity.Property(e => e.Value).HasColumnName("value");

                entity.HasOne(d => d.IdBarrelNavigation)
                    .WithMany(p => p.BarrelParametrValues)
                    .HasForeignKey(d => d.IdBarrel)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("Barrel");

                entity.HasOne(d => d.IdParametrNavigation)
                    .WithMany(p => p.BarrelParametrValues)
                    .HasForeignKey(d => d.IdParametr)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("Parametr");
            });

            modelBuilder.Entity<BarrelPossibleСonfiguration>(entity =>
            {
                entity.ToTable("BarrelPossibleСonfiguration");

                entity.HasIndex(e => e.IdBody, "IX_Body");

                entity.HasIndex(e => e.IdConfiguration, "IX_Configuration");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.IdBody).HasColumnName("id_body");

                entity.Property(e => e.IdConfiguration).HasColumnName("id_configuration");

                entity.HasOne(d => d.IdBodyNavigation)
                    .WithMany(p => p.BarrelPossibleСonfigurations)
                    .HasForeignKey(d => d.IdBody)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("Barrel");

                entity.HasOne(d => d.IdConfigurationNavigation)
                    .WithMany(p => p.BarrelPossibleСonfigurations)
                    .HasForeignKey(d => d.IdConfiguration)
                    .HasConstraintName("Configuration");
            });

            modelBuilder.Entity<BarrelSection>(entity =>
            {
                entity.ToTable("BarrelSection");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");
            });

            modelBuilder.Entity<BarrelSectionInСonfiguration>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .UseIdentityAlwaysColumn();
                //entity.HasKey(e => new { e.IdConfiguration, e.IdElement });

                entity.ToTable("BarrelSectionInСonfiguration");

                entity.Property(e => e.IdConfiguration).HasColumnName("id_configuration");

                entity.Property(e => e.IdElement).HasColumnName("id_element");

                entity.Property(e => e.Number).HasColumnName("number");

                entity.HasOne(d => d.IdConfigurationNavigation)
                    .WithMany(p => p.BarrelSectionInСonfigurations)
                    .HasForeignKey(d => d.IdConfiguration)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("Сonfiguration");

                entity.HasOne(d => d.IdElementNavigation)
                    .WithMany(p => p.BarrelSectionInСonfigurations)
                    .HasForeignKey(d => d.IdElement)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("Section");
            });

            modelBuilder.Entity<BarrelSectionParametr>(entity =>
            {
                entity.ToTable("BarrelSectionParametr");

                entity.HasIndex(e => e.IdUnit, "IX_Relationship21");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.Designation)
                    .IsRequired()
                    .HasColumnName("designation");

                entity.Property(e => e.IdUnit).HasColumnName("id_unit");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");

                entity.HasOne(d => d.IdUnitNavigation)
                    .WithMany(p => p.BarrelSectionParametrs)
                    .HasForeignKey(d => d.IdUnit)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("Unit");
            });

            modelBuilder.Entity<BarrelSectionParametrValue>(entity =>
            {
                entity.HasKey(e => new { e.IdParametr, e.IdElement });

                entity.ToTable("BarrelSectionParametrValue");

                entity.Property(e => e.IdParametr).HasColumnName("id_parametr");

                entity.Property(e => e.IdElement).HasColumnName("id_element");

                entity.Property(e => e.Value).HasColumnName("value");

                entity.HasOne(d => d.IdElementNavigation)
                    .WithMany(p => p.BarrelSectionParametrValues)
                    .HasForeignKey(d => d.IdElement)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("Section");

                entity.HasOne(d => d.IdParametrNavigation)
                    .WithMany(p => p.BarrelSectionParametrValues)
                    .HasForeignKey(d => d.IdParametr)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("Parametr");
            });

            modelBuilder.Entity<BarrelСonfiguration>(entity =>
            {
                entity.ToTable("BarrelСonfiguration");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Die>(entity =>
            {
                entity.ToTable("Die");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");
            });

            modelBuilder.Entity<DieElement>(entity =>
            {
                entity.ToTable("DieElement");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");
            });

            modelBuilder.Entity<DieElementInСonfiguration>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .UseIdentityAlwaysColumn();
                //entity.HasKey(e => new { e.IdElement, e.IdDie });

                entity.ToTable("DieElementInСonfiguration");

                entity.Property(e => e.IdElement).HasColumnName("id_element");

                entity.Property(e => e.IdConfiguration).HasColumnName("id_die");

                entity.Property(e => e.Number).HasColumnName("number");

                entity.HasOne(d => d.IdDieNavigation)
                    .WithMany(p => p.DieElementInСonfigurations)
                    .HasForeignKey(d => d.IdConfiguration)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("Die");

                entity.HasOne(d => d.IdElementNavigation)
                    .WithMany(p => p.DieElementInСonfigurations)
                    .HasForeignKey(d => d.IdElement)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("Element");
            });

            modelBuilder.Entity<DieElementParametr>(entity =>
            {
                entity.ToTable("DieElementParametr");

                entity.HasIndex(e => e.IdUnit, "IX_Parametr3");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.Designation)
                    .IsRequired()
                    .HasColumnName("designation");

                entity.Property(e => e.IdUnit).HasColumnName("id_unit");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");

                entity.HasOne(d => d.IdUnitNavigation)
                    .WithMany(p => p.DieElementParametrs)
                    .HasForeignKey(d => d.IdUnit)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("Unit");
            });

            modelBuilder.Entity<DieElementParametrValue>(entity =>
            {
                entity.HasKey(e => new { e.IdParametr, e.IdElement });

                entity.ToTable("DieElementParametrValue");

                entity.Property(e => e.IdParametr).HasColumnName("id_parametr");

                entity.Property(e => e.IdElement).HasColumnName("id_element");

                entity.Property(e => e.Value).HasColumnName("value");

                entity.HasOne(d => d.IdElementNavigation)
                    .WithMany(p => p.DieElementParametrValues)
                    .HasForeignKey(d => d.IdElement)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("Element");

                entity.HasOne(d => d.IdParametrNavigation)
                    .WithMany(p => p.DieElementParametrValues)
                    .HasForeignKey(d => d.IdParametr)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("Parametr");
            });

            modelBuilder.Entity<Extruder>(entity =>
            {
                entity.ToTable("Extruder");

                entity.HasIndex(e => e.IdScrew2, "IX_Relationship23");

                entity.HasIndex(e => e.IdScrew1, "IX_Relationship24");

                entity.HasIndex(e => e.IdDie, "IX_Relationship25");

                entity.HasIndex(e => e.IdBarrel, "IX_Relationship26");

                entity.HasIndex(e => e.IdType, "IX_Relationship3");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.Brand)
                    .IsRequired()
                    .HasColumnName("brand");

                entity.Property(e => e.IdBarrel).HasColumnName("id_barrel");

                entity.Property(e => e.IdDie).HasColumnName("id_die");

                entity.Property(e => e.IdScrew1).HasColumnName("id_screw1");

                entity.Property(e => e.IdScrew2).HasColumnName("id_screw2");

                entity.Property(e => e.IdType).HasColumnName("id_type");

                entity.HasOne(d => d.IdBarrelNavigation)
                    .WithMany(p => p.Extruders)
                    .HasForeignKey(d => d.IdBarrel)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("BodyConfiguration");

                entity.HasOne(d => d.IdDieNavigation)
                    .WithMany(p => p.Extruders)
                    .HasForeignKey(d => d.IdDie)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("HeadConfiguration");

                entity.HasOne(d => d.IdScrew1Navigation)
                    .WithMany(p => p.ExtruderIdScrew1Navigations)
                    .HasForeignKey(d => d.IdScrew1)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("ScreConfiguration");

                entity.HasOne(d => d.IdScrew2Navigation)
                    .WithMany(p => p.ExtruderIdScrew2Navigations)
                    .HasForeignKey(d => d.IdScrew2)
                    .HasConstraintName("ScrewConfiguration");

                entity.HasOne(d => d.IdTypeNavigation)
                    .WithMany(p => p.Extruders)
                    .HasForeignKey(d => d.IdType)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("Type");
            });

            modelBuilder.Entity<ExtruderType>(entity =>
            {
                entity.ToTable("ExtruderType");

                entity.HasIndex(e => e.IdModel, "IX_Relationship2");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.IdModel).HasColumnName("id_model");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");

                entity.HasOne(d => d.IdModelNavigation)
                    .WithMany(p => p.ExtruderTypes)
                    .HasForeignKey(d => d.IdModel)
                    .HasConstraintName("MathModel");
            });

            modelBuilder.Entity<Film>(entity =>
            {
                entity.ToTable("Film");

                entity.HasIndex(e => e.IdPolymer, "IX_Relationship6");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.IdPolymer).HasColumnName("id_polymer");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnName("type");

                entity.HasOne(d => d.IdPolymerNavigation)
                    .WithMany(p => p.Films)
                    .HasForeignKey(d => d.IdPolymer)
                    .HasConstraintName("Polymer");
            });

            modelBuilder.Entity<MathModel>(entity =>
            {
                entity.ToTable("MathModel");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");
            });

            modelBuilder.Entity<MathModelCoefficient>(entity =>
            {
                entity.ToTable("MathModelCoefficient");

                entity.HasIndex(e => e.IdUnit, "IX_Relationship13");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.Designation).HasColumnName("designation");

                entity.Property(e => e.IdUnit).HasColumnName("id_unit");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");

                entity.HasOne(d => d.IdUnitNavigation)
                    .WithMany(p => p.MathModelCoefficients)
                    .HasForeignKey(d => d.IdUnit)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("Unit");
            });

            modelBuilder.Entity<MathModelCoefficientValue>(entity =>
            {
                entity.HasKey(e => new { e.IdModel, e.IdCoefficient, e.IdPolymer });

                entity.ToTable("MathModelCoefficientValue");

                entity.Property(e => e.IdModel).HasColumnName("id_model");

                entity.Property(e => e.IdCoefficient).HasColumnName("id_coefficient");

                entity.Property(e => e.IdPolymer).HasColumnName("id_polymer");

                entity.Property(e => e.Value).HasColumnName("value");

                entity.HasOne(d => d.IdCoefficientNavigation)
                    .WithMany(p => p.MathModelCoefficientValues)
                    .HasForeignKey(d => d.IdCoefficient)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("Parametr");

                entity.HasOne(d => d.IdModelNavigation)
                    .WithMany(p => p.MathModelCoefficientValues)
                    .HasForeignKey(d => d.IdModel)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("MathModel");

                entity.HasOne(d => d.IdPolymerNavigation)
                    .WithMany(p => p.MathModelCoefficientValues)
                    .HasForeignKey(d => d.IdPolymer)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("Polymer");
            });

            modelBuilder.Entity<Polymer>(entity =>
            {
                entity.ToTable("Polymer");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");
            });

            modelBuilder.Entity<PolymerParametr>(entity =>
            {
                entity.ToTable("PolymerParametr");

                entity.HasIndex(e => e.IdUnit, "IX_Relationship5");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.Designation)
                    .IsRequired()
                    .HasColumnName("designation");

                entity.Property(e => e.IdUnit).HasColumnName("id_unit");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");

                entity.HasOne(d => d.IdUnitNavigation)
                    .WithMany(p => p.PolymerParametrs)
                    .HasForeignKey(d => d.IdUnit)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("Unit");
            });

            modelBuilder.Entity<PolymerParametrValue>(entity =>
            {
                entity.HasKey(e => new { e.IdPolymer, e.IdParametr });

                entity.ToTable("PolymerParametrValue");

                entity.Property(e => e.IdPolymer).HasColumnName("id_polymer");

                entity.Property(e => e.IdParametr).HasColumnName("id_parametr");

                entity.Property(e => e.Value).HasColumnName("value");

                entity.HasOne(d => d.IdParametrNavigation)
                    .WithMany(p => p.PolymerParametrValues)
                    .HasForeignKey(d => d.IdParametr)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("Parametr");

                entity.HasOne(d => d.IdPolymerNavigation)
                    .WithMany(p => p.PolymerParametrValues)
                    .HasForeignKey(d => d.IdPolymer)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("Polymer");
            });

            modelBuilder.Entity<ProcessParametr>(entity =>
            {
                entity.ToTable("ProcessParametr");

                entity.HasIndex(e => e.IdUnit, "IX_Relationship7");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.Designation)
                    .IsRequired()
                    .HasColumnName("designation");

                entity.Property(e => e.IdUnit).HasColumnName("id_unit");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");

                entity.HasOne(d => d.IdUnitNavigation)
                    .WithMany(p => p.ProcessParametrs)
                    .HasForeignKey(d => d.IdUnit)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("Unit");
            });

            modelBuilder.Entity<ProcessParametrValue>(entity =>
            {
                entity.HasKey(e => new { e.IdFilm, e.IdParametr });

                entity.ToTable("ProcessParametrValue");

                entity.Property(e => e.IdFilm).HasColumnName("id_film");

                entity.Property(e => e.IdParametr).HasColumnName("id_parametr");

                entity.Property(e => e.MaxValue).HasColumnName("max_value");

                entity.Property(e => e.MinValue).HasColumnName("min_value");

                entity.HasOne(d => d.IdFilmNavigation)
                    .WithMany(p => p.ProcessParametrValues)
                    .HasForeignKey(d => d.IdFilm)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("Film");

                entity.HasOne(d => d.IdParametrNavigation)
                    .WithMany(p => p.ProcessParametrValues)
                    .HasForeignKey(d => d.IdParametr)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("Parametr");
            });

            modelBuilder.Entity<Scenario>(entity =>
            {
                entity.ToTable("Scenario");

                entity.HasIndex(e => e.IdExtruder, "ExtruderIndex");

                entity.HasIndex(e => e.IdFilm, "FilmIndex");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.IdExtruder).HasColumnName("id_extruder");

                entity.Property(e => e.IdFilm).HasColumnName("id_film");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");

                entity.Property(e => e.Throughput).HasColumnName("throughput");
                entity.Property(e => e.Time).HasColumnName("time");

                entity.HasOne(d => d.IdExtruderNavigation)
                    .WithMany(p => p.Scenarios)
                    .HasForeignKey(d => d.IdExtruder)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("Extruder");

                entity.HasOne(d => d.IdFilmNavigation)
                    .WithMany(p => p.Scenarios)
                    .HasForeignKey(d => d.IdFilm)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("Film");
            });

            modelBuilder.Entity<Screw>(entity =>
            {
                entity.ToTable("Screw");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");
            });

            modelBuilder.Entity<ScrewElement>(entity =>
            {
                entity.ToTable("ScrewElement");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");
            });

            modelBuilder.Entity<ScrewElementInСonfiguration>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .UseIdentityAlwaysColumn();

                //entity.HasKey(e => new { e.IdConfiguration, e.IdElement });

                entity.ToTable("ScrewElementInСonfiguration");

                entity.Property(e => e.IdConfiguration).HasColumnName("id_configuration");

                entity.Property(e => e.IdElement).HasColumnName("id_element");

                entity.Property(e => e.Number).HasColumnName("number");

                entity.HasOne(d => d.IdConfigurationNavigation)
                    .WithMany(p => p.ScrewElementInСonfigurations)
                    .HasForeignKey(d => d.IdConfiguration)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("Сonfiguration");

                entity.HasOne(d => d.IdElementNavigation)
                    .WithMany(p => p.ScrewElementInСonfigurations)
                    .HasForeignKey(d => d.IdElement)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("Element");
            });

            modelBuilder.Entity<ScrewElementParametr>(entity =>
            {
                entity.ToTable("ScrewElementParametr");

                entity.HasIndex(e => e.IdUnit, "IX_Relationship34");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.Designation)
                    .IsRequired()
                    .HasColumnName("designation");

                entity.Property(e => e.IdUnit).HasColumnName("id_unit");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");

                entity.HasOne(d => d.IdUnitNavigation)
                    .WithMany(p => p.ScrewElementParametrs)
                    .HasForeignKey(d => d.IdUnit)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("Unit");
            });

            modelBuilder.Entity<ScrewElementParametrValue>(entity =>
            {
                entity.HasKey(e => new { e.IdParametr, e.IdElement });

                entity.ToTable("ScrewElementParametrValue");

                entity.Property(e => e.IdParametr).HasColumnName("id_parametr");

                entity.Property(e => e.IdElement).HasColumnName("id_element");

                entity.Property(e => e.Value).HasColumnName("value");

                entity.HasOne(d => d.IdElementNavigation)
                    .WithMany(p => p.ScrewElementParametrValues)
                    .HasForeignKey(d => d.IdElement)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("Element");

                entity.HasOne(d => d.IdParametrNavigation)
                    .WithMany(p => p.ScrewElementParametrValues)
                    .HasForeignKey(d => d.IdParametr)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("Parametr");
            });

            modelBuilder.Entity<ScrewParametr>(entity =>
            {
                entity.ToTable("ScrewParametr");

                entity.HasIndex(e => e.IdUnit, "IX_Relationship4");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.Designation)
                    .IsRequired()
                    .HasColumnName("designation");

                entity.Property(e => e.IdUnit).HasColumnName("id_unit");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");

                entity.HasOne(d => d.IdUnitNavigation)
                    .WithMany(p => p.ScrewParametrs)
                    .HasForeignKey(d => d.IdUnit)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("Unit");
            });

            modelBuilder.Entity<ScrewParametrValue>(entity =>
            {
                entity.HasKey(e => new { e.IdScrew, e.IdParametr });

                entity.ToTable("ScrewParametrValue");

                entity.Property(e => e.IdScrew).HasColumnName("id_screw");

                entity.Property(e => e.IdParametr).HasColumnName("id_parametr");

                entity.Property(e => e.Value).HasColumnName("value");

                entity.HasOne(d => d.IdParametrNavigation)
                    .WithMany(p => p.ScrewParametrValues)
                    .HasForeignKey(d => d.IdParametr)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("Parametr");

                entity.HasOne(d => d.IdScrewNavigation)
                    .WithMany(p => p.ScrewParametrValues)
                    .HasForeignKey(d => d.IdScrew)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("Screw");
            });

            modelBuilder.Entity<ScrewPossibleСonfiguration>(entity =>
            {
                entity.ToTable("ScrewPossibleСonfiguration");

                entity.HasIndex(e => e.IdScrew, "IX_Screw");

                entity.HasIndex(e => e.IdConfiguration, "IX_Сonfiguration");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.IdConfiguration).HasColumnName("id_configuration");

                entity.Property(e => e.IdScrew).HasColumnName("id_screw");

                entity.HasOne(d => d.IdConfigurationNavigation)
                    .WithMany(p => p.ScrewPossibleСonfigurations)
                    .HasForeignKey(d => d.IdConfiguration)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("Сonfiguration");

                entity.HasOne(d => d.IdScrewNavigation)
                    .WithMany(p => p.ScrewPossibleСonfigurations)
                    .HasForeignKey(d => d.IdScrew)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("Screw");
            });

            modelBuilder.Entity<ScrewСonfiguration>(entity =>
            {
                entity.ToTable("ScrewСonfiguration");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Unit>(entity =>
            {
                entity.ToTable("Unit");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.Designation)
                    .IsRequired()
                    .HasColumnName("designation");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
