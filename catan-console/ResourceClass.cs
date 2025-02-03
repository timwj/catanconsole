namespace CatanConsole
{

    /// <summary>
    /// Vector of 5 resources.
    /// </summary>
    public struct ResourceClass
    {

        // Variables containing the amount of resource of a certain type.
        public int lumber;
        public int brick;
        public int wool;
        public int grain;
        public int iron;

        public ResourceClass(int lumber, int brick, int wool, int grain, int iron)
        {
            this.lumber = lumber;
            this.brick = brick;
            this.wool = wool;
            this.grain = grain;
            this.iron = iron;
        }

        public ResourceClass()
        {
            this.lumber = 0;
            this.brick = 0;
            this.wool = 0;
            this.grain = 0;
            this.iron = 0;
        }
        public ResourceClass(enumResource whatKindOfResource, int amount)
        {
            switch (whatKindOfResource)
            {
                case enumResource.lumber: lumber += amount; break;
                case enumResource.brick: brick += amount; break;
                case enumResource.wool: wool += amount; break;
                case enumResource.grain: grain += amount; break;
                case enumResource.iron: iron += amount; break;
            };
        }

        public int total
        {
            get { return lumber + brick + grain + wool + iron; }
        }

        public override string ToString() => $"{lumber}  {brick}  {wool}  {grain}  {iron}";

        public static ResourceClass operator +(ResourceClass a) => a;

        public static ResourceClass operator -(ResourceClass a) => new ResourceClass(-a.lumber, -a.brick, -a.wool, -a.grain, -a.iron);



        public static ResourceClass operator +(ResourceClass a, ResourceClass b) => new ResourceClass(a.lumber + b.lumber, a.brick + b.brick, a.wool + b.wool, a.grain + b.grain, a.iron + b.iron);

        public static ResourceClass operator -(ResourceClass a, ResourceClass b) => a + (-b);

        public int totalResources { get { return lumber + brick + wool + grain + iron; } }

        // A struct value (in)equality requires one to implement 5 functions, as stated in the Microsoft C# documentation: 
        // https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/how-to-define-value-equality-for-a-type
        public static bool operator >=(ResourceClass a, ResourceClass b) => (a.lumber >= b.lumber && a.brick >= b.brick && a.wool >= b.wool && a.grain >= b.grain && a.iron >= b.iron);
        public static bool operator <=(ResourceClass a, ResourceClass b) => (a.lumber <= b.lumber && a.brick <= b.brick && a.wool <= b.wool && a.grain <= b.grain && a.iron <= b.iron);
        public static bool operator >(ResourceClass a, ResourceClass b) => (a.lumber > b.lumber || a.brick > b.brick || a.wool > b.wool || a.grain > b.grain || a.iron >= b.iron);
        public static bool operator <(ResourceClass a, ResourceClass b) => (a.lumber < b.lumber || a.brick < b.brick || a.wool < b.wool || a.grain < b.grain || a.iron >= b.iron);



        public override bool Equals(object obj) => obj is ResourceClass other && this.Equals(other);

        public bool Equals(ResourceClass p) => lumber == p.lumber && brick == p.brick && wool == p.wool && grain == p.grain && iron == p.iron;

        public override int GetHashCode() => (lumber, brick, wool, grain, iron).GetHashCode();

        public static bool operator ==(ResourceClass lhs, ResourceClass rhs) => lhs.Equals(rhs);

        public static bool operator !=(ResourceClass lhs, ResourceClass rhs) => !(lhs == rhs);

        public static ResourceClass operator +(ResourceClass a, (enumResource, int) tp)
        {
            return a.addResource(tp.Item1, tp.Item2);
        }
        public static ResourceClass operator -(ResourceClass a, (enumResource, int) tp)
        {
            return a.addResource(tp.Item1, -tp.Item2);
        }


        // amount will be either 1 or 2 depending on whether the player has a village or a city adjacent to the hex in question.
        public ResourceClass addResource(enumResource whatKindOfResource, int amount)
        {
            switch (whatKindOfResource)
            {
                case enumResource.lumber: lumber += amount; break;
                case enumResource.brick: brick += amount; break;
                case enumResource.wool: wool += amount; break;
                case enumResource.grain: grain += amount; break;
                case enumResource.iron: iron += amount; break;
            };
            return this;
        }
    }




}

